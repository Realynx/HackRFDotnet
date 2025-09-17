using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums;
using HackRFDotnet.NativeApi.Enums.System;
using HackRFDotnet.NativeApi.Structs;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.NativeApi.Lib;

public static partial class HackRfNativeLib {
    public unsafe static class DeviceStreaming {
        /// <summary>
        /// Query device streaming status
        /// </summary>
        /// <param name="device">Device to query</param>
        /// <returns>
        /// <see cref="HackrfError.HACKRF_TRUE"/> If the device is streaming, else one of <see cref="HackrfError.HACKRF_ERROR_STREAMING_THREAD_ERR"/>,
        /// <see cref="HackrfError.HACKRF_ERROR_STREAMING_STOPPED"/> or <see cref="HackrfError.HACKRF_ERROR_STREAMING_EXIT_CALLED"/>.
        /// </returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_is_streaming")]
        public static extern HackrfError IsStreaming(HackRFDevice* device);

        /// <summary>
        /// Set baseband filter bandwidth.
        /// 
        /// Possible values: 1.75, 2.5, 3.5, 5, 5.5, 6, 7, 8, 9, 10, 12, 14, 15, 20, 24, 28MHz, default \f$ \le 0.75 \cdot F_s \f$
        /// The functions <see cref="ComputeBasebandFilterBandWidth"/> and <see cref="ComputeBasebandFilterBandWidth_round_down_lt"/> can be used to get a valid value nearest to a given value.
        /// 
        /// Setting the sample rate causes the filter bandwidth to be (re)set to its default \f$ \le 0.75 \cdot F_s \f$ value, so setting sample rate should be done before setting filter bandwidth.
        /// </summary>
        /// <param name="device">device to configure</param>
        /// <param name="bandwidth_hz">baseband filter bandwidth in Hz</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_baseband_filter_bandwidth")]
        public static extern HackrfError SetBasebandFilterBandwidth(HackRFDevice* device, uint bandwidth_hz);

        /// <summary>
        /// Compute nearest valid baseband filter bandwidth lower than a specified value.
        ///
        /// The result can be used via <see cref="SetBasebandFilterBandwidth"/>.
        /// </summary>
        /// <param name="bandwidth_hz">Desired filter bandwidth in Hz.</param>
        /// <returns>The highest valid filter bandwidth lower than <paramref name="bandwidth_hz"/> in Hz.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_compute_baseband_filter_bw_round_down_lt")]
        public static extern uint ComputeBasebandFilterBandWidth_round_down_lt(uint bandwidth_hz);

        /// <summary>
        /// Compute nearest valid baseband filter bandwidth to specified value.
        ///
        /// The result can be used via <see cref="SetBasebandFilterBandwidth"/>.
        /// </summary>
        /// <param name="bandwidth_hz">Desired filter bandwidth in Hz</param>
        /// <returns>Nearest valid filter bandwidth in Hz</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_compute_baseband_filter_bw")]
        public static extern uint ComputeBasebandFilterBandWidth(uint bandwidth_hz);

        /// <summary>
        /// RF filter path setting enum.
        /// Used only when performing explicit tuning using <see cref="SetFrequency(HackRFDevice*, ulong, ulong, RfPathFilter)"/>, or can be converted into a human readable string using <see cref="FilterPathName"/>.
        /// This can select the image rejection filter(U3, U8 or none) to use - using switches U5, U6, U9 and U11.When no filter is selected, the mixer itself is bypassed.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_filter_path_name")]
        public static extern sbyte* FilterPathName(RfPathFilter path);

        /// <summary>
        /// Set the center frequency.
        /// 
        /// Simple(auto) tuning via specifying a center frequency in Hz.
        /// 
        /// This setting is not exact and depends on the PLL settings.Exact resolution is not determined, but the actual tuned frequency will be queryable in the future.
        /// </summary>
        /// <param name="device">Device to tune.</param>
        /// <param name="freq_hz">freq_hz center frequency in Hz.Defaults to 900MHz. Should be in range 1-6000MHz, but 0-7250MHz is possible. The resolution is ~50Hz, I could not find the exact number.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_freq")]
        public static extern HackrfError SetFrequency(HackRFDevice* device, ulong freq_hz);

