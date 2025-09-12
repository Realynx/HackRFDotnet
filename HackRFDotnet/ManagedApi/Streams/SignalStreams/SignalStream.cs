using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class SignalStream : IDisposable {
    public RadioBand Center { get; protected set; } = RadioBand.FromMHz(94.7f);
    public RadioBand Bandwith { get; protected set; } = RadioBand.FromKHz(200);

    internal RingBuffer<IQ> _filteredBuffer;
    protected FilterProcessor? _filterProcessor;
    protected SignalProcessingPipeline? _processingPipeline;

    protected readonly IIQStream _iQStream;
    protected readonly bool _keepOpen;

    public SignalStream(IIQStream iQStream, SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true) {
        _iQStream = iQStream;
        _processingPipeline = processingPipeline;
        _keepOpen = keepOpen;

        _filteredBuffer = new RingBuffer<IQ>((int)(TimeSpan.FromMilliseconds(150).TotalSeconds * _iQStream.SampleRate));
        new Thread(BufferKeeping).Start();
    }

    internal void ReadSpan(Span<IQ> iqPairs) {
        lock (_filteredBuffer) {
            var readBytes = _filteredBuffer.Read(iqPairs);
        }
    }

    private void BufferKeeping() {
        var chunkSize = CalculateChunkSize();

        while (true) {
            if (_filterProcessor is null || _iQStream.BufferLength < chunkSize) {
                Thread.Sleep(1);
                continue;
            }

            var iqPairs = ArrayPool<IQ>.Shared.Rent(chunkSize);
            try {
                var readBytes = _iQStream.ReadBuffer(iqPairs.AsSpan(0, chunkSize));
                var sampleCount = chunkSize;
                if (_processingPipeline != null) {
                    sampleCount = _processingPipeline.ApplyPipeline(iqPairs.AsSpan(0, chunkSize));
                }

                lock (_filteredBuffer) {
                    _filteredBuffer.Write(iqPairs.AsSpan(0, sampleCount));
                }
            }
            finally {
                ArrayPool<IQ>.Shared.Return(iqPairs);
            }
        }
    }

    private int CalculateChunkSize() {
        /*
        We will send 4096 frames to the audio resampler/player
        We have a much higher sampling rate than the audio player so first we must filter
        out the rest of the spectrum to the bandwith
        The bandwith get's centred to 0 meaning we can represent the entire bandwith with it's hz as MSPS
        We decrease the time domain samples to reduce CPU time when we filter it for the audio playback
        */

        var bufferChunk = 4096;

        var decimationFactor = (int)(_iQStream.SampleRate / Bandwith.Hz);
        var decimatedSize = bufferChunk * decimationFactor;
        return decimatedSize;
    }

    /// <summary>
    /// Set the band and bandwidth the filtering engine will use.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="bandwidth"></param>
    public void SetBand(RadioBand center, RadioBand bandwidth) {
        Center = center;
        Bandwith = bandwidth;

        _filterProcessor = new FilterProcessor(_iQStream.SampleRate, center, bandwidth);
    }

    public void Dispose() {
        if (!_keepOpen) {
            _iQStream.Dispose();
        }
    }
}
