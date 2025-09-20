using System.Runtime.InteropServices;

using FftwF.Dotnet;

using HackRFDotnet.Api.Streams;

using MathNet.Numerics;

namespace HackRFDotnet.Api.SignalProcessing.Effects;
/// <summary>
/// Fast Fourier Transform Effect.
/// Can be used for forward and inverse transforms.
/// Must be given a chunk with a size that is a multiple of 2 [Length % 2 == 0]
/// Must be configured with a chunk size for caching a convert buffer.
/// </summary>
public unsafe class FftEffect : SignalEffect<IQ, IQ>, IDisposable {
    private readonly bool _forward;
    private readonly Complex32* _processingChunk;
    private readonly int _processingChunkLength;

    private readonly FftwPlan _fftwPlan;

    public FftEffect(bool forward, int chunkSize) {
        _forward = forward;
        _processingChunkLength = chunkSize;
        _processingChunk = (Complex32*)Marshal.AllocHGlobal(_processingChunkLength * sizeof(Complex32));

        _fftwPlan = new FftwPlan(chunkSize, _processingChunk, _processingChunk,
            _forward, FftwFlags.Estimate);
    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        var complexFrame = signalTheta.Slice(0, length);

        var processingChunkSpan = new Span<Complex32>(_processingChunk, _processingChunkLength);
        MemoryMarshal.Cast<IQ, Complex32>(complexFrame).CopyTo(processingChunkSpan);
        _fftwPlan.Execute();
        MemoryMarshal.Cast<Complex32, IQ>(processingChunkSpan).CopyTo(signalTheta);

        return base.TransformSignal(signalTheta, length);
    }

    public void Dispose() {
        _fftwPlan.Dispose();
        Marshal.FreeHGlobal((nint)_processingChunk);
    }
}
