using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class PhaseCenteringEffect : SignalEffect, ISignalEffect {
    private readonly RadioBand _frequencyOffset;

    public PhaseCenteringEffect(RadioBand frequencyOffset) {
        _frequencyOffset = frequencyOffset;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int lendth) {
        return signalTheta.Length;
    }
}
