using System.Text;

using FftSharp;

using HackRFDotnet.ManagedApi;
using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;

namespace BasicScanner.Services;
public class SpectrumDisplayService {
    private readonly FftEffect _fft = new FftEffect(true);
    private readonly FftEffect _fftInvert = new FftEffect(false);
    private FrequencyCenteringEffect _frequencyCenteringEffect;
    public SpectrumDisplayService() {

    }

    public Task StartAsync(DigitalRadioDevice rfDevice, SignalStream signalStream, CancellationToken cancellationToken) {
        Console.Clear();
        // we want to move our frequency a bit so the spectrum can see it
        // otherwise DC is our base the spectrum would see half

        _frequencyCenteringEffect = new FrequencyCenteringEffect(RadioBand.FromKHz(-100), signalStream.SampleRate);

        var fftChunksize = signalStream.CalculateFFTChunkSize();

        var iqSamples = new IQ[8192];
        var magnitudes = new float[iqSamples.Length];

        var resolution = FFT.FrequencyResolution(iqSamples.Length, signalStream.SampleRate);

        new Thread(() => {
            while (true) {
                var newFrequency = rfDevice.Frequency + RadioBand.FromKHz(25);

                newFrequency %= RadioBand.FromMHz(110);

                if (newFrequency < RadioBand.FromMHz(80)) {
                    newFrequency = RadioBand.FromMHz(80);
                }
                rfDevice.SetFrequency(newFrequency);
                Thread.Sleep(150);
            }
        }).Start();

        var spectrumBuilder = new StringBuilder();
        while (true) {
            spectrumBuilder.Clear();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            spectrumBuilder.AppendLine($"{rfDevice.Frequency.Mhz}");

            signalStream.ReadSpan(iqSamples);
            _frequencyCenteringEffect.AffectSignal(iqSamples, iqSamples.Length);

            _fft.AffectSignal(iqSamples, iqSamples.Length);

            for (var y = 0; y < iqSamples.Length; y++) {
                magnitudes[y] = (float)iqSamples[y].Magnitude;
            }
            Array.Sort(magnitudes);

            var index = (int)(magnitudes.Length * .2f);
            var noiseFloor = magnitudes[index - 1];
            // var averageLevel = SignalUtilities.CalculateDb(iqSamples) / 2;
            var average = iqSamples.Average(i => i.Magnitude) / 3;
            var maxHeight = 150;

            for (var x = 0; x < iqSamples.Length; x++) {
                var freq = RadioBand.FromHz((int)(resolution * x));
                // var level = 10f * (float)Math.Log10(iqSamples[x].Magnitude + 1e-12f);

                if (freq < RadioBand.FromKHz(300)) {
                    var ticks = iqSamples[x].Magnitude / average;
                    spectrumBuilder.Append(new string('*', (int)(ticks > maxHeight ? maxHeight : ticks)));
                    spectrumBuilder.AppendLine(new string(' ', maxHeight));
                }
            }

            Console.WriteLine(spectrumBuilder);
        }
    }
}
