using System.Runtime.InteropServices;

using FftwF.Dotnet;

using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Utilities;

using MathNet.Numerics;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;

/// <summary>
/// <see cref="ReducerEffect"/> removes extraneous information from your signal using your desired bandwidth.
/// Example: an FM radio's band is around 200 kHz; the minimum sample rate required to represent this is 400 kS/s (400,000 samples per second).
/// It is recommended that you reduce the sample rate of your audio signal this way before further signal processing to save CPU.
/// </summary>
public unsafe class ReducerEffect : SignalEffect, ISignalEffect {
    private readonly SampleRate _iqSampleRate;
    private readonly SampleRate _reducedSampledRate;
    private readonly void* _fftwPlan;
    private readonly int _decimationFactor;

    public ReducerEffect(SampleRate sampleRate, SampleRate reducedSampleRate, out SampleRate newSampleRate) {
        _iqSampleRate = sampleRate;
        _reducedSampledRate = reducedSampleRate;

        /*
         * D - Decimation Factor
         * C - SampleCount
         * ΔC - ReducedCount
         * D = C / ΔC
         */

        var desiredDecFactor = sampleRate.Sps / reducedSampleRate.Sps;
        var realDeltaC = SignalStream.PROCESSING_SIZE / desiredDecFactor;
        var correctedDeltaC = BinaryUtilities.NextPowerOfTwo(realDeltaC);

        _decimationFactor = SignalStream.PROCESSING_SIZE / correctedDeltaC;
        newSampleRate = new SampleRate(sampleRate.Sps / _decimationFactor);
    }

    public unsafe override int AffectSignal(Span<IQ> signalTheta, int length) {
        /*
         * 6ghz crystal tuning = f
         * r = measured voltage
         * i = r(t) * cos(2 * pi * f * t)
         * q = r(t) * sin(2 * pi * f * t)
         * This creates a relationship to the signal with the overlapping of i and q, so we can resample and frequency shift to DC
         * 
         * (Msps / 20,000,000)
         * frequency shift -> 0
         */

        if (length % 2 != 0) {
            throw new Exception("Can not process this chunk size");
        }

        var resolution = SignalUtilities.FrequencyResolution(length, _iqSampleRate);

        var fftwArray = MemoryMarshal.Cast<IQ, Complex32>(signalTheta.Slice(0, length));
        using var fowardPlan = new FftwPlan(length, fftwArray, true, FftwFlags.Estimate);
        using var backwardPlan = new FftwPlan(length, fftwArray, false, FftwFlags.Estimate);

        fowardPlan.Execute();

        for (var x = 0; x < length; x++) {
            var currentFreq = x * resolution;
            if (currentFreq > _reducedSampledRate.Sps / 2) {
                signalTheta[x] = IQ.Zero;
            }
        }

        backwardPlan.Execute();

        var decimatedSize = length / _decimationFactor;
        for (var x = 0; x < decimatedSize; x++) {
            signalTheta[x] = signalTheta[x * _decimationFactor];
        }

        return decimatedSize;
    }
}