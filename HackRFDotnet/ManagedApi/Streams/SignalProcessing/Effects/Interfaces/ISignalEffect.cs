namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects.Interfaces;

public interface ISignalEffect {
    int AffectSignal(Span<IQ> signalTheta, int lendth);
}