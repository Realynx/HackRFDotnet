using HackRFDotnet.Api.Streams;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Interfaces;

public interface ISignalEffect {
    int AffectSignal(Span<IQ> signalTheta, int lendth);
}