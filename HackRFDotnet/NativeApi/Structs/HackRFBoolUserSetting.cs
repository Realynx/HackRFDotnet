using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Structs {
    /// <summary>
    /// Helper struct for <see cref="HackRFBiasTUserSettingReq"/>. If <see cref="do_update"/> is <see langword="true"/>, then the values of <see cref="change_on_mode_entry"/> and <see cref="enabled"/> will be used as the new default.
    /// If <see cref="do_update"/> is <see langword="false"/>, the current default will not change.
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
