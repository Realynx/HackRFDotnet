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
}
