using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Utilities;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;
public class WaveSignalStream : SignalStream, ISampleProvider, IDisposable {
    private readonly FftEffect _fft;
    private readonly FftEffect _fftinverse;

    public WaveFormat? WaveFormat { get; protected set; }
    public WaveSignalStream(IIQStream deviceStream, bool stero = true, SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true)
        : base(deviceStream, processingPipeline, keepOpen) {
        var sampleRate = Bandwith.Hz;
        if (stero) {
            sampleRate /= 2;
        }

        _fft = new FftEffect(true);
        _fftinverse = new FftEffect(false);
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, stero ? 2 : 1);
    }

    public virtual int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);

        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < count; i++) {
                buffer[offset + i] = (float)iqBuffer[i].Phase;
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

    protected void RemoveNoiseFloor(Span<IQ> buffer) {

        _fft.AffectSignal(buffer, buffer.Length);

        var magnitudes = new float[buffer.Length];
        for (var x = 0; x < buffer.Length; x++) {
            magnitudes[x] = (float)buffer[x].Magnitude;
        }

        Array.Sort(magnitudes);
        var index = (int)(magnitudes.Length * .2f);
        var noiseFloor = magnitudes[index - 1];

        for (var x = 0; x < buffer.Length; x++) {
            if (buffer[x].Magnitude <= noiseFloor) {
                buffer[x] = IQ.Zero;
            }
        }

        _fftinverse.AffectSignal(buffer, buffer.Length);
    }

    protected void Squelch(Span<IQ> buffer) {
        var averageDb = SignalUtilities.CalculateDb(buffer);
        if (averageDb < 2.5) {
            buffer.Fill(IQ.Zero);
        }
    }

}
