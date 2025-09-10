using HackRFDotnet.Enums;

namespace HackRFDotnet.Constants {
    public static class HackRfConstants {
        public const uint SAMPLES_PER_BLOCK = 8192;
        public const uint BYTES_PER_BLOCK = 16384;
        public const uint MAX_SWEEP_RANGES = 10;

        public const uint HACKRF_OPERACAKE_ADDRESS_INVALID = 0xFF;
        public const uint HACKRF_OPERACAKE_MAX_BOARDS = 8;
        public const uint HACKRF_OPERACAKE_MAX_DWELL_TIMES = 16;
        public const uint HACKRF_OPERACAKE_MAX_FREQ_RANGES = 8;

        public const uint HACKRF_BOARD_REV_GSG = (0x80);
        public const uint HACKRF_PLATFORM_JAWBREAKER = (1 << 0);
        public const uint HACKRF_PLATFORM_HACKRF1_OG = (1 << 1);
        public const uint HACKRF_PLATFORM_RAD1O = (1 << 2);
        public const uint HACKRF_PLATFORM_HACKRF1_R9 = (1 << 3);

        public const HackrfBoardId BOARD_ID_HACKRF_ONE = HackrfBoardId.BOARD_ID_HACKRF1_OG;
        public const HackrfBoardId BOARD_ID_INVALID = HackrfBoardId.BOARD_ID_UNDETECTED;
    }
}
