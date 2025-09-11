using HackRFDotnet.NativeApi;
using HackRFDotnet.Structs;

namespace HackRFDotnet.ManagedApi {
    public unsafe static class HackRfLib {
        static HackRfLib() {
            HackRfNativeFunctions.hackrf_init();
        }

        public static HackRFDeviceList FindDevices() {
            var device = HackRfNativeFunctions.hackrf_device_list();

            if (device is null) {
                Console.WriteLine("No Devices!");
            }

            return *device;
        }

        public static RfDevice? ConnectToFirstDevice() {
            HackRFDevice* localDevice = null;
            HackRfNativeFunctions.hackrf_open(&localDevice);

            if (localDevice is null) {
                return null;
            }

            return new RfDevice(localDevice);
        }

    }
}
