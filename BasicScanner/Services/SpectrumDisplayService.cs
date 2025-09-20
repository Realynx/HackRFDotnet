using System.Runtime.InteropServices;

using HackRFDotnet.Api;
using HackRFDotnet.Api.Interfaces;
using HackRFDotnet.Api.SignalProcessing;
using HackRFDotnet.Api.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams;
using HackRFDotnet.Api.Streams.SignalStreams;
using HackRFDotnet.Api.Utilities;

using Microsoft.Extensions.Hosting;

using RadioSpectrum.NativeMethods;

namespace RadioSpectrum.Services;
public class SpectrumDisplayService : IHostedService {
    private readonly short _inBandColor = (short)ConsoleColor.White;
    private readonly short _outbandColor = (short)ConsoleColor.DarkRed;
    private readonly char[] _intensityChars =
    {
        '˙','░','▒','▓','█',
    };

    private IQ[] _displayBuffer = [];
    private readonly IDigitalRadioDevice _radioDevice;
    private readonly SignalStream<IQ> _signalStream;

    private CancellationToken _cancellationToken;

    public SpectrumDisplayService(IDigitalRadioDevice radioDevice) {
        _radioDevice = radioDevice;
        if (_radioDevice.DeviceStream is null) {
            throw new ArgumentException("Radio device stream cannot be null!");
        }

        _signalStream = new SignalStream<IQ>(_radioDevice.DeviceStream);
    }

    private void SurfChannels(IDigitalRadioDevice rfDevice) {
        while (true) {
            Console.Title = $"{rfDevice.Frequency.Hz / 1_000_000d:0.000}Mhz";
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.RightArrow) {
                var newFrequency = rfDevice.Frequency + Frequency.FromKHz(25);
                rfDevice.SetFrequency(newFrequency, rfDevice.Bandwidth);
            }
            else if (key.Key == ConsoleKey.LeftArrow) {
                var newFrequency = rfDevice.Frequency - Frequency.FromKHz(25);
                rfDevice.SetFrequency(newFrequency, rfDevice.Bandwidth);
            }
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        _cancellationToken = cancellationToken;
        Console.Clear();

        _ = Task.Run(() => SurfChannels(_radioDevice));
        _ = Task.Run(SpectrumRenderLoop);
    }

    private async Task SpectrumRenderLoop() {
        var maxHeight = Console.WindowHeight;
        var maxWidth = Console.WindowWidth;
        var spectrumMatrix = new char[maxWidth, maxHeight];
        var buffer = new CHAR_INFO[maxWidth * maxHeight];

        var spectrumSize = Bandwidth.FromKHz(maxWidth);
        const int PROCESSING_SIZE = 65536;
        _displayBuffer = new IQ[PROCESSING_SIZE];

        var effectsPipeline = new SignalProcessingPipeline<IQ>();
        effectsPipeline
            .WithRootEffect(new IQDownSampleEffect(_signalStream.SampleRate, spectrumSize.NyquistSampleRate,
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
        while (!_cancellationToken.IsCancellationRequested) {
            // 120 FPS
            await Task.Delay(1000 / 120);

            _signalStream.ReadSpan(_displayBuffer.AsSpan());
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

                    var bufferIndex = y * maxWidth + x;
                    var matrixY = maxHeight - 1 - y;
                    buffer[bufferIndex].UnicodeChar = spectrumMatrix[x, matrixY];

                    var freq = _radioDevice.Frequency;
                    var band = _radioDevice.Bandwidth;

                    var binIndex = (int)((long)x * producedChunkSize / maxWidth);
                    var binOffsetHz = (binIndex - producedChunkSize / 2) * resolution;
                    var currentFrequency = freq.Hz + binOffsetHz;

                    if (currentFrequency >= freq.Hz - band.Hz / 2 &&
                        currentFrequency <= freq.Hz + band.Hz / 2) {
                        buffer[bufferIndex].Attributes = _inBandColor;
                    }
                    else {
                        buffer[bufferIndex].Attributes = _outbandColor;
                    }
                }
            }

            unsafe {
                fixed (CHAR_INFO* pBuffer = buffer) {
                    Kernel32Methods.WriteConsoleOutputW(consoleHandle, pBuffer, bufferSize, bufferCoord, ref writeRegion);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
