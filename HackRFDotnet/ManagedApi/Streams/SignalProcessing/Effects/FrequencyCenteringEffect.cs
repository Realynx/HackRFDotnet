using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;
using HackRFDotnet.ManagedApi.Utilities;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class FrequencyCenteringEffect : SignalEffect, ISignalEffect {
    private readonly RadioBand _frequencyOffset;
    private readonly SampleRate _sampleRate;

    public FrequencyCenteringEffect(RadioBand frequencyOffset, SampleRate sampleRate) {
        _frequencyOffset = frequencyOffset;
        _sampleRate = sampleRate;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        SignalUtilities.ApplyFrequencyOffset(signalTheta, _frequencyOffset, _sampleRate);
        return signalTheta.Length;
    }
}
