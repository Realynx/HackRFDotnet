namespace HackRFDotnet.Api.Streams.Interfaces;
public interface IIQStream {
    int BufferLength { get; }
    Frequency Frequency { get; }
    SampleRate SampleRate { get; }

    void Close();
    void Dispose();
    void OpenRx(SampleRate? sampleRate = null);
    int ReadBuffer(Span<IQ> iqBuffer);
    void SetSampleRate(SampleRate sampleRate);
    int TxBuffer(Span<IQ> iqFrame);
}