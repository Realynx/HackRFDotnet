using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing;
public class SignalProcessingBuilder {
    private readonly Queue<SignalEffect> _signalEffects = new();
    public SignalProcessingBuilder() {

    }

    public SignalProcessingBuilder AddSignalEffect(SignalEffect signalEffect) {
        _signalEffects.Enqueue(signalEffect);
        return this;
    }

    public SignalProcessingPipeline BuildPipeline() {
        return new SignalProcessingPipeline(_signalEffects.ToArray());
    }
}
