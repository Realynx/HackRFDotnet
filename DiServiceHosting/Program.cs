using DiServiceHosting.Services;

using HackRFDotnet.Api;
using HackRFDotnet.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiServiceHosting;

internal class Program {
    static async Task Main(string[] args) {

        var appHost = new HostBuilder();
        appHost
            .UseFirstRadioDevice(SampleRate.FromMsps(20))
            .ConfigureServices(ConfigureServices);

        await appHost.RunConsoleAsync();
    }

    static void ConfigureServices(IServiceCollection serviceCollection) {
        serviceCollection
            .AddHostedService<FmRadioService>();
    }
}
