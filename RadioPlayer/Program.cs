using HackRFDotnet.Api;
using HackRFDotnet.Api.Services;
using HackRFDotnet.Api.Streams.Device;
using HackRFDotnet.Api.Streams.SignalStreams.Analogue;

namespace RadioPlayer;

internal class Program {
    static void Main(string[] args) {
        var rfDeviceService = new RfDeviceService();

        Console.WriteLine("Looking for HackRf Device...");
        var deviceList = rfDeviceService.FindDevices();

        Console.WriteLine($"Found {deviceList.devicecount} HackRf devices... Opening Rx");
        using var rfDevice = rfDeviceService.ConnectToFirstDevice();

        if (rfDevice is null) {
            Console.WriteLine("Could not connect to Rf Device");
            return;
        }

        // Create an immutable read stream from an RF Device.
        using var deviceStream = new IQDeviceStream(rfDevice);

        // Open the receive channel on the SDR
        deviceStream.OpenRx(SampleRate.FromMsps(20));

        // Tune the SDR to the target frequency and bandwidth
        rfDevice.SetFrequency(Frequency.FromMHz(98.7f), Bandwidth.FromKHz(200));

        // Create a SignalStream configured for FM decoding
        var fmSignalStream = new FmSignalStream(deviceStream, Bandwidth.FromKHz(200), stereo: true);

        // Create an AnaloguePlayer to play the FM audio stream
        var fmPlayer = new AnaloguePlayer(fmSignalStream);
        fmPlayer.PlayStreamAsync(rfDevice.Frequency, rfDevice.Bandwidth, SampleRate.FromKsps(48));

        for (; ; ) {
            Console.ReadLine();
        }
    }
}
