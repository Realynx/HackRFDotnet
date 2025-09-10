using System.Numerics;

namespace HackRFDotnet.ManagedApi.Utilities {
    public static class IQConverter {
        /// <summary>
        /// Convert interleaved 8-bit signed IQ samples to FloatComplex[] with values -1..+1
        /// </summary>
        public static Complex[] ConvertIQBytes(ReadOnlySpan<byte> iqBytes) {
            var sampleCount = iqBytes.Length / 2;
            var iq = new Complex[sampleCount];

            for (var i = 0; i < sampleCount; i++) {
                var iVal = (sbyte)iqBytes[2 * i] / 128.0;
                var qVal = (sbyte)iqBytes[2 * i + 1] / 128.0;
                iq[i] = new Complex(iVal, qVal);
            }

            IQCorrection(iq);

            return iq;
        }

        private static void IQCorrection(Complex[] iq) {
            var meanI = 0.0;
            var meanQ = 0.0;

            var offset = iq.Length % 4;
            for (var i = 0; i < iq.Length - offset; i += 4) {
                meanI += iq[i + 0].Real;
                meanQ += iq[i + 0].Imaginary;
                meanI += iq[i + 1].Real;
                meanQ += iq[i + 1].Imaginary;
                meanI += iq[i + 2].Real;
                meanQ += iq[i + 2].Imaginary;
                meanI += iq[i + 3].Real;
                meanQ += iq[i + 3].Imaginary;
            }

            for (var i = iq.Length - offset; i < iq.Length; i++) {
                meanI += iq[i].Real;
                meanQ += iq[i].Imaginary;
            }

            meanI /= iq.Length;
            meanQ /= iq.Length;

            var mean = new Complex(meanI, meanQ);
            for (var i = 0; i < iq.Length - offset; i += 4) {
                iq[i + 0] -= mean;
                iq[i + 1] -= mean;
                iq[i + 2] -= mean;
                iq[i + 3] -= mean;
            }

            for (var i = iq.Length - offset; i < iq.Length; i++) {
                iq[i] -= mean;
            }
        }
    }
}
