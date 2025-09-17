namespace HackRFDotnet.NativeApi.Enums {
    public enum RfPathFilter {
        /// <summary>
        /// No filter is selected, **the mixer is bypassed**, \f$f_{center} = f_{IF}\f$
        /// </summary>
        RF_PATH_FILTER_BYPASS = 0,

        /// <summary>
        /// LPF is selected, \f$f_{center} = f_{IF} - f_{LO}\f$
        /// </summary>
        RF_PATH_FILTER_LOW_PASS = 1,

        /// <summary>
        /// HPF is selected, \f$f_{center} = f_{IF} + f_{LO}\f$
        /// </summary>
        RF_PATH_FILTER_HIGH_PASS = 2,
    }
}