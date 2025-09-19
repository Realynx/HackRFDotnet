using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters.Demodulators;


namespace HackRFDotnet.Api.Streams.SignalStreams.Analogue;
/// <summary>
/// Demodulate AM audio from the <see cref="IIQStream"/>.
/// </summary>
public class AmSignalStream : WaveSignalStream {
    public AmSignalStream(IIQStream deviceStream, Bandwidth stationBandwidth, bool keepOpen = true)
        : base(deviceStream, BuildFxChain(deviceStream, stationBandwidth, out var sampleRate), sampleRate, false, keepOpen) {
    }

    private static SignalProcessingPipeline<IQ> BuildFxChain(IIQStream deviceStream, Bandwidth stationBandwidth, out SampleRate reducedRate) {
        var signalPipeline = new SignalProcessingPipeline<IQ>();

        signalPipeline
            .WithRootEffect(new IQDownSampleEffect(deviceStream.SampleRate,
                stationBandwidth.NyquistSampleRate, out reducedRate, out var producedChunkSize))

            //.AddSignalEffect(new BasicSignalScanningEffect(rfDevice, Bandwidth.FromKHz(10), [Frequency.FromMHz(118.4f),
            //    Frequency.FromMHz(118.575f), Frequency.FromMHz(119.250f), Frequency.FromMHz(119.450f),
            //    Frequency.FromMHz(121.800f),Frequency.FromMHz(124.05f), Frequency.FromMHz(125.150f), Frequency.FromMHz(135f)]))
            .AddChildEffect(new SquelchEffect(reducedRate))
            .AddChildEffect(new FftEffect(true, producedChunkSize))
            .AddChildEffect(new LowPassFilterEffect(reducedRate, stationBandwidth))
            .AddChildEffect(new FftEffect(false, producedChunkSize))
            .AddChildEffect(new AmDemodulator());

        return signalPipeline;
    }
}
