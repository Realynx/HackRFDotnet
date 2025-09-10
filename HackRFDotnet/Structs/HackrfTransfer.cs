using System.Runtime.InteropServices;

namespace HackRFDotnet.Structs {
    public unsafe struct HackrfTransfer {
        /** HackRF USB device for this transfer */
        public HackRFDevice* device;

        /** transfer data buffer (interleaved 8 bit I/Q samples) */
        public byte* buffer;

        /** length of data buffer in bytes */
        public int buffer_length;

        /** number of buffer bytes that were transferred */
        public int valid_length;

        /** User provided RX context. Not used by the library, but available to transfer callbacks for use. Set along with the transfer callback using @ref hackrf_start_rx or @ref hackrf_start_rx_sweep */
        public void* rx_ctx;

        /** User provided TX context. Not used by the library, but available to transfer callbacks for use. Set along with the transfer callback using @ref hackrf_start_tx */
        public void* tx_ctx;
    }


    // Define the callback delegate
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int HackRFSampleBlockCallback(HackrfTransfer* transfer);

    // TX block complete callback delegate
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void HackRFTxBlockCompleteCallback(HackrfTransfer* transfer, int status);

    // Flush callback delegate
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void HackRFFlushCallback(nint flush_ctx, int status);

}
