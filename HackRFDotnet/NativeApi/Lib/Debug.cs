using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums.System;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public static unsafe partial class Debug {
        /// <summary>
        /// Convert <see cref="HackrfError"/> into human-readable string.
        /// </summary>
        /// <param name="errcode">Enum to convert.</param>
        /// <returns>Human-readable name of error.</returns>
        [LibraryImport(NativeConstants.HACK_RF_DLL, EntryPoint = "hackrf_error_name")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial sbyte* GetErrorName(HackrfError errcode);

    }
}
