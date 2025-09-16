using System.Text;

using HackRFDotnet.ManagedApi;
using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.Device;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Utilities;

namespace BasicScanner.Services;
public class SpectrumDisplayService : IDisposable {
    private FrequencyCenteringEffect _frequencyCenteringEffect;
    private DownSampleEffect _reducerEffect;
    private FftEffect _fftEffect;
    private IQ[] _displayBuffer = [];

    public SpectrumDisplayService() {

    }

    public void Dispose() {
        _fftEffect.Dispose();
    }

    private void SurfChannels(DigitalRadioDevice rfDevice) {
        while (true) {
            var newFrequency = rfDevice.Frequency + RadioBand.FromKHz(25);

            newFrequency %= RadioBand.FromMHz(110);

            if (newFrequency < RadioBand.FromMHz(80)) {
                newFrequency = RadioBand.FromMHz(80);
            }

            rfDevice.SetFrequency(newFrequency);
            Thread.Sleep(150);
        }
    }

    public Task StartAsync(DigitalRadioDevice rfDevice, SignalStream signalStream, CancellationToken cancellationToken) {
        //new Thread(() => {
        //  SurfChannels(rfDevice);
        //}).Start();

        Console.Clear();

        var processingSize = 65536;
        _displayBuffer = new IQ[processingSize];

        var effectsPipeline = new SignalProcessingBuilder()
        .AddSignalEffect(new DownSampleEffect(signalStream.SampleRate, rfDevice.Bandwidth.NyquistSampleRate,
            processingSize, out var reducedSampleRate, out var producedChunkSize))
        .AddSignalEffect(new FrequencyCenteringEffect(RadioBand.FromKHz(-100), reducedSampleRate))
        .AddSignalEffect(new FftEffect(true, producedChunkSize))
        .AddSignalEffect(new LowPassFilterEffect(reducedSampleRate, rfDevice.Bandwidth))
        .BuildPipeline();

        var resolution = SignalUtilities.FrequencyResolution(producedChunkSize, reducedSampleRate);
        var magnitudes = new float[producedChunkSize];

        var spectrumBuilder = new StringBuilder();
        var columns = new string[Console.WindowWidth - 16];
        float? average = null;
        while (true) {
            spectrumBuilder.Clear();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            signalStream.ReadSpan(_displayBuffer.AsSpan());
            var chunk = effectsPipeline.ApplyPipeline(_displayBuffer.AsSpan());

            for (var y = 0; y < chunk; y++) {
                magnitudes[y] = _displayBuffer[y].Magnitude;
            }

            var maxHeight = Console.WindowHeight - 16;
            average ??= _displayBuffer.Average(i => i.Magnitude) / 5;

            for (var x = 0; x < producedChunkSize; x += producedChunkSize / columns.Length) {
                var freq = RadioBand.FromHz(resolution * x);

                var power = maxHeight / (_displayBuffer[x].Magnitude / average);
                power *= maxHeight;
                power = power < maxHeight ? power : maxHeight;

                var binString = new string('█', (int)power);
                binString += new string(' ', maxHeight - (int)power);
                columns[x % columns.Length] = binString;
            }

            for (var y = maxHeight - 1; y >= 0; y--) {
                for (var x = 0; x < columns.Length - 1; x++) {
                    if (columns[x] is null) {
                        spectrumBuilder.Append(' ');
                        continue;
                    }

                    spectrumBuilder.Append(columns[x][y]);
                }
                spectrumBuilder.AppendLine(columns[columns.Length - 1]);
            }
            Console.WriteLine(spectrumBuilder);
        }
    }
}
