namespace HackRFDotnet.Api.Interfaces;

public interface ISignalInfoService {
    TimeSpan AggregationTime { get; set; }
    double AveragePower { get; }
    double DominantFrequency { get; }
    double MinPower { get; }
    double NoiseFloor { get; }
    double PeakPower { get; }
    double PowerVariance { get; }
    double RMS { get; }
    double SignalToNoiseRatio { get; }
}