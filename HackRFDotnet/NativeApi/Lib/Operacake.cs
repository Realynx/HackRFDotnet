using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums.Peripherals;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public static unsafe class Operacake {
        /// <summary>
        /// Query connected Opera Cake boards
        /// Returns a @ref HACKRF_OPERACAKE_MAX_BOARDS size array of addresses, with @ref HACKRF_OPERACAKE_ADDRESS_INVALID as a placeholder
        /// </summary>
        /// <param name="device"></param>
        /// <param name="boards"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_operacake_boards")]
        public static extern int GetOperacakeBoards(HackRFDevice* device, byte* boards);

        /// <summary>
        /// Setup Opera Cake operation mode
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_operacake_mode")]
        public static extern int SetOperacakeMode(HackRFDevice* device, byte address, OperacakeSwitchingMode mode);

        /// <summary>
        /// Query Opera Cake mode
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_operacake_mode")]
        public static extern int GetOperacakeMode(HackRFDevice* device, byte address, OperacakeSwitchingMode* mode);

        /// <summary>
        /// Setup Opera Cake ports in @ref OPERACAKE_MODE_MANUAL mode operation
        /// Should be called after @ref hackrf_set_operacake_mode. A0 and B0 must be connected to
        /// opposite sides (A->A and B->B or A->B and B->A but not A->A and B->A or A->B and B->B)
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="port_a"></param>
        /// <param name="port_b"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_operacake_ports")]
        public static extern int SetOperacakePorts(HackRFDevice* device, byte address, byte port_a, byte port_b);

        /// <summary>
        /// Setup Opera Cake dwell times in @ref OPERACAKE_MODE_TIME mode operation
        /// Should be called after @ref hackrf_set_operacake_mode
        /// **Note:** this configuration applies to all Opera Cake boards in @ref OPERACAKE_MODE_TIME mode 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="dwell_times"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_operacake_dwell_times")]
        public static extern int SetOperacakeDwellTimes(HackRFDevice* device, HackRFOperacakeDwellTime* dwell_times, byte count);

        /// <summary>
        /// Setup Opera Cake frequency ranges in @ref OPERACAKE_MODE_FREQUENCY mode operation
        /// Should be called after @ref hackrf_set_operacake_mode
        /// **Note:** this configuration applies to all Opera Cake boards in @ref OPERACAKE_MODE_FREQUENCY mode
        /// </summary>
        /// <param name="device"></param>
        /// <param name="freq_ranges"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_operacake_freq_ranges")]
        public static extern int SetOperacakeFrequencyRanges(HackRFDevice* device, HackRFOperacakeFreqRange* freq_ranges, byte count);

        /// <summary>
        /// Setup Opera Cake frequency ranges in @ref OPERACAKE_MODE_FREQUENCY mode operation
        /// Old function to set ranges with. Use @ref hackrf_set_operacake_freq_ranges instead!
        /// **Note:** this configuration applies to all Opera Cake boards in @ref OPERACAKE_MODE_FREQUENCY mode
        /// </summary>
        /// <param name="device"></param>
        /// <param name="ranges"></param>
        /// <param name="num_ranges"></param>
        /// <returns></returns>
        [Obsolete("Use hackrf_set_operacake_freq_ranges instead.")]
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_operacake_ranges")]
        public static extern int SetOperacakeRanges(HackRFDevice* device, byte* ranges, byte num_ranges);

        /// <summary>
        /// Perform GPIO test on an Opera Cake addon board
        ///  Value 0xFFFF means "GPIO mode disabled", and hackrf_operacake advises to remove additional add-on boards and retry.
        ///  Value 0 means all tests passed.
        ///  In any other values, a 1 bit signals an error. Bits are grouped in groups of 3. Encoding:
        ///  0 - u1ctrl - u3ctrl0 - u3ctrl1 - u2ctrl0 - u2ctrl1
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="test_result"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_operacake_gpio_test")]
        public static extern int OperacakeGpioTest(HackRFDevice* device, byte address, ushort* test_result);
    }
}
