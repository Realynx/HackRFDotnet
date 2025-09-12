using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Structs.Devices {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HackRFOperacakeDwellTime {
        /// <summary>
        /// Dwell time for port (in number of samples)
        /// </summary>
        public uint dwell;

        /// <summary>
        /// Port to connect A0 to (B0 mirrors this choice)
        /// Must be one of operacake_ports
        /// </summary>
        public byte port;

        // Optional: add padding to match C struct size if needed
        // C might pad this struct to 8 bytes depending on the compiler/architecture
        // public byte padding1;
        // public byte padding2;
        // public byte padding3;
    }
}
