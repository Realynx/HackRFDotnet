using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

namespace HackRFDotnet.ManagedApi.Streams.Interfaces;
public interface IIQStream {
    int BufferLength { get; }
    RadioBand Frequency { get; }
    SampleRate SampleRate { get; }

    void Close();
    void Dispose();
    void OpenRx(SampleRate? sampleRate = null);
    int ReadBuffer(Span<IQ> iqBuffer);
    void SetSampleRate(SampleRate sampleRate);
    int TxBuffer(Span<IQ> iqFrame);
}