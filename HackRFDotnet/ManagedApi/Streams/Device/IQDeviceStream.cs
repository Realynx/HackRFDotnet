using HackRFDotnet.ManagedApi.Streams.Buffers;
using HackRFDotnet.ManagedApi.Streams.Exceptions;
using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.ManagedApi.Streams.Device {
    public unsafe class IQDeviceStream : IDisposable, IIQStream {
        private readonly DigitalRadioDevice _rfDevice;
        private readonly int _transferBufferSize = 0;

        private ThreadedRingBuffer<IQ>? _iqBuffer = null;
        private readonly IQ[] _convertBuffer = [];

        public double SampleRate { get; private set; }

        public RadioBand Frequency {
            get {
                return _rfDevice.Frequency;
            }
        }

        public int BufferLength {
            get {
                return _iqBuffer?.BytesAvailable() ?? 0;
            }
        }

        public IQDeviceStream(DigitalRadioDevice rfDevice) {
            _rfDevice = rfDevice;

            _transferBufferSize = (int)HackRfNativeLib.DeviceStreaming.GetTransferBufferSize(rfDevice.DevicePtr);
            _convertBuffer = new IQ[_transferBufferSize];
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

            var bufferSize = (int)(TimeSpan.FromMilliseconds(75).TotalSeconds * SampleRate);
            _iqBuffer = new ThreadedRingBuffer<IQ>(bufferSize);

            HackRfNativeLib.DeviceStreaming.SetSampleRate(_rfDevice.DevicePtr, sampleRate);
        }

        public int TxBuffer(Span<IQ> iqFrame) {
            throw new NotImplementedException();
        }

        public int ReadBuffer(Span<IQ> iqBuffer) {
            if (_iqBuffer is null) {
                return 0;
            }

            while (iqBuffer.Length > _iqBuffer.Length) {
                Thread.Sleep(1);
            }

            var readSamples = _iqBuffer?.ReadSpan(iqBuffer) ?? throw new Exception("Empty Buffer");
            return readSamples;
        }

        private int BufferTransferChunk(HackrfTransfer* hackrfTransfer) {
            if (hackrfTransfer is null) {
                return 1;
            }

            // Each callback takes the same amount of time to happen as the frame size is in the time domain
            // 1MSPS = [131ms frame] 131072 / 131ms
            // 3MSPS = [43ms frame]  131072 / 43ms
            // 5MSPS = [26ms frame]  131072 / 26ms
            // 8MSPS = [16ms frame]  131072 / 16ms
            // 16MSPS =[8.2ms frame]  131072 / 8ms
            // 20MSPS =[6.5ms frame]  131072 / 6ms

            var interleavedSampels = new Span<InterleavedSample>(hackrfTransfer->buffer, hackrfTransfer->valid_length / 2);
            for (var x = 0; x < interleavedSampels.Length; x++) {
                var byteSample = interleavedSampels[x];
                _convertBuffer[x] = new IQ(byteSample.I, byteSample.Q);
            }

            _iqBuffer?.WriteSpan(_convertBuffer, interleavedSampels.Length);
            return 0;
        }

        public void Dispose() {
            Close();
        }
    }
}
