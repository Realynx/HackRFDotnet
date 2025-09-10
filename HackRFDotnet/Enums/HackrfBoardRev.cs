namespace HackRFDotnet.Enums {
    public enum HackrfBoardRev {
        /**
	     * Older than rev6
	     */
        BOARD_REV_HACKRF1_OLD = 0,
        /**
         * board revision 6, generic
         */
        BOARD_REV_HACKRF1_R6 = 1,
        /**
         * board revision 7, generic
         */
        BOARD_REV_HACKRF1_R7 = 2,
        /**
         * board revision 8, generic
         */
        BOARD_REV_HACKRF1_R8 = 3,
        /**
         * board revision 9, generic
         */
        BOARD_REV_HACKRF1_R9 = 4,
        /**
         * board revision 10, generic
         */
        BOARD_REV_HACKRF1_R10 = 5,

        /**
         * board revision 6, made by GSG
         */
        BOARD_REV_GSG_HACKRF1_R6 = 0x81,
        /**
         * board revision 7, made by GSG
         */
        BOARD_REV_GSG_HACKRF1_R7 = 0x82,
        /**
         * board revision 8, made by GSG
         */
        BOARD_REV_GSG_HACKRF1_R8 = 0x83,
        /**
         * board revision 9, made by GSG
         */
        BOARD_REV_GSG_HACKRF1_R9 = 0x84,
        /**
         * board revision 10, made by GSG
         */
        BOARD_REV_GSG_HACKRF1_R10 = 0x85,

        /**
         * unknown board revision (detection failed)
         */
        BOARD_REV_UNRECOGNIZED = 0xFE,
        /**
         * unknown board revision (detection not yet attempted)
         */
        BOARD_REV_UNDETECTED = 0xFF,
    }
}
