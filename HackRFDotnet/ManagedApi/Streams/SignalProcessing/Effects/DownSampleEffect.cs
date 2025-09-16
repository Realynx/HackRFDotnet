using System.Runtime.InteropServices;

using FftwF.Dotnet;

using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Utilities;

using ILGPU.Util;

using MathNet.Numerics;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;

/// <summary>
/// <see cref="DownSampleEffect"/> removes extraneous information from your signal using your desired bandwidth.
/// Example: an FM radio's band is around 200 kHz; the minimum sample rate required to represent this is 400 kS/s (400,000 samples per second).
/// It is recommended that you reduce the sample rate of your audio signal this way before further signal processing to save CPU.
/// </summary>
public unsafe class DownSampleEffect : SignalEffect, ISignalEffect, IDisposable {
    private readonly SampleRate _iqSampleRate;
    private readonly SampleRate _reducedSampledRate;

    private readonly Complex32[] _fftBuffer = [];
    private readonly FftwPlan _fftwPlan;
    private readonly FftwPlan _inverseFftwPlan;

    private readonly int _decimationFactor;

    public DownSampleEffect(SampleRate sampleRate, SampleRate reducedSampleRate, out SampleRate newSampleRate, out int producedChunkSize) {
        _iqSampleRate = sampleRate;
        _reducedSampledRate = reducedSampleRate;

        var desiredDecFactor = sampleRate.Sps / reducedSampleRate.Sps;
        var realDeltaC = SignalStream.PROCESSING_SIZE / desiredDecFactor;
        producedChunkSize = BinaryUtilities.NextPowerOfTwo(realDeltaC);

        _decimationFactor = SignalStream.PROCESSING_SIZE / producedChunkSize;
        newSampleRate = new SampleRate(sampleRate.Sps / _decimationFactor);

        _fftBuffer = new Complex32[SignalStream.PROCESSING_SIZE];
        _fftwPlan = new FftwPlan(SignalStream.PROCESSING_SIZE, _fftBuffer, true, FftwFlags.Estimate);
        _inverseFftwPlan = new FftwPlan(SignalStream.PROCESSING_SIZE, _fftBuffer, false, FftwFlags.Estimate);
    }

    public DownSampleEffect(SampleRate sampleRate, SampleRate reducedSampleRate, int processingSize, out SampleRate newSampleRate, out int producedChunkSize) {
        _iqSampleRate = sampleRate;
        _reducedSampledRate = reducedSampleRate;

        var desiredDecFactor = sampleRate.Sps / reducedSampleRate.Sps;
        var realDeltaC = processingSize / desiredDecFactor;
        producedChunkSize = BinaryUtilities.NextPowerOfTwo(realDeltaC);

        _decimationFactor = processingSize / producedChunkSize;
        newSampleRate = new SampleRate(sampleRate.Sps / _decimationFactor);

        _fftBuffer = new Complex32[processingSize];
        _fftwPlan = new FftwPlan(processingSize, _fftBuffer, true, FftwFlags.Estimate);
        _inverseFftwPlan = new FftwPlan(processingSize, _fftBuffer, false, FftwFlags.Estimate);
    }

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

    public unsafe override int AffectSignal(Span<IQ> signalTheta, int length) {
        if (length % 2 != 0) {
            throw new Exception("Can not process this chunk size");
        }

        MemoryMarshal.Cast<IQ, Complex32>(signalTheta.Slice(0, length)).CopyTo(_fftBuffer);
        _fftwPlan.Execute();

        var resolution = SignalUtilities.FrequencyResolution(length, _iqSampleRate, true);
        var nyquist = _reducedSampledRate.Sps / 2.0;
        for (var x = 0; x < length; x++) {
            var freq = (x < length / 2) ? x * resolution : (x - length) * resolution;

            if (Math.Abs(freq) > nyquist) {
                _fftBuffer[x] = Complex32.Zero;
            }
        }

        // Remove the DC spike
        _fftBuffer[0] = Complex32.Zero;
        _inverseFftwPlan.Execute();

        var decimatedSize = length / _decimationFactor;
        for (var x = 0; x < decimatedSize; x++) {
            _fftBuffer[x] = _fftBuffer[x * _decimationFactor];
        }

        MemoryMarshal.Cast<Complex32, IQ>(_fftBuffer.AsSpan().Slice(0, decimatedSize)).CopyTo(signalTheta);
        return decimatedSize;
    }

    public void Dispose() {
        _fftwPlan.Dispose();
        _inverseFftwPlan.Dispose();
    }
}