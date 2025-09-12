
using System.Numerics;
using System.Runtime.InteropServices;

using FftSharp;

using HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects.Interfaces;

namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing.Effects;
public class LowPassFilterEffect : SignalEffect, ISignalEffect {
    private readonly int _sampleRate;
    private readonly RadioBand _bandwith;

    /// <summary>
    /// Apply a low pass filter on the signal.
    /// </summary>
    /// <param name="sampleRate"></param>
    /// <param name="bandwith"></param>
    public LowPassFilterEffect(int sampleRate, RadioBand bandwith) {
        _sampleRate = sampleRate;
        _bandwith = bandwith;
    }

    public override int AffectSignal(Span<IQ> signalTheta, int length) {
        var complexFrame = MemoryMarshal
            .Cast<IQ, Complex>(signalTheta)
            .Slice(0, length);

        var resolution = FFT.FrequencyResolution(length, _sampleRate);

        FFT.Forward(complexFrame);

        for (var x = 0; x < length; x++) {
            var currentFreq = x * resolution;

            if (currentFreq > _bandwith.Hz + _bandwith.Hz / 2) {
                complexFrame[x] = Complex.Zero;
            }
        }

        FFT.Inverse(complexFrame);
        return length;
    }
}