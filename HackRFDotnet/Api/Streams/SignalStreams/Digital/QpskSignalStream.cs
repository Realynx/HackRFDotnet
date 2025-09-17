using System.Buffers;

using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;

namespace HackRFDotnet.Api.Streams.SignalStreams.Digital;
public class QpskSignalStream : SignalStream {
    public QpskSignalStream(IIQStream iQStream, SignalProcessingPipeline<IQ>? processingPipeline = null, bool keepOpen = true)
        : base(iQStream, processingPipeline, keepOpen) {

    }

    public int Read(Span<byte> buffer, int count) {
        // we need 4 IQ symbols to make 1 output byte (since 4 * 2 bits = 8 bits)
        var symbolsNeeded = count * 4;

        var iqBuffer = ArrayPool<IQ>.Shared.Rent(symbolsNeeded);

        try {
            ReadSpan(iqBuffer.AsSpan(0, symbolsNeeded));

            var bitIndex = 0;
            byte currentByte = 0;
            var outIndex = 0;

            for (var x = 0; x < symbolsNeeded; x++) {
                var I = iqBuffer[x].I;
                var Q = iqBuffer[x].Q;

                // QPSK hard decision (Gray coded):
                // (+,+) -> 00
                // (-,+) -> 01
                // (-,-) -> 11
                // (+,-) -> 10

                var bit0 = I < 0 ? 1 : 0; // first bit from I sign
                var bit1 = Q < 0 ? 1 : 0; // second bit from Q sign

                // pack into current byte (MSB first)
                currentByte = (byte)((currentByte << 1) | bit0);
                bitIndex++;
                currentByte = (byte)((currentByte << 1) | bit1);
                bitIndex++;

                if (bitIndex == 8) {
                    buffer[outIndex++] = currentByte;
                    currentByte = 0;
                    bitIndex = 0;
                }
            }

            return count;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }
    }
}
