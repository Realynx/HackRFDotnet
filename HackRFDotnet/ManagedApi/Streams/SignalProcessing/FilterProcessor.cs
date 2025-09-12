using HackRFDotnet.ManagedApi.Utilities;


namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing;
public class FilterProcessor {
    private readonly RadioBand _frequencyOffset;
    private readonly RadioBand _bandwith;
    private readonly double _sampleRate;

    public FilterProcessor(double sampleRate, RadioBand frequencyOffset, RadioBand bandwith) {
        _sampleRate = sampleRate;
        _frequencyOffset = frequencyOffset;
        _bandwith = bandwith;
    }

    public int ApplyPipeline(Span<IQ> iqFrame, out int newSampleRate) {
        return FilterFrame(iqFrame, out newSampleRate);
    }

    public int ApplyPipelineOffset(Span<IQ> iqFrame, RadioBand freqOffset, out int newSampleRate) {
        SignalUtilities.ApplyPhaseOffset(iqFrame, freqOffset, _sampleRate);
        return FilterFrame(iqFrame, out newSampleRate);
    }

    private int FilterFrame(Span<IQ> iqFrame, out int newSampleRate) {
        newSampleRate = 0;
        return 0;
    }
}
