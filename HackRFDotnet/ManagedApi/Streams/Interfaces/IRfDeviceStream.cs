using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Streams.Interfaces;
public interface IRfDeviceStream {
    int BufferLength { get; }
    RadioBand Frequency { get; }
    double SampleRate { get; }

    void Close();
    void Dispose();
    void Open(double sampleRate);
    int ReadBuffer(Span<IQ> iqBuffer);
    void SetSampleRate(double sampleRate);
    int TxBuffer(Span<IQ> iqFrame);
}