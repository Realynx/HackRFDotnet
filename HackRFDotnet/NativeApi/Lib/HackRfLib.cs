using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Lib {
    public static unsafe partial class HackRfNativeLib {
        /// <summary>
        /// Initialize libhackrf. Should be called before any other function.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_init")]
        public static extern int Init();

        /// <summary>
        /// Exit libhackrf. Should be called before application exit.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_exit")]
        public static extern int Exit();

        /// <summary>
        /// Get library version string.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_library_version")]
        public static extern sbyte* LibraryVersion();

        /// <summary>
        /// Get library release string.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_library_release")]
        public static extern sbyte* LibraryRelease();
    }
}
