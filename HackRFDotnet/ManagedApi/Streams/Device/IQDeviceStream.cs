using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Exceptions;
using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Threading;
using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.ManagedApi.Streams.Device {
    public unsafe class IQDeviceStream : IDisposable, IIQStream {
        private readonly DigitalRadioDevice _rfDevice;

        private ThreadedConvertingBuffer<InterleavedSample>? _interleavedBuffer = null;
        private RingBuffer<IQ>? _iqBuffer = null;

        public double SampleRate { get; private set; }
        public RadioBand Frequency {
            get {
                return _rfDevice.Frequency;
            }
        }

        public int BufferLength {
            get {
                return _iqBuffer?.Count ?? 0;
            }
        }

        public IQDeviceStream(DigitalRadioDevice rfDevice, int sampleRate) {
            _rfDevice = rfDevice;
            SetSampleRate(sampleRate);
        }

        public void OpenRx(double? sampleRate = null) {
            if (sampleRate is null) {
                sampleRate = SampleRate;
            }
            _rfDevice.StartRx(BufferTransferChunk);

            SetSampleRate(sampleRate ?? throw new ZeroSampleRateException("Cannot have a 0 sample rate"));
        }

        public void Close() {
            _rfDevice.StopRx();
        }

        public void SetSampleRate(double sampleRate) {
            SampleRate = sampleRate;

            var bufferSize = (int)(TimeSpan.FromMilliseconds(250).TotalSeconds * SampleRate);
            _interleavedBuffer = new ThreadedConvertingBuffer<InterleavedSample>(new RingBuffer<InterleavedSample>(bufferSize), ConvertInterleavedSamples);
            _iqBuffer = new RingBuffer<IQ>(bufferSize);

            HackRfNativeLib.DeviceStreaming.SetSampleRate(_rfDevice.DevicePtr, sampleRate);
        }

        public int TxBuffer(Span<IQ> iqFrame) {
            throw new NotImplementedException();
        }

        public int ReadBuffer(Span<IQ> iqBuffer) {
            if (_iqBuffer is null) {
                return 0;
            }

            while (iqBuffer.Length > _iqBuffer.Count) {
            }

            lock (_iqBuffer) {
                var readSamples = _iqBuffer?.Read(iqBuffer) ?? throw new Exception("Empty Buffer");
                return readSamples;
            }
        }

        private void ConvertInterleavedSamples(RingBuffer<InterleavedSample> buffer) {
            while (true) {
                if (buffer.IsEmpty) {
                    continue;
                }

                var sampleCount = buffer.Count;
                var interleavedBytes = ArrayPool<InterleavedSample>.Shared.Rent(sampleCount);
                buffer.Read(interleavedBytes.AsSpan(0, sampleCount));

                var iqSamples = ArrayPool<IQ>.Shared.Rent(sampleCount);

                try {
                    for (var x = 0; x < sampleCount; x++) {
                        iqSamples[x] = new IQ(interleavedBytes[x]);
                    }

                    if (_iqBuffer is null || iqSamples.Length == 0) {
                        return;
                    }

                    lock (_iqBuffer) {
                        _iqBuffer?.Write(iqSamples.AsSpan(0, sampleCount));
                    }
                }
                finally {
                    ArrayPool<InterleavedSample>.Shared.Return(interleavedBytes);
                    ArrayPool<IQ>.Shared.Return(iqSamples);
                }
            }
        }

        private DateTime _lastCall = DateTime.MinValue;
        internal int BufferTransferChunk(HackrfTransfer* hackrfTransfer) {
            if (hackrfTransfer is null) {
                return 1;
            }

            var timeBetween = DateTime.Now - _lastCall;
            Console.Title = $"Time Between Callback: {timeBetween.TotalMilliseconds} | Size {hackrfTransfer->valid_length}";
            _lastCall = DateTime.Now;

            // Each callback takes the same amount of time to happen as the frame size is in the time domain
            // 1MSPS = [131ms frame] 131072 / 131ms
            // 3MSPS = [43ms frame]  131072 / 43ms
            // 5MSPS = [26ms frame]  131072 / 26ms
            // 8MSPS = [16ms frame]  131072 / 16ms
            // 16MSPS =[8.2ms frame]  131072 / 8ms
            // 20MSPS =[6.5ms frame]  131072 / 6ms

            var iqBuffer = (InterleavedSample*)hackrfTransfer->buffer;
            var interleavedTransferFrame = new ReadOnlySpan<InterleavedSample>(iqBuffer, hackrfTransfer->valid_length / 2);

            lock (_interleavedBuffer) {
                _interleavedBuffer?.Write(interleavedTransferFrame);
            }
            return 0;
        }

        public void Dispose() {
        }
    }
}
