using System.Numerics;

using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

using MathNet.Numerics.Distributions;

namespace HackRFDotnet.ManagedApi.Utilities;

public unsafe class SignalUtilities {
    public static void IQCorrection(Span<IQ> iqFrame) {
        var meanI = 0.0f;
        var meanQ = 0.0f;

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

    //public static void ApplyFrequencyOffset(Span<IQ> iqFrame, RadioBand freqOffset, float sampleRate) {
    //    // Phase increment per sample in radians
    //    var phaseIncrement = -2.0f * MathF.PI * freqOffset.Hz / sampleRate;

    //    var oscIncrement = new IQ(MathF.Cos(phaseIncrement), MathF.Sin(phaseIncrement));
    //    var osc = new IQ(1.0f, 0.0f);

    //    for (var x = 0; x < iqFrame.Length; x++) {
    //        iqFrame[x] *= osc;
    //        osc *= oscIncrement;
    //    }
    //}

    public static void ApplyFrequencyOffset(Span<IQ> iqFrame, RadioBand freqOffset, SampleRate sampleRate) {
        // Phase increment per sample
        var phaseIncrement = 2.0f * MathF.PI * freqOffset.Hz / sampleRate.Sps;
        double cosInc = MathF.Cos(phaseIncrement);
        double sinInc = MathF.Sin(phaseIncrement);

        // Start oscillator at 1 + j0
        float cos = 1.0f, sin = 0.0f;

        for (int n = 0; n < iqFrame.Length; n++) {
            var s = iqFrame[n];

            // Complex multiply by oscillator
            float i = s.I * cos - s.Q * sin;
            float q = s.I * sin + s.Q * cos;
            iqFrame[n] = new IQ(i, q);

            // Recursively update oscillator
            float newCos = (float)(cos * cosInc - sin * sinInc);
            float newSin = (float)(cos * sinInc + sin * cosInc);
            cos = newCos;
            sin = newSin;

            // Optional: renormalize every few thousand samples
            if ((n & 0x3FFF) == 0) { // every 16k samples
                float mag = 1.0f / (float)(MathF.Sqrt(cos * cos + sin * sin));
                cos *= mag;
                sin *= mag;
            }
        }
    }

    public static int FrequencyResolution(int length, SampleRate sampleRate, bool positiveOnly = true) {
        return positiveOnly
            ? sampleRate.Sps / (length - 1) / 2
            : sampleRate.Sps / length;
    }


    public static float CalculateDb(Span<IQ> iqFrame) {
        // Compute RMS magnitude
        var power = 0f;
        for (var x = 0; x < iqFrame.Length; x++) {
            power += iqFrame[x].Magnitude;
        }
        power /= iqFrame.Length;

        // Convert to dB
        var dbAverage = 10f * MathF.Log10(power + 1e-12f);
        return dbAverage;
    }
}
