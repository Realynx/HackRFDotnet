using System.Runtime.InteropServices;

using HackRFDotnet.Enums;
using HackRFDotnet.Structs;

namespace HackRFDotnet.NativeApi {
    public static unsafe class HackRfNativeFunctions {
        /// <summary>
        /// Initialize libhackrf. Should be called before any other function.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_init();

        /// <summary>
        /// Exit libhackrf. Should be called before application exit.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_exit();

        /// <summary>
        /// Get library version string.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte* hackrf_library_version();

        /// <summary>
        /// Get library release string.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte* hackrf_library_release();

        /// <summary>
        /// Get list of connected HackRF devices.
        /// </summary>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern HackRFDeviceList* hackrf_device_list();

        // Open a HackRF device from a device list
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_device_list_open(
            HackRFDeviceList* list,
            int idx,
            HackRFDevice** device);

        // Free a previously allocated device list
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void hackrf_device_list_free(HackRFDeviceList* list);

        // Open the first available HackRF device
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_open(HackRFDevice** device);

        // Open HackRF device by serial number
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_open_by_serial(
            [MarshalAs(UnmanagedType.LPStr)] string desired_serial_number,
            HackRFDevice** device);

        // Close a previously opened HackRF device
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_close(HackRFDevice* device);

        // Start receiving (RX)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_start_rx(
            HackRFDevice* device,
            HackRFSampleBlockCallback callback,
            void* rx_ctx);

