using System.Numerics;

using BenchmarkDotNet.Attributes;

namespace HackRFDotnet.Benchmarks;

public class IQConvertBytesBenchmarks {
    private const int TWO_MEGASAMPLES_FRAME_SIZE = 262144;

    [Params(TWO_MEGASAMPLES_FRAME_SIZE)]
    public int IqBytesLength { get; set; }

    private byte[] _iqBytes = [];

    [GlobalSetup]
    public void Setup() {
        _iqBytes = Enumerable.Range(0, IqBytesLength)
            .Select(_ => (Random.Shared.NextDouble() * 2) - 1)
            .Select(x => x / 100)
            .Select(x => (byte)(x * 128.0))
            .ToArray();
    }

    [Benchmark]
    public Complex[] IQConvertBytes_QuadAccess() {
        var sampleCount = _iqBytes.Length / 2;
        var iq = new Complex[sampleCount];

        var offset = sampleCount % 4;
        for (var i = 0; i < sampleCount - offset; i += 4) {
            var iVal1 = (sbyte)_iqBytes[((i + 0) * 2)] / 128.0;
            var qVal1 = (sbyte)_iqBytes[((i + 0) * 2) + 1] / 128.0;
            var iVal2 = (sbyte)_iqBytes[((i + 1) * 2)] / 128.0;
            var qVal2 = (sbyte)_iqBytes[((i + 1) * 2) + 1] / 128.0;
            var iVal3 = (sbyte)_iqBytes[((i + 2) * 2)] / 128.0;
            var qVal3 = (sbyte)_iqBytes[((i + 2) * 2) + 1] / 128.0;
            var iVal4 = (sbyte)_iqBytes[((i + 3) * 2)] / 128.0;
            var qVal4 = (sbyte)_iqBytes[((i + 3) * 2) + 1] / 128.0;

            iq[i + 0] = new Complex(iVal1, qVal1);
            iq[i + 1] = new Complex(iVal2, qVal2);
            iq[i + 2] = new Complex(iVal3, qVal3);
            iq[i + 3] = new Complex(iVal4, qVal4);
        }

        for (var i = sampleCount - offset; i < sampleCount; i++) {
            var iVal = (sbyte)_iqBytes[(i * 2)] / 128.0;
            var qVal = (sbyte)_iqBytes[(i * 2) + 1] / 128.0;

            iq[i] = new Complex(iVal, qVal);
        }

        return iq;
    }

    [Benchmark(Baseline = true)]
    public Complex[] IQConvertBytes_SingleAccess() {
        var sampleCount = _iqBytes.Length / 2;
        var iq = new Complex[sampleCount];

        for (var i = 0; i < sampleCount; i++) {
            var iVal = (sbyte)_iqBytes[(i * 2)] / 128.0;
            var qVal = (sbyte)_iqBytes[(i * 2) + 1] / 128.0;

            iq[i] = new Complex(iVal, qVal);
        }

        return iq;
    }
}