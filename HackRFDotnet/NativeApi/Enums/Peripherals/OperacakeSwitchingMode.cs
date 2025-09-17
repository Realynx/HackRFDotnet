using HackRFDotnet.NativeApi.Lib;

namespace HackRFDotnet.NativeApi.Enums.Peripherals {
    /// <summary>
    /// Opera Cake port switching mode. Set via <see cref="HackRfNativeLib.Operacake.SetOperacakeMode"/> and queried via <see cref="HackRfNativeLib.Operacake.GetOperacakeMode"/>.
    /// </summary>
    public enum OperacakeSwitchingMode {
        /// <summary>
        /// Port connections are set manually using <see cref="HackRfNativeLib.Operacake.SetOperacakePorts"/>. Both ports can be specified, but not on the same side.
        /// </summary>
        OPERACAKE_MODE_MANUAL,

        /// <summary>
        /// Port connections are switched automatically when the frequency is changed. Frequency ranges can be set using <see cref="HackRfNativeLib.Operacake.SetOperacakeFrequencyRanges"/>.
        /// In this mode, B0 mirrors A0.
        /// </summary>
        OPERACAKE_MODE_FREQUENCY,

        /// <summary>
        /// Port connections are switched automatically over time. dwell times can be set with <see cref="HackRfNativeLib.Operacake.SetOperacakeDwellTimes"/>.
        /// In this mode, B0 mirrors A0.
        /// </summary>
        OPERACAKE_MODE_TIME,
    }
}