
using HackRFDotnet.ManagedApi.Services;
using HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;

namespace BasicScanner.Services;
public class SpectrumDisplayService {
    public SpectrumDisplayService() {

    }

    public Task StartAsync(WaveSignalStream signalStream, CancellationToken cancellationToken) {


        return Task.CompletedTask;
    }
}
