using System.Numerics;

using HackRFDotnet.ManagedApi.Types;

using MathNet.Filtering.FIR;

namespace HackRFDotnet.ManagedApi.Utilities {
    public static class SDRUtility {
        /// <summary>
        /// Frequency-shift and low-pass filter IQ samples around a target station.
        /// </summary>
        public static void ExtractChannelInPlace(Complex[] iq, double sampleRate, RadioBand freqOffset, RadioBand bandwidth) {
            // 1. Frequency shift
            for (int i = 0; i < iq.Length; i++) {
                double t = i / sampleRate;
                double phase = -2.0 * Math.PI * freqOffset.Hz * t;
                Complex osc = new Complex(Math.Cos(phase), Math.Sin(phase));
                iq[i] *= osc;
            }

            // 2. Low-pass filter around DC (FIR)
            var halfOrder = 55;
            var coeffs = FirCoefficients.LowPass(
                samplingRate: sampleRate,
                cutoff: (bandwidth.Hz / 2),
                dcGain: 2.0,
                halforder: halfOrder
            );

            var filterI = new OnlineFirFilter(coeffs);
            var filterQ = new OnlineFirFilter(coeffs);

            // 3. Apply filter to I and Q
            for (int i = 0; i < iq.Length; i++) {
                double iFiltered = filterI.ProcessSample(iq[i].Real);
                double qFiltered = filterQ.ProcessSample(iq[i].Imaginary);
                iq[i] = new Complex(iFiltered, qFiltered);
            }
        }

    }
}
