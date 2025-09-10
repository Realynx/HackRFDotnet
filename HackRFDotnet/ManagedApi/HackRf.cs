using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.NativeApi;
using HackRFDotnet.Structs;

namespace HackRFDotnet.ManagedApi {
    public unsafe class HackRf {
        public RadioBand AntennaTuneOffset { get; set; } = RadioBand.FromHz(0);

        public HackRf() {
            HackRfNativeFunctions.hackrf_init();
        }

        public HackRFDeviceList FindDevices() {
            var device = HackRfNativeFunctions.hackrf_device_list();

            if (device is null) {
                Console.WriteLine("No Devices!");
            }

            return *device;
        }

        public void AntennaTuning(int hzOffset) {
            AntennaTuneOffset = RadioBand.FromHz(hzOffset);
        }

        public RfDevice? ConnectToFirstDevice() {
            var rfDevice = new RfDevice();
            if (!rfDevice.IsConnected) {
                rfDevice.Dispose();
                return null;
            }

            rfDevice.AntennaTuneOffset = AntennaTuneOffset;
            return rfDevice;
        }
    }
}
