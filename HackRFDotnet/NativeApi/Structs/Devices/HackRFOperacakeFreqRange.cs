using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Structs.Devices {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HackRFOperacakeFreqRange {
        /// <summary>
        /// Start frequency (in MHz)
        /// </summary>
        public ushort freq_min;

        /// <summary>
        /// Stop frequency (in MHz)
        /// </summary>
        public ushort freq_max;

        /// <summary>
        /// Port (A0) to use for that frequency range. Port B0 mirrors this.
        /// Must be one of operacake_ports
        /// </summary>
        public byte port;

        // Optional padding to match C struct size (check with Marshal.SizeOf)
        // public byte padding1;
        // public byte padding2;
        // public byte padding3;
    }
}
