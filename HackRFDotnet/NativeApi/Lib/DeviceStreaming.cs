using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums;
using HackRFDotnet.NativeApi.Structs;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.NativeApi.Lib;

public static partial class HackRfNativeLib {
    public unsafe static class DeviceStreaming {
        /// <summary>
        /// Query device streaming status
        /// @param device device to query
        /// @return @ref HACKRF_TRUE if the device is streaming, else one of @ref HACKRF_ERROR_STREAMING_THREAD_ERR, @ref HACKRF_ERROR_STREAMING_STOPPED or @ref HACKRF_ERROR_STREAMING_EXIT_CALLED
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_is_streaming")]
        public static extern int IsStreaming(HackRFDevice* device);

        /// <summary>
        /// Set baseband filter bandwidth
        /// 
        /// Possible values: 1.75, 2.5, 3.5, 5, 5.5, 6, 7, 8, 9, 10, 12, 14, 15, 20, 24, 28MHz, default \f$ \le 0.75 \cdot F_s \f$
        /// The functions @ref hackrf_compute_baseband_filter_bw and @ref hackrf_compute_baseband_filter_bw_round_down_lt can be used to get a valid value nearest to a given value.
        /// 
        /// Setting the sample rate causes the filter bandwidth to be (re)set to its default \f$ \le 0.75 \cdot F_s \f$ value, so setting sample rate should be done before setting filter bandwidth.
        /// 
        /// @param device device to configure
        /// @param bandwidth_hz baseband filter bandwidth in Hz
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup configuration
        /// </summary>
        /// <param name="device"></param>
        /// <param name="bandwidth_hz"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_baseband_filter_bandwidth")]
        public static extern int SetBasebandFilterBandwidth(HackRFDevice* device, uint bandwidth_hz);

        /// <summary>
        /// Compute nearest valid baseband filter bandwidth lower than a specified value
        /// 
        /// The result can be used via @ref hackrf_set_baseband_filter_bandwidth
        /// 
        /// @param bandwidth_hz desired filter bandwidth in Hz
        /// @return the highest valid filter bandwidth lower than @p bandwidth_hz in Hz
        /// @ingroup configuration
        /// </summary>
        /// <param name="bandwidth_hz"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_compute_baseband_filter_bw_round_down_lt")]
        public static extern uint ComputeBasebandFilterBandWidth_round_down_lt(uint bandwidth_hz);

        /// <summary>
        /// Compute nearest valid baseband filter bandwidth to specified value
        ///The result can be used via @ref hackrf_set_baseband_filter_bandwidth
        ///@param bandwidth_hz desired filter bandwidth in Hz
        ///@return nearest valid filter bandwidth in Hz
        ///@ingroup configuration
        /// </summary>
        /// <param name="bandwidth_hz"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_compute_baseband_filter_bw")]
        public static extern uint ComputeBasebandFilterBandWidth(uint bandwidth_hz);

        /// <summary>
        /// RF filter path setting enum
        ///Used only when performing explicit tuning using @ref hackrf_set_freq_explicit, or can be converted into a human readable string using @ref hackrf_filter_path_name.
        ///This can select the image rejection filter(U3, U8 or none) to use - using switches U5, U6, U9 and U11.When no filter is selected, the mixer itself is bypassed.
        ///@ingroup configuration
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_filter_path_name")]
        public static extern sbyte* FilterPathName(RfPathFilter path);

        /// <summary>
        /// Set the center frequency
        /// 
        /// Simple(auto) tuning via specifying a center frequency in Hz
        /// 
        /// This setting is not exact and depends on the PLL settings.Exact resolution is not determined, but the actual tuned frequency will be queryable in the future.
        /// 
        /// @param device device to tune
        /// @param freq_hz center frequency in Hz.Defaults to 900MHz.Should be in range 1-6000MHz, but 0-7250MHz is possible.The resolution is ~50Hz, I could not find the exact number.
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup configuration
        /// </summary>
        /// <param name="device"></param>
        /// <param name="freq_hz"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_freq")]
        public static extern int SetFrequency(HackRFDevice* device, ulong freq_hz);

