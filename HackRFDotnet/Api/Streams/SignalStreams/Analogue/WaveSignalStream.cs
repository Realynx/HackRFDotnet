using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;

using NAudio.Wave;

namespace HackRFDotnet.Api.Streams.SignalStreams.Analogue;
/// <summary>
/// NAudio <see cref="ISampleProvider"/> base stream implementation, <see cref="AmSignalStream"/> and <see cref="FmSignalStream"/> stream inherit this.
/// </summary>
public class WaveSignalStream : SignalStream<float>, ISampleProvider, IDisposable {

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
        ReadSpan(buffer.AsSpan(0, count));
        return count;
    }
}
