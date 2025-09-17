using HackRFDotnet.Api.Utilities;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Effects;
/// <summary>
/// Low Pass Filter Effect to remove unwanted signals from the input signal.
/// Configured with a bandwidth to limit via the filter.
/// </summary>
public class LowPassFilterEffect : SignalEffect<IQ, IQ> {
    private readonly SampleRate _sampleRate;
    private readonly Bandwidth _bandwith;

    /// <summary>
    /// Apply a low pass filter on the signal. Expects Frequency Domain input.
    /// </summary>
    /// <param name="sampleRate"></param>
    /// <param name="bandwith"></param>
    public LowPassFilterEffect(SampleRate sampleRate, Bandwidth bandwith) {
        _sampleRate = sampleRate;
        _bandwith = bandwith;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        var resolution = SignalUtilities.FrequencyResolution(length, _sampleRate, false);

        for (var x = 0; x < length; x++) {
            var freq = (x < length / 2) ? x * resolution : (x - length) * resolution;
            var bandwidth = _bandwith.Hz;
            if (Math.Abs(freq) > bandwidth) {
                signalTheta[x] = IQ.Zero;
            }
        }

        return base.AffectSignal(signalTheta, length);
    }
}