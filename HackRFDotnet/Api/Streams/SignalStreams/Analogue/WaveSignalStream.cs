using System.Buffers;

using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;

using NAudio.Wave;

namespace HackRFDotnet.Api.Streams.SignalStreams.Analogue;
/// <summary>
/// NAudio <see cref="ISampleProvider"/> base stream implementation, <see cref="AmSignalStream"/> and <see cref="FmSignalStream"/> stream inherit this.
/// </summary>
public class WaveSignalStream : SignalStream, ISampleProvider, IDisposable {

    public WaveFormat? WaveFormat { get; protected set; }

    public WaveSignalStream(IIQStream deviceStream, SignalProcessingPipeline<IQ> processingPipeline, SampleRate sampleRate, bool stero = true, bool keepOpen = true)
        : base(deviceStream, processingPipeline, keepOpen) {

        var rate = sampleRate.Sps;
        if (stero) {
            rate /= 2;
        }

        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)rate, stero ? 2 : 1);
    }

    public virtual int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);

        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < count; i++) {
                buffer[offset + i] = iqBuffer[i].Phase;
            }

            NormalizeRms(buffer.AsSpan());
            return count;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }
    }

    protected void NormalizeRms(Span<float> buffer, float targetRms = 0.04f) {
        if (buffer == null || buffer.Length == 0) {
            return;
        }

        var sumSq = 0f;
        for (var x = 0; x < buffer.Length; x++) {
            var v = buffer[x];
            sumSq += v * v;
        }

        var rms = MathF.Sqrt(sumSq / buffer.Length);
        if (rms <= 0.0) {
            return;
        }

        var gain = (float)(targetRms / rms);

        for (var x = 0; x < buffer.Length; x++) {
            buffer[x] *= gain;
        }
    }
}
