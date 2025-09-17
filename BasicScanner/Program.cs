using HackRFDotnet.ManagedApi.Services;
using HackRFDotnet.ManagedApi.Streams.Device;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
using HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;

using Microsoft.Extensions.Hosting;

namespace BasicScanner {
    internal unsafe class Program {

        static void Main(string[] args) {
            var appHost = new HostBuilder();
            appHost.ConfigureServices(Startup.ConfigureServices);

            appHost.RunConsoleAsync();
        }
    }
}
