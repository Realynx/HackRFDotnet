using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;
using HackRFDotnet.ManagedApi.Utilities;

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
            cutoff: reducedSampleRate.Hz + (RadioBand.FromKHz(2).Hz / 2),
            dcGain: 1.0,
            halforder: halforder
        );

        _filterI = new OnlineFirFilter(cheapFirCoefficients);
        _filterQ = new OnlineFirFilter(cheapFirCoefficients);
    }

    public override int AffectSignal(Span<IQ> signalTheta, int lendth) {
        for (var x = 0; x < lendth; x++) {
            var iFiltered = _filterI.ProcessSample(signalTheta[x].I);
            var qFiltered = _filterQ.ProcessSample(signalTheta[x].Q);
            signalTheta[x] = new IQ(iFiltered, qFiltered);
        }

        var decimationFactor = (int)(_iqSampleRate / _reducedSampledRate.Hz);
        var decimatedSize = lendth / decimationFactor;

        var downsampled = ArrayPool<IQ>.Shared.Rent(decimatedSize);
        try {

            for (var x = 0; x < decimatedSize; x++) {
                downsampled[x] = signalTheta[x * decimationFactor];
            }

            // SignalUtilities.IQCorrection(downsampled.AsSpan());
            downsampled.AsSpan(0, decimatedSize).CopyTo(signalTheta);
        }
        finally {
            ArrayPool<IQ>.Shared.Return(downsampled);
        }

        return decimatedSize;
    }
}