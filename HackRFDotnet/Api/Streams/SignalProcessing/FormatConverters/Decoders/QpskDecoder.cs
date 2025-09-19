using System.Runtime.InteropServices;

using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters.Decoders;
public class QpskDecoder : SignalEffect<IQ, byte> {
    public QpskDecoder() {

    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        var convertionBuffer = MemoryMarshal.Cast<IQ, byte>(signalTheta);

        var bitIndex = 0;
        byte currentByte = 0;
        var outIndex = 0;

        for (var x = 0; x < length; x++) {
            var I = signalTheta[x].I;
            var Q = signalTheta[x].Q;

            // QPSK hard decision (Gray coded):
            // (+,+) -> 00
            // (-,+) -> 01
            // (-,-) -> 11
            // (+,-) -> 10

            var bit0 = I < 0 ? 1 : 0; // first bit from I sign
            var bit1 = Q < 0 ? 1 : 0; // second bit from Q sign

            // pack into current byte (MSB first)
            currentByte = (byte)(currentByte << 1 | bit0);
            bitIndex++;
            currentByte = (byte)(currentByte << 1 | bit1);
            bitIndex++;

            if (bitIndex == 8) {
                convertionBuffer[outIndex++] = currentByte;
                currentByte = 0;
                bitIndex = 0;
            }
        }

        return base.TransformSignal(signalTheta, outIndex);
    }
}
