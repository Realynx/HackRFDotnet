using System.Numerics;

using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Types;

using MathNet.Filtering.FIR;

namespace HackRFDotnet.ManagedApi.Services {
    internal class ChannelFilteringService : IDisposable {
        private readonly double[] _lowPassFirCoefficients;

        private readonly OnlineFirFilter _filterI;
        private readonly OnlineFirFilter _filterQ;
        private readonly RfStream _iQStream;

        public ChannelFilteringService(RfStream iQStream) {
            var halfOrder = 70;
            _lowPassFirCoefficients = FirCoefficients.LowPass(
                samplingRate: iQStream.SampleRate,
                cutoff: iQStream.Bandwith.Hz / 2,
                dcGain: 2.0,
                halforder: halfOrder
            );

            _filterI = new OnlineFirFilter(_lowPassFirCoefficients);
            _filterQ = new OnlineFirFilter(_lowPassFirCoefficients);
            _iQStream = iQStream;
        }

        public void ApplyFilter(Complex[] iqFrame) {
            FilterFrame(iqFrame);
        }

        public void ApplyFilterOffset(Complex[] iqFrame, RadioBand freqOffset) {
            for (var x = 0; x < iqFrame.Length; x++) {
                var t = x / _iQStream.SampleRate;
                var phase = -2.0 * Math.PI * freqOffset.Hz * t;

                var osc = new Complex(Math.Cos(phase), Math.Sin(phase));
                iqFrame[x] *= osc;
            }

            FilterFrame(iqFrame);
        }

        private void FilterFrame(Complex[] iqFrame) {
            for (var x = 0; x < iqFrame.Length; x++) {
                var iFiltered = _filterI.ProcessSample(iqFrame[x].Real);
                var qFiltered = _filterQ.ProcessSample(iqFrame[x].Imaginary);
                iqFrame[x] = new Complex(iFiltered, qFiltered);
            }
        }

        public void Dispose() {

        }
    }
}
