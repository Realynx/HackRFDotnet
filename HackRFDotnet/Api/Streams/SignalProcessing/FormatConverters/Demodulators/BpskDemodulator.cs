using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters.Demodulators;
public class BpskDemodulator : SignalEffect<IQ, byte> {
    public BpskDemodulator() {

    }
    public override int TransformSignal(Span<IQ> signalTheta, int length) {

        return base.TransformSignal(signalTheta, length);
    }
}
