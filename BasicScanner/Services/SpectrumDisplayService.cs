using System.Text;

using HackRFDotnet.ManagedApi;
using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Utilities;

namespace BasicScanner.Services;
public class SpectrumDisplayService : IDisposable {
    private FrequencyCenteringEffect _frequencyCenteringEffect;
    private ReducerEffect _reducerEffect;
    private FftEffect _fftEffect;
    private IQ[] _displayBuffer = [];

    public SpectrumDisplayService() {

    }

    public void Dispose() {
        _fftEffect.Dispose();
    }

    public Task StartAsync(DigitalRadioDevice rfDevice, SignalStream signalStream, CancellationToken cancellationToken) {
        Console.Clear();

        //new Thread(() => {
        //    while (true) {
        //        var newFrequency = rfDevice.Frequency + RadioBand.FromKHz(25);

        //        newFrequency %= RadioBand.FromMHz(110);

        //        if (newFrequency < RadioBand.FromMHz(80)) {
        //            newFrequency = RadioBand.FromMHz(80);
        //        }

        //        rfDevice.SetFrequency(newFrequency);
        //        Thread.Sleep(150);
        //    }
        //}).Start();


        var processingSize = 65536;
        _displayBuffer = new IQ[processingSize];

        _reducerEffect = new ReducerEffect(signalStream.SampleRate, signalStream.BandWidth.NyquistSampleRate,
            out var newSampleRate, out var producedChunkSize);

        var magnitudes = new float[producedChunkSize];

        _fftEffect = new FftEffect(true, producedChunkSize);
        _frequencyCenteringEffect = new FrequencyCenteringEffect(RadioBand.FromKHz(-100), newSampleRate);

        var resolution = SignalUtilities.FrequencyResolution(producedChunkSize, newSampleRate);
        var spectrumBuilder = new StringBuilder();
        while (true) {
            spectrumBuilder.Clear();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            spectrumBuilder.AppendLine($"{rfDevice.Frequency.Mhz}");

            signalStream.ReadSpan(_displayBuffer.AsSpan());
            var chunk = _reducerEffect.AffectSignal(_displayBuffer.AsSpan(), processingSize);

            _frequencyCenteringEffect.AffectSignal(_displayBuffer.AsSpan(), chunk);
            _fftEffect.AffectSignal(_displayBuffer.AsSpan(), chunk);

            for (var y = 0; y < chunk; y++) {
                magnitudes[y] = _displayBuffer[y].Magnitude;
            }

            Array.Sort(magnitudes);
            var average = _displayBuffer.Average(i => i.Magnitude) / 5;
            var maxHeight = 200;

            for (var x = 0; x < producedChunkSize; x++) {
                var freq = RadioBand.FromHz(resolution * x);
                if (x % 4 == 0 && freq < RadioBand.FromKHz(300)) {
                    if (average == 0) {
                        continue;
                    }

                    var dotCount = _displayBuffer[x].Magnitude / average;
                    dotCount = dotCount is float.NaN ? 0f : dotCount;

                    spectrumBuilder.Append(new string('*', (int)(dotCount > maxHeight ? maxHeight : dotCount)));
                    spectrumBuilder.AppendLine(new string(' ', maxHeight));
                }
            }

            Console.WriteLine(spectrumBuilder);
        }
    }
}
