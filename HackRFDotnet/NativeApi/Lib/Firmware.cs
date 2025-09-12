using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public unsafe static class Firmware {
        /// <summary>
        /// Enable / disable CLKOUT
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_clkout_enable")]
        public static extern int SetClkoutEnable(HackRFDevice* device, byte value);

        /// <summary>
        /// Get CLKIN status
        /// Check if an external clock signal is detected on the CLKIN port.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_clkin_status")]
        public static extern int GetClkinStatus(HackRFDevice* device, byte* status);

        /// <summary>
        /// Get the state of the M0 code on the LPC43xx MCU
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_m0_state")]
        public static extern int GetMcuState(HackRFDevice* device, HackRFM0State* value);

        /// <summary>
        /// Lookup platform ID (HACKRF_PLATFORM_xxx) from board id (@ref hackrf_board_id)
        /// </summary>
        /// <param name="board_id"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_id_platform")]
        public static extern uint LookupBoardIdPlatform(HackrfBoardId board_id);

        /// <summary>
        /// Read version as MM.mm 16-bit value, where MM is the major and mm is the minor version, encoded as the hex digits of the 16-bit number. 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_usb_api_version_read")]
        public static extern int ReadUsbApiVersion(HackRFDevice* device, ushort* version);

        /// <summary>
        /// Erase firmware image on the SPI flash
        /// Should be followed by writing a new image, or the HackRF will be soft-bricked (still rescuable in DFU mode)
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_erase")]
        public static extern int EraseSpiflash(HackRFDevice* device);

        /// <summary>
        /// Write firmware image on the SPI flash
        /// Should only be used for firmware updating. Can brick the device, but it's still rescuable in DFU mode.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_write")]
        public static extern int WriteSpiflash(HackRFDevice* device, uint address, ushort length, byte* data);

        /// <summary>
        /// Read firmware image on the SPI flash
        /// Should only be used for firmware verification.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_read")]
        public static extern int ReadSpiflash(HackRFDevice* device, uint address, ushort length, byte* data);

        /// <summary>
        /// Read the status registers of the W25Q80BV SPI flash chip
        /// See the datasheet for details of the status registers. The two registers are read in order.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_status")]
        public static extern int SpiflashStatus(HackRFDevice* device, byte* data);

        /// <summary>
        /// Clear the status registers of the W25Q80BV SPI flash chip
        /// See the datasheet for details of the status registers.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_clear_status")]
        public static extern int ClearSpiflashStatus(HackRFDevice* device);

        /// <summary>
        /// Write configuration bitstream into the XC2C64A-7VQ100C CPLD
        /// device will need to be reset after hackrf_cpld_write 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <param name="total_length"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_cpld_write")]
        public static extern int WriteCpld(HackRFDevice* device, byte* data, uint total_length);

        /// <summary>
        /// Read @ref hackrf_board_id from a device
        /// The result can be converted into a human-readable string via @ref hackrf_board_id_name
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_id_read")]
        public static extern int ReadBoardId(HackRFDevice* device, byte* value);

        /// <summary>
        /// Read HackRF firmware version as a string
        /// </summary>
        /// <param name="device"></param>
        /// <param name="version"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_version_string_read")]
        public static extern int ReadVersion(HackRFDevice* device, byte* version, byte length);

        /// <summary>
        /// Directly read the registers of the MAX2837 transceiver IC,
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="register_number"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_max2837_read")]
        public static extern int ReadMax2837(HackRFDevice* device, byte register_number, ushort* value);

        /// <summary>
        /// Directly write the registers of the MAX2837 transceiver IC
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="register_number"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_max2837_write")]
        public static extern int hackrf_max2837_write(HackRFDevice* device, byte register_number, ushort value);

        /// <summary>
        /// Directly read the registers of the Si5351C clock generator IC
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="register_number"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_si5351c_read")]
        public static extern int ReadSi5351c(HackRFDevice* device, ushort register_number, ushort* value);

        /// <summary>
        /// Directly write the registers of the Si5351 clock generator IC
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="register_number"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_si5351c_write")]
        public static extern int WriteSi5351c(HackRFDevice* device, ushort register_number, ushort value);

        /// <summary>
        /// Directly read the registers of the RFFC5071/5072 mixer-synthesizer IC
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="register_number"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_rffc5071_read")]
        public static extern int ReadRffc5071(HackRFDevice* device, byte register_number, ushort* value);

        /// <summary>
        /// Directly write the registers of the RFFC5071/5072 mixer-synthesizer IC
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="register_number"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_rffc5071_write")]
        public static extern int WriteRffc5071(HackRFDevice* device, byte register_number, ushort value);

        /// <summary>
        /// Read board part ID and serial number
        /// Read MCU part id and serial number. See the documentation of the MCU for details!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="read_partid_serialno"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_partid_serialno_read")]
        public static extern int ReadBoardPartIdSerialNo(HackRFDevice* device, ReadPartidSerialNo* read_partid_serialno);

        /// <summary>
        /// Convert @ref hackrf_board_id into human-readable string
        /// </summary>
        /// <param name="board_id"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_id_name")]
        public static extern sbyte* BoardIdName(HackrfBoardId board_id);

        /// <summary>
        /// Convert @ref hackrf_usb_board_id into human-readable string.
        /// </summary>
        /// <param name="usb_board_id"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_usb_board_id_name")]
        public static extern sbyte* UsbBoardIdName(HackrfUsbBoardId usb_board_id);

        /// <summary>
        /// Set hardware sync mode (hardware triggering)
        /// See the documentation on hardware triggering for details
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_hw_sync_mode")]
        public static extern int SetHardwareSyncMode(HackRFDevice* device, byte value);

        /// <summary>
        /// Read board revision of device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_rev_read")]
        public static extern int ReadBoardRev(HackRFDevice* device, byte* value);

        /// <summary>
        /// Convert board revision name
        /// </summary>
        /// <param name="board_rev"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_rev_name")]
        public static extern nint BoardRevName(HackrfBoardRev board_rev);

        /// <summary>
        /// Read supported platform of device
        /// Returns a combination of @ref HACKRF_PLATFORM_JAWBREAKER | @ref HACKRF_PLATFORM_HACKRF1_OG | @ref HACKRF_PLATFORM_RAD1O | @ref HACKRF_PLATFORM_HACKRF1_R9
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_supported_platform_read")]
        public static extern int ReadSupportedPlatform(HackRFDevice* device, uint* value);

#if HACKRF_ISSUE_609_IS_FIXED
        /// <summary>
        /// Read CPLD checksum
        /// This function is not always available, see [issue 609](https://github.com/greatscottgadgets/hackrf/issues/609)
        /// </summary>
        /// <param name="device"></param>
        /// <param name="crc"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_cpld_checksum")]
        public static extern int CpldChecksum(HackRFDevice* device, uint* crc);
#endif
    }
}