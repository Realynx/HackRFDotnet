using System.Buffers;

using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalProcessing;
using HackRFDotnet.Api.Streams.SignalProcessing.Effects;

namespace HackRFDotnet.Api.Streams.SignalStreams.Analogue;
/// <summary>
/// Demodulate FM audio from <see cref="IIQStream"/>.
/// </summary>
public class FmSignalStream : WaveSignalStream {
    public FmSignalStream(IIQStream deviceStream, Bandwidth stationBandwidth, bool stereo = true)
        : base(deviceStream, BuildFxChain(deviceStream, stationBandwidth, out var sampleRate), sampleRate, stereo, false) {
    }

    private static SignalProcessingPipeline BuildFxChain(IIQStream deviceStream, Bandwidth stationBandwidth, out SampleRate reducedRate) {
        return new SignalProcessingBuilder()
        .AddSignalEffect(new DownSampleEffect(deviceStream.SampleRate,
            stationBandwidth.NyquistSampleRate, out reducedRate, out var producedChunkSize))

        .AddSignalEffect(new FftEffect(true, producedChunkSize))
        .AddSignalEffect(new LowPassFilterEffect(reducedRate, stationBandwidth))
        .AddSignalEffect(new FftEffect(false, producedChunkSize))
        .BuildPipeline();
    }

    public override int Read(float[] buffer, int offset, int count) {
        var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);
        try {
            ReadSpan(iqBuffer.AsSpan(0, count));

            for (var x = 1; x < count; x++) {
                var delta = iqBuffer[x] * IQ.Conjugate(iqBuffer[x - 1]);
                buffer[x - 1] = delta.Phase;
            }
            buffer[count - 1] = buffer[count - 2];


            NormalizeRms(buffer);
        }
        finally {
            ArrayPool<IQ>.Shared.Return(iqBuffer);
        }

        return count;
    }

    //public override int Read(float[] buffer, int offset, int count) {
    //    var iqBuffer = ArrayPool<IQ>.Shared.Rent(count);
    //    try {
    //        ReadSpan(iqBuffer.AsSpan(0, count));
    //        // Squelch(iqBuffer.AsSpan(0, count));

    //        // split into primitive arrays for SIMD
    //        var real = new double[count];
    //        var imag = new double[count];

    //        for (var x = 0; x < count; x++) {
    //            real[x] = iqBuffer[x].I;
    //            imag[x] = iqBuffer[x].Q;
    //        }

    //        var simdWidth = Vector<double>.Count;
    //        var i = 1;
    //        for (; i <= count - simdWidth; i += simdWidth) {
    //            var r2 = new Vector<double>(real, i);
    //            var i2 = new Vector<double>(imag, i);
    //            var r1 = new Vector<double>(real, i - 1);
    //            var i1 = new Vector<double>(imag, i - 1);

    //            var realPart = r2 * r1 + i2 * i1;
    //            var imagPart = i2 * r1 - r2 * i1;

    //            for (var j = 0; j < simdWidth; j++) {
    //                buffer[offset + i - 1 + j] = (float)MathF.Atan2(imagPart[j], realPart[j]);
    //            }
    //        }

    //        // Scalar tail for remaining elements
    //        for (; i < count; i++) {
    //            var delta = iqBuffer[i] * IQ.Conjugate(iqBuffer[i - 1]);
    //            buffer[offset + i - 1] = (float)delta.Phase;
    //        }

    //        NormalizeRms(buffer);
    //    }
    //    finally {
    //        ArrayPool<IQ>.Shared.Return(iqBuffer);
    //    }

    //    return count;
    //}


}


