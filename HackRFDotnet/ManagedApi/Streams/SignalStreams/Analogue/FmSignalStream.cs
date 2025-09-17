using System.Buffers;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

namespace HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;
public class FmSignalStream : WaveSignalStream {
    public FmSignalStream(IIQStream deviceStream, SampleRate sampleRate, bool stereo = true, SignalProcessingPipeline? processingPipeline = null, bool keepOpen = true)
        : base(deviceStream, sampleRate, stereo, processingPipeline, keepOpen) {

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


            // NormalizeRms(buffer);
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


