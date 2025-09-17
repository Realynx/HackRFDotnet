namespace HackRFDotnet.Api.Streams.Interfaces;
/// <summary>
/// <see cref="IIQStream"/> is an immutable stream that buffers the data directly from the IQ device.
/// </summary>
public interface IIQStream {
    /// <summary>
    /// The capture sample rate from the device.
    /// </summary>
    SampleRate SampleRate { get; }

    /// <summary>
    /// The number of bytes available to read in the buffer.
    /// </summary>
    int BufferLength { get; }

    /// <summary>
    /// Close the stream on the device.
    /// </summary>
    void Close();

    /// <summary>
    /// Dispose and free resources from the stream and the device.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Open an Rx stream to read IQ samples.
    /// </summary>
    /// <param name="sampleRate"></param>
    void OpenRx(SampleRate? sampleRate = null);

    /// <summary>
    /// Fill a span with data from the ring buffer.
    /// </summary>
    /// <param name="iqBuffer"></param>
    /// <returns></returns>
    int ReadBuffer(Span<IQ> iqBuffer);

    /// <summary>
    /// Set the capture sample rate of the stream and device.
    /// </summary>
    /// <param name="sampleRate"></param>
    void SetSampleRate(SampleRate sampleRate);

    /// <summary>
    /// Open a Tx stream to write IQ samples.
    /// </summary>
    /// <param name="iqFrame"></param>
    /// <returns></returns>
    int TxBuffer(Span<IQ> iqFrame);
}