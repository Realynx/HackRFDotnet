using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Enums;
using HackRFDotnet.NativeApi.Structs;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.NativeApi.Lib;

public static partial class HackRfNativeLib {
    public unsafe static class DeviceStreaming {
        // Query streaming status
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_is_streaming")]
        public static extern int IsStreaming(HackRFDevice* device);

        // Set baseband filter bandwidth
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_baseband_filter_bandwidth")]
        public static extern int SetBasebandFilterBandith(HackRFDevice* device, uint bandwidth_hz);

        // Compute nearest valid baseband filter bandwidth lower than given value
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_compute_baseband_filter_bw_round_down_lt")]
        public static extern uint ComputeBasebandFilterBandWidth_round_down_lt(uint bandwidth_hz);

        // Compute nearest valid baseband filter bandwidth
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_compute_baseband_filter_bw")]
        public static extern uint ComputeBasebandFilterBandWith(uint bandwidth_hz);

        // Convert rf_path_filter to human-readable string
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_filter_path_name")]
        public static extern sbyte* FilterPathName(RfPathFilter path);

        // Set center frequency (simple)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_freq")]
        public static extern int SetFrequency(HackRFDevice* device, ulong freq_hz);

        // Set center frequency (explicit tuning)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_freq_explicit")]
        public static extern int SetFrequency(HackRFDevice* device, ulong if_freq_hz, ulong lo_freq_hz, RfPathFilter path);

        // Initialize sweep mode (requires USB API >= 0x0102)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_init_sweep")]
        public static extern int InitSweep(
            HackRFDevice* device,
            ushort* frequency_list, // array of start-stop pairs in MHz
            int num_ranges,         // number of frequency pairs
            uint num_bytes,         // number of bytes per tuning
            uint step_width,        // width of each tuning step in Hz
            uint offset,            // frequency offset
            SweepStyle style);     // enum sweep_style

        // Start RX sweep
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_rx_sweep")]
        public static extern int StartRxSweep(HackRFDevice* device, HackRFSampleBlockCallback callback, void* rx_ctx);

        // Start receiving (RX)
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_start_rx")]
        public static extern int StartRx(HackRFDevice* device, HackRFSampleBlockCallback callback, void* rx_ctx);

        // Stop receiving (RX)
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

        // Set TX block complete callback
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_tx_block_complete_callback")]
        public static extern int SetTxBlockCompleteCallback(HackRFDevice* device, HackRFTxBlockCompleteCallback callback);

        // Enable TX flush callback
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_enable_tx_flush")]
        public static extern int EnableTxFlush(HackRFDevice* device, HackRFFlushCallback callback, void* flush_ctx);

        // Stop TX
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_stop_tx")]
        public static extern int StopTx(HackRFDevice* device);

        // Set TX underrun limit
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_tx_underrun_limit")]
        public static extern int SetTxUnderrunLimit(HackRFDevice* device, uint value);

        // Set RX overrun limit
        [DllImport(NativeConstants.HACK_RF_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hackrf_set_rx_overrun_limit")]
        public static extern int SetRxOverrunLimit(HackRFDevice* device, uint value);

        // Enable/disable RF amplifier (~14dB)
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

        // Set sample rate (auto)
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