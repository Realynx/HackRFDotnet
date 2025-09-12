using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;

using FftSharp;

using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.ManagedApi.Utilities;

using MathNet.Filtering.FIR;


namespace HackRFDotnet.ManagedApi.SignalProcessing;
public class FilterProcessor {
    private readonly RadioBand _frequencyOffset;
    private readonly RadioBand _bandwith;
    private readonly double _sampleRate;

    private readonly OnlineFirFilter _filterI_dec;
    private readonly OnlineFirFilter _filterQ_dec;

    public FilterProcessor(double sampleRate, RadioBand frequencyOffset, RadioBand bandwith) {
        _sampleRate = sampleRate;
        _frequencyOffset = frequencyOffset;
        _bandwith = bandwith;

        var cheapFirCoefficients = FirCoefficients.LowPass(
            samplingRate: sampleRate,
            cutoff: bandwith.Hz + 5_000 / 2,
            dcGain: 1.0,
            halforder: 12
        );
        _filterI_dec = new OnlineFirFilter(cheapFirCoefficients);
        _filterQ_dec = new OnlineFirFilter(cheapFirCoefficients);
    }

    public int ApplyPipeline(Span<IQ> iqFrame, out int newSampleRate) {
        return FilterFrame(iqFrame, out newSampleRate);
    }

    public int ApplyPipelineOffset(Span<IQ> iqFrame, RadioBand freqOffset, out int newSampleRate) {
        SignalUtilities.ApplyPhaseOffset(iqFrame, freqOffset, _sampleRate);
        return FilterFrame(iqFrame, out newSampleRate);
    }

    private int FilterFrame(Span<IQ> iqFrame, out int newSampleRate) {
        for (var x = 0; x < iqFrame.Length; x++) {
            var iFiltered = _filterI_dec.ProcessSample(iqFrame[x].I);
            var qFiltered = _filterQ_dec.ProcessSample(iqFrame[x].Q);
            iqFrame[x] = new IQ(iFiltered, qFiltered);
        }

        var decimationFactor = (int)(_sampleRate / _bandwith.Hz);
        newSampleRate = (int)_sampleRate / decimationFactor;
        var decimatedSize = iqFrame.Length / decimationFactor;

        var downsampled = ArrayPool<IQ>.Shared.Rent(decimatedSize);
        try {

            for (var x = 0; x < decimatedSize; x++) {
                downsampled[x] = iqFrame[x * decimationFactor];
            }

            SignalUtilities.IQCorrection(downsampled.AsSpan());
            downsampled.AsSpan(0, decimatedSize).CopyTo(iqFrame);

            FFTLowpass(downsampled);

            return decimatedSize;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(downsampled);
        }
    }

    private void FFTLowpass(Span<IQ> iqFrame) {
        var complexFrame = MemoryMarshal.Cast<IQ, Complex>(iqFrame);

        var frameLength = complexFrame.Length;
        var resolution = FFT.FrequencyResolution(frameLength, _sampleRate);

        FFT.Forward(complexFrame);

        for (var x = 0; x < frameLength; x++) {
            var currentFreq = x * resolution;

            if (currentFreq > _bandwith.Hz + (_bandwith.Hz / 2)) {
                complexFrame[x] = Complex.Zero;
            }
        }

        FFT.Inverse(complexFrame);
    }
}
