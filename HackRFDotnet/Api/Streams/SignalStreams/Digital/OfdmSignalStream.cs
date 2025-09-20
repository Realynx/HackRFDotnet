using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters;

namespace HackRFDotnet.Api.Streams.SignalStreams.Digital;
public class OfdmSignalStream : SignalStream<byte> {
    public OfdmSignalStream(IIQStream iQStream, Bandwidth signalBandwidth)
        : base(iQStream, BuildFxChain(iQStream, signalBandwidth, out _), false) {
    }

    private static SignalProcessingPipeline<IQ> BuildFxChain(IIQStream deviceStream, Bandwidth stationBandwidth, out SampleRate reducedRate) {
        var signalPipeline = new SignalProcessingPipeline<IQ>();

        signalPipeline
            .WithRootEffect(new IQDownSampleEffect(deviceStream.SampleRate,
                stationBandwidth.NyquistSampleRate, out reducedRate, out var producedChunkSize))

            .AddChildEffect(new OfdmChannelizer());

        return signalPipeline;
    }

    public int Read(Span<byte> buffer, int count) {
        ReadSpan(buffer.Slice(0, count));
        return count;
    }
}
