using System.Runtime.InteropServices;

using HackRFDotnet.Api.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams;
using HackRFDotnet.Api.Utilities;

namespace HackRFDotnet.Api.SignalProcessing.FormatConverters.Demodulators;
public class FmDemodulator : SignalEffect<IQ, float> {
    public FmDemodulator() {

    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        var convertedBuffer = MemoryMarshal.Cast<IQ, float>(signalTheta);

        for (var x = 1; x < length; x++) {
            var delta = signalTheta[x] * IQ.Conjugate(signalTheta[x - 1]);
            convertedBuffer[x - 1] = delta.Phase;
        }
        convertedBuffer[length - 1] = convertedBuffer[length - 2];

        SignalUtilities.NormalizeRms(convertedBuffer.Slice(0, length));
        return base.TransformSignal(signalTheta, length);
    }
}