        // Stop receiving (RX)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_stop_rx(
            HackRFDevice* device);

        // Start transmitting (TX)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_start_tx(
            HackRFDevice* device,
            HackRFSampleBlockCallback callback,
            void* tx_ctx);


        // Set TX block complete callback
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_tx_block_complete_callback(
            HackRFDevice* device,
            HackRFTxBlockCompleteCallback callback);

        // Enable TX flush callback
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_enable_tx_flush(
            HackRFDevice* device,
            HackRFFlushCallback callback,
            void* flush_ctx);

        // Stop TX
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_stop_tx(
            HackRFDevice* device);

        // Get M0 state
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_get_m0_state(
            HackRFDevice* device,
            HackRFM0State* value);

        // Set TX underrun limit
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_tx_underrun_limit(
            HackRFDevice* device,
            uint value);

        // Set RX overrun limit
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_rx_overrun_limit(
            HackRFDevice* device,
            uint value);

        // Query streaming status
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_is_streaming(
            HackRFDevice* device);

        // Read MAX2837 register
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_max2837_read(
            HackRFDevice* device,
            byte register_number,
            ushort* value);

        // Write MAX2837 register
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_max2837_write(
            HackRFDevice* device,
            byte register_number,
            ushort value);

        // Read Si5351C register
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_si5351c_read(
            HackRFDevice* device,
            ushort register_number,
            ushort* value);

        // Write Si5351C register
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_si5351c_write(
            HackRFDevice* device,
            ushort register_number,
            ushort value);

        // Set baseband filter bandwidth
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_baseband_filter_bandwidth(
            HackRFDevice* device,
            uint bandwidth_hz);

        // Read RFFC5071 register
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_rffc5071_read(
            HackRFDevice* device,
            byte register_number,
            ushort* value);

        // Write RFFC5071 register
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_rffc5071_write(
            HackRFDevice* device,
            byte register_number,
            ushort value);

        // Read USB API version
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_usb_api_version_read(
            HackRFDevice* device,
            ushort* version);

        // Erase SPI flash firmware
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_spiflash_erase(
            HackRFDevice* device);

        // Write SPI flash
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_spiflash_write(
            HackRFDevice* device,
            uint address,
            ushort length,
            byte* data);

        // Read SPI flash
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_spiflash_read(
            HackRFDevice* device,
            uint address,
            ushort length,
            byte* data);

        // Read SPI flash status registers (2 bytes)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_spiflash_status(
            HackRFDevice* device,
            byte* data);

        // Clear SPI flash status registers
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_spiflash_clear_status(
            HackRFDevice* device);

        // Write CPLD bitstream
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_cpld_write(
            HackRFDevice* device,
            byte* data,
            uint total_length);

        // Read board ID
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_board_id_read(
            HackRFDevice* device,
            byte* value);

        // Read firmware version string
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_version_string_read(
            HackRFDevice* device,
            byte* version,   // use byte* instead of char* in C#
            byte length);

        // Set center frequency (simple)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_freq(
            HackRFDevice* device,
            ulong freq_hz);

        // Set center frequency (explicit tuning)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_freq_explicit(
            HackRFDevice* device,
            ulong if_freq_hz,
            ulong lo_freq_hz,
            RfPathFilter path);

        // Set sample rate manually with divider
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_sample_rate_manual(
            HackRFDevice* device,
            uint freq_hz,
            uint divider);

        // Set sample rate (auto)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_sample_rate(
            HackRFDevice* device,
            double freq_hz);

        // Enable/disable RF amplifier (~14dB)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_amp_enable(
            HackRFDevice* device,
            byte value); // 0 = disable, 1 = enable

        // Read board part ID and serial number
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_board_partid_serialno_read(
            HackRFDevice* device,
            ReadPartidSerialNo* read_partid_serialno);

        // Set RX LNA gain (IF gain)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_lna_gain(
            HackRFDevice* device,
            uint value); // 0-40 dB, steps of 8 dB

        // Set RX baseband gain (VGA gain)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_vga_gain(
            HackRFDevice* device,
            uint value); // 0-62 dB, steps of 2 dB

        // Set TX gain (VGA/IF)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_txvga_gain(
            HackRFDevice* device,
            uint value); // 0-47 dB, steps of 1 dB

        // Enable/disable Bias-Tee (3.3V @ max 50mA)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_antenna_enable(
            HackRFDevice* device,
            byte value); // 0 = disable, 1 = enable

        // Convert hackrf_error to string
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte* hackrf_error_name(HackrfError errcode);

        // Convert hackrf_board_id to human-readable string
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte* hackrf_board_id_name(HackrfBoardId board_id);

        // Lookup platform ID from board ID
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint hackrf_board_id_platform(HackrfBoardId board_id);

        // Convert hackrf_usb_board_id to human-readable string
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte* hackrf_usb_board_id_name(HackrfUsbBoardId usb_board_id);

        // Convert rf_path_filter to human-readable string
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte* hackrf_filter_path_name(RfPathFilter path);

        // Compute nearest valid baseband filter bandwidth lower than given value
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint hackrf_compute_baseband_filter_bw_round_down_lt(uint bandwidth_hz);

        // Compute nearest valid baseband filter bandwidth
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint hackrf_compute_baseband_filter_bw(uint bandwidth_hz);

        // Set hardware sync mode (requires USB API >= 0x0102)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_hw_sync_mode(
            HackRFDevice* device,
            byte value); // 0 = disable, 1 = enable

        // Initialize sweep mode (requires USB API >= 0x0102)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_init_sweep(
            HackRFDevice* device,
            ushort* frequency_list, // array of start-stop pairs in MHz
            int num_ranges,         // number of frequency pairs
            uint num_bytes,         // number of bytes per tuning
            uint step_width,        // width of each tuning step in Hz
            uint offset,            // frequency offset
            SweepStyle style);     // enum sweep_style

        // Query connected Opera Cake boards
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_get_operacake_boards(
            HackRFDevice* device,
            byte* boards); // output array of size HACKRF_OPERACAKE_MAX_BOARDS

        // Set Opera Cake operation mode
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_operacake_mode(
            HackRFDevice* device,
            byte address,
            OperacakeSwitchingMode mode);

        // Query Opera Cake mode
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_get_operacake_mode(
            HackRFDevice* device,
            byte address,
            OperacakeSwitchingMode* mode);

        // Set Opera Cake ports in manual mode
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_operacake_ports(
            HackRFDevice* device,
            byte address,
            byte port_a,
            byte port_b);

        // Set Opera Cake dwell times in time mode
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_operacake_dwell_times(
            HackRFDevice* device,
            HackRFOperacakeDwellTime* dwell_times,
            byte count);

        // Set Opera Cake frequency ranges in frequency mode
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_operacake_freq_ranges(
            HackRFDevice* device,
            HackRFOperacakeFreqRange* freq_ranges,
            byte count);

        // Reset HackRF device
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_reset(HackRFDevice* device);

        // Deprecated: old Opera Cake frequency ranges setter
        [Obsolete("Use hackrf_set_operacake_freq_ranges instead.")]
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_operacake_ranges(
            HackRFDevice* device,
            byte* ranges,
            byte num_ranges);

        // Enable/disable CLKOUT
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_clkout_enable(
            HackRFDevice* device,
            byte value);

        // Get CLKIN status
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_get_clkin_status(
            HackRFDevice* device,
            byte* status);

        // Perform GPIO test on an Opera Cake addon board
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_operacake_gpio_test(
            HackRFDevice* device,
            byte address,
            ushort* test_result);

#if HACKRF_ISSUE_609_IS_FIXED
        // Read CPLD checksum
        [DllImport(HackRFDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_cpld_checksum(
            HackRFDevice* device,
            uint* crc);
#endif

        // Enable/disable UI display (Rad1o, PortaPack, etc.)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_ui_enable(
            HackRFDevice* device,
            byte value);


        // Start RX sweep
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_start_rx_sweep(
            HackRFDevice* device,
            HackRFSampleBlockCallback callback,
            void* rx_ctx);

        // Get USB transfer buffer size
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint hackrf_get_transfer_buffer_size(
            HackRFDevice* device);

        // Get the total number of USB transfer buffers
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint hackrf_get_transfer_queue_depth(
            HackRFDevice* device);

        // Read board revision
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_board_rev_read(
            HackRFDevice* device,
            byte* value);

        // Convert board revision enum to string
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint hackrf_board_rev_name(
            HackrfBoardRev board_rev);

        // Read supported platform bitfield
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_supported_platform_read(
            HackRFDevice* device,
            uint* value);

        // Turn on/off LEDs (override)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_leds(
            HackRFDevice* device,
            byte state);

        // Configure bias tee behavior for RF states
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_user_bias_t_opts(
            HackRFDevice* device,
            HackRFBiasTUserSettingReq* req);
    }
}
