using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalProcessing.Interfaces;

namespace HackRFDotnet.Api.Streams.SignalProcessing;
/// <summary>
/// Effects chain processor.
/// </summary>
public class SignalProcessingPipeline<TInput> where TInput : struct {
    private ISignalEffectInput<TInput> _rootEffect;

    public SignalEffect<TInput, TOutput> WithRootEffect<TOutput>(SignalEffect<TInput, TOutput> rootEffect) where TOutput : struct {
        _rootEffect = rootEffect;
        return rootEffect;
    }

    public int ApplyPipeline(Span<TInput> signalTheta) {
        var affectedLength = _rootEffect.TransformSignal(signalTheta, signalTheta.Length);

        return affectedLength;
    }
}
