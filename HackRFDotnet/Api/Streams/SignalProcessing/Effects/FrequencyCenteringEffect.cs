using HackRFDotnet.Api.Utilities;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;
/// <summary>
/// Shift the frequency by a <see cref="Frequency"/> offset.
/// This only works for IQ samples, we can shift frequency without losing information.
/// </summary>
public class FrequencyCenteringEffect : SignalEffect<IQ, IQ> {
    private readonly Frequency _frequencyOffset;
    private readonly SampleRate _sampleRate;

    public FrequencyCenteringEffect(Frequency frequencyOffset, SampleRate sampleRate) {
        _frequencyOffset = frequencyOffset;
        _sampleRate = sampleRate;
    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        SignalUtilities.ApplyFrequencyOffset(signalTheta, _frequencyOffset, _sampleRate);

        return base.TransformSignal(signalTheta, length);
    }
}
