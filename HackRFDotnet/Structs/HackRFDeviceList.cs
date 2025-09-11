using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums;

namespace HackRFDotnet.Structs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HackRFDeviceList {
        /// <summary>
        /// Array of human-readable serial numbers. Each entry can be NULL.
        /// </summary>
        public char** serial_numbers; // char**

        /// <summary>
        /// ID of each board, based on USB product ID
        /// </summary>
        public HackrfUsbBoardId* usb_board_ids; // enum hackrf_usb_board_id*

        /// <summary>
        /// USB device index for each HW entry
        /// </summary>
        public int* usb_device_index; // int*

        /// <summary>
        /// Number of connected HackRF devices
        /// </summary>
        public int devicecount;

        /// <summary>
        /// All USB devices (as libusb_device** array)
        /// </summary>
        public void** usb_devices; // void**

        /// <summary>
        /// Number of all queried USB devices
        /// </summary>
        public int usb_devicecount;
    }

}
