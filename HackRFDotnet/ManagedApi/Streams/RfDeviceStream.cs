using System.Buffers;

using NAudio.Utils;

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
                return _dataBuffer?.Count ?? 0;
            }
        }

        public double SampleRate { get; private set; }

        private RingBuffer<IQ>? _dataBuffer = null;


        private readonly RfDevice _managedRfDevice;

        public RfDeviceStream(RfDevice managedRfDevice) {
            _managedRfDevice = managedRfDevice;
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

            _dataBuffer = new RingBuffer<IQ>((int)(TimeSpan.FromMilliseconds(250).TotalSeconds * SampleRate));
            _managedRfDevice.SetSampleRate(SampleRate);
        }

        public int TxBuffer(Span<IQ> iqFrame) {
            throw new NotImplementedException();
        }

        public int ReadBuffer(Span<IQ> iqBuffer) {
            return _dataBuffer?.Read(iqBuffer) ?? throw new Exception("Empty Buffer");
        }

        internal void BufferTransferChunk(HackrfTransfer hackrfTransfer) {
            var iqBuffer = (InterleavedSample*)hackrfTransfer.buffer;
            var interleavedTransferFrame = new ReadOnlySpan<InterleavedSample>(iqBuffer, hackrfTransfer.valid_length / 2);

            var iqSamples = ArrayPool<IQ>.Shared.Rent(interleavedTransferFrame.Length);

            try {
                for (var x = 0; x < interleavedTransferFrame.Length; x++) {
                    iqSamples[x] = new IQ(interleavedTransferFrame[x]);
                }

                if (iqSamples.Length == 0) {
                    return;
                }

                lock (this) {
                    _dataBuffer?.Write(iqSamples.AsSpan(0, interleavedTransferFrame.Length));
                }
            }
            finally {
                ArrayPool<IQ>.Shared.Return(iqSamples);
            }
        }

        public void Dispose() {

        }
    }
}