        /// <summary>
        ///Set the center frequency via explicit tuning
        ///
        ///Center frequency is set to \f$f_{center} = f_{IF
        /// k\cdot f_ { LO }\f$ where \f$k\in\left\{-1; 0; 1\right\}\f$, depending on the value of @p path. See the documentation of @ref rf_path_filter for details
        ///
        ///@param device device to tune
        ///@param if_freq_hz tuning frequency of the MAX2837 transceiver IC in Hz. Must be in the range of 2150-2750MHz
        ///@param lo_freq_hz tuning frequency of the RFFC5072 mixer/synthesizer IC in Hz. Must be in the range 84.375-5400MHz, defaults to 1000MHz. No effect if @p path is set to @ref RF_PATH_FILTER_BYPASS
        ///@param path filter path for mixer. See the documentation for @ref rf_path_filter for details
        ///@return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        ///@ingroup configuration
        /// </summary>
        /// <param name="device"></param>
        /// <param name="if_freq_hz"></param>
        /// <param name="lo_freq_hz"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_freq_explicit")]
        public static extern int SetFrequency(HackRFDevice* device, ulong if_freq_hz, ulong lo_freq_hz, RfPathFilter path);

        /// <summary>
        /// Initialize sweep mode
        /// 
        /// In this mode, in a single data transfer(single call to the RX transfer callback), multiple blocks of size @p num_bytes bytes are received with different center frequencies.At the beginning of each block, a 10-byte frequency header is present in `0x7F - 0x7F - uint64_t frequency(LSBFIRST, in Hz)` format, followed by the actual samples.
        /// 
        /// Requires USB API version 0x0102 or above!
        /// @param device device to configure
        /// @param frequency_list list of start-stop frequency pairs in MHz
        /// @param num_ranges length of array @p frequency_list(in pairs, so total array length / 2!). Must be less than @ref MAX_SWEEP_RANGES
        /// @param num_bytes number of bytes to capture per tuning, must be a multiple of @ref BYTES_PER_BLOCK
        /// @param step_width width of each tuning step in Hz
        /// @param offset frequency offset added to tuned frequencies.sample_rate / 2 is a good value
        /// @param style sweep style
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <param name="frequency_list"></param>
        /// <param name="num_ranges"></param>
        /// <param name="num_bytes"></param>
        /// <param name="step_width"></param>
        /// <param name="offset"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_init_sweep")]
        public static extern int InitSweep(
            HackRFDevice* device,
            ushort* frequency_list, // array of start-stop pairs in MHz
            int num_ranges,         // number of frequency pairs
            uint num_bytes,         // number of bytes per tuning
            uint step_width,        // width of each tuning step in Hz
            uint offset,            // frequency offset
            SweepStyle style);     // enum sweep_style

        /// <summary>
        /// Start RX sweep
        /// 
        /// See @ref hackrf_init_sweep for more info
        /// 
        /// Requires USB API version 0x0104 or above!
        /// @param device device to start sweeping
        /// @param callback rx callback processing the received data
        /// @param rx_ctx User provided RX context.Not used by the library, but available to @p callback as @ref hackrf_transfer.rx_ctx.
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <param name="callback"></param>
        /// <param name="rx_ctx"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_rx_sweep")]
        public static extern int StartRxSweep(HackRFDevice* device, HackRFSampleBlockCallback callback, void* rx_ctx);

        /// <summary>
        /// Start receiving
        /// 
        /// Should be called after setting gains, frequency and sampling rate, as these values won't get reset but instead keep their last value, thus their state is unknown.
        /// 
        /// The callback is called with a @ref hackrf_transfer object whenever the buffer is full. The callback is called in an async context so no libhackrf functions should be called from it. The callback should treat its argument as read-only.
        /// @param device device to configure
        /// @param callback rx_callback
        /// @param rx_ctx User provided RX context. Not used by the library, but available to @p callback as @ref hackrf_transfer.rx_ctx.
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <param name="callback"></param>
        /// <param name="rx_ctx"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_rx")]
        public static extern int StartRx(HackRFDevice* device, HackRFSampleBlockCallback callback, void* rx_ctx);

