using HackRFDotnet.Api.Streams;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Interfaces;

public interface ISignalEffect {
    int AffectSignal(Span<IQ> signalTheta, int lendth);
}