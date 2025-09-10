using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Streams;
public class IQStream {
    public RadioBand CenterFrequency { get; set; }
    public RadioBand Bandwith { get; set; }

    private readonly RfDeviceStream _rfStream;

    public IQStream(RfDeviceStream rfStream) {
        _rfStream = rfStream;
    }
}
