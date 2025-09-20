using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.Api.Interfaces;
public interface IRfDeviceService {
    DigitalRadioDevice? ConnectToFirstDevice();
    HackRFDeviceList FindDevices();
}