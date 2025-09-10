using System.Numerics;

using HackRFDotnet.ManagedApi.Utilities;

namespace HackRFDotnet.ManagedApi.Types;
public class IQFrame {
    public uint SampleRate { get; set; }
    public ulong CenterFrequency { get; set; }

    private readonly Complex[] _iqSample;

    public IQFrame(ReadOnlySpan<byte> interleavedBytes) {
        _iqSample = IQConverter.ConvertIQBytes(interleavedBytes);
    }
}
