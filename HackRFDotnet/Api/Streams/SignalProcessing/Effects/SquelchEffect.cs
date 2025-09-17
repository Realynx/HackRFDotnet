namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;
/// <summary>
/// Squelch Effect to remove noise when there is no detected signal present.
/// </summary>
public class SquelchEffect : SignalEffect<IQ, IQ> {
    private readonly SampleRate _sampleRate;

    private readonly uint _sampleLength;
    private readonly uint _mod;
    private readonly float[] _previousSamples = [];

    private uint _sampleIndex;


    public SquelchEffect(SampleRate sampleRate) {
        _sampleRate = sampleRate;

        _sampleLength = (uint)sampleRate.Sps * 8u;
        _previousSamples = new float[512];
        _mod = (uint)(_sampleLength / _previousSamples.Length);
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
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


        if (channelNoiseFloor < basebandNoiseFloor * 1.026) {
            for (var x = 0; x < length; x++) {
                signalTheta[x] = IQ.Zero;
            }
        }

        return base.AffectSignal(signalTheta, length);
    }
}
