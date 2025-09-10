using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Streams;
public class IQStream {
    public RadioBand CenterFrequency { get; set; }
    public RadioBand Bandwith { get; set; }

    private readonly RfStream _rfStream;

    public IQStream(RfStream rfStream) {
        _rfStream = rfStream;
    }
}
