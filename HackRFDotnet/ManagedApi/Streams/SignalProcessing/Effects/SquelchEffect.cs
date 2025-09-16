namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class SquelchEffect : SignalEffect {
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
        //if (length == 0) {
        //    return 0;
        //}

        //var db = SignalUtilities.CalculateSignalDb(signalTheta.Slice(0, length));
        //var averageOverTime = _previousSamples.Average();

        //if (db <= averageOverTime) {
        //    for (var x = 0; x < length; x++) {
        //        signalTheta[x] = IQ.Zero;
        //    }
        //}

        //_previousSamples[_sampleIndex] = db;
        //_sampleIndex = (_sampleIndex + 1) % _mod;
        //_sampleIndex = _sampleIndex >= _previousSamples.Length ? (uint)_previousSamples.Length - 1 : _sampleIndex;

        return length;
    }
}
