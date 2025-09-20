using HackRFDotnet.Api.Interfaces;
using HackRFDotnet.Api.Services;
using HackRFDotnet.Api.Streams.Device;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HackRFDotnet.Api.Extensions;
public static class HostBuilderExtensions {
    /// <summary>
    /// Will automatically open the first device, create a stream, and add the instances as singletons to the DI host.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="sampleRate"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static HostBuilder UseFirstRadioDevice(this HostBuilder hostBuilder, SampleRate sampleRate) {
        hostBuilder.ConfigureServices((serviceCollection) => {
            var deviceService = new RfDeviceService();
            var rfDevice = deviceService.ConnectToFirstDevice()
                ?? throw new Exception("Could not connect to a radio device!");

            var immutableStream = new IQDeviceStream(rfDevice);
            immutableStream.OpenRx(sampleRate);

            serviceCollection
                .AddSingleton<IRfDeviceService>(deviceService)
                .AddSingleton<IDigitalRadioDevice>(rfDevice);
        });

        return hostBuilder;
    }

    /// <summary>
    /// You must open the IQDeviceStream manually for each device managed via the <see cref="RfDeviceService"/>
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static HostBuilder UseRfDeviceController(this HostBuilder hostBuilder) {
        hostBuilder.ConfigureServices((serviceCollection) => {
            serviceCollection
                .AddSingleton<IRfDeviceService, RfDeviceService>();
        });

        return hostBuilder;
    }
}
