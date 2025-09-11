using System.Numerics;

using HackRFDotnet.ManagedApi.SignalProcessing;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.Streams;
public class FmSignalStream : ISampleProvider, IDisposable {
    private readonly RfDeviceStream _rfDeviceStream;
    private readonly bool _keepOpen;

    private RadioBand _center = RadioBand.FromMHz(94.7f);
    private RadioBand _bandwith = RadioBand.FromKHz(200);
    private FilterProcessor _filterProcessor;

    public WaveFormat WaveFormat { get; private set; }

    public FmSignalStream(RfDeviceStream deviceStream, bool keepOpen = true) {
        _rfDeviceStream = deviceStream;
        _keepOpen = keepOpen;

        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)_rfDeviceStream.SampleRate, 1);
    }

    public void SetBand(RadioBand center, RadioBand bandwith) {
        _center = center;
        _bandwith = bandwith;

        _filterProcessor = new FilterProcessor(_rfDeviceStream.SampleRate, center, bandwith);
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)_rfDeviceStream.SampleRate, 1);
    }

    public int Read(float[] buffer, int offset, int count) {
        while (count > _rfDeviceStream.BufferLength) {
        }

        var iqBuffer = new Complex[count];
        var readBytes = _rfDeviceStream.ReadBuffer(iqBuffer);
        _filterProcessor.ApplyFilter(iqBuffer);

        for (var x = 1; x < count; x++) {
            var delta = iqBuffer[x] * Complex.Conjugate(iqBuffer[x - 1]);
            var fmSample = Math.Atan2(delta.Imaginary, delta.Real);
            buffer[x - 1] = (float)fmSample;
        }
        buffer[0] = 0;

        return readBytes;
    }

    public void Dispose() {
        if (!_keepOpen) {
            _rfDeviceStream.Dispose();
        }

    }
}
