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
