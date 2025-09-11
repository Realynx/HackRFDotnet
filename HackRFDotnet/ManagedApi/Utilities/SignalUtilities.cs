using System.Numerics;

using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Utilities;
public class SignalUtilities {
    public static void ApplyPhaseOffset(Span<Complex> iqFrame, RadioBand freqOffset, double _sampleRate) {
        for (var x = 0; x < iqFrame.Length; x++) {
            var theta = x / _sampleRate;
            var phase = -2.0 * Math.PI * freqOffset.Hz * theta;

            var osc = new Complex(Math.Cos(phase), Math.Sin(phase));
            iqFrame[x] *= osc;
        }
    }
}
