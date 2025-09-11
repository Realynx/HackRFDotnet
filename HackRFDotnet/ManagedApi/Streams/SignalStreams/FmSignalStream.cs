using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class FmSignalStream : SignalStream {
    public FmSignalStream(IRfDeviceStream deviceStream, bool keepOpen = true) : base(deviceStream, keepOpen) {
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)_rfDeviceStream.SampleRate, 2);
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
