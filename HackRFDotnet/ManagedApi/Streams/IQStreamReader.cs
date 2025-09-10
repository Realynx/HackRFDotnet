using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Streams;
public class IQStreamReader : IDisposable {
    private readonly RfStream _iQStream;
    private readonly bool _keepOpen;

    private RadioBand _center;
    private RadioBand _bandwith;


    public IQStreamReader(RfStream iQStream, bool keepOpen = true) {
        _iQStream = iQStream;
        _keepOpen = keepOpen;
    }

    public void SetCenterFrequency(RadioBand center) {
        _center = center;
    }

    public void SetBandwith(RadioBand bandwith) {
        _bandwith = bandwith;
    }

    public void Dispose() {
        if (!_keepOpen) {
            _iQStream.Dispose();
        }
    }
}
