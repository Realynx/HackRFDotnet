using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters;

namespace HackRFDotnet.Api.Streams.SignalStreams.Analogue;
/// <summary>
/// Demodulate FM audio from <see cref="IIQStream"/>.
/// </summary>
public class FmSignalStream : WaveSignalStream {
    public FmSignalStream(IIQStream deviceStream, Bandwidth stationBandwidth, bool stereo = true)
        : base(deviceStream, BuildFxChain(deviceStream, stationBandwidth, out var sampleRate), sampleRate, stereo, false) {
    }

    private static SignalProcessingPipeline<IQ> BuildFxChain(IIQStream deviceStream, Bandwidth stationBandwidth, out SampleRate reducedRate) {
        var signalPipeline = new SignalProcessingPipeline<IQ>();

        signalPipeline
            .WithRootEffect(new IQDownSampleEffect(deviceStream.SampleRate,
                stationBandwidth.NyquistSampleRate, out reducedRate, out var producedChunkSize))

            .AddChildEffect(new FftEffect(true, producedChunkSize))
            .AddChildEffect(new LowPassFilterEffect(reducedRate, stationBandwidth))
            .AddChildEffect(new FftEffect(false, producedChunkSize))
            .AddChildEffect(new FmDecoder());

        return signalPipeline;
    }
}
