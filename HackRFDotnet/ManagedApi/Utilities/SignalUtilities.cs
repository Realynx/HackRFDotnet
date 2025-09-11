
using System.Runtime.CompilerServices;

using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Utilities;

public unsafe class SignalUtilities {

    public static void ApplyPhaseOffset(Span<IQ> iqFrame, RadioBand freqOffset, double _sampleRate) {
        for (var x = 0; x < iqFrame.Length; x++) {
            var theta = x / _sampleRate;
            var phase = -2.0 * Math.PI * freqOffset.Hz * theta;

            var osc = new IQ(Math.Cos(phase), Math.Sin(phase));
            iqFrame[x] *= osc;
        }
    }


    /*
     * NAudio FFT Transform https://github.com/naudio/NAudio
     * <see cref="FFT(bool, int, Complex[])"/>
     * Copyright 2020 Mark Heath
     *
     * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
     *
     * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
     *
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
     */

    /// <summary>
    /// This computes an in-place complex-to-complex FFT 
    /// x and y are the real and imaginary arrays of 2^m points.
    /// </summary>
    public static void FFT(bool forward, int m, Span<IQ> iqPair) {
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
                tx = iqPair[i].I;
                ty = iqPair[i].Q;
                iqPair[i].I = iqPair[j].I;
                iqPair[j].Q = iqPair[j].Q;

                iqPair[j].I = tx;
                iqPair[j].Q = ty;
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
                    t1 = u1 * iqPair[i1].I - u2 * iqPair[i1].Q;
                    t2 = u1 * iqPair[i1].Q + u2 * iqPair[i1].I;
                    iqPair[i1].I = iqPair[i].I - t1;
                    iqPair[j].Q = iqPair[i].Q - t2;
                    iqPair[i].I += t1;
                    iqPair[i].Q += t2;
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
                iqPair[i].I /= n;
                iqPair[i].Q /= n;
            }
        }
    }

}
