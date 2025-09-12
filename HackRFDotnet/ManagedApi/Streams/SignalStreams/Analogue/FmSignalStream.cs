using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;
public class FmSignalStream : WaveSignalStream {
    public FmSignalStream(IIQStream deviceStream, bool stero = true, SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true)
        : base(deviceStream, stero, processingPipeline, keepOpen) {
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);
        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var x = 1; x < count; x++) {
                // Conjugate of a Complex number: the conjugate of x+i*y is x-i*y
                var delta = iqBuffer[x] * IQ.Conjugate(iqBuffer[x - 1]);
                buffer[x - 1] = (float)delta.Phase;
            }

            return count;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }
    }
}
