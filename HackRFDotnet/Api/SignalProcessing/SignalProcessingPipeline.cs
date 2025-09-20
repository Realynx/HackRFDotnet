using HackRFDotnet.Api.SignalProcessing.Effects;
using HackRFDotnet.Api.SignalProcessing.Interfaces;

namespace HackRFDotnet.Api.SignalProcessing;
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
