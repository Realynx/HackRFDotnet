using HackRFDotnet.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;

using RadioSpectrum.Services;

namespace RadioSpectrum;
internal class Startup {
    public static void ConfigureServices(IServiceCollection serviceCollection) {
        serviceCollection
            .AddHackRfDotnetServices()
            .AddHostedService<SpectrumDisplayService>()
            .AddHostedService<MainService>();
    }
}
