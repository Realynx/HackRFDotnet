using System.Runtime.InteropServices;

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
    private readonly short _inBandColor = (short)ConsoleColor.Blue;

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
            var newFrequency = rfDevice.Frequency + Frequency.FromKHz(25);

            newFrequency %= Frequency.FromMHz(110);

            if (newFrequency < Frequency.FromMHz(80)) {
                newFrequency = Frequency.FromMHz(80);
            }

            rfDevice.SetFrequency(newFrequency, Bandwidth.FromKHz(200));
            Thread.Sleep(20);
        }
    }

    public async Task StartAsync(DigitalRadioDevice rfDevice, SignalStream<IQ> signalStream, CancellationToken cancellationToken) {
        //new Thread(() => {
        //    SurfChannels(rfDevice);
        //}).Start();
        Console.Clear();

        var maxHeight = Console.WindowHeight;
        var maxWidth = Console.WindowWidth;
        var spectrumMatrix = new char[maxWidth, maxHeight];
        var buffer = new CHAR_INFO[maxWidth * maxHeight];

        var spectrumSize = Bandwidth.FromKHz(maxWidth);
        const int PROCESSING_SIZE = 65536;
        _displayBuffer = new IQ[PROCESSING_SIZE];

        var effectsPipeline = new SignalProcessingPipeline<IQ>();
        effectsPipeline
            .WithRootEffect(new IQDownSampleEffect(signalStream.SampleRate, spectrumSize.NyquistSampleRate,
                PROCESSING_SIZE, out var reducedSampleRate, out var producedChunkSize))

            .AddChildEffect(new FrequencyCenteringEffect(new Bandwidth(reducedSampleRate.Sps / 2), reducedSampleRate))
            .AddChildEffect(new FftEffect(true, producedChunkSize));

        var resolution = SignalUtilities.FrequencyResolution(producedChunkSize, reducedSampleRate);

        var consoleHandle = Kernel32Methods.GetStdHandle(Kernel32Methods.STD_OUTPUT_HANDLE);
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
            _ = effectsPipeline.ApplyPipeline(_displayBuffer.AsSpan());

            ref var spectrumPtr = ref MemoryMarshal.GetArrayDataReference(spectrumMatrix);
            var spectrumSpan = MemoryMarshal.Cast<byte, char>(MemoryMarshal
                .CreateSpan(ref spectrumPtr, spectrumMatrix.Length * spectrumMatrix.Rank * sizeof(char)));
            spectrumSpan.Fill(empty);

            var average = _displayBuffer.Average(i => i.Magnitude) / 5;
            for (var x = 0; x < maxWidth; x++) {
                var binIndex = (int)((long)x * producedChunkSize / maxWidth);

                var magSafe = Math.Max(_displayBuffer[binIndex].Magnitude, 1e-9f);
                var db = 10f * MathF.Log10(magSafe / average);
                var power = (int)Math.Clamp(db, 0, maxHeight);

                for (var i = 0; i < power; i++) {
                    var level = (int)(i / (float)power * (_intensityChars.Length - 1));
                    spectrumMatrix[x, i] = _intensityChars[level];
                }
            }

            for (var y = 0; y < maxHeight; y++) {
                for (var x = 0; x < maxWidth; x++) {
                    var color = ((int)(x / (float)rainbowWidth * (_rainbowColors.Length - 1))) % _rainbowColors.Length;

                    var bufferIndex = (y * maxWidth) + x;
                    var matrixY = maxHeight - 1 - y;
                    buffer[bufferIndex].UnicodeChar = spectrumMatrix[x, matrixY];

                    var freq = rfDevice.Frequency;
                    var band = rfDevice.Bandwidth;

                    var binIndex = (int)((long)x * producedChunkSize / maxWidth);
                    var binOffsetHz = (binIndex - (producedChunkSize / 2)) * resolution;
                    var currentFrequency = freq.Hz + binOffsetHz;

                    if (currentFrequency >= (freq.Hz - (band.Hz / 2)) &&
                        currentFrequency <= (freq.Hz + (band.Hz / 2))) {
                        buffer[bufferIndex].Attributes = _inBandColor;
                    }
                    else {
                        buffer[bufferIndex].Attributes = _rainbowColors[color];
                    }
                }
            }

            fixed (CHAR_INFO* pBuffer = buffer) {
                Kernel32Methods.WriteConsoleOutputW(consoleHandle, pBuffer, bufferSize, bufferCoord, ref writeRegion);
            }
        }
    }
}
