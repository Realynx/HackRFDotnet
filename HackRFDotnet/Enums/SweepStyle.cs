namespace HackRFDotnet.Enums {
    public enum SweepStyle {
        /**
         * step_width is added to the current frequency at each step.
         */
        LINEAR = 0,
        /**
         * each step is divided into two interleaved sub-steps, allowing the host to select the best portions of the FFT of each sub-step and discard the rest.
         */
        INTERLEAVED = 1,
    }
}
