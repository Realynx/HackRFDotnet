using System;


using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.ManagedApi.Utilities;
using HackRFDotnet.Structs;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams {
    public unsafe class RfDeviceStream : IDisposable {
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

        private RingBuffer<float> _noiseHistory = new(100);
        private RingBuffer<IQ>? _dataBuffer = null;

        private readonly RfDevice _managedRfDevice;

        public RfDeviceStream(RfDevice managedRfDevice) {
            _managedRfDevice = managedRfDevice;
        }

        public void Open(double sampleRate) {
            _managedRfDevice.StartRx();
            SetSampleRate(sampleRate);
        }

        public void SetSampleRate(double sampleRate) {
            SampleRate = sampleRate;

            _dataBuffer = new RingBuffer<IQ>((int)(TimeSpan.FromMilliseconds(250).TotalSeconds * SampleRate));
            _managedRfDevice.SetSampleRate(SampleRate);
        }

        public void Close() {
            _managedRfDevice.StopRx();
        }

        public int ReadBuffer(Span<IQ> iqBuffer) {
            return _dataBuffer?.Read(iqBuffer) ?? throw new Exception("Empty Buffer");
        }

        internal void BufferTransferChunk(HackrfTransfer hackrfTransfer) {
            var rfTransferBuffer = new ReadOnlySpan<byte>(hackrfTransfer.buffer, hackrfTransfer.valid_length);
            var iqSample = IQConverter.ConvertIQBytes(rfTransferBuffer);

            if (rfTransferBuffer.Length == 0) {
                return;
            }

            lock (this) {
                _dataBuffer?.Write(iqSample);
            }
        }

        public float GetNoiseFloorDb() {
            if (_noiseHistory.Count == 0) {
                return 0f;
            }

            // Sort values
            var noiseFloorBuffer = new float[_noiseHistory.Count];
            _noiseHistory.Peek(noiseFloorBuffer);

            var sorted = noiseFloorBuffer.OrderBy(x => x).ToList();

            var trimCount = (int)(sorted.Count * .15f);

            // Remove lowest and highest values
            var trimmed = sorted.Skip(trimCount).Take(sorted.Count - (2 * trimCount));

            return trimmed.Average();
        }

        //public float CalculateDb() {
        //    // Compute RMS magnitude
        //    double power = 0f;
        //    for (var x = 0; x < _iqSample.Length; x++) {
        //        var s = _iqSample[x];
        //        power += (s.Real * s.Real) + (s.Imaginary * s.Imaginary); // sum |x|^2
        //    }
        //    power /= _iqSample.Length;

        //    // Convert to dB
        //    var dbAverage = 10f * (float)Math.Log10(power + 1e-12f);
        //    return dbAverage;
        //}

        public void Dispose() {
        }
    }
}
