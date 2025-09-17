namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;
public class BasicSignalScanningEffect : SignalEffect {
    private readonly DigitalRadioDevice _digitalRadioDevice;
    private readonly Bandwidth _bandwidth;

    private readonly RadioBand[] _scanChannels;
    private int _scanChannelsIndex;

    private DateTime _lastChannelSwicth = DateTime.MinValue;


    public BasicSignalScanningEffect(DigitalRadioDevice digitalRadioDevice, Bandwidth bandwidth, RadioBand[] scanChannels) {
        _digitalRadioDevice = digitalRadioDevice;
        _scanChannels = scanChannels;
        _bandwidth = bandwidth;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        if (length == 0) {
            return 0;
        }

        var basebandNoise = 0d;
        var channelNoise = 0d;
        for (var x = length; x < signalTheta.Length; x++) {
            var i = signalTheta[x].I;
            var q = signalTheta[x].Q;
            basebandNoise += (i * i) + (q * q);
        }
        var basebandNoiseFloor = basebandNoise / signalTheta.Length;


        for (var x = 0; x < length; x++) {
            var i = signalTheta[x].I;
            var q = signalTheta[x].Q;
            channelNoise += (i * i) + (q * q);
        }
        var channelNoiseFloor = channelNoise / length;


        if (DateTime.Now - _lastChannelSwicth > TimeSpan.FromSeconds(.2f)
            && channelNoiseFloor < basebandNoiseFloor * 1.026) {
            _lastChannelSwicth = DateTime.Now;
            _digitalRadioDevice.SetFrequency(_scanChannels[_scanChannelsIndex]);
            _scanChannelsIndex = (_scanChannelsIndex + 1) % _scanChannels.Length;
        }

        return length;
    }
}
