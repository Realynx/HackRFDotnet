using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;

using MathNet.Filtering.FIR;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class ReducerEffect : SignalEffect, ISignalEffect {
    private readonly double _iqSampleRate;
    private readonly RadioBand _reducedSampledRate;

    private readonly OnlineFirFilter _filterI;
    private readonly OnlineFirFilter _filterQ;

    public ReducerEffect(double sampleRate, RadioBand reducedSampleRate, out int newSampleRate, int halforder = 12) {
        _iqSampleRate = sampleRate;
        _reducedSampledRate = reducedSampleRate;
        newSampleRate = reducedSampleRate.Hz;

        var cheapFirCoefficients = FirCoefficients.LowPass(
            samplingRate: sampleRate,
            cutoff: (reducedSampleRate.Hz + RadioBand.FromKHz(2).Hz) / 2,
            dcGain: 1.0,
            halforder: halforder
        );

        _filterI = new OnlineFirFilter(cheapFirCoefficients);
        _filterQ = new OnlineFirFilter(cheapFirCoefficients);
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        for (var x = 0; x < length; x++) {
            var iFiltered = _filterI.ProcessSample(signalTheta[x].I);
            var qFiltered = _filterQ.ProcessSample(signalTheta[x].Q);
            signalTheta[x] = new IQ(iFiltered, qFiltered);
        }

        var decimationFactor = (int)(_iqSampleRate / _reducedSampledRate.Hz);
        var decimatedSize = length / decimationFactor;

        for (var x = 0; x < decimatedSize; x++) {
            signalTheta[x] = signalTheta[x * decimationFactor];
        }

        return decimatedSize;
    }
}