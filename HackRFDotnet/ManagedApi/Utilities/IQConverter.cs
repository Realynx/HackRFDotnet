

using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Utilities {
    public static class IQConverter {
        /// <summary>
        /// Convert interleaved 8-bit signed IQ samples to FloatComplex[] with values -1..+1
        /// </summary>
        //public static IQ[] ConvertIQBytes(ReadOnlySpan<byte> iqBytes) {
        //    var sampleCount = iqBytes.Length / 2;
        //    var iq = new IQ[sampleCount];

        //    for (var i = 0; i < sampleCount; i++) {
        //        var iVal = (sbyte)iqBytes[2 * i] / 128.0;
        //        var qVal = (sbyte)iqBytes[2 * i + 1] / 128.0;
        //        iq[i] = new IQ(iVal, qVal);
        //    }

        //    return iq;
        //}

        private static void IQCorrection(IQ[] iq) {
            var meanI = 0.0;
            var meanQ = 0.0;

            for (var i = 0; i < iq.Length; i++) {
                meanI += iq[i].I;
                meanQ += iq[i].Q;
            }

            meanI /= iq.Length;
            meanQ /= iq.Length;

            var mean = new IQ(meanI, meanQ);
            for (var i = 0; i < iq.Length; i++) {
                iq[i] -= mean;
            }
        }
    }
}
