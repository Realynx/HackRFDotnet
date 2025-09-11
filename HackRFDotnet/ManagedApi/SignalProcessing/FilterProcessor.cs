

using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.ManagedApi.Utilities;

using MathNet.Filtering.FIR;

namespace HackRFDotnet.ManagedApi.SignalProcessing;
public class FilterProcessor {
    private readonly RadioBand _frequencyOffset;
    private readonly RadioBand _bandwith;
    private readonly double _sampleRate;


    private readonly OnlineFirFilter _filterI;
    private readonly OnlineFirFilter _filterQ;

    public FilterProcessor(double sampleRate, RadioBand frequencyOffset, RadioBand bandwith) {
        _sampleRate = sampleRate;
        _frequencyOffset = frequencyOffset;
        _bandwith = bandwith;

        var halfOrder = 100;
        var lowPassFirCoefficients = FirCoefficients.LowPass(
            samplingRate: sampleRate,
            cutoff: bandwith.Hz / 2,
            dcGain: 2.0,
            halforder: halfOrder
        );

        _filterI = new OnlineFirFilter(lowPassFirCoefficients);
        _filterQ = new OnlineFirFilter(lowPassFirCoefficients);
    }

    public void ApplyFilter(Span<IQ> iqFrame) {
        FilterFrame(iqFrame);
    }

    public void ApplyPhaseOffset(Span<IQ> iqFrame, RadioBand freqOffset) {
        SignalUtilities.ApplyPhaseOffset(iqFrame, freqOffset, _sampleRate);
        FilterFrame(iqFrame);
    }

    private void FilterFrame(Span<IQ> iqFrame) {
        // SignalUtilities.FFT(true, 256, iqFrame);

        for (var x = 0; x < iqFrame.Length; x++) {
            var iFiltered = _filterI.ProcessSample(iqFrame[x].I);
            var qFiltered = _filterQ.ProcessSample(iqFrame[x].Q);
            iqFrame[x] = new IQ(iFiltered, qFiltered);
        }
    }
}
