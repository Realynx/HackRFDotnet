using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums.System;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public static unsafe class Debug {
        /// <summary>
        /// Convert <see cref="HackrfError"/> into human-readable string.
        /// </summary>
        /// <param name="errcode">Enum to convert.</param>
        /// <returns>Human-readable name of error.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_error_name")]
        public static extern sbyte* GetErrorName(HackrfError errcode);

    }
}
