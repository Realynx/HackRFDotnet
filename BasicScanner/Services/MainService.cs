using HackRFDotnet.ManagedApi;
using HackRFDotnet.ManagedApi.AudioPlayers;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.ManagedApi.Extensions;

using Microsoft.Extensions.Hosting;

namespace BasicScanner.Services;
internal class MainService : IHostedService {
    public Task StartAsync(CancellationToken cancellationToken) {
        Console.WriteLine("looking for HackRf Device...");

        var deviceList = HackRfLib.FindDevices();
        Console.WriteLine($"Found {deviceList.devicecount} HackRf devices... Opening Rx");

        using var rfDevice = HackRfLib.ConnectToFirstDevice();
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
        rfDevice.SetFrequency(RadioBand.FromMHz(98.7f), RadioBand.FromKHz(200));

        //rfDevice.StartRecordingToFile("Recording.bin");

        //var rfFileStream = new RfFileStream("Recording.bin");
        //rfFileStream.Open(20_000_000);

        rfDevice.AttenuateAmplification();
        var fmPlayer = new AnaloguePlayer(new FmSignalStream(rfDevice.RfDeviceStream));
        fmPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, 48000);

        ControlChannel(rfDevice);

        return Task.CompletedTask;
    }

    private static void ControlChannel(RfDevice rfDevice) {
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
