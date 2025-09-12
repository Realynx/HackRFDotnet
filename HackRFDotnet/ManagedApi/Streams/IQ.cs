/*
 * System.Numerics.Complex struct licensed under the MIT license
 * https://source.dot.net/#System.Runtime.Numerics/System/Numerics/Complex.cs,3e2d1e25b7131757
 */

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using HackRFDotnet.ManagedApi.Streams.Device;

namespace HackRFDotnet.ManagedApi.Streams;

public struct IQ : IEquatable<IQ>, IFormattable {
    public static readonly IQ Zero = new IQ(0.0, 0.0);
    public static readonly IQ One = new IQ(1.0, 0.0);
    public static readonly IQ ImaginaryOne = new IQ(0.0, 1.0);
    public static readonly IQ NaN = new IQ(double.NaN, double.NaN);
    public static readonly IQ Infinity = new IQ(double.PositiveInfinity, double.PositiveInfinity);

    private const double InverseOfLog10 = 0.43429448190325; // 1 / Log(10)

    // This is the largest x for which (Hypot(x,x) + x) will not overflow. It is used for branching inside Sqrt.
    private static readonly double s_sqrtRescaleThreshold = double.MaxValue / (Math.Sqrt(2.0) + 1.0);

    // This is the largest x for which 2 x^2 will not overflow. It is used for branching inside Asin and Acos.
    private static readonly double s_asinOverflowThreshold = Math.Sqrt(double.MaxValue) / 2.0;

    // This value is used inside Asin and Acos.
    private static readonly double s_log2 = Math.Log(2.0);

    private double m_real;
    private double m_imaginary;

    public IQ(double real, double imaginary) {
        m_real = real;
        m_imaginary = imaginary;
    }

    public IQ(InterleavedSample interleavedSample) {
        m_real = interleavedSample.I;
        m_imaginary = interleavedSample.Q;
    }

    /// <summary>
    /// Real
    /// </summary>
    public double I { get { return m_real; } set { m_real = value; } }

    /// <summary>
    /// Imaginary
    /// </summary>
    public double Q { get { return m_imaginary; } set { m_imaginary = value; } }

    public double Magnitude { get { return Abs(this); } }
    public double Phase { get { return Math.Atan2(m_imaginary, m_real); } }

    public static IQ FromPolarCoordinates(double magnitude, double phase) {
        (var sin, var cos) = Math.SinCos(phase);
        return new IQ(magnitude * cos, magnitude * sin);
    }

    public static IQ Negate(IQ value) {
        return -value;
    }

    public static IQ Add(IQ left, IQ right) {
        return left + right;
    }

    public static IQ Add(IQ left, double right) {
        return left + right;
    }

    public static IQ Add(double left, IQ right) {
        return left + right;
    }

    public static IQ Subtract(IQ left, IQ right) {
        return left - right;
    }

    public static IQ Subtract(IQ left, double right) {
        return left - right;
    }

    public static IQ Subtract(double left, IQ right) {
        return left - right;
    }

    public static IQ Multiply(IQ left, IQ right) {
        return left * right;
    }

    public static IQ Multiply(IQ left, double right) {
        return left * right;
    }

    public static IQ Multiply(double left, IQ right) {
        return left * right;
    }

    public static IQ Divide(IQ dividend, IQ divisor) {
        return dividend / divisor;
    }

    public static IQ Divide(IQ dividend, double divisor) {
        return dividend / divisor;
    }

    public static IQ Divide(double dividend, IQ divisor) {
        return dividend / divisor;
    }

    public static IQ operator -(IQ value) /* Unary negation of a complex number */ {
        return new IQ(-value.m_real, -value.m_imaginary);
    }

    public static IQ operator +(IQ left, IQ right) {
        return new IQ(left.m_real + right.m_real, left.m_imaginary + right.m_imaginary);
    }

    public static IQ operator +(IQ left, double right) {
        return new IQ(left.m_real + right, left.m_imaginary);
    }

    public static IQ operator +(double left, IQ right) {
        return new IQ(left + right.m_real, right.m_imaginary);
    }

    public static IQ operator -(IQ left, IQ right) {
        return new IQ(left.m_real - right.m_real, left.m_imaginary - right.m_imaginary);
    }

    public static IQ operator -(IQ left, double right) {
        return new IQ(left.m_real - right, left.m_imaginary);
    }

