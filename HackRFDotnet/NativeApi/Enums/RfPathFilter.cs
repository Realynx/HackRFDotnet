namespace HackRFDotnet.NativeApi.Enums {
    public enum RfPathFilter {
        /**
         * No filter is selected, **the mixer is bypassed**, \f$f_{center} = f_{IF}\f$
         */
        RF_PATH_FILTER_BYPASS = 0,
        /**
         * LPF is selected, \f$f_{center} = f_{IF} - f_{LO}\f$
         */
        RF_PATH_FILTER_LOW_PASS = 1,
        /**
         * HPF is selected, \f$f_{center} = f_{IF} + f_{LO}\f$
         */
        RF_PATH_FILTER_HIGH_PASS = 2,
    }
}
