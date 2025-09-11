using System.Numerics;

using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Utilities;
public class SignalUtilities {
    public static void ApplyPhaseOffset(Span<Complex> iqFrame, RadioBand freqOffset, double _sampleRate) {
        for (var x = 0; x < iqFrame.Length; x++) {
            var theta = x / _sampleRate;
            var phase = -2.0 * Math.PI * freqOffset.Hz * theta;

            var osc = new Complex(Math.Cos(phase), Math.Sin(phase));
            iqFrame[x] *= osc;
        }
    }

    /// <summary>
    /// This computes an in-place complex-to-complex FFT 
    /// x and y are the real and imaginary arrays of 2^m points.
    /// </summary>
    public static void FFT(bool forward, int m, Complex[] iqPair) {
        int n, i, i1, j, k, i2, l, l1, l2;
        double c1, c2, tx, ty, t1, t2, u1, u2, z;

        // Calculate the number of pointsdata[i].Real
        n = 1;
        for (i = 0; i < m; i++) {
            n *= 2;
        }

        // Do the bit reversal
        i2 = n >> 1;
        j = 0;
        for (i = 0; i < n - 1; i++) {
            if (i < j) {
                tx = iqPair[i].Real;
                ty = iqPair[i].Imaginary;
                iqPair[i].Real = iqPair[j].Real;
                iqPair[i].Imaginary = iqPair[j].Imaginary;
                iqPair[j].Real = tx;
                iqPair[j].Imaginary = ty;
            }
            k = i2;

            while (k <= j) {
                j -= k;
                k >>= 1;
            }
            j += k;
        }

        // Compute the FFT 
        c1 = -1.0f;
        c2 = 0.0f;
        l2 = 1;
        for (l = 0; l < m; l++) {
            l1 = l2;
            l2 <<= 1;
            u1 = 1.0f;
            u2 = 0.0f;
            for (j = 0; j < l1; j++) {
                for (i = j; i < n; i += l2) {
                    i1 = i + l1;
                    t1 = u1 * iqPair[i1].Real - u2 * iqPair[i1].Imaginary;
                    t2 = u1 * iqPair[i1].Imaginary + u2 * iqPair[i1].Real;
                    iqPair[i1].Real = iqPair[i].Real - t1;
                    iqPair[i1].Imaginary = iqPair[i].Imaginary - t2;
                    iqPair[i].Real += t1;
                    iqPair[i].Imaginary += t2;
                }
                z = u1 * c1 - u2 * c2;
                u2 = u1 * c2 + u2 * c1;
                u1 = z;
            }
            c2 = (float)Math.Sqrt((1.0f - c1) / 2.0f);
            if (forward) {
                c2 = -c2;
            }

            c1 = (float)Math.Sqrt((1.0f + c1) / 2.0f);
        }

        // Scaling for forward transform 
        if (forward) {
            for (i = 0; i < n; i++) {
                iqPair[i].Real /= n;
                iqPair[i].Imaginary /= n;
            }
        }
    }

}