    public static IQ operator -(double left, IQ right) {
        return new IQ(left - right.m_real, -right.m_imaginary);
    }

    public static IQ operator *(IQ left, IQ right) {
        // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
        var result_realpart = left.m_real * right.m_real - left.m_imaginary * right.m_imaginary;
        var result_imaginarypart = left.m_imaginary * right.m_real + left.m_real * right.m_imaginary;
        return new IQ(result_realpart, result_imaginarypart);
    }

    public static IQ operator *(IQ left, double right) {
        if (!double.IsFinite(left.m_real)) {
            if (!double.IsFinite(left.m_imaginary)) {
                return new IQ(double.NaN, double.NaN);
            }

            return new IQ(left.m_real * right, double.NaN);
        }

        if (!double.IsFinite(left.m_imaginary)) {
            return new IQ(double.NaN, left.m_imaginary * right);
        }

        return new IQ(left.m_real * right, left.m_imaginary * right);
    }

    public static IQ operator *(double left, IQ right) {
        if (!double.IsFinite(right.m_real)) {
            if (!double.IsFinite(right.m_imaginary)) {
                return new IQ(double.NaN, double.NaN);
            }

            return new IQ(left * right.m_real, double.NaN);
        }

        if (!double.IsFinite(right.m_imaginary)) {
            return new IQ(double.NaN, left * right.m_imaginary);
        }

        return new IQ(left * right.m_real, left * right.m_imaginary);
    }

