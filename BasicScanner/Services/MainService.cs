using System.Security.Cryptography;

using HackRFDotnet.ManagedApi;
using HackRFDotnet.ManagedApi.Extensions;
using HackRFDotnet.ManagedApi.Services;
using HackRFDotnet.ManagedApi.Streams.Device;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;

using Microsoft.Extensions.Hosting;

namespace BasicScanner.Services;
internal class MainService : IHostedService {
    private readonly RfDeviceControllerService _rfDeviceControllerService;
    private readonly SpectrumDisplayService _spectrumDisplayService;

    public MainService(RfDeviceControllerService rfDeviceControllerService, SpectrumDisplayService spectrumDisplayService) {
        _rfDeviceControllerService = rfDeviceControllerService;
        _spectrumDisplayService = spectrumDisplayService;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        Console.WriteLine("looking for HackRf Device...");

        var deviceList = _rfDeviceControllerService.FindDevices();
        Console.WriteLine($"Found {deviceList.devicecount} HackRf devices... Opening Rx");

        using var rfDevice = _rfDeviceControllerService.ConnectToFirstDevice();
        if (rfDevice is null) {
            Console.WriteLine("Could not connect to Rf Device");
            return Task.CompletedTask;
        }

        //rfDevice.SetFrequency(RadioBand.FromMHz(94.7f), RadioBand.FromKHz(200));
        //rfDevice.StartRecordingToFile("Recording.bin");

        //var rfFileStream = new RfFileStream("Recording.bin");
        //rfFileStream.Open(20_000_000);

        rfDevice.AttenuateAmplification();
        using var deviceStream = new IQDeviceStream(rfDevice);
        deviceStream.OpenRx(new SampleRate(20_000_000));

        FrquencyDemodulateAndPlayAsAudio(rfDevice, deviceStream);
        //AmplitudeDemodulateAndPlayAsAudio(rfDevice, deviceStream);

        DisplaySpectrumCliBasic(rfDevice, deviceStream);

        ControlChannel(rfDevice);
        return Task.CompletedTask;
    }

    private static void FrquencyDemodulateAndPlayAsAudio(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        //rfDevice.SetFrequency(RadioBand.FromMHz(162.55f), RadioBand.FromKHz(20));
        rfDevice.SetFrequency(RadioBand.FromMHz(98.7f), RadioBand.FromKHz(200));

        // We must build an effects pipeline to clean up our recived signal from the SDR.
        var effectsPipeline = new SignalProcessingBuilder()
            // Reducer decimates your signal down to it's bandwidth. Since our signal has been frequency shifted by the SDR mixer
            // our target frequency has been shifted to Direct Current (DC).
            // Meaning we don't need any more sample rate than the band of the signal to represent it in the time domain,
            // so we "Reduce" it's extraneous information
            .AddSignalEffect(new DownSampleEffect(deviceStream.SampleRate,
                rfDevice.Bandwidth.NyquistSampleRate, out var reducedSampleRate, out var producedChunkSize))

            .AddSignalEffect(new SquelchEffect(reducedSampleRate))
            // Fast Fourier Transform from the Time domain signal to the Frequency domain
            .AddSignalEffect(new FftEffect(true, producedChunkSize))

            // Low pass filter our band (Since we are mixed to DC, we only need to low pass filter the signal it gets affected on + and -)
            .AddSignalEffect(new LowPassFilterEffect(reducedSampleRate, rfDevice.Bandwidth))

            // Inverse Fast Fourier Transform from the Frequency domain back to the Time domain.
            .AddSignalEffect(new FftEffect(false, producedChunkSize))

            // Compile our effect pipeline
            .BuildPipeline();

        // Create a signal stream and configure it with our effects pipeline,
        // it will allow us to read from it as a stream with pre-demoulated results, like a StreamReader
        var fmSignalStream = new FmSignalStream(deviceStream, reducedSampleRate, true, processingPipeline: effectsPipeline, keepOpen: false);

        // And AnaloguePlayer let's us resample and pipe an audio out the speakers.
        var fmPlayer = new AnaloguePlayer(fmSignalStream);
        fmPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, 48000);
    }

    private static void AmplitudeDemodulateAndPlayAsAudio(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        //rfDevice.SetFrequency(RadioBand.FromMHz(125.150f), RadioBand.FromKHz(8));
        //rfDevice.SetFrequency(RadioBand.FromMHz(162.4f), RadioBand.FromKHz(20));

        //var scanningService = new ChannelScanningService(iqStream, rfDevice);
        //scanningService.StartScanning(RadioBand.FromMHz(118.4f), RadioBand.FromMHz(118.575f),
        //    RadioBand.FromMHz(119.250f), RadioBand.FromMHz(119.450f), RadioBand.FromMHz(121.800f),
        //    RadioBand.FromMHz(124.05f), RadioBand.FromMHz(125.150f), RadioBand.FromMHz(135f));

        rfDevice.SetFrequency(RadioBand.FromMHz(118.4f), RadioBand.FromKHz(10));

        var effectsPipeline = new SignalProcessingBuilder()
            .AddSignalEffect(new DownSampleEffect(deviceStream.SampleRate,
                rfDevice.Bandwidth.NyquistSampleRate, out var reducedSampleRate, out var producedChunkSize))

            .AddSignalEffect(new SquelchEffect(reducedSampleRate))
            .AddSignalEffect(new FftEffect(true, producedChunkSize))
            .AddSignalEffect(new LowPassFilterEffect(reducedSampleRate, rfDevice.Bandwidth))
            .AddSignalEffect(new FftEffect(false, producedChunkSize))

            .BuildPipeline();

        var amSignalStream = new AmSignalStream(deviceStream, reducedSampleRate, effectsPipeline, keepOpen: false);

        var amPlayer = new AnaloguePlayer(amSignalStream);
        amPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, 48000);
    }

    private void DisplaySpectrumCliBasic(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        var _thread = new Thread(async () => {
            var signalStream = new SignalStream(deviceStream);
            await _spectrumDisplayService.StartAsync(rfDevice, signalStream, new CancellationTokenSource().Token);
        });

        _thread.Start();
    }

    private static void ControlChannel(DigitalRadioDevice rfDevice) {
        for (; ; ) {
            Console.Write($"[{rfDevice.Frequency.Mhz} Mhz] Frequency: ");
            var freq = Console.ReadLine();

            if (double.TryParse(freq, out var userFrequency)) {
                rfDevice.SetFrequency(RadioBand.FromMHz((float)userFrequency));
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
