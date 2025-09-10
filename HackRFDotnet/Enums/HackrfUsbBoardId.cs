namespace HackRFDotnet.Enums {
    public enum HackrfUsbBoardId {
        /**
	     * Jawbreaker (beta platform) USB product id
	     */
        USB_BOARD_ID_JAWBREAKER = 0x604B,
        /**
         * HackRF One USB product id
         */
        USB_BOARD_ID_HACKRF_ONE = 0x6089,
        /**
         * RAD1O (custom version) USB product id
         */
        USB_BOARD_ID_RAD1O = 0xCC15,
        /**
         * Invalid / unknown USB product id
         */
        USB_BOARD_ID_INVALID = 0xFFFF,
    }
}
