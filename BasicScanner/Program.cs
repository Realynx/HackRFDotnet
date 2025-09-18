using Microsoft.Extensions.Hosting;

namespace BasicScanner {
    internal class Program {

        static async Task Main(string[] args) {
            var appHost = new HostBuilder();
            appHost.ConfigureServices(Startup.ConfigureServices);

            await appHost.RunConsoleAsync();
        }
    }
}