        /// <summary>
        /// Stop receiving
        /// 
        /// @param device device to stop RX on
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_stop_rx")]
        public static extern int StopRx(HackRFDevice* device);

        /// <summary>
        /// Enable or disable the **3.3V (max 50mA)** bias-tee (antenna port power). Defaults to disabled.
        /// **NOTE:** the firmware auto-disables this after returning to IDLE mode, so a perma-set is not possible, which means all software
        /// supporting HackRF devices must support enabling bias-tee, as setting it externally is not possible like it is with RTL-SDR for example.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_antenna_enable")]
        public static extern int EnableAntenna(HackRFDevice* device, byte value);

        /// Start transmitting (TX)
        /// <summary>
        /// ⚠️ Warning: Transmitting radio signals may be subject to national and international 
        /// regulations. Use of this function without the appropriate license or authorization 
        /// may violate FCC regulations (or equivalent regulatory authorities in your region) 
        /// and could result in legal penalties.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="callback"></param>
        /// <param name="tx_ctx"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_tx")]
        public static extern int StartTx(HackRFDevice* device, HackRFSampleBlockCallback callback, void* tx_ctx);

        /// <summary>
        /// Setup callback to be called when an USB transfer is completed.
        /// 
        /// This callback will be called whenever an USB transfer to the device is completed, regardless if it was successful or not(indicated by the second parameter).
        /// 
        /// @param device device to configure
        /// @param callback callback to call when a transfer is completed
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant 
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_tx_block_complete_callback")]
        public static extern int SetTxBlockCompleteCallback(HackRFDevice* device, HackRFTxBlockCompleteCallback callback);

        /// <summary>
        /// Setup flush(end-of-transmission) callback
        /// 
        /// This callback will be called when all the data was transmitted and all data transfers were completed.First parameter is supplied context, second parameter is success flag.
        /// 
        /// @param device device to configure
        /// @param callback callback to call when all transfers were completed
        /// @param flush_ctx context (1st parameter of callback)
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <param name="callback"></param>
        /// <param name="flush_ctx"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_enable_tx_flush")]
        public static extern int EnableTxFlush(HackRFDevice* device, HackRFFlushCallback callback, void* flush_ctx);

        /// <summary>
        /// Stop transmission
        /// 
        /// @param device device to stop TX on
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant 
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_stop_tx")]
        public static extern int StopTx(HackRFDevice* device);

        /// <summary>
        /// Set transmit underrun limit
        /// 
        /// When this limit is set, after the specified number of samples (bytes, not whole IQ pairs) missing the device will automatically return to IDLE mode, thus stopping operation. Useful for handling cases like program/computer crashes or other problems. The default value 0 means no limit.
        /// 
        /// Requires USB API version 0x0106 or above!
        /// @param device device to configure
        /// @param value number of samples to wait before auto-stopping
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant   
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_tx_underrun_limit")]
        public static extern int SetTxUnderrunLimit(HackRFDevice* device, uint value);

        /// <summary>
        /// Set receive overrun limit
        /// 
        /// When this limit is set, after the specified number of samples (bytes, not whole IQ pairs) missing the device will automatically return to IDLE mode, thus stopping operation. Useful for handling cases like program/computer crashes or other problems. The default value 0 means no limit.
        /// 
        /// Requires USB API version 0x0106 or above!
        /// @param device device to configure
        /// @param value number of samples to wait before auto-stopping
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup streaming
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_rx_overrun_limit")]
        public static extern int SetRxOverrunLimit(HackRFDevice* device, uint value);

        /// <summary>
        /// Enable/disable 14dB RF amplifier
        /// 
        /// Enable / disable the ~11dB RF RX/TX amplifiers U13/U25 via controlling switches U9 and U14.
        /// 
        /// @param device device to configure
        /// @param value enable(1) or disable(0) amplifier
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup configuration
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_amp_enable")]
        public static extern int EnableAmp(HackRFDevice* device, byte value);