        /// <summary>
        /// Set the center frequency via explicit tuning.
        ///
        /// Center frequency is set to \f$f_{center} = f_{IF
        /// k\cdot f_ { LO }\f$ where \f$k\in\left\{-1; 0; 1\right\}\f$, depending on the value of <paramref name="path"/>. See the documentation of <see cref="RfPathFilter"/> for details.
        /// </summary>
        /// <param name="device">Device to tune.</param>
        /// <param name="if_freq_hz">Tuning frequency of the MAX2837 transceiver IC in Hz. Must be in the range of 2150-2750MHz.</param>
        /// <param name="lo_freq_hz">Tuning frequency of the RFFC5072 mixer/synthesizer IC in Hz. Must be in the range 84.375-5400MHz, defaults to 1000MHz. No effect if <paramref name="path"/> is set to <see cref="RfPathFilter.RF_PATH_FILTER_BYPASS"/>.</param>
        /// <param name="path">Filter path for mixer. See the documentation for <see cref="RfPathFilter"/> for details.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_freq_explicit")]
        public static extern HackrfError SetFrequency(HackRFDevice* device, ulong if_freq_hz, ulong lo_freq_hz, RfPathFilter path);

        /// <summary>
        /// Initialize sweep mode.
        /// 
        /// In this mode, in a single data transfer(single call to the RX transfer callback), multiple blocks of size <paramref name="num_bytes"/> bytes are
        /// received with different center frequencies.At the beginning of each block, a 10-byte frequency header is present in
        /// <c>0x7F - 0x7F - uint64_t frequency(LSBFIRST, in Hz)</c> format, followed by the actual samples.
        /// 
        /// Requires USB API version 0x0102 or above!
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="frequency_list">List of start-stop frequency pairs in MHz.</param>
        /// <param name="num_ranges">Length of array <paramref name="frequency_list"/> (in pairs, so total array length / 2!). Must be less than <see name="NativeConstants.MAX_SWEEP_RANGES"/>.</param>
        /// <param name="num_bytes">Number of bytes to capture per tuning, must be a multiple of <see cref="NativeConstants.BYTES_PER_BLOCK"/>.</param>
        /// <param name="step_width">Width of each tuning step in Hz.</param>
        /// <param name="offset">Frequency offset added to tuned frequencies.sample_rate / 2 is a good value.</param>
        /// <param name="style">Sweep style.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_init_sweep")]
        public static extern HackrfError InitSweep(HackRFDevice* device, ushort* frequency_list, int num_ranges, uint num_bytes, uint step_width, uint offset, SweepStyle style);

        /// <summary>
        /// Start RX sweep
        /// 
        /// See <see cref="InitSweep"/> for more info
        /// 
        /// Requires USB API version 0x0104 or above!
        /// </summary>
        /// <param name="device">Device to start sweeping.</param>
        /// <param name="callback">Rx callback processing the received data.</param>
        /// <param name="rx_ctx">User provided RX context.Not used by the library, but available to <paramref name="callback"/> as <see cref="HackrfTransfer.rx_ctx"/>.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_rx_sweep")]
        public static extern HackrfError StartRxSweep(HackRFDevice* device, HackRFSampleBlockCallback callback, void* rx_ctx);

        /// <summary>
        /// Start receiving.
        /// 
        /// Should be called after setting gains, frequency and sampling rate, as these values won't get reset but instead keep their last value, thus their state is unknown.
        /// 
        /// The callback is called with a <see cref="HackrfTransfer"/> object whenever the buffer is full. The callback is called in an async context so no libhackrf functions should be called from it.
        /// The callback should treat its argument as read-only.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="callback">Rx_callback.</param>
        /// <param name="rx_ctx">User provided RX context. Not used by the library, but available to <paramref name="callback"/> as <see cref="HackrfTransfer.rx_ctx"/>.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_rx")]
        public static extern HackrfError StartRx(HackRFDevice* device, HackRFSampleBlockCallback callback, void* rx_ctx);

