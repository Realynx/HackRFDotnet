using System.Numerics;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Utilities;

namespace HackRFDotnet.ManagedApi.Streams;
public class IQFrame : IIQFrame {
    public uint SampleRate { get; set; }
    public ulong CenterFrequency { get; set; }

    protected readonly Complex[] _iqSample;

    public IQFrame(ReadOnlySpan<byte> interleavedBytes) {
        _iqSample = IQConverter.ConvertIQBytes(interleavedBytes);
    }

    public ReadOnlySpan<Complex> GetIqSamples() {
        return _iqSample.AsSpan();
    }

    public float CalculateDb() {
        // Compute RMS magnitude
        double power = 0f;
        for (var x = 0; x < _iqSample.Length; x++) {
            var s = _iqSample[x];
            power += (s.Real * s.Real) + (s.Imaginary * s.Imaginary); // sum |x|^2
        }
        power /= _iqSample.Length;

        // Convert to dB
        var dbAverage = 10f * (float)Math.Log10(power + 1e-12f);
        return dbAverage;
    }
}
