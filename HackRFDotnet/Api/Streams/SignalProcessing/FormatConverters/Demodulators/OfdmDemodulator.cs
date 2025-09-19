using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters.Demodulators;

public class OfdmDemodulator : SignalEffect<IQ, byte> {

    public OfdmDemodulator() {

    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        var outputIndex = 0;

        return base.TransformSignal(signalTheta, outputIndex);
    }
}