        /// <summary>
        /// Stop receiving,
        /// </summary>
        /// <param name="device">device to stop RX on</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_stop_rx")]
        public static extern HackrfError StopRx(HackRFDevice* device);

        /// <summary>
        /// Enable / disable bias-tee (antenna port power).
        ///
        /// Enable or disable the **3.3V (max 50mA)** bias-tee (antenna port power). Defaults to disabled.
        ///
        /// **NOTE:** the firmware auto-disables this after returning to IDLE mode, so a perma-set is not possible, which means all software supporting HackRF devices must support enabling bias-tee,
        /// as setting it externally is not possible like it is with RTL-SDR for example.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">Enable (1) or disable (0) bias-tee.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_antenna_enable")]
        public static extern HackrfError EnableAntenna(HackRFDevice* device, byte value);

        /// <summary>
        /// Start transmitting (TX).
        ///
        /// ⚠️ Warning: Transmitting radio signals may be subject to national and international regulations. Use of this function without the appropriate license or authorization may violate FCC
        /// regulations (or equivalent regulatory authorities in your region) and could result in legal penalties.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="callback">Tx_callback.</param>
        /// <param name="tx_ctx">User provided TX context. Not used by the library, but available to <paramref name="callback"/> as <see cref="HackrfTransfer.tx_ctx"/>.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_tx")]
        public static extern HackrfError StartTx(HackRFDevice* device, HackRFSampleBlockCallback callback, void* tx_ctx);

        /// <summary>
        /// Setup callback to be called when an USB transfer is completed.
        /// 
        /// This callback will be called whenever an USB transfer to the device is completed, regardless if it was successful or not (indicated by the second parameter).
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="callback">Callback to call when a transfer is completed.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_tx_block_complete_callback")]
        public static extern HackrfError SetTxBlockCompleteCallback(HackRFDevice* device, HackRFTxBlockCompleteCallback callback);

        /// <summary>
        /// Setup flush (end-of-transmission) callback.
        /// 
        /// This callback will be called when all the data was transmitted and all data transfers were completed. First parameter is supplied context, second parameter is success flag.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="callback">callback to call when all transfers were completed.</param>
        /// <param name="flush_ctx">context (1st parameter of callback).</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_enable_tx_flush")]
        public static extern HackrfError EnableTxFlush(HackRFDevice* device, HackRFFlushCallback callback, void* flush_ctx);

        /// <summary>
        /// Stop transmission.
        /// </summary>
        /// <param name="device">Device to stop TX on.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_stop_tx")]
        public static extern HackrfError StopTx(HackRFDevice* device);

