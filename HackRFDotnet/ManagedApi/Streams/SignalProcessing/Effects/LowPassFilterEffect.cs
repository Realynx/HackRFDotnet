
using System.Numerics;

using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;
using HackRFDotnet.ManagedApi.Utilities;

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
        var resolution = SignalUtilities.FrequencyResolution(length, _sampleRate);

        for (var x = 0; x < length; x++) {

            var currentFreq = x * resolution;

            if (currentFreq > _bandwith.Hz + (_bandwith.Hz / 2)) {
                signalTheta[x] = Complex.Zero;
            }
        }
        return length;
    }
}