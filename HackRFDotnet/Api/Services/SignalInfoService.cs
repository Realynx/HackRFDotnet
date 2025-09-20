using System.Buffers;

using HackRFDotnet.Api.Interfaces;
using HackRFDotnet.Api.Streams;
using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalStreams;

namespace HackRFDotnet.Api.Services;
/// <summary>
/// <see cref="SignalInfoService"/> Will poll your device capture signal and track information about it's power and noise.
/// You can access this types properties to get up to date signal information for the device's capture bandwidth.
/// </summary>
public class SignalInfoService : ISignalInfoService {
    /// <summary>
    /// The amount of time over which signal metrics are aggregated to represent transient behavior.
    /// </summary>
    public TimeSpan AggregationTime { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The highest instantaneous power seen in the signal buffer.
    /// </summary>
    public double PeakPower { get; private set; }

    /// <summary>
    /// The minimum power observed in the signal over the aggregation period.
    /// </summary>
    public double MinPower { get; private set; }

    /// <summary>
    /// The variance of signal power over the aggregation period, representing signal fluctuations.
    /// </summary>
    public double PowerVariance { get; private set; }

    /// <summary>
    /// The average power of the signal over the aggregation period.
    /// </summary>
    public double AveragePower { get; private set; }

    /// <summary>
    /// The root-mean-square (RMS) of the signal samples, commonly used to measure signal strength.
    /// </summary>
    public double RMS { get; private set; }

    /// <summary>
    /// An estimate of the signal's noise floor, representing the background noise level.
    /// </summary>
    public double NoiseFloor { get; private set; }

    /// <summary>
    /// The signal-to-noise ratio (SNR) calculated over the aggregation period.
    /// </summary>
    public double SignalToNoiseRatio { get; private set; }

    /// <summary>
    /// The frequency bin with the strongest signal, typically determined via an FFT of the signal buffer.
    /// </summary>
    public double DominantFrequency { get; private set; }

    private readonly SignalStream<IQ> _signalStream;
    private readonly IDigitalRadioDevice _rfDeviceService;
    private readonly IIQStream _iQStream;
    private DateTime _lastReset = DateTime.MinValue;

    public SignalInfoService(IDigitalRadioDevice rfDeviceService) {
        _rfDeviceService = rfDeviceService;

        if (rfDeviceService.DeviceStream is null) {
            throw new ArgumentNullException("Radio device stream was not opened!");
        }

        _iQStream = rfDeviceService.DeviceStream;
        _signalStream = new SignalStream<IQ>(_iQStream) {
            BufferKeepingDelay = TimeSpan.FromSeconds(1),
            BufferKeepingCallback = PollSignal
        };
    }

    private void PollSignal(Span<IQ> signal) {
        if (signal.IsEmpty) {
            return;
        }

        var length = signal.Length;
        if (DateTime.Now > _lastReset + AggregationTime) {
            MinPower = double.MaxValue;
            PeakPower = double.MinValue;
            _lastReset = DateTime.Now;
        }

        var sumPower = 0d;
        var sumPowerSquared = 0d;
        var powers = ArrayPool<double>.Shared.Rent(length);
        try {
            for (var x = 0; x < length; x++) {
                var sample = signal[x];
                double power = (sample.I * sample.I) + (sample.Q * sample.Q);

                powers[x] = power;
                sumPower += power;
                sumPowerSquared += power * power;

                if (power > PeakPower) {
                    PeakPower = power;
                }

                if (power < MinPower) {
                    MinPower = power;
                }
            }

            AveragePower = sumPower / length;
            RMS = Math.Sqrt(AveragePower);
            PowerVariance = (sumPowerSquared / length) - (AveragePower * AveragePower);

            Array.Sort(powers);
            var noiseCount = Math.Max(1, length / 10);
            NoiseFloor = powers.Take(noiseCount).Average();

            SignalToNoiseRatio = NoiseFloor > 0 ? 10 * Math.Log10(AveragePower / NoiseFloor) : double.PositiveInfinity;
        }
        finally {
            ArrayPool<double>.Shared.Return(powers);
        }
    }
}
