using HackRFDotnet.Api.Streams;

namespace HackRFDotnet.Api.Utilities;
internal static class OfdmUtilities {
    /// <summary>
    /// Try to detect OFDM symbol length and cyclic prefix length
    /// from a raw stream of IQ samples.
    /// </summary>
    public static (int fftSize, int cpSize)? Detect(Span<IQ> samples, int minSymbol, int maxSymbol, double threshold = 0.8) {
        for (var symbolSize = minSymbol; symbolSize <= maxSymbol; symbolSize++) {
            // Try candidate CP ratios, e.g., CP = 1/4, 1/8 of symbol
            foreach (var cpRatio in new double[] { 0.25, 0.125 }) {
                var cpSize = (int)(symbolSize * cpRatio);
                var fftSize = symbolSize - cpSize;

                if (fftSize <= 0) {
                    continue;
                }

                // Compute correlation between CP and tail of the symbol
                var correlation = Correlate(samples, cpSize, fftSize);

                if (correlation > threshold) {
                    return (fftSize, cpSize);
                }
            }
        }

        return null; // nothing found
    }

    /// <summary>
    /// Correlation between cyclic prefix and the tail of symbol.
    /// </summary>
    private static double Correlate(Span<IQ> samples, int cpSize, int fftSize) {
        if (samples.Length < cpSize + fftSize) {
            return 0;
        }

        var sum = 0d;
        double power1 = 0, power2 = 0;

        for (var x = 0; x < cpSize; x++) {
            var prefix = samples[x];
            var tail = samples[fftSize + x];

            sum += (prefix.I * tail.I) + (prefix.Q * tail.Q);
            power1 += (prefix.I * prefix.I) + (prefix.Q * prefix.Q);
            power2 += (tail.I * tail.I) + (tail.Q * tail.Q);
        }

        return sum / Math.Sqrt((power1 * power2) + 1e-12);
    }
}
