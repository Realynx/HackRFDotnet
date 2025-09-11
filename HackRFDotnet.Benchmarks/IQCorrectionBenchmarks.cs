// ReSharper disable ForCanBeConvertedToForeach



using BenchmarkDotNet.Attributes;

namespace HackRFDotnet.Benchmarks;

public class IQCorrectionBenchmarks {
    private const int TWO_MEGASAMPLES_FRAME_SIZE = 131_072;

    [Params(TWO_MEGASAMPLES_FRAME_SIZE)]
    public int IqLength { get; set; }

    private Complex[] _iq = [];

    [GlobalSetup]
    public void Setup() {
        _iq = Enumerable.Range(0, IqLength)
            .Select(_ => new Complex((Random.Shared.NextDouble() * 2) - 1, (Random.Shared.NextDouble() * 2) - 1))
            .Select(x => x / 100)
            .ToArray();
    }

    [Benchmark]
    public Complex[] IQCorrection_QuadAccess() {
        var iq = _iq.ToArray();

        var meanI = 0.0;
        var meanQ = 0.0;

        var offset = iq.Length % 4;
        for (var i = 0; i < iq.Length - offset; i += 4) {
            meanI += iq[i + 0].Real;
            meanQ += iq[i + 0].Imaginary;
            meanI += iq[i + 1].Real;
            meanQ += iq[i + 1].Imaginary;
            meanI += iq[i + 2].Real;
            meanQ += iq[i + 2].Imaginary;
            meanI += iq[i + 3].Real;
            meanQ += iq[i + 3].Imaginary;
        }

        for (var i = iq.Length - offset; i < iq.Length; i++) {
            meanI += iq[i].Real;
            meanQ += iq[i].Imaginary;
        }

        meanI /= iq.Length;
        meanQ /= iq.Length;

        var mean = new Complex(meanI, meanQ);
        for (var i = 0; i < iq.Length - offset; i += 4) {
            iq[i + 0] -= mean;
            iq[i + 1] -= mean;
            iq[i + 2] -= mean;
            iq[i + 3] -= mean;
        }

        for (var i = iq.Length - offset; i < iq.Length; i++) {
            iq[i] -= mean;
        }

        return iq;
    }

    [Benchmark(Baseline = true)]
    public Complex[] IQCorrection_SingleAccess() {
        var iq = _iq.ToArray();

        var meanI = 0.0;
        var meanQ = 0.0;

        for (var i = 0; i < iq.Length; i++) {
            meanI += iq[i].Real;
            meanQ += iq[i].Imaginary;
        }

        meanI /= iq.Length;
        meanQ /= iq.Length;

        var mean = new Complex(meanI, meanQ);
        for (var i = 0; i < iq.Length; i++) {
            iq[i] -= mean;
        }

        return iq;
    }
}