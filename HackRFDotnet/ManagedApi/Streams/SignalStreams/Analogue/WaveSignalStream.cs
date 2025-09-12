using System.Buffers;

using HackRFDotnet.ManagedApi.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.Interfaces;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;
public class WaveSignalStream : SignalStream, ISampleProvider, IDisposable {
    public WaveFormat? WaveFormat { get; protected set; }
    public WaveSignalStream(IIQStream deviceStream, bool stero = true, bool keepOpen = true) : base(deviceStream, keepOpen) {
        var sampleRate = Bandwith.Hz;
        if (stero) {
            sampleRate /= 2;
        }

        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, stero ? 2 : 1);
    }

    protected void ReadSpan(Span<IQ> iqPairs) {
        lock (_filteredBuffer) {
            var readBytes = _filteredBuffer.Read(iqPairs);
        }
    }

    public virtual int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);

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
}
