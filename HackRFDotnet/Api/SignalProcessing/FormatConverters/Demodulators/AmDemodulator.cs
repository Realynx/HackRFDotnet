using System.Runtime.InteropServices;

using HackRFDotnet.Api.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams;

namespace HackRFDotnet.Api.SignalProcessing.FormatConverters.Demodulators;
public class AmDemodulator : SignalEffect<IQ, float> {
    public AmDemodulator() {

    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        var convertedBuffer = MemoryMarshal.Cast<IQ, float>(signalTheta);
        for (var i = 0; i < length; i++) {
            convertedBuffer[i] = signalTheta[i].Magnitude - 1.0f;
        }

        return base.TransformSignal(signalTheta, length);
    }
}
