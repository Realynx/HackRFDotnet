namespace HackRFDotnet.Api.Streams.SignalProcessing.Interfaces;

public interface ISignalEffectInput<TInput> {
    /// <summary>
    /// Manipulate in-place, the signal provided and return a new length is samples were reduced.
    /// </summary>
    /// <param name="signalTheta"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    int AffectSignal(Span<TInput> signalTheta, int length);
}