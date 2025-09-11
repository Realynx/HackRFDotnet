using System.Numerics;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class FmSignalStream : SignalStream, ISampleProvider, IDisposable {
    public FmSignalStream(RfDeviceStream deviceStream, bool keepOpen = true) : base(deviceStream, keepOpen) {
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)_rfDeviceStream.SampleRate, 2);
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = new Complex[count];
        ReadSpan(iqBuffer);

        for (var x = 1; x < count; x++) {
            var delta = iqBuffer[x] * Complex.Conjugate(iqBuffer[x - 1]);
            var fmSample = Math.Atan2(delta.Imaginary, delta.Real);
            buffer[x - 1] = (float)fmSample;
        }
        buffer[0] = 0;

        return count;
    }
}