        /// <summary>
        /// Set transmit underrun limit
        /// 
        /// When this limit is set, after the specified number of samples (bytes, not whole IQ pairs) missing the device will automatically return to IDLE mode,
        /// thus stopping operation. Useful for handling cases like program/computer crashes or other problems. The default value 0 means no limit.
        /// 
        /// Requires USB API version 0x0106 or above!
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">Number of samples to wait before auto-stopping.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_tx_underrun_limit")]
        public static extern HackrfError SetTxUnderrunLimit(HackRFDevice* device, uint value);

        /// <summary>
        /// Set receive overrun limit
        /// 
        /// When this limit is set, after the specified number of samples (bytes, not whole IQ pairs) missing the device will automatically return to IDLE mode,
        /// thus stopping operation. Useful for handling cases like program/computer crashes or other problems. The default value 0 means no limit.
        /// 
        /// Requires USB API version 0x0106 or above!
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">Number of samples to wait before auto-stopping.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_rx_overrun_limit")]
        public static extern HackrfError SetRxOverrunLimit(HackRFDevice* device, uint value);

        /// <summary>
        /// Enable/disable 14dB RF amplifier.
        /// 
        /// Enable / disable the ~11dB RF RX/TX amplifiers U13/U25 via controlling switches U9 and U14.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">Enable (1) or disable (0) amplifier.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_amp_enable")]
        public static extern HackrfError EnableAmp(HackRFDevice* device, byte value);

        /// <summary>
        /// Set LNA gain.
        ///
        /// Set the RF RX gain of the MAX2837 transceiver IC ("IF" gain setting) in decibels. Must be in range 0-40dB, with 8dB steps.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">RX IF gain value in dB.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_lna_gain")]
        public static extern HackrfError SetLnaGain(HackRFDevice* device, uint value);

        /// <summary>
        /// Set baseband RX gain of the MAX2837 transceiver IC ("BB" or "VGA" gain setting) in decibels. Must be in range 0-62dB with 2dB steps.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">RX BB gain value in dB.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_vga_gain")]
        public static extern HackrfError SetVgaGain(HackRFDevice* device, uint value);

        /// <summary>
        /// Set RF TX gain of the MAX2837 transceiver IC ("IF" or "VGA" gain setting) in decibels. Must be in range 0-47dB in 1dB steps.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="value">TX IF gain value in dB.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_txvga_gain")]
        public static extern HackrfError SetTxVgaGain(HackRFDevice* device, uint value);

        /// <summary>
        /// Set sample rate explicitly.
        ///
        /// Sample rate should be in the range 2-20MHz, with the default being 10MHz.Lower & higher values are technically possible, but the performance is not guaranteed.
        ///
        /// This function sets the sample rate by specifying a clock frequency in Hz and a divider, so the resulting sample rate will be <paramref name="freq_hz"/> / <paramref name="divider"/>.
        /// This function also sets the baseband filter bandwidth to a value \f$ \le 0.75 \cdot F_s \f$, so any calls to <see cref="SetBasebandFilterBandwidth"/> should only be made after this.
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="freq_hz">Sample rate base frequency in Hz</param>
        /// <param name="divider">Frequency divider. Must be in the range 1-31.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_sample_rate_manual")]
        public static extern HackrfError SetClockSampleRate(HackRFDevice* device, uint freq_hz, uint divider);

        /// <summary>
        /// Set sample rate.
        ///
        /// Sample rate should be in the range 2-20MHz, with the default being 10MHz. Lower & higher values are technically possible, but the performance is not guaranteed.
        /// This function also sets the baseband filter bandwidth to a value \f$ \le 0.75 \cdot F_s \f$, so any calls to <see cref="SetBasebandFilterBandwidth"/> should only be made after this.
        /// 
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="freq_hz">Sample rate base frequency in Hz.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_sample_rate")]
        public static extern HackrfError SetSampleRate(HackRFDevice* device, double freq_hz);

        /// <summary>
        /// Get USB transfer buffer size.
        /// </summary>
        /// <param name="device">Unused.</param>
        /// <returns>Size in bytes.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_transfer_buffer_size")]
        public static extern nuint GetTransferBufferSize(HackRFDevice* device);

        /// <summary>
        /// Get the total number of USB transfer buffers.
        /// </summary>
        /// <param name="device">Unused.</param>
        /// <returns>Number of buffers.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_get_transfer_queue_depth")]
        public static extern uint GetTransferQueueDepth(HackRFDevice* device);

        /// <summary>
        /// Configure bias tee behavior of the HackRF device when changing RF states.
        ///
        /// This function allows the user to configure bias tee behavior so that it can be turned on or off automatically by the HackRF when entering the RX, TX,
        /// or OFF state. By default, the HackRF switches off the bias tee when the RF path switches to OFF mode.
        /// 
        /// The bias tee configuration is specified via a bitfield:
        /// 0000000TmmRmmOmm
        /// 
        /// Where setting T/R/O bits indicates that the TX/RX/Off behavior should be set to mode 'mm', 0 = don't modify
        ///
        /// mm specifies the bias tee mode:
        /// 
        /// 00 - do nothing
        /// 01 - reserved, do not use
        /// 10 - disable bias tee
        /// 11 - enable bias tee
        /// </summary>
        /// <param name="device">Device to configure.</param>
        /// <param name="req">Bias tee states, as a bitfield.</param>
        /// <returns><see cref="HackrfError.HACKRF_SUCCESS"/> on success or <see cref="HackrfError"/> variant.</returns>
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_user_bias_t_opts")]
        public static extern HackrfError SetBiasTOptions(HackRFDevice* device, HackRFBiasTUserSettingReq* req);
    }
}