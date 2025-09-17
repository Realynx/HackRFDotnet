namespace HackRFDotnet.NativeApi.Enums {
    public enum SweepStyle {
        /// <summary>
        /// step_width is added to the current frequency at each step.
        /// </summary>
        LINEAR = 0,

        /// <summary>
        /// Each step is divided into two interleaved sub-steps, allowing the host to select the best portions of the FFT of each sub-step and discard the rest.
        /// </summary>
        INTERLEAVED = 1,
    }
}