using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public abstract class SignalEffect : ISignalEffect {
    public abstract int AffectSignal(Span<IQ> signalTheta, int lendth);
}
