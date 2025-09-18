using System.Runtime.InteropServices;

using FftwF.Dotnet;

using HackRFDotnet.Api.Streams.SignalStreams;
using HackRFDotnet.Api.Utilities;

using MathNet.Numerics;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;

/// <summary>
/// <see cref="IQDownSampleEffect"/> removes extraneous information from your signal using your desired bandwidth.
/// Example: an FM radio's band is around 200 kHz; the minimum sample rate required to represent this is 400 kS/s (400,000 samples per second).
/// It is recommended that you reduce the sample rate of your audio signal this way before further signal processing to save CPU.
/// </summary>
public unsafe class IQDownSampleEffect : SignalEffect<IQ, IQ>, IDisposable {
    private readonly SampleRate _iqSampleRate;
    private readonly SampleRate _reducedSampledRate;

    private readonly Complex32[] _fftInBuffer = [];
    private readonly Complex32[] _fftOutBuffer = [];
    private readonly FftwPlan _fftwPlan;
    private readonly FftwPlan _inverseFftwPlan;

    private readonly int _decimationFactor;

    /// <summary>
    /// Configure a signal down sampler. You should do this to reduce cpu time when processing your signal.
    /// </summary>
    /// <param name="sampleRate">Sample rate of the incoming signal.</param>
    /// <param name="reducedSampleRate">Desired reduced sample rate.</param>
    /// <param name="newSampleRate">The closest possible sample rate achievable.</param>
    /// <param name="producedChunkSize">The chunk size after down sampling.</param>
    public IQDownSampleEffect(SampleRate sampleRate, SampleRate reducedSampleRate, out SampleRate newSampleRate, out int producedChunkSize) {
        _iqSampleRate = sampleRate;
        _reducedSampledRate = reducedSampleRate;

        var desiredDecFactor = (int)(sampleRate.Sps / reducedSampleRate.Sps);
        var realDeltaC = SignalStream<IQ>.PROCESSING_SIZE / desiredDecFactor;
        producedChunkSize = BinaryUtilities.NextPowerOfTwo(realDeltaC);

        _decimationFactor = SignalStream<IQ>.PROCESSING_SIZE / producedChunkSize;
        newSampleRate = new SampleRate(sampleRate.Sps / _decimationFactor);

        _fftInBuffer = new Complex32[SignalStream<IQ>.PROCESSING_SIZE];
        _fftOutBuffer = new Complex32[SignalStream<IQ>.PROCESSING_SIZE];

        _fftwPlan = new FftwPlan(SignalStream<IQ>.PROCESSING_SIZE, _fftInBuffer, _fftOutBuffer, true, FftwFlags.Estimate);
        _inverseFftwPlan = new FftwPlan(SignalStream<IQ>.PROCESSING_SIZE, _fftOutBuffer, _fftInBuffer, false, FftwFlags.Estimate);
    }

    /// <summary>
    /// Configure a signal down sampler. You should do this to reduce cpu time when processing your signal.
    /// </summary>
    /// <param name="sampleRate">Sample rate of the incoming signal.</param>
    /// <param name="reducedSampleRate">Desired reduced sample rate.</param>
    /// <param name="processingSize">The input chunk size. Used to calculate the nearest achievable sample rate.</param>
    /// <param name="newSampleRate">The closest possible sample rate achievable.</param>
    /// <param name="producedChunkSize">The chunk size after down sampling.</param>
    public IQDownSampleEffect(SampleRate sampleRate, SampleRate reducedSampleRate, int processingSize, out SampleRate newSampleRate, out int producedChunkSize) {
        _iqSampleRate = sampleRate;
        _reducedSampledRate = reducedSampleRate;

        var desiredDecFactor = (int)(sampleRate.Sps / reducedSampleRate.Sps);
        var realDeltaC = processingSize / desiredDecFactor;
        producedChunkSize = BinaryUtilities.NextPowerOfTwo(realDeltaC);

        _decimationFactor = processingSize / producedChunkSize;
        newSampleRate = new SampleRate(sampleRate.Sps / _decimationFactor);

        _fftInBuffer = new Complex32[processingSize];
        _fftOutBuffer = new Complex32[processingSize];

        _fftwPlan = new FftwPlan(processingSize, _fftInBuffer, _fftOutBuffer, true, FftwFlags.Estimate);
        _inverseFftwPlan = new FftwPlan(processingSize, _fftOutBuffer, _fftInBuffer, false, FftwFlags.Estimate);
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

    public unsafe override int TransformSignal(Span<IQ> signalTheta, int length) {
        if (length % 2 != 0) {
            throw new Exception("Can not process this chunk size");
        }

        MemoryMarshal.Cast<IQ, Complex32>(signalTheta.Slice(0, length)).CopyTo(_fftInBuffer);
        _fftwPlan.Execute();

        var resolution = SignalUtilities.FrequencyResolution(length, _iqSampleRate, true);
        var nyquist = _reducedSampledRate.Sps / 2.0;
        for (var x = 0; x < length; x++) {
            var freq = (x < length / 2) ? x * resolution : (x - length) * resolution;

            if (Math.Abs(freq) > nyquist) {
                _fftOutBuffer[x] = Complex32.Zero;
            }
        }

        _inverseFftwPlan.Execute();
        MemoryMarshal.Cast<Complex32, IQ>(_fftInBuffer).CopyTo(signalTheta.Slice(0, length));

        var decimatedSize = length / _decimationFactor;
        for (var x = 0; x < decimatedSize; x++) {
            signalTheta[x] = signalTheta[x * _decimationFactor];
        }


        return base.TransformSignal(signalTheta, decimatedSize);
    }

    public void Dispose() {
        _fftwPlan.Dispose();
        _inverseFftwPlan.Dispose();
    }
}