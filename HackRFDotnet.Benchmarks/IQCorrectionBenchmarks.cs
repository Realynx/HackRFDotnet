// ReSharper disable ForCanBeConvertedToForeach

using System.Numerics;

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
            .Select(x => x /= 100)
            .ToArray();
    }

    [Benchmark]
    public void IQCorrection_QuadAccess() {
        var meanI = 0.0;
        var meanQ = 0.0;

        var offset = _iq.Length % 4;
        for (var i = 0; i < _iq.Length - offset; i += 4) {
            meanI += _iq[i].Real;
            meanQ += _iq[i].Imaginary;
            meanI += _iq[i + 1].Real;
            meanQ += _iq[i + 1].Imaginary;
            meanI += _iq[i + 2].Real;
            meanQ += _iq[i + 2].Imaginary;
            meanI += _iq[i + 3].Real;
            meanQ += _iq[i + 3].Imaginary;
        }

        for (var i = _iq.Length - offset; i < _iq.Length; i++) {
            meanI += _iq[i].Real;
            meanQ += _iq[i].Imaginary;
        }

        meanI /= _iq.Length;
        meanQ /= _iq.Length;

        var mean = new Complex(meanI, meanQ);
        for (var i = 0; i < _iq.Length - offset; i += 4) {
            _iq[i] -= mean;
            _iq[i + 1] -= mean;
            _iq[i + 2] -= mean;
            _iq[i + 3] -= mean;
        }

        for (var i = _iq.Length - offset; i < _iq.Length; i++) {
            _iq[i] -= mean;
        }
    }

    [Benchmark(Baseline = true)]
    public void IQCorrection_SingleAccess() {
        var meanI = 0.0;
        var meanQ = 0.0;

        for (var i = 0; i < _iq.Length; i++) {
            meanI += _iq[i].Real;
            meanQ += _iq[i].Imaginary;
        }

        meanI /= _iq.Length;
        meanQ /= _iq.Length;

        var mean = new Complex(meanI, meanQ);
        for (var i = 0; i < _iq.Length; i++) {
            _iq[i] -= mean;
        }
    }
}