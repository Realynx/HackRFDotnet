using BasicScanner.NativeMethods;

using HackRFDotnet.Api;
using HackRFDotnet.Api.Streams;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalStreams;
using HackRFDotnet.Api.Utilities;

namespace BasicScanner.Services;
public unsafe class SpectrumDisplayService {
    private IQ[] _displayBuffer = [];

    // This idea doesn't work so well with these colors :c
    private readonly short[] _rainbowColors =
    [
        // Red Orange Yellow Green Blue Idagao Violet
        (short)ConsoleColor.DarkRed,
        //(short)ConsoleColor.Red,
        //(short)ConsoleColor.DarkYellow,
        //(short)ConsoleColor.Yellow,
        //(short)ConsoleColor.Green,
        //(short)ConsoleColor.Cyan,
        //(short)ConsoleColor.DarkBlue,
        //(short)ConsoleColor.DarkMagenta,
    ];

    private readonly char[] _intensityChars =
    {
        //'·',   // very light dot
        '˙',   // slightly heavier dot
        '░',   // light shade
        '▒',   // medium shade
        '▓',   // dark shade
        '█',   // full block
    };

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
            Thread.Sleep(20);
        }
    }

    public async Task StartAsync(DigitalRadioDevice rfDevice, SignalStream signalStream, CancellationToken cancellationToken) {
        //new Thread(() => {
        //    SurfChannels(rfDevice);
        //}).Start();
        Console.Clear();

        var maxHeight = Console.WindowHeight;
        var maxWidth = Console.WindowWidth;
        var spectrumMatrix = new char[maxWidth, maxHeight];
        var buffer = new CHAR_INFO[maxWidth * maxHeight];


        var spectrumSize = Bandwidth.FromKHz(maxWidth);
        var processingSize = 65536;
        _displayBuffer = new IQ[processingSize];

        var effectsPipeline = new SignalProcessingBuilder()
        .AddSignalEffect(new DownSampleEffect(signalStream.SampleRate, spectrumSize.NyquistSampleRate,
            processingSize, out var reducedSampleRate, out var producedChunkSize))

        .AddSignalEffect(new FrequencyCenteringEffect(new RadioBand(spectrumSize), reducedSampleRate))
        .AddSignalEffect(new FftEffect(true, producedChunkSize))
        .BuildPipeline();

        var resolution = SignalUtilities.FrequencyResolution(producedChunkSize, reducedSampleRate);
        var magnitudes = new float[producedChunkSize];
        float? average = null;


        var consoleHandle = kernel32Methods.GetStdHandle(kernel32Methods.STD_OUTPUT_HANDLE);
        var bufferSize = new COORD((short)maxWidth, (short)maxHeight);
        var bufferCoord = new COORD(0, 0);
        var writeRegion = new SMALL_RECT {
            Left = 0,
            Top = 0,
            Right = (short)(maxWidth - 1),
            Bottom = (short)(maxHeight - 1)
        };

        var empty = ' ';
        Console.CursorVisible = false;

        var rainbowWidth = maxWidth;
        while (!cancellationToken.IsCancellationRequested) {
            // 120 FPS
            Thread.Sleep(1000 / 120);

            signalStream.ReadSpan(_displayBuffer.AsSpan());
            var chunk = effectsPipeline.ApplyPipeline(_displayBuffer.AsSpan());

            average = _displayBuffer.Average(i => i.Magnitude) / 5;

            for (var x = 0; x < maxWidth; x++) {
                var binIndex = (int)((long)x * producedChunkSize / maxWidth);

                var magSafe = Math.Max(_displayBuffer[binIndex].Magnitude, 1e-9f);
                var db = 10f * MathF.Log10(magSafe / average ?? 1f);
                var power = (int)Math.Clamp(db, 0, maxHeight);

                for (var i = 0; i < power; i++) {
                    var level = (int)(i / (float)power * (_intensityChars.Length - 1));
                    spectrumMatrix[x, i] = _intensityChars[level];
                }

                for (var i = power; i < maxHeight; i++) {
                    spectrumMatrix[x, i] = empty;
                }
            }

            for (var y = 0; y < maxHeight; y++) {
                for (var x = 0; x < maxWidth; x++) {
                    var color = ((int)(x / (float)rainbowWidth * (_rainbowColors.Length - 1))) % _rainbowColors.Length;

                    var bufferIndex = (y * maxWidth) + x;
                    var matrixY = maxHeight - 1 - y;
                    buffer[bufferIndex].UnicodeChar = spectrumMatrix[x, matrixY];
                    buffer[bufferIndex].Attributes = _rainbowColors[color];
                }
            }

            fixed (CHAR_INFO* pBuffer = buffer) {
                kernel32Methods.WriteConsoleOutputW(consoleHandle, pBuffer, bufferSize, bufferCoord, ref writeRegion);
            }
        }
    }
}
