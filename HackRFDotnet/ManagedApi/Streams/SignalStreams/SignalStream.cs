using System.Buffers;
using System.Numerics;

using HackRFDotnet.ManagedApi.SignalProcessing;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams;
public class SignalStream : ISampleProvider, IDisposable {
    public WaveFormat? WaveFormat { get; protected set; }

    protected readonly bool _keepOpen;
    protected readonly RfDeviceStream _rfDeviceStream;

    protected RadioBand _center = RadioBand.FromMHz(94.7f);
    protected RadioBand _bandwith = RadioBand.FromKHz(200);

    protected FilterProcessor? _filterProcessor;

    public SignalStream(RfDeviceStream deviceStream, bool keepOpen = true) {
        _keepOpen = keepOpen;
        _rfDeviceStream = deviceStream;
    }

    public void SetBand(RadioBand center, RadioBand bandwidth) {
        _center = center;
        _bandwith = bandwidth;

        _filterProcessor = new FilterProcessor(_rfDeviceStream.SampleRate, center, bandwidth);
    }

    protected int ReadSpan(Span<Complex> iqPairs) {
        var readBytes = _rfDeviceStream.ReadBuffer(iqPairs);
        _filterProcessor.ApplyFilter(iqPairs);

        return readBytes;
    }

    public virtual int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<Complex>.Shared.Rent(count);
        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var i = 0; i < count; i++) {
                buffer[offset + i] = (float)iqBuffer[i].Real;
            }

            return count;
        }
        finally {
            ArrayPool<Complex>.Shared.Return(iqBuffer);
        }
    }

    public void Dispose() {
        if (!_keepOpen) {
            _rfDeviceStream.Dispose();
        }
    }
}
