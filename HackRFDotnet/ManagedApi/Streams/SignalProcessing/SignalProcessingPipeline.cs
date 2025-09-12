using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing;
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
