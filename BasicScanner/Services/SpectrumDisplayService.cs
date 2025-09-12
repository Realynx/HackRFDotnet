using HackRFDotnet.ManagedApi.Streams.SignalStreams;

namespace BasicScanner.Services;
public class SpectrumDisplayService {
    public SpectrumDisplayService() {

    }

    public Task StartAsync(SignalStream signalStream, CancellationToken cancellationToken) {

        return Task.CompletedTask;
    }
}
