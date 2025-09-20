using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiServiceHosting;

internal class Program {
    static async Task Main(string[] args) {
        var appHost = new HostBuilder();
        appHost.ConfigureServices(ConfigureServices);

        await appHost.RunConsoleAsync();
    }

    static void ConfigureServices(IServiceCollection serviceCollection) {
        serviceCollection
            .AddHostedService<MainService>()
            .AddSingleton<SpectrumDisplayService>();

        serviceCollection
            .AddSingleton<RfDeviceControllerService>();
    }
}
