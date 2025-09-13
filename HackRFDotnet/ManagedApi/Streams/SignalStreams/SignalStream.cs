using HackRFDotnet.ManagedApi.Streams.Buffers;
using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class SignalStream : IDisposable {
    public RadioBand Center { get; protected set; } = RadioBand.FromMHz(94.7f);
    public RadioBand Bandwith { get; protected set; } = RadioBand.FromKHz(200);
    public double SampleRate {
        get {
            return _iQStream.SampleRate;
        }
    }

    internal RingBuffer<IQ> _filteredBuffer;
    protected SignalProcessingPipeline? _processingPipeline;

    protected readonly IIQStream _iQStream;
    protected readonly bool _keepOpen;

    public SignalStream(IIQStream iQStream, SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true) {
        _iQStream = iQStream;
        _processingPipeline = processingPipeline;
        _keepOpen = keepOpen;

        _filteredBuffer = new RingBuffer<IQ>((int)(TimeSpan.FromMilliseconds(25).TotalSeconds * _iQStream.SampleRate));
        new Thread(BufferKeeping).Start();
    }

    public void ReadSpan(Span<IQ> iqPairs) {
        while (_filteredBuffer.AvailableBytes < iqPairs.Length) {
            Thread.Sleep(1);
        }

        lock (_filteredBuffer) {
            _filteredBuffer.Read(iqPairs);
        }
    }

    private void BufferKeeping() {
        var chunkSize = CalculateFFTChunkSize();
        var iqPairs = new IQ[chunkSize];

        while (true) {
            _iQStream.ReadBuffer(iqPairs);
            var sampleCount = chunkSize;
            if (_processingPipeline != null) {
                sampleCount = _processingPipeline.ApplyPipeline(iqPairs);
            }

            lock (_filteredBuffer) {
                _filteredBuffer.Write(iqPairs.AsSpan(0, sampleCount));
            }
        }
    }

    public int CalculateFFTChunkSize() {
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
    }

    public void Dispose() {
        if (!_keepOpen) {
            _iQStream.Dispose();
        }
    }
}
