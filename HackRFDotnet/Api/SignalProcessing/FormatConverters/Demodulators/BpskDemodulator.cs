using HackRFDotnet.Api.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams;

namespace HackRFDotnet.Api.SignalProcessing.FormatConverters.Demodulators;
public class BpskDemodulator : SignalEffect<IQ, byte> {
    public BpskDemodulator() {

    }
    public override int TransformSignal(Span<IQ> signalTheta, int length) {

        return base.TransformSignal(signalTheta, length);
    }
}
