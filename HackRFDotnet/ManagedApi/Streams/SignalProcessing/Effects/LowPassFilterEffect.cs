
using System.Numerics;

using FftSharp;

using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class LowPassFilterEffect : SignalEffect, ISignalEffect {
    private readonly int _sampleRate;
    private readonly RadioBand _bandwith;

    /// <summary>
    /// Apply a low pass filter on the signal. Expects Frequency Domain
    /// </summary>
    /// <param name="sampleRate"></param>
    /// <param name="bandwith"></param>
    public LowPassFilterEffect(int sampleRate, RadioBand bandwith) {
        _sampleRate = sampleRate;
        _bandwith = bandwith;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        var resolution = FFT.FrequencyResolution(length, _sampleRate);

        for (var x = 0; x < length; x++) {

            var currentFreq = x * resolution;

            if (currentFreq > _bandwith.Hz + (_bandwith.Hz / 2)) {
                signalTheta[x] = Complex.Zero;
            }
        }
        return length;
    }
}