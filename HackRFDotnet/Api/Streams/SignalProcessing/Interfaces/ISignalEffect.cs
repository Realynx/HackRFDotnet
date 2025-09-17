namespace HackRFDotnet.Api.Streams.SignalProcessing.Interfaces;

/// <summary>
/// Signal Effects are fundamental building blocks for a <see cref="SignalProcessingPipeline"/>.
/// </summary>
public interface ISignalEffect {
    /// <summary>
    /// Manipulate in-place, the signal provided and return a new length is samples were reduced.
    /// </summary>
    /// <param name="signalTheta"></param>
    /// <param name="lendth"></param>
    /// <returns></returns>
    int AffectSignal(Span<IQ> signalTheta, int lendth);
}