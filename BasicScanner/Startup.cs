using BasicScanner.Services;

using Microsoft.Extensions.DependencyInjection;

namespace BasicScanner;
internal class Startup {
    public static void ConfigureServices(IServiceCollection serviceCollection) {
        serviceCollection
            .AddHostedService<MainService>();
    }
}
