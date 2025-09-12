using System.Buffers;

using HackRFDotnet.ManagedApi.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.Interfaces;


namespace HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;
public class AmSignalStream : WaveSignalStream {
    public AmSignalStream(IIQStream deviceStream, bool stero = true, bool keepOpen = true) : base(deviceStream, stero, keepOpen) {
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);
        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < iqBuffer.Length; i++) {
                buffer[i] = (float)iqBuffer[i].Magnitude;
            }

            return count;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }
    }
}
