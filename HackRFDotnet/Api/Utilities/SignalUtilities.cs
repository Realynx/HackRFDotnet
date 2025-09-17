using HackRFDotnet.Api.Streams;

namespace HackRFDotnet.Api.Utilities;

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

    public static void ApplyFrequencyOffset(Span<IQ> iqFrame, Frequency freqOffset, SampleRate sampleRate) {
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

    public static long FrequencyResolution(int length, SampleRate sampleRate, bool positiveOnly = true) {
        return positiveOnly
            ? sampleRate.Sps / (length - 1) / 2
            : sampleRate.Sps / length;
    }

    public static float CalculateSignalDb(ReadOnlySpan<IQ> iqFrame) {
        if (iqFrame.Length == 0) {
            return float.NegativeInfinity;
        }

        double power = 0;
        for (var x = 0; x < iqFrame.Length; x++) {
            var s = iqFrame[x];
            power += (s.I * s.I) + (s.Q * s.Q); // instantaneous power
        }

        power /= iqFrame.Length; // mean power
        return 10f * MathF.Log10((float)power + 1e-12f);
    }

    public static float CalculateRmsDb(ReadOnlySpan<IQ> iqFrame) {
        if (iqFrame.Length == 0) {
            return float.NegativeInfinity;
        }

        double sumSq = 0;
        for (var x = 0; x < iqFrame.Length; x++) {
            var s = iqFrame[x];
            sumSq += (s.I * s.I) + (s.Q * s.Q);
        }

        var rms = Math.Sqrt(sumSq / iqFrame.Length);
        return 20f * MathF.Log10((float)rms + 1e-12f);
    }

    public static void NormalizeRms(Span<float> buffer, float targetRms = 0.04f) {
        if (buffer == null || buffer.Length == 0) {
            return;
        }

        var sumSq = 0f;
        for (var x = 0; x < buffer.Length; x++) {
            var v = buffer[x];
            sumSq += v * v;
        }

        var rms = MathF.Sqrt(sumSq / buffer.Length);
        if (rms <= 0.0) {
            return;
        }

        var gain = (float)(targetRms / rms);

        for (var x = 0; x < buffer.Length; x++) {
            buffer[x] *= gain;
        }
    }
}
