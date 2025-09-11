using System.Buffers;
using System.Numerics;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class AmSignalStream : SignalStream {
    public AmSignalStream(RfDeviceStream deviceStream, bool keepOpen = true) : base(deviceStream, keepOpen) {
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<Complex>.Shared.Rent(count);
        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < iqBuffer.Length; i++) {
                buffer[i] = (float)iqBuffer[i].Magnitude;
            }

            return count;
        }
        finally {
            ArrayPool<Complex>.Shared.Return(iqBuffer);
        }
    }
}
