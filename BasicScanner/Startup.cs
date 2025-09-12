using BasicScanner.Services;

using HackRFDotnet.ManagedApi.Services;

using Microsoft.Extensions.DependencyInjection;

namespace BasicScanner;
internal class Startup {
    public static void ConfigureServices(IServiceCollection serviceCollection) {
        serviceCollection
            .AddHostedService<MainService>()
            .AddSingleton<SpectrumDisplayService>();

        serviceCollection
            .AddSingleton<RfDeviceControllerService>();
    }
}
