using System.Numerics;

using HackRFDotnet.ManagedApi.Types;

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

    public void ApplyFilterOffset(Span<Complex> iqFrame, RadioBand freqOffset) {
        for (var x = 0; x < iqFrame.Length; x++) {
            var theta = x / _sampleRate;
            var phase = -2.0 * Math.PI * freqOffset.Hz * theta;

            var osc = new Complex(Math.Cos(phase), Math.Sin(phase));
            iqFrame[x] *= osc;
        }

        FilterFrame(iqFrame);
    }

    private void FilterFrame(Span<Complex> iqFrame) {
        for (var x = 0; x < iqFrame.Length; x++) {
            var iFiltered = _filterI.ProcessSample(iqFrame[x].Real);
            var qFiltered = _filterQ.ProcessSample(iqFrame[x].Imaginary);
            iqFrame[x] = new Complex(iFiltered, qFiltered);
        }
    }
}
