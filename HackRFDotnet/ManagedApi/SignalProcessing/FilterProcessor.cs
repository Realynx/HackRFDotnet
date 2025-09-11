using System.Numerics;

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

    public void ApplyFilter(Span<Complex> iqFrame) {
        FilterFrame(iqFrame);
    }

    public void ApplyPhaseOffset(Span<Complex> iqFrame, RadioBand freqOffset) {
        SignalUtilities.ApplyPhaseOffset(iqFrame, freqOffset, _sampleRate);
        FilterFrame(iqFrame);
    }

    private void FilterFrame(Span<Complex> iqFrame) {


        NAudio.Dsp.FastFourierTransform.FFT(true, 200, );

        for (var x = 0; x < iqFrame.Length; x++) {
            var iFiltered = _filterI.ProcessSample(iqFrame[x].Real);
            var qFiltered = _filterQ.ProcessSample(iqFrame[x].Imaginary);
            iqFrame[x] = new Complex(iFiltered, qFiltered);
        }
    }
}
