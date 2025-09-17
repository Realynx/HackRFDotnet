using HackRFDotnet.NativeApi.Enums.System;

namespace HackRFDotnet.NativeApi {
    internal class NativeConstants {
        internal const string HACK_RF_DLL = "hackrf.dll";

        /// <summary>
        /// Number of samples per tuning when sweeping
        /// </summary>
        public const uint SAMPLES_PER_BLOCK = 8192;

        /// <summary>
        /// Number of bytes per tuning for sweeping
        /// </summary>
        public const uint BYTES_PER_BLOCK = 16384;

        /// <summary>
        /// Maximum number of sweep ranges to be specified for @ref hackrf_init_sweep
        /// </summary>
        public const uint MAX_SWEEP_RANGES = 10;

        /// <summary>
        /// Invalid Opera Cake add-on board address, placeholder in @ref hackrf_get_operacake_boards
        /// </summary>
        public const uint HACKRF_OPERACAKE_ADDRESS_INVALID = 0xFF;

        /// <summary>
        /// Maximum number of connected Opera Cake add-on boards
        /// </summary>
        public const uint HACKRF_OPERACAKE_MAX_BOARDS = 8;

        /// <summary>
        /// Maximum number of specifiable dwell times for Opera Cake add-on boards
        /// </summary>
        public const uint HACKRF_OPERACAKE_MAX_DWELL_TIMES = 16;

        /// <summary>
        /// Maximum number of specifiable frequency ranges for Opera Cake add-on boards
        /// </summary>
        public const uint HACKRF_OPERACAKE_MAX_FREQ_RANGES = 8;

        /// <summary>
        /// Made by GSG bit in @ref hackrf_board_rev enum and in platform ID
        /// </summary>
        public const uint HACKRF_BOARD_REV_GSG = 0x80;

        /// <summary>
        /// JAWBREAKER platform bit in result of @ref hackrf_supported_platform_read
        /// </summary>
        public const uint HACKRF_PLATFORM_JAWBREAKER = 1 << 0;

        /// <summary>
        /// HACKRF ONE (pre r9) platform bit in result of @ref hackrf_supported_platform_read
        /// </summary>
        public const uint HACKRF_PLATFORM_HACKRF1_OG = 1 << 1;

        /// <summary>
        /// RAD1O platform bit in result of @ref hackrf_supported_platform_read
        /// </summary>
        public const uint HACKRF_PLATFORM_RAD1O = 1 << 2;

        /// <summary>
        /// HACKRF ONE (r9 or later) platform bit in result of @ref hackrf_supported_platform_read
        /// </summary>
        public const uint HACKRF_PLATFORM_HACKRF1_R9 = 1 << 3;

        /// <summary>
        /// These deprecated board ID names are provided for API compatibility.
        /// </summary>
        public const HackrfBoardId BOARD_ID_HACKRF_ONE = HackrfBoardId.BOARD_ID_HACKRF1_OG;

        /// <summary>
        /// These deprecated board ID names are provided for API compatibility.
        /// </summary>
        public const HackrfBoardId BOARD_ID_INVALID = HackrfBoardId.BOARD_ID_UNDETECTED;
    }
}
