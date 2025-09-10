using System.Runtime.InteropServices;

namespace HackRFDotnet.Structs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ReadPartidSerialNo {
        // MCU part ID register value (2 x UInt32)
        public fixed uint part_id[2];

        // MCU device unique ID (serial number, 4 x UInt32)
        public fixed uint serial_no[4];
    }
}
