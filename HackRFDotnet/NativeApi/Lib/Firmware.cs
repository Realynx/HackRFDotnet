using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums.System;
using HackRFDotnet.NativeApi.Structs.Devices;
using HackRFDotnet.NativeApi.Structs.System;

namespace HackRFDotnet.NativeApi.Lib;
public static partial class HackRfNativeLib {
    public unsafe static class Firmware {
        /// <summary>
        /// Enable / disable CLKOUT.
        /// 
        /// Requires USB API version 0x0103 or above!
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">Clock output enabled (0/1).</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_clkout_enable")]
        public static extern HackrfError SetClkoutEnable(HackRFDevice* device, byte value);

        /// <summary>
        /// Get CLKIN status.
        ///
        /// Check if an external clock signal is detected on the CLKIN port.
        ///
        /// Requires USB API version 0x0106 or above!
        /// </summary>
        /// <param name="device">Device to read status from.</param>
        /// <param name="status">External clock detected (0/1).</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_clkin_status")]
        public static extern HackrfError GetClkinStatus(HackRFDevice* device, byte* status);

        /// <summary>
        /// Get the state of the M0 code on the LPC43xx MCU.
        ///
        /// Requires USB API version 0x0106 or above!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="value">MCU code state.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_m0_state")]
        public static extern HackrfError GetMcuState(HackRFDevice* device, HackRFM0State* value);

        /// <summary>Lookup platform ID (HACKRF_PLATFORM_xxx) from board id (<see cref="HackrfBoardId"/>).</summary>
        /// <param name="board_id"><see cref="HackrfBoardId"/> enum variant to convert.</param>
        /// <returns>
        /// <see cref="NativeConstants.HACKRF_PLATFORM_JAWBREAKER"/>, <see cref="NativeConstants.HACKRF_PLATFORM_HACKRF1_OG"/>, <see cref="NativeConstants.HACKRF_PLATFORM_RAD1O"/>,
        /// <see cref="NativeConstants.HACKRF_PLATFORM_HACKRF1_R9"/> or 0
        /// </returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_id_platform")]
        public static extern uint LookupBoardIdPlatform(HackrfBoardId board_id);

        /// <summary>
        /// Read HackRF USB API version.
        ///
        /// Read version as MM.mm 16-bit value, where MM is the major and mm is the minor version, encoded as the hex digits of the 16-bit number.
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="version">USB API version.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_usb_api_version_read")]
        public static extern HackrfError ReadUsbApiVersion(HackRFDevice* device, ushort* version);

        /// <summary>
        /// Erase firmware image on the SPI flash.
        ///
        /// Should be followed by writing a new image, or the HackRF will be soft-bricked (still rescuable in DFU mode).
        /// </summary>
        /// <param name="device">Device to erase.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_erase")]
        public static extern HackrfError EraseSpiflash(HackRFDevice* device);

        /// <summary>
        /// Write firmware image on the SPI flash.
        ///
        /// Should only be used for firmware updating. Can brick the device, but it's still rescuable in DFU mode.
        /// </summary>
        /// <param name="device">Device to write on.</param>
        /// <param name="address">Address to write to. Should start at 0.</param>
        /// <param name="length">Length of data to write. Must be at most 256.</param>
        /// <param name="data">Data to write.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_write")]
        public static extern HackrfError WriteSpiflash(HackRFDevice* device, uint address, ushort length, byte* data);

        /// <summary>
        /// Read firmware image on the SPI flash.
        ///
        /// Should only be used for firmware verification.
        /// </summary>
        /// <param name="device">Device to read from.</param>
        /// <param name="address">Address to read from. Firmware should start at 0</param>
        /// <param name="length">Length of data to read. Must be at most 256. </param>
        /// <param name="data">Pointer to buffer.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_read")]
        public static extern HackrfError ReadSpiflash(HackRFDevice* device, uint address, ushort length, byte* data);

        /// <summary>
        /// Read the status registers of the W25Q80BV SPI flash chip.
        ///
        /// See the datasheet for details of the status registers. The two registers are read in order.
        ///
        /// Requires USB API version 0x0103 or above!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="data">char[2] array of the status registers.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_status")]
        public static extern HackrfError SpiflashStatus(HackRFDevice* device, byte* data);

        /// <summary>
        /// Clear the status registers of the W25Q80BV SPI flash chip.
        ///
        /// See the datasheet for details of the status registers.
        ///
        /// Requires USB API version 0x0103 or above!
        /// </summary>
        /// <param name="device">Device to clear.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_spiflash_clear_status")]
        public static extern HackrfError ClearSpiflashStatus(HackRFDevice* device);

        /// <summary>
        /// Write configuration bitstream into the XC2C64A-7VQ100C CPLD.
        ///
        /// Device will need to be reset after hackrf_cpld_write.
        /// </summary>
        /// <param name="device">device to configure.</param>
        /// <param name="data">CPLD bitstream data.</param>
        /// <param name="total_length">length of the bitstream to write.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [Obsolete("This function writes the bitstream, but the firmware auto-overrides at each reset, so no changes will take effect.")]
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_cpld_write")]
        public static extern HackrfError WriteCpld(HackRFDevice* device, byte* data, uint total_length);

        /// <summary>
        /// Read <see cref="HackrfBoardId"/> from a device.
        ///
        /// The result can be converted into a human-readable string via <see cref="HackrfBoardId"/>.
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="value"><see cref="HackrfBoardId"/> enum value.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_id_read")]
        public static extern HackrfError ReadBoardId(HackRFDevice* device, byte* value);

        /// <summary>Read HackRF firmware version as a string.</summary>
        /// <param name="device">Device to query.</param>
        /// <param name="version">Version string.</param>
        /// <param name="length">Length of allocated string **without null byte** (so set it to `length(arr)-1`).</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_version_string_read")]
        public static extern HackrfError ReadVersion(HackRFDevice* device, byte* version, byte length);

        /// <summary>
        /// Directly read the registers of the MAX2837 transceiver IC.
        ///
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="register_number">Register number to read.</param>
        /// <param name="value">Value of the specified register.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_max2837_read")]
        public static extern HackrfError ReadMax2837(HackRFDevice* device, byte register_number, ushort* value);

        /// <summary>
        /// Directly write the registers of the MAX2837 transceiver IC.
        ///
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="register_number">Register number to read.</param>
        /// <param name="value">Value of the specified register.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_max2837_write")]
        public static extern HackrfError hackrf_max2837_write(HackRFDevice* device, byte register_number, ushort value);

        /// <summary>
        /// Directly read the registers of the Si5351C clock generator IC.
        ///
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="register_number">Register number to read.</param>
        /// <param name="value">Value of the specified register.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_si5351c_read")]
        public static extern HackrfError ReadSi5351c(HackRFDevice* device, ushort register_number, ushort* value);

        /// <summary>
        /// Directly write the registers of the Si5351 clock generator IC.
        ///
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device">Device to write.</param>
        /// <param name="register_number">Register number to write.</param>
        /// <param name="value">Value to write in the specified register.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_si5351c_write")]
        public static extern HackrfError WriteSi5351c(HackRFDevice* device, ushort register_number, ushort value);

        /// <summary>
        /// Directly read the registers of the RFFC5071/5072 mixer-synthesizer IC.
        ///
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="register_number">Register number to read.</param>
        /// <param name="value">Value of the specified register.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_rffc5071_read")]
        public static extern HackrfError ReadRffc5071(HackRFDevice* device, byte register_number, ushort* value);

        /// <summary>
        /// Directly write the registers of the RFFC5071/5072 mixer-synthesizer IC.
        ///
        /// Intended for debugging purposes only!
        /// </summary>
        /// <param name="device">Device to write.</param>
        /// <param name="register_number">Register number to write.</param>
        /// <param name="value">Value to write in the specified register.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_rffc5071_write")]
        public static extern HackrfError WriteRffc5071(HackRFDevice* device, byte register_number, ushort value);

        /// <summary>
        /// Read board part ID and serial number.
        ///
        /// Read MCU part id and serial number. See the documentation of the MCU for details!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="read_partid_serialno">Result of query.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_partid_serialno_read")]
        public static extern HackrfError ReadBoardPartIdSerialNo(HackRFDevice* device, ReadPartidSerialNo* read_partid_serialno);

        /// <summary>Convert <see cref="HackrfBoardId"/> into human-readable string.</summary>
        /// <param name="board_id">Enum to convert.</param>
        /// <returns>Human-readable name of board id.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_id_name")]
        public static extern sbyte* BoardIdName(HackrfBoardId board_id);

        /// <summary>Convert <see cref="HackrfBoardId"/> into human-readable string.</summary>
        /// <param name="usb_board_id">Enum to convert.</param>
        /// <returns>Human-readable name of board id.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_usb_board_id_name")]
        public static extern sbyte* UsbBoardIdName(HackrfUsbBoardId usb_board_id);

        /// <summary>
        /// Set hardware sync mode (hardware triggering).
        ///
        /// See the documentation on hardware triggering for details.
        ///
        /// Requires USB API version 0x0102 or above!
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">Enable (1) or disable (0) hardware triggering.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_hw_sync_mode")]
        public static extern HackrfError SetHardwareSyncMode(HackRFDevice* device, byte value);

        /// <summary>Read board revision of device.</summary>
        /// <param name="device">Device to read board revision from.</param>
        /// <param name="value">Revision enum, will become one of <see cref="HackrfBoardRev"/>. Should be initialized with <see cref="HackrfBoardRev.BOARD_REV_UNDETECTED"/>.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError.HACKRF_ERROR_LIBUSB"/>.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_rev_read")]
        public static extern HackrfError ReadBoardRev(HackRFDevice* device, HackrfBoardRev* value);

        /// <summary>Convert board revision name.</summary>
        /// <param name="board_rev">Board revision enum from <see cref="ReadBoardRev"/>.</param>
        /// <returns>Human-readable name of board revision. Discards GSG bit.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_board_rev_name")]
        public static extern sbyte* BoardRevName(HackrfBoardRev board_rev);

        /// <summary>
        /// Read supported platform of device.
        ///
        /// Returns a combination of <see cref="NativeConstants.HACKRF_PLATFORM_JAWBREAKER"/> | <see cref="NativeConstants.HACKRF_PLATFORM_HACKRF1_OG"/> |
        /// <see cref="NativeConstants.HACKRF_PLATFORM_RAD1O"/> | <see cref="NativeConstants.HACKRF_PLATFORM_HACKRF1_R9"/>.
        ///
        /// Requires USB API version 0x0106 or above!
        /// </summary>
        /// <param name="device">Device to query.</param>
        /// <param name="value">Supported platform bitfield.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_supported_platform_read")]
        public static extern HackrfError ReadSupportedPlatform(HackRFDevice* device, uint* value);

#if HACKRF_ISSUE_609_IS_FIXED
        /// <summary>
        /// Read CPLD checksum.
        ///
        /// This function is not always available, see [issue 609](https://github.com/greatscottgadgets/hackrf/issues/609).
        ///
        /// Requires USB API version 0x0103 or above!
        /// </summary>
        /// <param name="device">device to read checksum from.</param>
        /// <param name="crc">CRC checksum of the CPLD configuration.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_cpld_checksum")]
        public static extern HackrfError CpldChecksum(HackRFDevice* device, uint* crc);
#endif
    }
}