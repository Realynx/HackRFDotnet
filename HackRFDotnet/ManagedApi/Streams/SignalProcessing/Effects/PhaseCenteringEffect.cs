using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;
using HackRFDotnet.ManagedApi.Utilities;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class PhaseCenteringEffect : SignalEffect, ISignalEffect {
    private readonly RadioBand _frequencyOffset;
    private readonly double _sampleRate;

    public PhaseCenteringEffect(RadioBand frequencyOffset, double sampleRate) {
        _frequencyOffset = frequencyOffset;
        _sampleRate = sampleRate;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        SignalUtilities.ApplyPhaseOffset(signalTheta, _frequencyOffset, _sampleRate);
        return signalTheta.Length;
    }
}
