using HackRFDotnet.Api;
using HackRFDotnet.Api.Extensions;
using HackRFDotnet.Api.Services;
using HackRFDotnet.Api.Streams.Device;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalStreams;
using HackRFDotnet.Api.Streams.SignalStreams.Analogue;
using HackRFDotnet.Api.Streams.SignalStreams.Digital;

using Microsoft.Extensions.Hosting;

namespace BasicScanner.Services;
internal class MainService : IHostedService {
    private readonly RfDeviceControllerService _rfDeviceControllerService;
    private readonly SpectrumDisplayService _spectrumDisplayService;

    public MainService(RfDeviceControllerService rfDeviceControllerService, SpectrumDisplayService spectrumDisplayService) {
        _rfDeviceControllerService = rfDeviceControllerService;
        _spectrumDisplayService = spectrumDisplayService;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        Console.WriteLine("looking for HackRf Device...");

        var deviceList = _rfDeviceControllerService.FindDevices();
        Console.WriteLine($"Found {deviceList.devicecount} HackRf devices... Opening Rx");

        using var rfDevice = _rfDeviceControllerService.ConnectToFirstDevice();
        if (rfDevice is null) {
            Console.WriteLine("Could not connect to Rf Device");
            return;
        }

        //rfDevice.SetFrequency(RadioBand.FromMHz(94.7f), RadioBand.FromKHz(200));
        //rfDevice.StartRecordingToFile("Recording.bin");

        //var rfFileStream = new RfFileStream("Recording.bin");
        //rfFileStream.Open(20_000_000);

        rfDevice.AttenuateAmplification();
        using var deviceStream = new IQDeviceStream(rfDevice);
        deviceStream.OpenRx(new SampleRate(20_000_000));

        FrequencyDemodulateAndPlayAsAudio(rfDevice, deviceStream);
        //AmplitudeDemodulateAndPlayAsAudio(rfDevice, deviceStream);
        //HdRadioPlay(rfDevice, deviceStream);

        DisplaySpectrumCliBasic(rfDevice, deviceStream);

        for (; ; ) {
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }

    private static void HdRadioPlay(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        rfDevice.SetFrequency(Frequency.FromMHz(98.7f), Bandwidth.FromKHz(200));
        var effectsPipeline = new SignalProcessingBuilder()
            .AddSignalEffect(new DownSampleEffect(deviceStream.SampleRate,
                rfDevice.Bandwidth.NyquistSampleRate, out var reducedSampleRate, out var producedChunkSize))

            .AddSignalEffect(new FftEffect(true, producedChunkSize))
            .AddSignalEffect(new FrequencyCenteringEffect(Frequency.FromKHz(-192), reducedSampleRate))
            .AddSignalEffect(new LowPassFilterEffect(reducedSampleRate, Bandwidth.FromKHz(8)))
            .AddSignalEffect(new FftEffect(false, producedChunkSize))

            .BuildPipeline();

        var pskSignalStream = new QpskSignalStream(deviceStream, effectsPipeline, keepOpen: false);


        var hdRadioSignalStream = new HdRadioSignalStream(deviceStream, reducedSampleRate, stereo: true,
            processingPipeline: effectsPipeline, keepOpen: false);

        // And AnaloguePlayer let's us resample and pipe an audio out the speakers.
        var digitalPlayer = new DigitalPlayer(hdRadioSignalStream);
        digitalPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, 48000);
    }

    private static void FrequencyDemodulateAndPlayAsAudio(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        //rfDevice.SetFrequency(RadioBand.FromMHz(162.55f), RadioBand.FromKHz(20));
        rfDevice.SetFrequency(Frequency.FromMHz(98.7f), Bandwidth.FromKHz(200));

        var fmSignalStream = new FmSignalStream(deviceStream, Bandwidth.FromKHz(200), stereo: true);

        var fmPlayer = new AnaloguePlayer(fmSignalStream);
        fmPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, 48000);
    }

    private static void AmplitudeDemodulateAndPlayAsAudio(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        rfDevice.SetFrequency(Frequency.FromMHz(118.4f), Bandwidth.FromKHz(10));

        var amSignalStream = new AmSignalStream(deviceStream, Bandwidth.FromKHz(10));

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

    public Task StopAsync(CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
