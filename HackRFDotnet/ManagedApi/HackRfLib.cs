using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.ManagedApi {
    public unsafe static class HackRfLib {
        static HackRfLib() {
            HackRfNativeLib.Init();
        }

        public static HackRFDeviceList FindDevices() {
            var deviceList = HackRfNativeLib.Devices.QueryDeviceList();

            if (deviceList is null) {
                Console.WriteLine("No Devices!");
            }

            return *deviceList;
        }

        public static RfDevice? ConnectToFirstDevice() {
            HackRFDevice* localDevice = null;
            HackRfNativeLib.Devices.OpenDevice(&localDevice);

            if (localDevice is null) {
                return null;
            }

            return new RfDevice(localDevice);
        }
    }
}
