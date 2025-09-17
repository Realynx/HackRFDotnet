using HackRFDotnet.NativeApi.Lib;

namespace HackRFDotnet.NativeApi.Enums {
    /// <summary>
    /// RF filter path setting enum.
    ///
    /// Used only when performing explicit tuning using <see cref="HackRfNativeLib.DeviceStreaming.SetFrequency(HackRFDotnet.NativeApi.Structs.Devices.HackRFDevice*, ulong, ulong, RfPathFilter)"/>,
    /// or can be converted into a human-readable string using <see cref="HackRfNativeLib.DeviceStreaming.FilterPathName"/>.
    ///
    /// This can select the image rejection filter (U3, U8 or none) to use - using switches U5, U6, U9 and U11. When no filter is selected, the mixer itself is bypassed.
    /// </summary>
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