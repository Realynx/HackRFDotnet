using BenchmarkDotNet.Running;

namespace HackRFDotnet.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<IQCorrectionBenchmarks>();
    }
}