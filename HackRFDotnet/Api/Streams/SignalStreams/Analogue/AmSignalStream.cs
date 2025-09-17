using System.Buffers;

using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;


namespace HackRFDotnet.Api.Streams.SignalStreams.Analogue;
/// <summary>
/// Demodulate AM audio from the <see cref="IIQStream"/>.
/// </summary>
public class AmSignalStream : WaveSignalStream {
    public AmSignalStream(IIQStream deviceStream, SampleRate sampleRate,
        SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true)
        : base(deviceStream, sampleRate, false, processingPipeline, keepOpen) {
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);
        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < count; i++) {
                buffer[i] = iqBuffer[i].Magnitude - 1.0f;
            }

            NormalizeRms(buffer);
            return count;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }
    }
}
