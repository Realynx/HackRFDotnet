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

        //rfDevice.SetFrequency(RadioBand.FromMHz(162.55f), RadioBand.FromKHz(20));
        //rfDevice.SetFrequency(RadioBand.FromMHz(125.150f), RadioBand.FromKHz(8));
        //rfDevice.SetFrequency(RadioBand.FromMHz(162.4f), RadioBand.FromKHz(20));
        //rfDevice.SetFrequency(RadioBand.FromMHz(118.4f), RadioBand.FromKHz(8));

        //var scanningService = new ChannelScanningService(iqStream, rfDevice);

        //scanningService.StartScanning(RadioBand.FromMHz(118.4f), RadioBand.FromMHz(118.575f),
        //    RadioBand.FromMHz(119.250f), RadioBand.FromMHz(119.450f), RadioBand.FromMHz(121.800f),
        //    RadioBand.FromMHz(124.05f), RadioBand.FromMHz(125.150f), RadioBand.FromMHz(135f));

        //var amPlayer = new AMPlayer(iqStream);
        //amPlayer.PlayStreamAsync(44100);

        //rfDevice.SetFrequency(RadioBand.FromMHz(94.7f), RadioBand.FromKHz(200));
        rfDevice.SetFrequency(RadioBand.FromMHz(98.7f));

        //rfDevice.StartRecordingToFile("Recording.bin");

        //var rfFileStream = new RfFileStream("Recording.bin");
        //rfFileStream.Open(20_000_000);

        rfDevice.AttenuateAmplification();
        using var deviceStream = new IQDeviceStream(rfDevice, 10_000_000);
        deviceStream.OpenRx();

        var effectsPipeline = new SignalProcessingBuilder()
            .AddSignalEffect(new ReducerEffect(deviceStream.SampleRate, RadioBand.FromKHz(200), out var reducedSampleRate))
            .AddSignalEffect(new LowPassFilterEffect(reducedSampleRate, RadioBand.FromKHz(200)))
            .BuildPipeline();

        using var fmSignalStream = new FmSignalStream(deviceStream, processingPipeline: effectsPipeline, keepOpen: false);
        var fmPlayer = new AnaloguePlayer(fmSignalStream);
        fmPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, 48000);

        using var signalStream = new SignalStream(deviceStream);
        ControlChannel(rfDevice);

        return Task.CompletedTask;
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
