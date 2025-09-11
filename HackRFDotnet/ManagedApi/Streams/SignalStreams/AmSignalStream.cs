using System.Numerics;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class AmSignalStream : SignalStream, ISampleProvider, IDisposable {
    public AmSignalStream(RfDeviceStream deviceStream, bool keepOpen = true) : base(deviceStream, keepOpen) {
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = new Complex[count];
        ReadSpan(iqBuffer);

        for (var i = 0; i < iqBuffer.Length; i++) {
            var s = iqBuffer[i];
            buffer[i] = (float)Math.Sqrt(s.Real * s.Real + s.Imaginary * s.Imaginary);
        }

        return buffer.Length;
    }
}
