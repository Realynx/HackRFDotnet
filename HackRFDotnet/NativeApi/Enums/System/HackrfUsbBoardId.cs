namespace HackRFDotnet.NativeApi.Enums.System {
    /// <summary>
    /// USB board ID (product ID) enum
    ///
    /// Contains USB-IF product id (field `idProduct` in `libusb_device_descriptor`). Can be used to identify general type of hardware.
    /// Only used in <see cref="HackRFDotnet.NativeApi.Structs.Devices.HackRFDeviceList.usb_board_ids"/> field of <see cref="HackRFDotnet.NativeApi.Lib.HackRfNativeLib.Devices.QueryDeviceList"/>,
    /// and can be converted into human-readable string via <see cref="HackRFDotnet.NativeApi.Lib.HackRfNativeLib.Firmware.UsbBoardIdName"/>.
    /// </summary>
    public enum HackrfUsbBoardId {
        /// <summary>
        /// Jawbreaker (beta platform) USB product id
        /// </summary>
        USB_BOARD_ID_JAWBREAKER = 0x604B,

        /// <summary>
        /// HackRF One USB product id
        /// </summary>
        USB_BOARD_ID_HACKRF_ONE = 0x6089,

        /// <summary>
        /// RAD1O (custom version) USB product id
        /// </summary>
        USB_BOARD_ID_RAD1O = 0xCC15,

        /// <summary>
        /// Invalid / unknown USB product id
        /// </summary>
        USB_BOARD_ID_INVALID = 0xFFFF,
    }
}