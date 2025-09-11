using System.Buffers;
using System.Numerics;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class AmSignalStream : SignalStream {
    public AmSignalStream(RfDeviceStream deviceStream, bool keepOpen = true) : base(deviceStream, keepOpen) {
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<Complex>.Shared.Rent(count);
        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < iqBuffer.Length; i++) {
                var s = iqBuffer[i];
                buffer[i] = (float)Math.Sqrt(s.Real * s.Real + s.Imaginary * s.Imaginary);
            }

            return count;
        }
        finally {
            ArrayPool<Complex>.Shared.Return(iqBuffer);
        }
    }
}
