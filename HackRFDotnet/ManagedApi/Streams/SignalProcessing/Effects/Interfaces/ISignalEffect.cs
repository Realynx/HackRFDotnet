namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;

public interface ISignalEffect {
    int AffectSignal(Span<IQ> signalTheta, int lendth);
}