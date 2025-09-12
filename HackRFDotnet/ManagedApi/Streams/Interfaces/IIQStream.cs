using HackRFDotnet.ManagedApi.SignalProcessing;

namespace HackRFDotnet.ManagedApi.Streams.Interfaces;
public interface IIQStream {
    int BufferLength { get; }
    RadioBand Frequency { get; }
    double SampleRate { get; }

    void Close();
    void Dispose();
    void OpenRx(double? sampleRate = null);
    int ReadBuffer(Span<IQ> iqBuffer);
    void SetSampleRate(double sampleRate);
    int TxBuffer(Span<IQ> iqFrame);
}