        /// <summary>
        /// Set the RF RX gain of the MAX2837 transceiver IC ("IF" gain setting) in decibels. Must be in range 0-40dB, with 8dB steps.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_lna_gain")]
        public static extern int SetLnaGain(HackRFDevice* device, uint value);

        /// <summary>
        /// Set baseband RX gain of the MAX2837 transceiver IC ("BB" or "VGA" gain setting) in decibels. Must be in range 0-62dB with 2dB steps.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_vga_gain")]
        public static extern int SetVgaGain(HackRFDevice* device, uint value);

        /// <summary>
        /// Set RF TX gain of the MAX2837 transceiver IC ("IF" or "VGA" gain setting) in decibels. Must be in range 0-47dB in 1dB steps.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_txvga_gain")]
        public static extern int SetTxVgaGain(HackRFDevice* device, uint value);

        /// <summary>
        ///  * Set sample rate explicitly
        ///Sample rate should be in the range 2-20MHz, with the default being 10MHz.Lower & higher values are technically possible, but the performance is not guaranteed.
        ///This function sets the sample rate by specifying a clock frequency in Hz and a divider, so the resulting sample rate will be @p freq_hz / @p divider.
        ///This function also sets the baseband filter bandwidth to a value \f$ \le 0.75 \cdot F_s \f$, so any calls to @ref hackrf_set_baseband_filter_bandwidth should only be made after this.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="freq_hz"></param>
        /// <param name="divider"></param>
        /// <returns></returns>
        // Set sample rate manually with divider
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_sample_rate_manual")]
        public static extern int SetClockSampleRate(HackRFDevice* device, uint freq_hz, uint divider);

        /// <summary>
        /// Set sample rate explicitly
        /// 
        /// Sample rate should be in the range 2-20MHz, with the default being 10MHz.Lower & higher values are technically possible, but the performance is not guaranteed.
        /// 
        /// This function sets the sample rate by specifying a clock frequency in Hz and a divider, so the resulting sample rate will be @p freq_hz / @p divider.
        /// This function also sets the baseband filter bandwidth to a value \f$ \le 0.75 \cdot F_s \f$, so any calls to @ref hackrf_set_baseband_filter_bandwidth should only be made after this.
        /// 
        /// @param device device to configure
        /// @param freq_hz sample rate base frequency in Hz
        /// @param divider frequency divider.Must be in the range 1-31
        /// @return @ref HACKRF_SUCCESS on success or @ref hackrf_error variant
        /// @ingroup configuration
        /// </summary>
        /// <param name="device"></param>
        /// <param name="freq_hz"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_sample_rate")]
        public static extern int SetSampleRate(HackRFDevice* device, double freq_hz);

        /// <summary>
        /// Get USB transfer buffer size.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_transfer_buffer_size")]
        public static extern nuint GetTransferBufferSize(HackRFDevice* device);

        /// <summary>
        /// Get the total number of USB transfer buffers
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_transfer_queue_depth")]
        public static extern uint GetTransferQueueDepth(HackRFDevice* device);


        /// <summary>
        /// Configure bias tee behavior of the HackRF device when changing RF states
        /// This function allows the user to configure bias tee behavior so that it can be turned on or off automatically by
        /// the HackRF when entering the RX, TX, or OFF state. By default, the HackRF switches off the bias tee when the RF path switches to OFF mode.
        /// 
        /// The bias tee configuration is specified via a bitfield:
        /// 0000000TmmRmmOmm
        /// 
        /// Where setting T/R/O bits indicates that the TX/RX/Off behavior should be set to mode 'mm', 0=don't modify
        /// mm specifies the bias tee mode:
        /// 
        /// 00 - do nothing
        /// 01 - reserved, do not use
        /// 10 - disable bias tee
        /// 11 - enable bias tee
        /// </summary>
        /// <param name="device"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_user_bias_t_opts")]
        public static extern int SetBiasTOptions(HackRFDevice* device, HackRFBiasTUserSettingReq* req);
    }
}