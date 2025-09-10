using System.Numerics;

using HackRFDotnet.ManagedApi.Services;
using HackRFDotnet.ManagedApi.Utilities;
using HackRFDotnet.Structs;

namespace HackRFDotnet.ManagedApi.Types {
    public unsafe class IQStream : IDisposable {
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

        public RadioBand Bandwith {
            get {
                return _managedRfDevice.Bandwidth;
            }
        }

        public double SampleRate {
            get {
                return _sampleRate;
            }
        }
        private readonly double _sampleRate;


        private readonly Queue<float> _noiseHistory = new Queue<float>(100);
        private float _lastNoiseLevel = -100;
        private readonly ChannelFilteringService _channelFilteringService;

        private readonly Queue<Complex[]> _complexFrames = new(1_000);
        private readonly RfDevice _managedRfDevice;
        public ManualResetEvent WaitForFrame { get; set; } = new ManualResetEvent(false);



        public IQStream(RfDevice managedRfDevice, double sampleRate) {
            _managedRfDevice = managedRfDevice;
            _sampleRate = sampleRate;
            _managedRfDevice.OnSample(QueueSampleFrame);

            _channelFilteringService = new ChannelFilteringService(this);
        }

        public void StartListening() {
            _managedRfDevice.SetSampleRate(_sampleRate);
            _managedRfDevice.StartRx();
        }

        private void QueueSampleFrame(HackrfTransfer hackrfTransfer) {
            var rfTransferBuffer = new ReadOnlySpan<byte>(hackrfTransfer.buffer, hackrfTransfer.valid_length);

            var iqFrame = IQConverter.ConvertIQBytes(rfTransferBuffer);
            if (iqFrame is null) {
                return;
            }

            lock (this) {
                _complexFrames.Enqueue(iqFrame);
            }
        }

        public void StopListening() {
            _managedRfDevice.StopRx();
        }


        public Complex[] WaitAndDequeue() {
            while (_complexFrames.Count < 1) {
                Thread.Sleep(1);
            }

            lock (this) {
                var iqFrame = _complexFrames.Dequeue();
                if (_noiseHistory.Count >= 100) {
                    _noiseHistory.Dequeue();
                }

                var level = CalculateDb(iqFrame);
                _noiseHistory.Enqueue(level);

                _channelFilteringService.ApplyFilter(iqFrame);

                _lastNoiseLevel = CalculateDb(iqFrame);

                WaitForFrame.Set();
                WaitForFrame.Reset();
                return iqFrame;
            }
        }

        private float CalculateDb(Complex[] iqFrame) {
            // Compute RMS magnitude
            double power = 0f;
            for (var x = 0; x < iqFrame.Length; x++) {
                var s = iqFrame[x];
                power += s.Real * s.Real + s.Imaginary * s.Imaginary; // sum |x|^2
            }
            power /= iqFrame.Length;

            // Convert to dB
            var frameDb = 10f * (float)Math.Log10(power + 1e-12f);
            return frameDb;
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
