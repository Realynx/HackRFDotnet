using BasicScanner.NativeMethods;

using HackRFDotnet.ManagedApi;
using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Utilities;

namespace BasicScanner.Services;
public unsafe class SpectrumDisplayService {
    private IQ[] _displayBuffer = [];

    private readonly short[] _rainbowColors =
    [
        0x0004,           // red
        //0x0004,           // red
        //0x0006,           // yellow (red + green)
        //0x0006,           // yellow (red + green)
        //0x0002,           // green
        //0x0002,           // green
        //0x0003,           // cyan (green + blue)
        //0x0003,           // cyan (green + blue)
        //0x0001,           // blue
        //0x0001,           // blue
        //0x0005,           // magenta (red + blue)
        //0x0005,           // magenta (red + blue)
        //0x0008 | 0x0004,  // bright red
        //0x0008 | 0x0004,  // bright red
        //0x0008 | 0x0001,  // bright blue
        //0x0008 | 0x0001,  // bright blue
    ];



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
        //    SurfChannels(rfDevice);
        //}).Start();

        Console.Clear();
        var spectrumSize = RadioBand.FromMHz(1);
        var processingSize = 65536;
        _displayBuffer = new IQ[processingSize];

        var effectsPipeline = new SignalProcessingBuilder()
        .AddSignalEffect(new DownSampleEffect(signalStream.SampleRate, spectrumSize.NyquistSampleRate,
            processingSize, out var reducedSampleRate, out var producedChunkSize))

        .AddSignalEffect(new FrequencyCenteringEffect((spectrumSize), reducedSampleRate))
        .AddSignalEffect(new FftEffect(true, producedChunkSize))
        .BuildPipeline();

        var resolution = SignalUtilities.FrequencyResolution(producedChunkSize, reducedSampleRate);
        var magnitudes = new float[producedChunkSize];
        float? average = null;


        var maxHeight = Console.WindowHeight;
        var maxWidth = Console.WindowWidth;
        var spectrumMatrix = new char[maxWidth, maxHeight];
        var buffer = new CHAR_INFO[maxWidth * maxHeight];

        var consoleHandle = kernel32Methods.GetStdHandle(kernel32Methods.STD_OUTPUT_HANDLE);
        var bufferSize = new COORD((short)maxWidth, (short)maxHeight);
        var bufferCoord = new COORD(0, 0);
        var writeRegion = new SMALL_RECT {
            Left = 0,
            Top = 0,
            Right = (short)(maxWidth - 1),
            Bottom = (short)(maxHeight - 1)
        };

        var block = '█';
        var empty = ' ';
        Console.CursorVisible = false;

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
                    spectrumMatrix[x, i] = block;
                }

                for (var i = power; i < maxHeight; i++) {
                    spectrumMatrix[x, i] = empty;
                }
            }

            for (var y = 0; y < maxHeight; y++) {
                for (var x = 0; x < maxWidth; x++) {
                    var bufferIndex = (y * maxWidth) + x;
                    var matrixY = maxHeight - 1 - y;
                    buffer[bufferIndex].UnicodeChar = spectrumMatrix[x, matrixY];
                    buffer[bufferIndex].Attributes = _rainbowColors[(maxHeight - y) % _rainbowColors.Length];
                }
            }

            fixed (CHAR_INFO* pBuffer = buffer) {
                kernel32Methods.WriteConsoleOutputW(consoleHandle, pBuffer, bufferSize, bufferCoord, ref writeRegion);
            }
        }
    }
}
