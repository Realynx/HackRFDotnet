using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Structs {
    /// <summary>
    /// Helper struct for hackrf_bias_t_user_setting. If 'do_update' is true, then the values of 'change_on_mode_entry' and 'enabled' will be used as the new default.
    /// If 'do_update' is false, the current default will not change.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HackRFBoolUserSetting {
        /// <summary>
        /// If true, update default values.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool do_update;

        /// <summary>
        /// Change value on mode entry.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool change_on_mode_entry;

        /// <summary>
        /// Enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool enabled;

        // Optional padding to match C struct alignment
        // public byte padding1;
    }
}
