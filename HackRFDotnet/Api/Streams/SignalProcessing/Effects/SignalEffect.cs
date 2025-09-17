using HackRFDotnet.Api.Streams.SignalProcessing.Interfaces;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;
/// <summary>
/// Signal effect base class.
/// </summary>
public abstract class SignalEffect : ISignalEffect {
    public abstract int AffectSignal(Span<IQ> signalTheta, int length);
}
