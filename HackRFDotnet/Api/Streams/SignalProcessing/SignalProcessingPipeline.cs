using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing;
/// <summary>
/// Effects chain processor.
/// </summary>
public class SignalProcessingPipeline {
    private readonly SignalEffect[] _signalFxPipe;

    public SignalProcessingPipeline(SignalEffect[] signalFxPipe) {
        _signalFxPipe = signalFxPipe;
    }

    public int ApplyPipeline(Span<IQ> signalTheta) {

        var sampleLength = signalTheta.Length;
        foreach (var signalEffect in _signalFxPipe) {
            sampleLength = signalEffect.AffectSignal(signalTheta, sampleLength);
        }

        return sampleLength;
    }
}
