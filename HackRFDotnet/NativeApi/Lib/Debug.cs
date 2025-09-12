using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public static unsafe class Debug {
        /// <summary>
        /// Convert @ref hackrf_error into human-readable string
        /// </summary>
        /// <param name="errcode"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_error_name")]
        public static extern sbyte* GetErrorName(HackrfError errcode);

    }
}
