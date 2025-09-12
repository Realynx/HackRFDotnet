namespace HackRFDotnet.NativeApi.Enums.System {
    public enum HackrfBoardId {
        /**
	     * Jellybean (pre-production revision, not supported)
	     */
        BOARD_ID_JELLYBEAN = 0,
        /**
         * Jawbreaker (beta platform, 10-6000MHz, no bias-tee)
         */
        BOARD_ID_JAWBREAKER = 1,
        /**
         * HackRF One (prior to rev 9, same limits: 1-6000MHz, 20MSPS, bias-tee)
         */
        BOARD_ID_HACKRF1_OG = 2,
        /**
         * RAD1O (Chaos Computer Club special edition with LCD & other features. 50M-4000MHz, 20MSPS, no bias-tee)
         */
        BOARD_ID_RAD1O = 3,
        /**
         * HackRF One (rev. 9 & later. 1-6000MHz, 20MSPS, bias-tee)
         */
        BOARD_ID_HACKRF1_R9 = 4,
        /**
         * Unknown board (failed detection)
         */
        BOARD_ID_UNRECOGNIZED = 0xFE,
        /**
         * Unknown board (detection not yet attempted, should be default value)
         */
        BOARD_ID_UNDETECTED = 0xFF,
    }
}
