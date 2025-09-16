using System.Runtime.InteropServices;

using FftwF.Dotnet;

using MathNet.Numerics;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public unsafe class FftEffect : SignalEffect {
    private readonly bool _forward;

    public FftEffect(bool forward) {
        _forward = forward;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        var complexFrame = signalTheta
            .Slice(0, length);

        using var plan = new FftwPlan(length, MemoryMarshal.Cast<IQ, Complex32>(complexFrame), _forward, FftwFlags.Estimate);
        plan.Execute();

        return length;
    }
}
