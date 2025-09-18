using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Structs.System {
    /// <summary>
    /// State of the SGPIO loop running on the M0 core.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HackRFM0State {
        /// <summary>
        /// Requested mode. Possible values: 0 (IDLE), 1 (WAIT), 2 (RX), 3 (TX_START), 4 (TX_RUN).
        /// </summary>
        public ushort requested_mode;

        /// <summary>
        /// Request flag, 0 means request is completed, any other value means request is pending.
        /// </summary>
        public ushort request_flag;

        // C may pad here automatically, Pack = 1 avoids that

        /// <summary>
        /// Active mode. Same values as requested_mode. Possible values are the same as in <see cref="requested_mode"/>.
        /// </summary>
        public uint active_mode;

        /// <summary>
        /// Number of bytes transferred by the M0.
        /// </summary>
        public uint m0_count;

        /// <summary>
        /// Number of bytes transferred by the M4.
        /// </summary>
        public uint m4_count;

        /// <summary>
        /// Number of shortfalls.
        /// </summary>
        public uint num_shortfalls;

        /// <summary>
        /// Longest shortfall in bytes.
        /// </summary>
        public uint longest_shortfall;

        /// <summary>
        /// Shortfall limit in bytes.
        /// </summary>
        public uint shortfall_limit;

        /// <summary>
        /// Threshold <see cref="m0_count"/> value (in bytes) for next mode change.
        /// </summary>
        public uint threshold;

        /// <summary>
        /// Mode which will be switched to when threshold is reached. Possible values are the same as in <see cref="requested_mode"/>.
        /// </summary>
        public uint next_mode;

        /// <summary>
        /// Error that caused M0 to revert to IDLE. 0 (NONE), 1 (RX_TIMEOUT), 2 (TX_TIMEOUT), 3 (MISSED_DEADLINE).
        /// </summary>
        public uint error;
    }
}
