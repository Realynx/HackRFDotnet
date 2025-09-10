namespace HackRFDotnet.Enums {
    public enum OperacakeSwitchingMode {
        /**
         * Port connections are set manually using @ref hackrf_set_operacake_ports. Both ports can be specified, but not on the same side.
         */
        OPERACAKE_MODE_MANUAL,
        /**
         * Port connections are switched automatically when the frequency is changed. Frequency ranges can be set using @ref hackrf_set_operacake_freq_ranges. In this mode, B0 mirrors A0
         */
        OPERACAKE_MODE_FREQUENCY,
        /**
         * Port connections are switched automatically over time. dwell times can be set with @ref hackrf_set_operacake_dwell_times. In this mode, B0 mirrors A0
         */
        OPERACAKE_MODE_TIME,
    }
}
