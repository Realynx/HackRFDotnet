using HackRFDotnet.Api;
using HackRFDotnet.Api.Extensions;

using Microsoft.Extensions.Hosting;

namespace RadioSpectrum {
    internal class Program {
        static async Task Main(string[] args) {
            var appHost = new HostBuilder();

            appHost
                .UseFirstRadioDevice(SampleRate.FromMsps(20))
                .ConfigureServices(Startup.ConfigureServices);

            await appHost.RunConsoleAsync();
        }
    }
}
