
using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Utilities;

using NAudio.Dsp;

namespace HackRFDotnet.ManagedApi.Streams;
public class ReadOnlyIQFrame : IIQFrame {
    public uint SampleRate { get; set; }
    public ulong CenterFrequency { get; set; }

    protected readonly Complex[] _iqSample;

    public ReadOnlyIQFrame(ReadOnlySpan<byte> interleavedBytes) {
        _iqSample = IQConverter.ConvertIQBytes(interleavedBytes);

    }

}
