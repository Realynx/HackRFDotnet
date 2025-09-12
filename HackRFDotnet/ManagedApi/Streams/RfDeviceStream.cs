using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.ManagedApi.Streams {
    public unsafe class RfDeviceStream : IDisposable, IRfDeviceStream {
        public RadioBand Frequency {
            get {
                return _managedRfDevice.Frequency;
            }
        }

        public int BufferLength {
            get {
                return _iqBuffer?.Count ?? 0;
            }
        }

        public double SampleRate { get; private set; }

        private const int HACK_RF_TRANSFER_SIZE = 131072;
        private RingBuffer<InterleavedSample>? _interleavedBuffer = null;
        private RingBuffer<IQ>? _iqBuffer = null;
        private Thread _bufferKeeping;

        private readonly RfDevice _managedRfDevice;

        public RfDeviceStream(RfDevice managedRfDevice) {
            _managedRfDevice = managedRfDevice;

            _interleavedBuffer = new RingBuffer<InterleavedSample>(5);
            _bufferKeeping = new Thread(ConvertInterleavedSamples);
            _bufferKeeping.Start();
        }

        public void Open(double sampleRate) {
            _managedRfDevice.StartRx();

            SetSampleRate(sampleRate);
        }

        public void Close() {
            _managedRfDevice.StopRx();
        }

        public void SetSampleRate(double sampleRate) {
            SampleRate = sampleRate;

            _interleavedBuffer = new RingBuffer<InterleavedSample>((int)(TimeSpan.FromMilliseconds(500).TotalSeconds * SampleRate));
            _iqBuffer = new RingBuffer<IQ>((int)(TimeSpan.FromMilliseconds(500).TotalSeconds * SampleRate));
            _managedRfDevice.SetSampleRate(SampleRate);
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


        private DateTime _lastCall = DateTime.MinValue;
        internal void BufferTransferChunk(HackrfTransfer hackrfTransfer) {
            var timeBetween = DateTime.Now - _lastCall;
            Console.Title = $"Time Between Callback: {timeBetween.TotalMilliseconds} | Size {hackrfTransfer.valid_length}";
            _lastCall = DateTime.Now;

            // Each callback takes the same amount of time to happen as the frame size is in the time domain
            // 1MSPS = [131ms frame] 131072 / 131ms
            // 3MSPS = [43ms frame]  131072 / 43ms
            // 5MSPS = [26ms frame]  131072 / 26ms
            // 8MSPS = [16ms frame]  131072 / 16ms
            // 16MSPS =[8.2ms frame]  131072 / 8ms
            // 20MSPS =[6.5ms frame]  131072 / 6ms

            var iqBuffer = (InterleavedSample*)hackrfTransfer.buffer;
            var interleavedTransferFrame = new ReadOnlySpan<InterleavedSample>(iqBuffer, hackrfTransfer.valid_length / 2);

            lock (_interleavedBuffer) {
                _interleavedBuffer?.Write(interleavedTransferFrame);
            }
        }

        private void ConvertInterleavedSamples() {
            while (true) {
                if (_interleavedBuffer.IsEmpty) {
                    continue;
                }

                var sampleCount = _interleavedBuffer.Count;
                var interleavedBytes = ArrayPool<InterleavedSample>.Shared.Rent(sampleCount);
                _interleavedBuffer.Read(interleavedBytes.AsSpan(0, sampleCount));

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


        public void Dispose() {
        }
    }
}
