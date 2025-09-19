using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters.Decoders;
public class OfdmDecoder : SignalEffect<IQ, byte> {
    public OfdmDecoder() {

    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {

        return base.TransformSignal(signalTheta, length);
    }
}
