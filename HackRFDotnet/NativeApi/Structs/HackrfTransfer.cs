using System.Runtime.InteropServices;

using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.NativeApi.Structs {
    /// <summary>
    /// USB transfer information passed to RX or TX callback.
    /// A callback should treat all these fields as read-only except that a TX callback should write to the data buffer and may write to <see cref="valid_length"/>
    /// to indicate that a smaller number of bytes is to be transmitted.
    /// </summary>
    public unsafe struct HackrfTransfer {
        /// <summary>
        /// HackRF USB device for this transfer.
        /// </summary>
        public HackRFDevice* device;

        /// <summary>
        /// Transfer data buffer (interleaved 8 bit I/Q samples).
        /// </summary>
        public byte* buffer;

        /// <summary>
        /// Length of data buffer in bytes.
        /// </summary>
        public int buffer_length;

        /// <summary>
        /// Number of buffer bytes that were transferred.
        /// </summary>
        public int valid_length;

        /// <summary>
        /// User provided RX context. Not used by the library, but available to transfer callbacks for use. Set along with the transfer callback using <see cref="HackRfNativeLib.DeviceStreaming.StartRx"/>
        /// or <see cref="HackRfNativeLib.DeviceStreaming.StartRxSweep"/>.
        /// </summary>
        public void* rx_ctx;

        /// <summary>
        /// User provided TX context. Not used by the library, but available to transfer callbacks for use. Set along with the transfer callback using <see cref="HackRfNativeLib.DeviceStreaming.StartRx"/>.
        /// </summary>
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