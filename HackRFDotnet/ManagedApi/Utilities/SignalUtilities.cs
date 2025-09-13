using System.Numerics;

using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

using MathNet.Numerics.Distributions;

namespace HackRFDotnet.ManagedApi.Utilities;

public unsafe class SignalUtilities {
    public static void IQCorrection(Span<IQ> iqFrame) {
        var meanI = 0.0;
        var meanQ = 0.0;

        for (var i = 0; i < iqFrame.Length; i++) {
            meanI += iqFrame[i].I;
            meanQ += iqFrame[i].Q;
        }

        meanI /= iqFrame.Length;
        meanQ /= iqFrame.Length;

        var mean = new IQ(meanI, meanQ);
        for (var i = 0; i < iqFrame.Length; i++) {
            iqFrame[i] -= mean;
        }
    }

    public static void ApplyFrequencyOffset(Span<IQ> iqFrame, RadioBand freqOffset, double sampleRate) {
        // Phase increment per sample in radians
        var phaseIncrement = -2.0 * Math.PI * freqOffset.Hz / sampleRate;

        var oscIncrement = new IQ(Math.Cos(phaseIncrement), Math.Sin(phaseIncrement));
        var osc = new IQ(1.0, 0.0);


        var simdChunk = new Vector<IQ>();

        for (var x = 0; x < iqFrame.Length; x++) {
            iqFrame[x] *= osc;
            osc *= oscIncrement;
        }
    }


    public static float CalculateDb(Span<IQ> iqFrame) {
        // Compute RMS magnitude
        double power = 0f;
        for (var x = 0; x < iqFrame.Length; x++) {
            power += iqFrame[x].Magnitude;
        }
        power /= iqFrame.Length;

        // Convert to dB
        var dbAverage = 10f * (float)Math.Log10(power + 1e-12f);
        return dbAverage;
    }
}
