using BasicScanner.Services;

using HackRFDotnet.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;

namespace BasicScanner;
internal class Startup {
    public static void ConfigureServices(IServiceCollection serviceCollection) {
        serviceCollection
            .AddHackRfDotnetServices()
            .AddHostedService<MainService>()
            .AddSingleton<SpectrumDisplayService>();

    }
}
