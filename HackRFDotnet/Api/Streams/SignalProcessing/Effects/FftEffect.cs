using System.Runtime.InteropServices;

using FftwF.Dotnet;

using MathNet.Numerics;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;
/// <summary>
/// Fast Fourier Transform Effect.
/// Can be used for forward and inverse transforms.
/// Must be given a chunk with a size that is a multiple of 2 [Length % 2 == 0]
/// Must be configured with a chunk size for caching a convert buffer.
/// </summary>
public unsafe class FftEffect : SignalEffect<IQ, IQ>, IDisposable {
    private readonly bool _forward;
    private readonly Complex32[] _processingChunk = [];

    private readonly FftwPlan _fftwPlan;

    public FftEffect(bool forward, int chunkSize) {
        _forward = forward;
        _processingChunk = new Complex32[chunkSize];

        _fftwPlan = new FftwPlan(chunkSize, _processingChunk.AsSpan(), _processingChunk.AsSpan(),
            _forward, FftwFlags.Estimate);
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        var complexFrame = signalTheta.Slice(0, length);

        MemoryMarshal.Cast<IQ, Complex32>(complexFrame).CopyTo(_processingChunk);
        _fftwPlan.Execute();
        MemoryMarshal.Cast<Complex32, IQ>(_processingChunk).CopyTo(signalTheta);

        return base.AffectSignal(signalTheta, length);
    }

    public void Dispose() {
        _fftwPlan.Dispose();
    }
}
