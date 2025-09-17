namespace HackRFDotnet.NativeApi.Enums.System {
    /// <summary>
    /// HACKRF board id enum.
    ///
    /// Returned by <see cref="HackRFDotnet.NativeApi.Lib.HackRfNativeLib.Firmware.ReadBoardId"/> and can be converted to a human-readable string using <see cref="HackRFDotnet.NativeApi.Lib.HackRfNativeLib.Firmware.BoardIdName"/>.
    /// </summary>
    public enum HackrfBoardId {
        /// <summary>
        /// Jellybean (pre-production revision, not supported).
        /// </summary>
        BOARD_ID_JELLYBEAN = 0,

        /// <summary>
        /// Jawbreaker (beta platform, 10-6000MHz, no bias-tee).
        /// </summary>
        BOARD_ID_JAWBREAKER = 1,

        /// <summary>
        /// HackRF One (prior to rev 9, same limits: 1-6000MHz, 20MSPS, bias-tee).
        /// </summary>
        BOARD_ID_HACKRF1_OG = 2,

        /// <summary>
        /// RAD1O (Chaos Computer Club special edition with LCD & other features. 50M-4000MHz, 20MSPS, no bias-tee).
        /// </summary>
        BOARD_ID_RAD1O = 3,

        /// <summary>
        /// HackRF One (rev. 9 & later. 1-6000MHz, 20MSPS, bias-tee).
        /// </summary>
        BOARD_ID_HACKRF1_R9 = 4,

        /// <summary>
        /// Unknown board (failed detection).
        /// </summary>
        BOARD_ID_UNRECOGNIZED = 0xFE,

        /// <summary>
        /// Unknown board (detection not yet attempted, should be default value).
        /// </summary>
        BOARD_ID_UNDETECTED = 0xFF,
    }
}