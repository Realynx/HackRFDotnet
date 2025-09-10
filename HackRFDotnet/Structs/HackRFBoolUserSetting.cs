using System.Runtime.InteropServices;

namespace HackRFDotnet.Structs {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HackRFBoolUserSetting {
        /// <summary>
        /// If true, update default values
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool do_update;

        /// <summary>
        /// Change value on mode entry
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool change_on_mode_entry;

        /// <summary>
        /// Enabled
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool enabled;

        // Optional padding to match C struct alignment
        // public byte padding1;
    }
}
