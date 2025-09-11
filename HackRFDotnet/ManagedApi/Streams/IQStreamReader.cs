using System.Numerics;

using HackRFDotnet.ManagedApi.SignalProcessing;
using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Streams;
public class IQStreamReader : IDisposable {
    private readonly RfDeviceStream _rfDeviceStream;
    private readonly bool _keepOpen;

    private RadioBand _center;
    private RadioBand _bandwith;
    private FilterProcessor _filterProcessor;

    public IQStreamReader(RfDeviceStream deviceStream, bool keepOpen = true) {
        _rfDeviceStream = deviceStream;
        _keepOpen = keepOpen;
    }

    public void SetBand(RadioBand center, RadioBand bandwith) {
        _center = center;
        _bandwith = bandwith;

        _filterProcessor = new FilterProcessor(_rfDeviceStream.SampleRate, center, bandwith);
    }

    public int ReadBuffer(Span<Complex> iqBuffer) {
        while (iqBuffer.Length > _rfDeviceStream.BufferLength) {
            Thread.Sleep(1);
        }

        return _rfDeviceStream.ReadBuffer(iqBuffer);
    }

    public Complex[] ReadTime(TimeSpan chunkSize) {
        var samples = (int)(chunkSize.TotalSeconds * _rfDeviceStream.SampleRate);

        var sampleChunk = new Complex[samples];
        ReadBuffer(sampleChunk);

        return sampleChunk;
    }

    public void Dispose() {
        if (!_keepOpen) {
            _rfDeviceStream.Dispose();
        }
    }
}