    public static IQ operator /(IQ left, IQ right) {
        // Division : Smith's formula.
        var a = left.m_real;
        var b = left.m_imaginary;
        var c = right.m_real;
        var d = right.m_imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (Math.Abs(d) < Math.Abs(c)) {
            var doc = d / c;
            return new IQ((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
        }
        else {
            var cod = c / d;
            return new IQ((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
        }
    }

    public static IQ operator /(IQ left, double right) {
        // IEEE prohibit optimizations which are value changing
        // so we make sure that behaviour for the simplified version exactly match
        // full version.
        if (right == 0) {
            return new IQ(double.NaN, double.NaN);
        }

        if (!double.IsFinite(left.m_real)) {
            if (!double.IsFinite(left.m_imaginary)) {
                return new IQ(double.NaN, double.NaN);
            }

            return new IQ(left.m_real / right, double.NaN);
        }

        if (!double.IsFinite(left.m_imaginary)) {
            return new IQ(double.NaN, left.m_imaginary / right);
        }

        // Here the actual optimized version of code.
        return new IQ(left.m_real / right, left.m_imaginary / right);
    }

    public static IQ operator /(double left, IQ right) {
        // Division : Smith's formula.
        var a = left;
        var c = right.m_real;
        var d = right.m_imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (Math.Abs(d) < Math.Abs(c)) {
            var doc = d / c;
            return new IQ(a / (c + d * doc), -a * doc / (c + d * doc));
        }
        else {
            var cod = c / d;
            return new IQ(a * cod / (d + c * cod), -a / (d + c * cod));
        }
    }

    public static double Abs(IQ value) {
        return double.Hypot(value.m_real, value.m_imaginary);
    }

    private static double Log1P(double x) {
        // Compute log(1 + x) without loss of accuracy when x is small.

        // Our only use case so far is for positive values, so this isn't coded to handle negative values.
        Debug.Assert(x >= 0.0 || double.IsNaN(x));

        var xp1 = 1.0 + x;
        if (xp1 == 1.0) {
            return x;
        }
        else if (x < 0.75) {
            // This is accurate to within 5 ulp with any floating-point system that uses a guard digit,
            // as proven in Theorem 4 of "What Every Computer Scientist Should Know About Floating-Point
            // Arithmetic" (https://docs.oracle.com/cd/E19957-01/806-3568/ncg_goldberg.html)
            return x * Math.Log(xp1) / (xp1 - 1.0);
        }
        else {
            return Math.Log(xp1);
        }
    }

    public static IQ Conjugate(IQ value) {
        // Conjugate of a Complex number: the conjugate of x+i*y is x-i*y
        return new IQ(value.m_real, -value.m_imaginary);
    }

    public static IQ Reciprocal(IQ value) {
        // Reciprocal of a Complex number : the reciprocal of x+i*y is 1/(x+i*y)
        if (value.m_real == 0 && value.m_imaginary == 0) {
            return Zero;
        }

        return One / value;
    }

    public static bool operator ==(IQ left, IQ right) {
        return left.m_real == right.m_real && left.m_imaginary == right.m_imaginary;
    }

    public static bool operator !=(IQ left, IQ right) {
        return left.m_real != right.m_real || left.m_imaginary != right.m_imaginary;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) {
        return obj is IQ other && Equals(other);
    }

    public bool Equals(IQ value) {
        return m_real.Equals(value.m_real) && m_imaginary.Equals(value.m_imaginary);
    }

    public override int GetHashCode() => HashCode.Combine(m_real, m_imaginary);

    public override string ToString() => ToString(null, null);

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) => ToString(format, null);

    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider) {
        // $"<{m_real.ToString(format, provider)}; {m_imaginary.ToString(format, provider)}>";
        var handler = new DefaultInterpolatedStringHandler(4, 2, provider, stackalloc char[512]);
        handler.AppendLiteral("<");
        handler.AppendFormatted(m_real, format);
        handler.AppendLiteral("; ");
        handler.AppendFormatted(m_imaginary, format);
        handler.AppendLiteral(">");
        return handler.ToStringAndClear();
    }

    public static IQ Sin(IQ value) {
        (var sin, var cos) = Math.SinCos(value.m_real);
        return new IQ(sin * Math.Cosh(value.m_imaginary), cos * Math.Sinh(value.m_imaginary));
        // There is a known limitation with this algorithm: inputs that cause sinh and cosh to overflow, but for
        // which sin or cos are small enough that sin * cosh or cos * sinh are still representable, nonetheless
        // produce overflow. For example, Sin((0.01, 711.0)) should produce (~3.0E306, PositiveInfinity), but
        // instead produces (PositiveInfinity, PositiveInfinity).
    }

    public static IQ Sinh(IQ value) {
        // Use sinh(z) = -i sin(iz) to compute via sin(z).
        var sin = Sin(new IQ(-value.m_imaginary, value.m_real));
        return new IQ(sin.m_imaginary, -sin.m_real);
    }

    public static IQ Asin(IQ value) {
        double b, bPrime, v;
        Asin_Internal(Math.Abs(value.I), Math.Abs(value.Q), out b, out bPrime, out v);

        double u;
        if (bPrime < 0.0) {
            u = Math.Asin(b);
        }
        else {
            u = Math.Atan(bPrime);
        }

        if (value.I < 0.0)
            u = -u;
        if (value.Q < 0.0)
            v = -v;

        return new IQ(u, v);
    }

    public static IQ Cos(IQ value) {
        (var sin, var cos) = Math.SinCos(value.m_real);
        return new IQ(cos * Math.Cosh(value.m_imaginary), -sin * Math.Sinh(value.m_imaginary));
    }

    public static IQ Cosh(IQ value) {
        // Use cosh(z) = cos(iz) to compute via cos(z).
        return Cos(new IQ(-value.m_imaginary, value.m_real));
    }

    public static IQ Acos(IQ value) {
        double b, bPrime, v;
        Asin_Internal(Math.Abs(value.I), Math.Abs(value.Q), out b, out bPrime, out v);

        double u;
        if (bPrime < 0.0) {
            u = Math.Acos(b);
        }
        else {
            u = Math.Atan(1.0 / bPrime);
        }

        if (value.I < 0.0)
            u = Math.PI - u;
        if (value.Q > 0.0)
            v = -v;

        return new IQ(u, v);
    }

    public static IQ Tan(IQ value) {
        // tan z = sin z / cos z, but to avoid unnecessary repeated trig computations, use
        //   tan z = (sin(2x) + i sinh(2y)) / (cos(2x) + cosh(2y))
        // (see Abramowitz & Stegun 4.3.57 or derive by hand), and compute trig functions here.

        // This approach does not work for |y| > ~355, because sinh(2y) and cosh(2y) overflow,
        // even though their ratio does not. In that case, divide through by cosh to get:
        //   tan z = (sin(2x) / cosh(2y) + i \tanh(2y)) / (1 + cos(2x) / cosh(2y))
        // which correctly computes the (tiny) real part and the (normal-sized) imaginary part.

        var x2 = 2.0 * value.m_real;
        var y2 = 2.0 * value.m_imaginary;
        (var sin, var cos) = Math.SinCos(x2);
        var cosh = Math.Cosh(y2);
        if (Math.Abs(value.m_imaginary) <= 4.0) {
            var D = cos + cosh;
            return new IQ(sin / D, Math.Sinh(y2) / D);
        }
        else {
            var D = 1.0 + cos / cosh;
            return new IQ(sin / cosh / D, Math.Tanh(y2) / D);
        }
    }

    public static IQ Tanh(IQ value) {
        // Use tanh(z) = -i tan(iz) to compute via tan(z).
        var tan = Tan(new IQ(-value.m_imaginary, value.m_real));
        return new IQ(tan.m_imaginary, -tan.m_real);
    }

    public static IQ Atan(IQ value) {
        var two = new IQ(2.0, 0.0);
        return ImaginaryOne / two * (Log(One - ImaginaryOne * value) - Log(One + ImaginaryOne * value));
    }

    private static void Asin_Internal(double x, double y, out double b, out double bPrime, out double v) {
        // This method for the inverse complex sine (and cosine) is described in Hull, Fairgrieve,
        // and Tang, "Implementing the Complex Arcsine and Arccosine Functions Using Exception Handling",
        // ACM Transactions on Mathematical Software (1997)
        // (https://www.researchgate.net/profile/Ping_Tang3/publication/220493330_Implementing_the_Complex_Arcsine_and_Arccosine_Functions_Using_Exception_Handling/links/55b244b208ae9289a085245d.pdf)

        // First, the basics: start with sin(w) = (e^{iw} - e^{-iw}) / (2i) = z. Here z is the input
        // and w is the output. To solve for w, define t = e^{i w} and multiply through by t to
        // get the quadratic equation t^2 - 2 i z t - 1 = 0. The solution is t = i z + sqrt(1 - z^2), so
        //   w = arcsin(z) = - i log( i z + sqrt(1 - z^2) )
        // Decompose z = x + i y, multiply out i z + sqrt(1 - z^2), use log(s) = |s| + i arg(s), and do a
        // bunch of algebra to get the components of w = arcsin(z) = u + i v
        //   u = arcsin(beta)  v = sign(y) log(alpha + sqrt(alpha^2 - 1))
        // where
        //   alpha = (rho + sigma) / 2      beta = (rho - sigma) / 2
        //   rho = sqrt((x + 1)^2 + y^2)    sigma = sqrt((x - 1)^2 + y^2)
        // These formulas appear in DLMF section 4.23. (http://dlmf.nist.gov/4.23), along with the analogous
        //   arccos(w) = arccos(beta) - i sign(y) log(alpha + sqrt(alpha^2 - 1))
        // So alpha and beta together give us arcsin(w) and arccos(w).

        // As written, alpha is not susceptible to cancelation errors, but beta is. To avoid cancelation, note
        //   beta = (rho^2 - sigma^2) / (rho + sigma) / 2 = (2 x) / (rho + sigma) = x / alpha
        // which is not subject to cancelation. Note alpha >= 1 and |beta| <= 1.

        // For alpha ~ 1, the argument of the log is near unity, so we compute (alpha - 1) instead,
        // write the argument as 1 + (alpha - 1) + sqrt((alpha - 1)(alpha + 1)), and use the log1p function
        // to compute the log without loss of accuracy.
        // For beta ~ 1, arccos does not accurately resolve small angles, so we compute the tangent of the angle
        // instead.
        // Hull, Fairgrieve, and Tang derive formulas for (alpha - 1) and beta' = tan(u) that do not suffer
        // from cancelation in these cases.

        // For simplicity, we assume all positive inputs and return all positive outputs. The caller should
        // assign signs appropriate to the desired cut conventions. We return v directly since its magnitude
        // is the same for both arcsin and arccos. Instead of u, we usually return beta and sometimes beta'.
        // If beta' is not computed, it is set to -1; if it is computed, it should be used instead of beta
        // to determine u. Compute u = arcsin(beta) or u = arctan(beta') for arcsin, u = arccos(beta)
        // or arctan(1/beta') for arccos.

        Debug.Assert(x >= 0.0 || double.IsNaN(x));
        Debug.Assert(y >= 0.0 || double.IsNaN(y));

        // For x or y large enough to overflow alpha^2, we can simplify our formulas and avoid overflow.
        if (x > s_asinOverflowThreshold || y > s_asinOverflowThreshold) {
            b = -1.0;
            bPrime = x / y;

            double small, big;
            if (x < y) {
                small = x;
                big = y;
            }
            else {
                small = y;
                big = x;
            }

            var ratio = small / big;
            v = s_log2 + Math.Log(big) + 0.5 * Log1P(ratio * ratio);
        }
        else {
            var r = double.Hypot(x + 1.0, y);
            var s = double.Hypot(x - 1.0, y);

            var a = (r + s) * 0.5;
            b = x / a;

            if (b > 0.75) {
                if (x <= 1.0) {
                    var amx = (y * y / (r + (x + 1.0)) + (s + (1.0 - x))) * 0.5;
                    bPrime = x / Math.Sqrt((a + x) * amx);
                }
                else {
                    // In this case, amx ~ y^2. Since we take the square root of amx, we should
                    // pull y out from under the square root so we don't lose its contribution
                    // when y^2 underflows.
                    var t = (1.0 / (r + (x + 1.0)) + 1.0 / (s + (x - 1.0))) * 0.5;
                    bPrime = x / y / Math.Sqrt((a + x) * t);
                }
            }
            else {
                bPrime = -1.0;
            }

            if (a < 1.5) {
                if (x < 1.0) {
                    // This is another case where our expression is proportional to y^2 and
                    // we take its square root, so again we pull out a factor of y from
                    // under the square root.
                    var t = (1.0 / (r + (x + 1.0)) + 1.0 / (s + (1.0 - x))) * 0.5;
                    var am1 = y * y * t;
                    v = Log1P(am1 + y * Math.Sqrt(t * (a + 1.0)));
                }
                else {
                    var am1 = (y * y / (r + (x + 1.0)) + (s + (x - 1.0))) * 0.5;
                    v = Log1P(am1 + Math.Sqrt(am1 * (a + 1.0)));
                }
            }
            else {
                // Because of the test above, we can be sure that a * a will not overflow.
                v = Math.Log(a + Math.Sqrt((a - 1.0) * (a + 1.0)));
            }
        }
    }

    public static bool IsFinite(IQ value) => double.IsFinite(value.m_real) && double.IsFinite(value.m_imaginary);

    public static bool IsInfinity(IQ value) => double.IsInfinity(value.m_real) || double.IsInfinity(value.m_imaginary);

    public static bool IsNaN(IQ value) => !IsInfinity(value) && !IsFinite(value);

    public static IQ Log(IQ value) {
        return new IQ(Math.Log(Abs(value)), Math.Atan2(value.m_imaginary, value.m_real));
    }

    public static IQ Log(IQ value, double baseValue) {
        return Log(value) / Log(baseValue);
    }

    public static IQ Log10(IQ value) {
        var tempLog = Log(value);
        return Scale(tempLog, InverseOfLog10);
    }

    public static IQ Exp(IQ value) {
        var expReal = Math.Exp(value.m_real);
        return FromPolarCoordinates(expReal, value.m_imaginary);
    }

    public static IQ Sqrt(IQ value) {
        // Handle NaN input cases according to IEEE 754
        if (double.IsNaN(value.m_real)) {
            if (double.IsInfinity(value.m_imaginary)) {
                return new IQ(double.PositiveInfinity, value.m_imaginary);
            }

            return new IQ(double.NaN, double.NaN);
        }

        if (double.IsNaN(value.m_imaginary)) {
            if (double.IsPositiveInfinity(value.m_real)) {
                return new IQ(double.NaN, double.PositiveInfinity);
            }

            if (double.IsNegativeInfinity(value.m_real)) {
                return new IQ(double.PositiveInfinity, double.NaN);
            }

            return new IQ(double.NaN, double.NaN);
        }

        if (value.m_imaginary == 0.0) {
            // Handle the trivial case quickly.
            if (value.m_real < 0.0) {
                return new IQ(0.0, Math.Sqrt(-value.m_real));
            }

            return new IQ(Math.Sqrt(value.m_real), 0.0);
        }

        // One way to compute Sqrt(z) is just to call Pow(z, 0.5), which coverts to polar coordinates
        // (sqrt + atan), halves the phase, and reconverts to cartesian coordinates (cos + sin).
        // Not only is this more expensive than necessary, it also fails to preserve certain expected
        // symmetries, such as that the square root of a pure negative is a pure imaginary, and that the
        // square root of a pure imaginary has exactly equal real and imaginary parts. This all goes
        // back to the fact that Math.PI is not stored with infinite precision, so taking half of Math.PI
        // does not land us on an argument with cosine exactly equal to zero.

        // To find a fast and symmetry-respecting formula for complex square root,
        // note x + i y = \sqrt{a + i b} implies x^2 + 2 i x y - y^2 = a + i b,
        // so x^2 - y^2 = a and 2 x y = b. Cross-substitute and use the quadratic formula to obtain
        //   x = \sqrt{\frac{\sqrt{a^2 + b^2} + a}{2}}  y = \pm \sqrt{\frac{\sqrt{a^2 + b^2} - a}{2}}
        // There is just one complication: depending on the sign on a, either x or y suffers from
        // cancelation when |b| << |a|. We can get around this by noting that our formulas imply
        // x^2 y^2 = b^2 / 4, so |x| |y| = |b| / 2. So after computing the one that doesn't suffer
        // from cancelation, we can compute the other with just a division. This is basically just
        // the right way to evaluate the quadratic formula without cancelation.

        // All this reduces our total cost to two sqrts and a few flops, and it respects the desired
        // symmetries. Much better than atan + cos + sin!

        // The signs are a matter of choice of branch cut, which is traditionally taken so x > 0 and sign(y) = sign(b).

        // If the components are too large, Hypot will overflow, even though the subsequent sqrt would
        // make the result representable. To avoid this, we re-scale (by exact powers of 2 for accuracy)
        // when we encounter very large components to avoid intermediate infinities.
        var rescale = false;
        var realCopy = value.m_real;
        var imaginaryCopy = value.m_imaginary;
        if (Math.Abs(realCopy) >= s_sqrtRescaleThreshold || Math.Abs(imaginaryCopy) >= s_sqrtRescaleThreshold) {
            if (double.IsInfinity(value.m_imaginary)) {
                // We need to handle infinite imaginary parts specially because otherwise
                // our formulas below produce inf/inf = NaN.
                return new IQ(double.PositiveInfinity, imaginaryCopy);
            }

            realCopy *= 0.25;
            imaginaryCopy *= 0.25;
            rescale = true;
        }

        // This is the core of the algorithm. Everything else is special case handling.
        double x, y;
        if (realCopy >= 0.0) {
            x = Math.Sqrt((double.Hypot(realCopy, imaginaryCopy) + realCopy) * 0.5);
            y = imaginaryCopy / (2.0 * x);
        }
        else {
            y = Math.Sqrt((double.Hypot(realCopy, imaginaryCopy) - realCopy) * 0.5);
            if (imaginaryCopy < 0.0)
                y = -y;
            x = imaginaryCopy / (2.0 * y);
        }

        if (rescale) {
            x *= 2.0;
            y *= 2.0;
        }

        return new IQ(x, y);
    }

    public static IQ Pow(IQ value, IQ power) {
        if (power == Zero) {
            return One;
        }

        if (value == Zero) {
            return Zero;
        }

        var valueReal = value.m_real;
        var valueImaginary = value.m_imaginary;
        var powerReal = power.m_real;
        var powerImaginary = power.m_imaginary;

        var rho = Abs(value);
        var theta = Math.Atan2(valueImaginary, valueReal);
        var newRho = powerReal * theta + powerImaginary * Math.Log(rho);

        var t = Math.Pow(rho, powerReal) * Math.Exp(-powerImaginary * theta);

        return FromPolarCoordinates(t, newRho);
    }

    public static IQ Pow(IQ value, double power) {
        return Pow(value, new IQ(power, 0));
    }

    private static IQ Scale(IQ value, double factor) {
        var realResult = factor * value.m_real;
        var imaginaryResuilt = factor * value.m_imaginary;
        return new IQ(realResult, imaginaryResuilt);
    }

    //
    // Explicit Conversions To IQ
    //

    public static explicit operator IQ(decimal value) {
        return new IQ((double)value, 0.0);
    }

    //
    // Implicit Conversions To IQ
    //

    public static implicit operator IQ(byte value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(char value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(double value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(Half value) {
        return new IQ((double)value, 0.0);
    }

    public static implicit operator IQ(short value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(int value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(long value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(nint value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(sbyte value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(float value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(ushort value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(uint value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(ulong value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(nuint value) {
        return new IQ(value, 0.0);
    }

    public static implicit operator IQ(System.Numerics.Complex value) {
        return new IQ(value.Real, value.Imaginary);
    }

    public static implicit operator System.Numerics.Complex(IQ value) {
        return new System.Numerics.Complex(value.I, value.Q);
    }
}