namespace HackRFDotnet.NativeApi.Enums.System {
    public enum HackrfBoardRev {
        /// <summary>
        /// Older than rev6
        /// </summary>
        BOARD_REV_HACKRF1_OLD = 0,

        /// <summary>
        /// Board revision 6, generic
        /// </summary>
        BOARD_REV_HACKRF1_R6 = 1,

        /// <summary>
        /// Board revision 7, generic
        /// </summary>
        BOARD_REV_HACKRF1_R7 = 2,

        /// <summary>
        /// Board revision 8, generic
        /// </summary>
        BOARD_REV_HACKRF1_R8 = 3,

        /// <summary>
        /// Board revision 9, generic
        /// </summary>
        BOARD_REV_HACKRF1_R9 = 4,

        /// <summary>
        /// Board revision 10, generic
        /// </summary>
        BOARD_REV_HACKRF1_R10 = 5,

        /// <summary>
        /// Board revision 6, made by GSG
        /// </summary>
        BOARD_REV_GSG_HACKRF1_R6 = 0x81,

        /// <summary>
        /// Board revision 7, made by GSG
        /// </summary>
        BOARD_REV_GSG_HACKRF1_R7 = 0x82,

        /// <summary>
        /// Board revision 8, made by GSG
        /// </summary>
        BOARD_REV_GSG_HACKRF1_R8 = 0x83,

        /// <summary>
        /// Board revision 9, made by GSG
        /// </summary>
        BOARD_REV_GSG_HACKRF1_R9 = 0x84,

        /// <summary>
        /// Board revision 10, made by GSG
        /// </summary>
        BOARD_REV_GSG_HACKRF1_R10 = 0x85,

        /// <summary>
        /// Unknown board revision (detection failed)
        /// </summary>
        BOARD_REV_UNRECOGNIZED = 0xFE,

        /// <summary>
        /// Unknown board revision (detection not yet attempted)
        /// </summary>
        BOARD_REV_UNDETECTED = 0xFF,
    }
}