namespace HackRFDotnet.NativeApi.Enums.System {
    public enum HackrfError : int {
        /**
	     * no error happened
	     */
        HACKRF_SUCCESS = 0,
        /**
         * TRUE value, returned by some functions that return boolean value. Only a few functions can return this variant, and this fact should be explicitly noted at those functions.
         */
        HACKRF_TRUE = 1,
        /**
         * The function was called with invalid parameters.
         */
        HACKRF_ERROR_INVALID_PARAM = -2,
        /**
         * USB device not found, returned at opening.
         */
        HACKRF_ERROR_NOT_FOUND = -5,
        /**
         * Resource is busy, possibly the device is already opened.
         */
        HACKRF_ERROR_BUSY = -6,
        /**
         * Memory allocation (on host side) failed
         */
        HACKRF_ERROR_NO_MEM = -11,
        /**
         * LibUSB error, use @ref hackrf_error_name to get a human-readable error string (using `libusb_strerror`)
         */
        HACKRF_ERROR_LIBUSB = -1000,
        /**
         * Error setting up transfer thread (pthread-related error)
         */
        HACKRF_ERROR_THREAD = -1001,
        /**
         * Streaming thread could not start due to an error
         */
        HACKRF_ERROR_STREAMING_THREAD_ERR = -1002,
        /**
         * Streaming thread stopped due to an error
         */
        HACKRF_ERROR_STREAMING_STOPPED = -1003,
        /**
         * Streaming thread exited (normally)
         */
        HACKRF_ERROR_STREAMING_EXIT_CALLED = -1004,
        /**
         * The installed firmware does not support this function
         */
        HACKRF_ERROR_USB_API_VERSION = -1005,
        /**
         * Can not exit library as one or more HackRFs still in use
         */
        HACKRF_ERROR_NOT_LAST_DEVICE = -2000,
        /**
         * Unspecified error
         */
        HACKRF_ERROR_OTHER = -9999,
    }
}
