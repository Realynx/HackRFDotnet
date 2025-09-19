using System.Runtime.InteropServices;

using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters.Decoders;
public class AmDecoder : SignalEffect<IQ, float> {
    public AmDecoder() {

    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        var convertedBuffer = MemoryMarshal.Cast<IQ, float>(signalTheta);
        for (var i = 0; i < length; i++) {
            convertedBuffer[i] = signalTheta[i].Magnitude - 1.0f;
        }

        return base.TransformSignal(signalTheta, length);
    }
}
