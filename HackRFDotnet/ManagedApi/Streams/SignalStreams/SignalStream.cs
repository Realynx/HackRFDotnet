using HackRFDotnet.ManagedApi.Streams.Buffers;
using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Utilities;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class SignalStream : IDisposable {
    public RadioBand Center { get; protected set; } = RadioBand.FromMHz(94.7f);
    public RadioBand BandWidth { get; protected set; } = RadioBand.FromKHz(200);
    public SampleRate SampleRate {
        get {
            return _iQStream.SampleRate;
        }
    }

    // TODO: This will actually need to scale with the IQ's sample rate :c
    internal const int PROCESSING_SIZE = 262144;
    internal RingBuffer<IQ> _filteredBuffer;
    protected SignalProcessingPipeline? _processingPipeline;

    protected readonly IIQStream _iQStream;
    protected readonly bool _keepOpen;

    public SignalStream(IIQStream iQStream, SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true) {
        _iQStream = iQStream;
        _processingPipeline = processingPipeline;
        _keepOpen = keepOpen;

        _filteredBuffer = new RingBuffer<IQ>((int)(TimeSpan.FromMilliseconds(25).TotalSeconds * _iQStream.SampleRate.Sps));
        new Thread(BufferKeeping).Start();
    }

    public void ReadSpan(Span<IQ> iqPairs) {
        while (_filteredBuffer.AvailableBytes < iqPairs.Length) {
            Thread.Sleep(1);
        }

        _filteredBuffer.Read(iqPairs);
    }

    private void BufferKeeping() {
        var convertedPairs = new IQ[PROCESSING_SIZE];

        while (true) {
            _iQStream.ReadBuffer(convertedPairs);

            // This Re-Shifts the signal to the intended center, since we shift it by half the sample size earlier to avoid the HackRFOne's 0 DC spike
            SignalUtilities.ApplyFrequencyOffset(convertedPairs, -_iQStream.SampleRate.NyquistFrequencyRange / 2, _iQStream.SampleRate);
            var sampleCount = PROCESSING_SIZE;
            if (_processingPipeline != null) {
                sampleCount = _processingPipeline.ApplyPipeline(convertedPairs.AsSpan());
            }

            _filteredBuffer.Write(convertedPairs.AsSpan(0, sampleCount));
        }
    }

    /// <summary>
    /// Set the band and bandwidth the filtering engine will use.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="bandwidth"></param>
    public void SetBand(RadioBand center, RadioBand bandwidth) {
        Center = center;
        BandWidth = bandwidth;
    }

    public void Dispose() {
        if (!_keepOpen) {
            _iQStream.Dispose();
        }
    }
}
