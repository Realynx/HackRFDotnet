
using System.Numerics;
using System.Runtime.InteropServices;

using FftSharp;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class FftEffect : SignalEffect {
    private readonly bool _forward;

    public FftEffect(bool forward) {
        _forward = forward;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        var complexFrame = MemoryMarshal
            .Cast<IQ, Complex>(signalTheta)
            .Slice(0, length);

        if (_forward) {
            FFT.Forward(complexFrame);
        }
        else {
            FFT.Inverse(complexFrame);
        }

        return length;
    }
}
