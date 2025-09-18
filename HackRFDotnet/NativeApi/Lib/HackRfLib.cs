using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Lib {
    public static unsafe partial class HackRfNativeLib {
        /// <summary>
        /// Initialize libhackrf. Should be called before any other function.
        /// </summary>
        [LibraryImport(NativeConstants.HACK_RF_DLL, EntryPoint = "hackrf_init")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int Init();

        /// <summary>
        /// Exit libhackrf. Should be called before application exit.
        /// </summary>
        [LibraryImport(NativeConstants.HACK_RF_DLL, EntryPoint = "hackrf_exit")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int Exit();

        /// <summary>
        /// Get library version string.
        /// </summary>
        [LibraryImport(NativeConstants.HACK_RF_DLL, EntryPoint = "hackrf_library_version")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial sbyte* LibraryVersion();

        /// <summary>
        /// Get library release string.
        /// </summary>
        [LibraryImport(NativeConstants.HACK_RF_DLL, EntryPoint = "hackrf_library_release")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial sbyte* LibraryRelease();
    }
}