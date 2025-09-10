namespace HackRFDotnet.ManagedApi.Streams.Interfaces;
public interface IIQFrame {
    public uint SampleRate { get; set; }
    public ulong CenterFrequency { get; set; }
}
