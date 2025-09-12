using System.Buffers;

using HackRFDotnet.ManagedApi.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class SignalStream : ISampleProvider, IDisposable {
    public WaveFormat? WaveFormat { get; protected set; }

    protected readonly bool _keepOpen;
    protected readonly IRfDeviceStream _rfDeviceStream;

    protected RadioBand _center = RadioBand.FromMHz(94.7f);
    protected RadioBand _bandwith = RadioBand.FromKHz(200);

    private RingBuffer<IQ> _filteredBuffer;
    //private RingBuffer<float> _noiseHistory = new(100);

    protected FilterProcessor? _filterProcessor;

    public SignalStream(IRfDeviceStream deviceStream, bool keepOpen = true) {
        _keepOpen = keepOpen;
        _rfDeviceStream = deviceStream;
        _filteredBuffer = new RingBuffer<IQ>((int)(TimeSpan.FromMilliseconds(50).TotalSeconds
            * _rfDeviceStream.SampleRate));

        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_bandwith.Hz, 1);
    }

    public void SetBand(RadioBand center, RadioBand bandwidth) {
        _center = center;
        _bandwith = bandwidth;

        _filterProcessor = new FilterProcessor(_rfDeviceStream.SampleRate, center, bandwidth);

        new Thread(BufferKeeping).Start();
    }

    private void BufferKeeping() {
        var bufferChunk = 4096;
        var decimationFactor = (int)(_rfDeviceStream.SampleRate / _bandwith.Hz);
        var decimatedSize = bufferChunk * decimationFactor;

        while (true) {
            if (_rfDeviceStream.BufferLength < decimatedSize) {
                continue;
            }

            var iqPairs = ArrayPool<IQ>.Shared.Rent(decimatedSize);
            try {
                var readBytes = _rfDeviceStream.ReadBuffer(iqPairs.AsSpan(0, decimatedSize));

                var downSampledIq = _filterProcessor.ApplyPipeline(iqPairs.AsSpan(0, decimatedSize), out var updatedSampleRate);

                lock (_filteredBuffer) {
                    _filteredBuffer.Write(iqPairs.AsSpan(0, downSampledIq));
                }
            }
            finally {
                ArrayPool<IQ>.Shared.Return(iqPairs);
            }
        }
    }

    //public float GetNoiseFloorDb() {
    //    if (_noiseHistory.Count == 0) {
    //        return 0f;
    //    }

    //    // Sort values
    //    var noiseFloorBuffer = new float[_noiseHistory.Count];
    //    _noiseHistory.Peek(noiseFloorBuffer);

    //    var sorted = noiseFloorBuffer.OrderBy(x => x).ToList();

    //    var trimCount = (int)(sorted.Count * .15f);

    //    // Remove lowest and highest values
    //    var trimmed = sorted.Skip(trimCount).Take(sorted.Count - (2 * trimCount));

    //    return trimmed.Average();
    //}

    protected void ReadSpan(Span<IQ> iqPairs) {
        lock (_filteredBuffer) {
            var readBytes = _filteredBuffer.Read(iqPairs);
        }
    }

    public virtual int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);

        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < count; i++) {
                buffer[offset + i] = (float)iqBuffer[i].Phase;
            }

            return count;
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }
    }

    public void Dispose() {
        if (!_keepOpen) {
            _rfDeviceStream.Dispose();
        }
    }
}
