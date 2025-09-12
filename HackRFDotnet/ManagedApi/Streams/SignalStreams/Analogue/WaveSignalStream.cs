using System;
using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;
public class WaveSignalStream : SignalStream, ISampleProvider, IDisposable {
    public WaveFormat? WaveFormat { get; protected set; }
    public WaveSignalStream(IIQStream deviceStream, bool stero = true, SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true)
        : base(deviceStream, processingPipeline, keepOpen) {
        var sampleRate = Bandwith.Hz;
        if (stero) {
            sampleRate /= 2;
        }

        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, stero ? 2 : 1);
    }

    public virtual int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);

        NormalizeRms(buffer.AsSpan());

        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < count; i++) {
                buffer[offset + i] = (float)iqBuffer[i].Phase;
            }

            return count;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }
    }

    protected void NormalizeRms(Span<float> buffer, float targetRms = 0.01f) {
        if (buffer == null || buffer.Length == 0) {
            return;
        }

        var sumSq = 0d;
        for (var x = 0; x < buffer.Length; x++) {
            double v = buffer[x];
            sumSq += v * v;
        }

        var rms = Math.Sqrt(sumSq / buffer.Length);
        if (rms <= 0.0) {
            return;
        }

        var gain = (float)(targetRms / rms);

        for (var x = 0; x < buffer.Length; x++) {
            buffer[x] *= gain;
        }
    }



    //protected void RmsPeak(float[] buffer) {
    //    var max = buffer.Max(Math.Abs);
    //    if (max > 0) {
    //        for (var x = 0; x < buffer.Length; x++) {
    //            buffer[x] /= max % 1f;
    //        }
    //    }
    //}
}
