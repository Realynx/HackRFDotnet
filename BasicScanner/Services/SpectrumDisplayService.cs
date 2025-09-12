using System;
using System.Text;

using FftSharp;

using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Utilities;

namespace BasicScanner.Services;
public class SpectrumDisplayService {
    private readonly FftEffect _fft = new FftEffect(true);
    private readonly FftEffect _fftInvert = new FftEffect(false);
    public SpectrumDisplayService() {

    }

    public Task StartAsync(SignalStream signalStream, CancellationToken cancellationToken) {
        var fftChunksize = signalStream.CalculateFFTChunkSize();

        var iqSamples = new IQ[65536];
        var resolution = FFT.FrequencyResolution(iqSamples.Length, signalStream.SampleRate);



        var spectrumBuilder = new StringBuilder();
        while (true) {
            spectrumBuilder.Clear();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            signalStream.ReadSpan(iqSamples);
            _fft.AffectSignal(iqSamples, iqSamples.Length);

            var magnitudes = new float[iqSamples.Length];
            for (var y = 0; y < iqSamples.Length; y++) {
                magnitudes[y] = (float)iqSamples[y].Magnitude;
            }

            Array.Sort(magnitudes);
            var index = (int)(magnitudes.Length * .2f);
            var noiseFloor = magnitudes[index - 1];
            var averageLevel = SignalUtilities.CalculateDb(iqSamples) / 2;

            for (var x = 0; x < iqSamples.Length; x++) {
                var freq = RadioBand.FromHz((int)(resolution * x));
                var level = 10f * (float)Math.Log10(iqSamples[x].Magnitude + 1e-12f);

                if (freq < RadioBand.FromKHz(200) && x % (iqSamples.Length / 3072) == 0) {
                    var ticks = level;
                    spectrumBuilder.Append(new string('*', (int)ticks));
                    spectrumBuilder.AppendLine("                                 ");
                }
            }
            _fftInvert.AffectSignal(iqSamples, iqSamples.Length);
            Console.WriteLine(spectrumBuilder);
        }
    }
}
