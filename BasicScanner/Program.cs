using HackRFDotnet.Api.Services;
using HackRFDotnet.Api.Streams.Device;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Streams.SignalStreams.Analogue;

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
