using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.Api.Services;
public unsafe class RfDeviceControllerService {
    public readonly List<DigitalRadioDevice> RfDevices = new();

    public RfDeviceControllerService() {
        HackRfNativeLib.Init();

    }
    public HackRFDeviceList FindDevices() {
        var deviceList = HackRfNativeLib.Devices.QueryDeviceList();

        if (deviceList is null) {
            Console.WriteLine("No Devices!");
        }

        return *deviceList;
    }

    public DigitalRadioDevice? ConnectToFirstDevice() {
        HackRFDevice* localDevice = null;
        HackRfNativeLib.Devices.OpenDevice(&localDevice);

        if (localDevice is null) {
            return null;
        }

        var rfDevice = new DigitalRadioDevice(localDevice);

        RfDevices.Add(rfDevice);
        return rfDevice;
    }
}
