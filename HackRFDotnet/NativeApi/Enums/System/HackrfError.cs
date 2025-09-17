namespace HackRFDotnet.NativeApi.Enums.System {
    /// <summary>
    /// Error enum, returned by many libhackrf functions.
    /// </summary>
    public enum HackrfError : int {
        ///<summary>
        /// no error happened
        ///</summary>
        HACKRF_SUCCESS = 0,

        ///<summary>
        /// TRUE value, returned by some functions that return boolean value. Only a few functions can return this variant, and this fact should be explicitly noted at those functions.
        ///</summary>
        HACKRF_TRUE = 1,

        ///<summary>
        /// The function was called with invalid parameters.
        ///</summary>
        HACKRF_ERROR_INVALID_PARAM = -2,

        ///<summary>
        /// USB device not found, returned at opening.
        ///</summary>
        HACKRF_ERROR_NOT_FOUND = -5,

        ///<summary>
        /// Resource is busy, possibly the device is already opened.
        ///</summary>
        HACKRF_ERROR_BUSY = -6,

        ///<summary>
        /// Memory allocation (on host side) failed
        ///</summary>
        HACKRF_ERROR_NO_MEM = -11,

        ///<summary>
        /// LibUSB error, use @ref hackrf_error_name to get a human-readable error string (using `libusb_strerror`)
        ///</summary>
        HACKRF_ERROR_LIBUSB = -1000,

        ///<summary>
        /// Error setting up transfer thread (pthread-related error)
        ///</summary>
        HACKRF_ERROR_THREAD = -1001,

        ///<summary>
        /// Streaming thread could not start due to an error
        ///</summary>
        HACKRF_ERROR_STREAMING_THREAD_ERR = -1002,

        ///<summary>
        /// Streaming thread stopped due to an error
        ///</summary>
        HACKRF_ERROR_STREAMING_STOPPED = -1003,

        ///<summary>
        /// Streaming thread exited (normally)
        ///</summary>
        HACKRF_ERROR_STREAMING_EXIT_CALLED = -1004,

        ///<summary>
        /// The installed firmware does not support this function
        ///</summary>
        HACKRF_ERROR_USB_API_VERSION = -1005,

        ///<summary>
        /// Can not exit library as one or more HackRFs still in use
        ///</summary>
        HACKRF_ERROR_NOT_LAST_DEVICE = -2000,

        ///<summary>
        /// Unspecified error
        ///</summary>
        HACKRF_ERROR_OTHER = -9999,
    }
}