using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters;
public class BpskDecoder : SignalEffect<IQ, byte> {
    public BpskDecoder() {

    }
    public override int AffectSignal(Span<IQ> signalTheta, int length) {

        return base.AffectSignal(signalTheta, length);
    }
}
