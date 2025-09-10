using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.Structs;

namespace HackRFDotnet.ManagedApi.Streams {
    public unsafe class RfDeviceStream : IDisposable {
        public bool DataAvailable {
            get {
                return _complexFrames.Count > 0;
            }
        }

        public RadioBand Frequency {
            get {
                return _managedRfDevice.Frequency;
            }
        }

        public double SampleRate { get; init; }

        private readonly Queue<float> _noiseHistory = new Queue<float>(100);
        private float _lastNoiseLevel = -100;

        private readonly Queue<IQFrame> _complexFrames = new(1_000);
        private readonly RfDevice _managedRfDevice;
        public ManualResetEvent WaitForFrame { get; set; } = new ManualResetEvent(false);


        public RfDeviceStream(RfDevice managedRfDevice, double sampleRate) {
            _managedRfDevice = managedRfDevice;
            SampleRate = sampleRate;

            _managedRfDevice.RfDataAvailable += QueueSampleFrame;

            _managedRfDevice.SetSampleRate(SampleRate);
            _managedRfDevice.StartRx();
        }

        private void QueueSampleFrame(HackrfTransfer hackrfTransfer) {
            var rfTransferBuffer = new ReadOnlySpan<byte>(hackrfTransfer.buffer, hackrfTransfer.valid_length);
            var iqFrame = new IQFrame(rfTransferBuffer);

            if (rfTransferBuffer.Length == 0) {
                return;
            }

            lock (this) {
                if (_complexFrames.Count > 100) {
                    _ = _complexFrames.Dequeue();
                }
                _complexFrames.Enqueue(iqFrame);
            }
        }

        public void StopListening() {
            _managedRfDevice.StopRx();
        }


        public IQFrame WaitAndDequeue() {
            while (_complexFrames.Count < 1) {
                Thread.Sleep(1);
            }

            lock (this) {
                var iqFrame = _complexFrames.Dequeue();

                WaitForFrame.Set();
                WaitForFrame.Reset();
                return iqFrame;
            }
        }

        public float GetNoiseFloorDb() {
            return _noiseHistory.Count == 0 ? 0f : _noiseHistory.Average();
        }

        public float GetLastLevelDb() {
            return _lastNoiseLevel;
        }

        public void Dispose() {

            StopListening();
            lock (this) {
                _complexFrames.Clear();
            }
        }
    }
}
