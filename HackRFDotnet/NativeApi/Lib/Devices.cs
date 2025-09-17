using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums.System;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public static unsafe class Devices {
        /// <summary>Open first available HackRF device.</summary>
        /// <param name="device">Device handle.</param>
        /// <returns>
        /// <see cref="HackrfError.HACKRF_SUCCESS"/> on success, <see cref="HackrfError.HACKRF_ERROR_INVALID_PARAM"/> if <paramref name="device"/> is NULL,
        /// <see cref="HackrfError.HACKRF_ERROR_NOT_FOUND"/> if no HackRF devices are found or other <see cref="HackrfError"/> variant.
        /// </returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_open")]
        public static extern HackrfError OpenDevice(HackRFDevice** device);

        /// <summary>List connected HackRF devices.</summary>
        /// <returns>List of connected devices. The list should be freed with <see cref="DeviceListFree"/>.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_device_list")]
        public static extern HackRFDeviceList* QueryDeviceList();

        /// <summary>
        /// Open a <see cref="HackRFDevice"/> from a device list.
        /// </summary>
        /// <param name="list">Device list to open device from.</param>
        /// <param name="idx">Index of the device to open.</param>
        /// <param name="device">Device handle to open.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success, <see cref="HackrfError.HACKRF_ERROR_INVALID_PARAM"/> on invalid parameters or other <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_device_list_open")]
        public static extern HackrfError DeviceListOpen(HackRFDeviceList* list, int idx, HackRFDevice** device);

        /// <summary>Open HackRF device by serial number.</summary>
        /// <param name="desired_serial_number">Serial number of device to open. If NULL then default to first device found.</param>
        /// <param name="device">Device handle.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success, <see cref="HackrfError.HACKRF_ERROR_INVALID_PARAM"/> on invalid parameters or other <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_open_by_serial")]
        public static extern HackrfError OpenDeviceBySerial([MarshalAs(UnmanagedType.LPStr)] string desired_serial_number, HackRFDevice** device);

        /// <summary>Free a previously allocated <see cref="HackRFDevice"/> list.</summary>
        /// <param name="list">List to free.</param>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_device_list_free")]
        public static extern void DeviceListFree(HackRFDeviceList* list);

        /// <summary>Close a previously opened device.</summary>
        /// <param name="device">Device to close.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or variant of <see cref="HackrfError"/>.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_close")]
        public static extern HackrfError CloseDevice(HackRFDevice* device);

        /// <summary>
        /// Reset HackRF device.
        ///
        /// Requires USB API version 0x0102 or above!
        /// </summary>
        /// <param name="device">Device to reset.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_reset")]
        public static extern HackrfError ResetDevice(HackRFDevice* device);

        /// <summary>
        /// Turn on or off (override) the LEDs of the HackRF device.
        /// This function can turn on or off the LEDs of the device. There are 3 controllable LEDs on the HackRF one:
        /// USB, RX and TX. On the Rad1o, there are 4 LEDs. Each LED can be set individually, but the setting might get overridden by other functions.
        /// 
        /// The LEDs can be set via specifying them as bits of a 8 bit number @p state, bit 0 representing the first (USB on the HackRF One)
        /// and bit 3 or 4 representing the last LED. The upper 4 or 5 bits are unused.
        /// For example, binary value 0bxxxxx101 turns on the USB and TX LEDs on the HackRF One. 
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="state">LED states as a bitfield.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_leds")]
        public static extern HackrfError SetDeviceLeds(HackRFDevice* device, byte state);

        /// <summary>
        /// Enable / disable UI display (RAD1O, PortaPack, etc.).
        /// Enable or disable the display on display-enabled devices (Rad1o, PortaPack).
        ///
        /// Requires USB API version 0x0104 or above!
        /// </summary>
        /// <param name="device">device to enable/disable UI on.</param>
        /// <param name="value">Enable UI. Must be 1 or 0.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError.HACKRF_ERROR_LIBUSB"/> on usb error.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_ui_enable")]
        public static extern int SetDeviceUiEnabled(HackRFDevice* device, byte value);
    }
}