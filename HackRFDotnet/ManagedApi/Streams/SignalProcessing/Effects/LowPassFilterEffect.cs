
using System.Numerics;

using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;
using HackRFDotnet.ManagedApi.Utilities;

using MathNet.Numerics;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class LowPassFilterEffect : SignalEffect, ISignalEffect {
    private readonly SampleRate _sampleRate;
    private readonly RadioBand _bandwith;

    /// <summary>
    /// Apply a low pass filter on the signal. Expects Frequency Domain
    /// </summary>
    /// <param name="sampleRate"></param>
    /// <param name="bandwith"></param>
    public LowPassFilterEffect(SampleRate sampleRate, RadioBand bandwith) {
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
        return length;
    }
}