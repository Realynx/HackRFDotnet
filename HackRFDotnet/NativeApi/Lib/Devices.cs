using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public static unsafe class Devices {
        /// <summary>
        /// Open first available HackRF device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_open")]
        public static extern int OpenDevice(HackRFDevice** device);

        /// <summary>
        /// List connected HackRF devices
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_device_list")]
        public static extern HackRFDeviceList* QueryDeviceList();

        /// <summary>
        /// Open a @ref hackrf_device from a device list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="idx"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_device_list_open")]
        public static extern int DeviceListOpen(HackRFDeviceList* list, int idx, HackRFDevice** device);

        /// <summary>
        /// Open HackRF device by serial number
        /// </summary>
        /// <param name="desired_serial_number"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_open_by_serial")]
        public static extern int OpenDeviceBySerial([MarshalAs(UnmanagedType.LPStr)] string desired_serial_number, HackRFDevice** device);

        /// <summary>
        /// Free a previously allocated @ref hackrf_device_list list.
        /// </summary>
        /// <param name="list"></param>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_device_list_free")]
        public static extern void DeviceListFree(HackRFDeviceList* list);

        /// <summary>
        /// Close a previously opened device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_close")]
        public static extern int CloseDevice(HackRFDevice* device);

        /// <summary>
        /// Reset HackRF device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_reset")]
        public static extern int ResetDevice(HackRFDevice* device);

        /// <summary>
        /// Turn on or off (override) the LEDs of the HackRF device
        /// This function can turn on or off the LEDs of the device. There are 3 controllable LEDs on the HackRF one:
        /// USB, RX and TX. On the Rad1o, there are 4 LEDs. Each LED can be set individually, but the setting might get overridden by other functions.
        /// 
        /// The LEDs can be set via specifying them as bits of a 8 bit number @p state, bit 0 representing the first (USB on the HackRF One)
        /// and bit 3 or 4 representing the last LED. The upper 4 or 5 bits are unused.
        /// For example, binary value 0bxxxxx101 turns on the USB and TX LEDs on the HackRF One. 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_leds")]
        public static extern int SetDeviceLeds(HackRFDevice* device, byte state);

        /// <summary>
        /// Enable / disable UI display (RAD1O, PortaPack, etc.)
        /// Enable or disable the display on display-enabled devices (Rad1o, PortaPack)
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_ui_enable")]
        public static extern int SetDeviceUiEnabled(HackRFDevice* device, byte value);
    }
}