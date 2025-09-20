using HackRFDotnet.Api.Streams.SignalProcessing.Effects;
using HackRFDotnet.Api.Utilities;

namespace HackRFDotnet.Api.Streams.SignalProcessing.FormatConverters;

/// <summary>
/// This is discover the channel division inside an OFDM stream.
/// It will then divide the samples into each channel via FFT.
/// Each bin will be a channel with a complex number.
/// Use this complex number at each bin to demodulate information from the complex plane.
/// </summary>
public class OfdmChannelizer : SignalEffect<IQ, IQ> {

    public OfdmChannelizer() {

    }

    public override int TransformSignal(Span<IQ> signalTheta, int length) {
        var outputIndex = 0;

        var detectedSymbolDetails = OfdmUtilities.Detect(signalTheta.Slice(0, length), 32, 8192);
        if (detectedSymbolDetails is null) {
            return length;
        }

        (int fftSize, int prefixSize) = detectedSymbolDetails.Value;

        return base.TransformSignal(signalTheta, length);
    }
}
