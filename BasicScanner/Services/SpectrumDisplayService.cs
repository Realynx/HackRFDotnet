using System.Text;

using HackRFDotnet.ManagedApi;
using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Utilities;

namespace BasicScanner.Services;
public class SpectrumDisplayService {
    private IQ[] _displayBuffer = [];

    public SpectrumDisplayService() {

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

    public async Task StartAsync(DigitalRadioDevice rfDevice, SignalStream signalStream, CancellationToken cancellationToken) {
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
        .BuildPipeline();

        var resolution = SignalUtilities.FrequencyResolution(producedChunkSize, reducedSampleRate);
        var magnitudes = new float[producedChunkSize];

        var spectrumBuilder = new StringBuilder();
        var columns = new string[Console.WindowWidth];
        float? average = null;

        Console.ForegroundColor = ConsoleColor.Red;
        while (!cancellationToken.IsCancellationRequested) {
            // 60 FPS
            Thread.Sleep(1000 / 60);

            spectrumBuilder.Clear();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            signalStream.ReadSpan(_displayBuffer.AsSpan());
            var chunk = effectsPipeline.ApplyPipeline(_displayBuffer.AsSpan());

            var maxHeight = Console.WindowHeight - 4;
            average = _displayBuffer.Average(i => i.Magnitude) / 5;

            for (var x = 0; x < columns.Length; x++) {
                var binIndex = (int)((long)x * producedChunkSize / columns.Length);

                var magSafe = Math.Max(_displayBuffer[binIndex].Magnitude, 1e-9f);
                var db = 5f * MathF.Log10(magSafe / average ?? 1f);
                var power = (int)Math.Clamp(db, 0, maxHeight);

                var binString = new string('█', power) + new string(' ', maxHeight - power);
                columns[x] = binString;
            }

            for (var y = maxHeight - 1; y >= 0; y--) {
                for (var x = 0; x < columns.Length; x++) {
                    if (columns[x] is null) {
                        spectrumBuilder.Append(' ');
                        continue;
                    }

                    spectrumBuilder.Append(columns[x][y]);
                }

                spectrumBuilder.AppendLine();
            }
            Console.WriteLine(spectrumBuilder);
        }
    }
}
