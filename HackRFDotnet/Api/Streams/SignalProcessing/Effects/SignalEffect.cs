using System.Runtime.InteropServices;

using HackRFDotnet.Api.Streams.SignalProcessing.Interfaces;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;
/// <summary>
/// Signal effect base class.
/// </summary>
public abstract class SignalEffect<TInput, TOutput> : ISignalEffectInput<TInput>
    where TInput : struct
    where TOutput : struct {

    private ISignalEffectInput<TOutput>? _child;

    public virtual int TransformSignal(Span<TInput> signalTheta, int length) {
        if (_child is null) {
            return length;
        }

        var castedSignalSpan = MemoryMarshal.Cast<TInput, TOutput>(signalTheta);
        return _child.TransformSignal(castedSignalSpan, length);
    }

    public SignalEffect<TOutput, TFormat> AddChildEffect<TFormat>(SignalEffect<TOutput, TFormat> childEffect) where TFormat : struct {
        _child = childEffect;

        return childEffect;
    }
}