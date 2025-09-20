
using HackRFDotnet.Api;
using HackRFDotnet.Api.Interfaces;
using HackRFDotnet.Api.Services;
using HackRFDotnet.Api.Streams.SignalStreams.Analogue;

using Microsoft.Extensions.Hosting;

namespace DiServiceHosting.Services;
internal class FmRadioService : IHostedService, IDisposable {
    private readonly IDigitalRadioDevice _radioDevice;
    private readonly FmSignalStream _signalStream;

    public FmRadioService(IDigitalRadioDevice radioDevice) {
        _radioDevice = radioDevice;

        if (_radioDevice.DeviceStream is null) {
            throw new Exception("Radio device stream cannot be null!");
        }

        _signalStream = new FmSignalStream(_radioDevice.DeviceStream, Bandwidth.FromKHz(200));
    }

    public void Dispose() {
        _signalStream.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _radioDevice.SetFrequency(Frequency.FromMHz(98.7f), Bandwidth.FromKHz(120));

        var fmPlayer = new AnaloguePlayer(_signalStream);
        fmPlayer.PlayStreamAsync(_radioDevice.Frequency, _radioDevice.Bandwidth, SampleRate.FromKsps(48));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
