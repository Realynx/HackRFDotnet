using System.Buffers;
using System.Text;
using System.Text.Unicode;

using HackRFDotnet.Api;
using HackRFDotnet.Api.Extensions;
using HackRFDotnet.Api.Services;
using HackRFDotnet.Api.Streams;
using HackRFDotnet.Api.Streams.Device;
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

    /// <summary>
    /// There was a bug when running from a thread/task without a reference.
    /// It would cause audio stuttering after about 10 mins of running.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken) {
        Console.WriteLine("Looking for HackRf Device...");

        var deviceList = _rfDeviceControllerService.FindDevices();
        Console.WriteLine($"Found {deviceList.devicecount} HackRf devices... Opening Rx");

        using var rfDevice = _rfDeviceControllerService.ConnectToFirstDevice();
        if (rfDevice is null) {
            Console.WriteLine("Could not connect to Rf Device");
            return;
        }

        rfDevice.AttenuateAmplification();
        using var deviceStream = new IQDeviceStream(rfDevice);
        deviceStream.OpenRx(SampleRate.FromMsps(20));

        //FrequencyDemodulateAndPlayAsAudio(rfDevice, deviceStream);
        //AmplitudeDemodulateAndPlayAsAudio(rfDevice, deviceStream);
        var task = Task.Run(() => HdRadioPlay(rfDevice, deviceStream));

        DisplaySpectrumCliBasic(rfDevice, deviceStream);

        for (; ; ) {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    private static void HdRadioPlay(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        var fmFrequency = Frequency.FromMHz(98.7f);

        rfDevice.SetFrequency(Frequency.FromMHz(98.862500f), Bandwidth.FromKHz(76));
        var pskSignalStream = new QpskSignalStream(deviceStream, Bandwidth.FromKHz(76));

        var top = Console.CursorTop;
        while (true) {
            var dataBuffer = ArrayPool<byte>.Shared.Rent(256);

            try {
                Console.CursorLeft = 0;
                Console.CursorTop = top;

                pskSignalStream.Read(dataBuffer, 256);

                var hexString = string.Join(string.Empty, dataBuffer.Select(x => x.ToString("x2")));
                // Console.Write(Encoding.UTF8.GetString(dataBuffer));
            }
            finally {
                ArrayPool<byte>.Shared.Return(dataBuffer);
            }
        }
        //var hdRadioSignalStream = new HdRadioSignalStream(deviceStream, reducedSampleRate, stereo: true,
        //    processingPipeline: effectsPipeline, keepOpen: false);

        //// And AnaloguePlayer let's us resample and pipe an audio out the speakers.
        //var digitalPlayer = new DigitalPlayer(hdRadioSignalStream);
        //digitalPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, 48000);
    }

    private static void FrequencyDemodulateAndPlayAsAudio(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        //rfDevice.SetFrequency(RadioBand.FromMHz(162.55f), RadioBand.FromKHz(20));
        rfDevice.SetFrequency(Frequency.FromMHz(98.7f), Bandwidth.FromKHz(200));
        var fmSignalStream = new FmSignalStream(deviceStream, Bandwidth.FromKHz(200), stereo: true);

        var fmPlayer = new AnaloguePlayer(fmSignalStream);
        fmPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, SampleRate.FromKsps(48));
    }

    private static void AmplitudeDemodulateAndPlayAsAudio(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        rfDevice.SetFrequency(Frequency.FromMHz(118.4f), Bandwidth.FromKHz(10));
        var amSignalStream = new AmSignalStream(deviceStream, Bandwidth.FromKHz(10));

        var amPlayer = new AnaloguePlayer(amSignalStream);
        amPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, SampleRate.FromKsps(48));
    }

    private void DisplaySpectrumCliBasic(DigitalRadioDevice rfDevice, IQDeviceStream deviceStream) {
        var _thread = new Thread(async () => {
            var signalStream = new SignalStream<IQ>(deviceStream);
            await _spectrumDisplayService.StartAsync(rfDevice, signalStream, new CancellationTokenSource().Token);
        });

        _thread.Start();
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
