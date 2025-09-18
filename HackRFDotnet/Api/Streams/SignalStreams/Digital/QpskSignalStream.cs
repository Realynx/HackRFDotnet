using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters;

namespace HackRFDotnet.Api.Streams.SignalStreams.Digital;
public class QpskSignalStream : SignalStream<byte> {
    public QpskSignalStream(IIQStream iQStream, Bandwidth signalBandwidth)
        : base(iQStream, BuildFxChain(iQStream, signalBandwidth, out _), false) {

    }

    private static SignalProcessingPipeline<IQ> BuildFxChain(IIQStream deviceStream, Bandwidth stationBandwidth, out SampleRate reducedRate) {
        var signalPipeline = new SignalProcessingPipeline<IQ>();

        signalPipeline
            .WithRootEffect(new IQDownSampleEffect(deviceStream.SampleRate,
                stationBandwidth.NyquistSampleRate, out reducedRate, out var producedChunkSize))

            .AddChildEffect(new FftEffect(true, producedChunkSize))
            .AddChildEffect(new LowPassFilterEffect(reducedRate, stationBandwidth))
            .AddChildEffect(new FftEffect(false, producedChunkSize))
            .AddChildEffect(new QpskDecoder());

        return signalPipeline;
    }

    public int Read(Span<byte> buffer, int count) {
        ReadSpan(buffer.Slice(0, count));
        return count;
    }
}