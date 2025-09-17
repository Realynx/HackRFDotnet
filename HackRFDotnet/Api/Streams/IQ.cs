/*
 * System.Numerics.Complex struct licensed under the MIT license
 * https://source.dot.net/#System.Runtime.Numerics/System/Numerics/Complex.cs,3e2d1e25b7131757
 */

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HackRFDotnet.Api.Streams;
/// <summary>
/// This represents a 32bit complex number.
/// The real represents the InPhase Sin of real voltage measurement in time.
/// The imaginary represents the Quadrature of the real voltage measurement in time.
/// The relationship between the I and Q allow us to represent the signal in lower sample rate than it was captured.
/// </summary>
public struct IQ {
    public static readonly IQ Zero = new IQ(0.0f, 0.00f);
    public static readonly IQ One = new IQ(1.0f, 0.0f);
    public static readonly IQ ImaginaryOne = new IQ(0.0f, 1.0f);
    public static readonly IQ NaN = new IQ(float.NaN, float.NaN);
    public static readonly IQ Infinity = new IQ(float.PositiveInfinity, float.PositiveInfinity);

    // These are the only local variabels to the struct
    // layout MUST be [I,Q] for SIMD vector
    private float m_real = 0f;
    private float m_imaginary = 0f;

    public IQ(float real, float imaginary) {
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
    public float I { get { return m_real; } set { m_real = value; } }

    /// <summary>
    /// Imaginary
    /// </summary>
    public float Q { get { return m_imaginary; } set { m_imaginary = value; } }

    public float Magnitude { get { return Abs(this); } }
    public float Phase { get { return (float)MathF.Atan2(m_imaginary, m_real); } }

    public static IQ Negate(IQ value) {
        return -value;
    }

    public static IQ Add(IQ left, IQ right) {
        return left + right;
    }

    public static IQ Add(IQ left, float right) {
        return left + right;
    }

    public static IQ Add(float left, IQ right) {
        return left + right;
    }

    public static IQ Subtract(IQ left, IQ right) {
        return left - right;
    }

    public static IQ Subtract(IQ left, float right) {
        return left - right;
    }

    public static IQ Subtract(float left, IQ right) {
        return left - right;
    }

    public static IQ Multiply(IQ left, IQ right) {
        return left * right;
    }

    public static IQ Multiply(IQ left, float right) {
        return left * right;
    }

    public static IQ Multiply(float left, IQ right) {
        return left * right;
    }

    public static IQ Divide(IQ dividend, IQ divisor) {
        return dividend / divisor;
    }

    public static IQ Divide(IQ dividend, float divisor) {
        return dividend / divisor;
    }

    public static IQ Divide(float dividend, IQ divisor) {
        return dividend / divisor;
    }

    public static IQ operator -(IQ value) /* Unary negation of a complex number */ {
        return new IQ(-value.m_real, -value.m_imaginary);
    }

    public static IQ operator +(IQ left, IQ right) {
        return new IQ(left.m_real + right.m_real, left.m_imaginary + right.m_imaginary);
    }

    public static IQ operator +(IQ left, float right) {
        return new IQ(left.m_real + right, left.m_imaginary);
    }

    public static IQ operator +(float left, IQ right) {
        return new IQ(left + right.m_real, right.m_imaginary);
    }

    public static IQ operator -(IQ left, IQ right) {
        return new IQ(left.m_real - right.m_real, left.m_imaginary - right.m_imaginary);
    }

    public static IQ operator -(IQ left, float right) {
        return new IQ(left.m_real - right, left.m_imaginary);
    }

    public static IQ operator -(float left, IQ right) {
        return new IQ(left - right.m_real, -right.m_imaginary);
    }

    public static IQ operator *(IQ left, IQ right) {
        // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
        var result_realpart = left.m_real * right.m_real - left.m_imaginary * right.m_imaginary;
        var result_imaginarypart = left.m_imaginary * right.m_real + left.m_real * right.m_imaginary;
        return new IQ(result_realpart, result_imaginarypart);
    }

    public static IQ operator *(IQ left, float right) {
        if (!float.IsFinite(left.m_real)) {
            if (!float.IsFinite(left.m_imaginary)) {
                return new IQ(float.NaN, float.NaN);
            }

            return new IQ(left.m_real * right, float.NaN);
        }

        if (!float.IsFinite(left.m_imaginary)) {
            return new IQ(float.NaN, left.m_imaginary * right);
        }

        return new IQ(left.m_real * right, left.m_imaginary * right);
    }

    public static IQ operator *(float left, IQ right) {
        if (!float.IsFinite(right.m_real)) {
            if (!float.IsFinite(right.m_imaginary)) {
                return new IQ(float.NaN, float.NaN);
            }

            return new IQ(left * right.m_real, float.NaN);
        }

        if (!float.IsFinite(right.m_imaginary)) {
            return new IQ(float.NaN, left * right.m_imaginary);
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
        if (MathF.Abs(d) < MathF.Abs(c)) {
            var doc = d / c;
            return new IQ((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
        }
        else {
            var cod = c / d;
            return new IQ((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
        }
    }

    public static IQ operator /(IQ left, float right) {
        // IEEE prohibit optimizations which are value changing
        // so we make sure that behaviour for the simplified version exactly match
        // full version.
        if (right == 0) {
            return new IQ(float.NaN, float.NaN);
        }

        if (!float.IsFinite(left.m_real)) {
            if (!float.IsFinite(left.m_imaginary)) {
                return new IQ(float.NaN, float.NaN);
            }

            return new IQ(left.m_real / right, float.NaN);
        }

        if (!float.IsFinite(left.m_imaginary)) {
            return new IQ(float.NaN, left.m_imaginary / right);
        }

        // Here the actual optimized version of code.
        return new IQ(left.m_real / right, left.m_imaginary / right);
    }

    public static IQ operator /(float left, IQ right) {
        // Division : Smith's formula.
        var a = left;
        var c = right.m_real;
        var d = right.m_imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (MathF.Abs(d) < MathF.Abs(c)) {
            var doc = d / c;
            return new IQ(a / (c + d * doc), -a * doc / (c + d * doc));
        }
        else {
            var cod = c / d;
            return new IQ(a * cod / (d + c * cod), -a / (d + c * cod));
        }
    }

    public static float Abs(IQ value) {
        return float.Hypot(value.m_real, value.m_imaginary);
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
        var handler = new DefaultInterpolatedStringHandler(4, 2, provider, stackalloc char[512]);
        handler.AppendLiteral("<");
        handler.AppendFormatted(m_real, format);
        handler.AppendLiteral("; ");
        handler.AppendFormatted(m_imaginary, format);
        handler.AppendLiteral(">");
        return handler.ToStringAndClear();
    }

    public static IQ Sin(IQ value) {
        (var sin, var cos) = MathF.SinCos(value.m_real);
        return new IQ((float)(sin * MathF.Cosh(value.m_imaginary)), (float)(cos * MathF.Sinh(value.m_imaginary)));
    }

    public static IQ Sinh(IQ value) {
        var sin = Sin(new IQ(-value.m_imaginary, value.m_real));
        return new IQ(sin.m_imaginary, -sin.m_real);
    }

    public static IQ Cos(IQ value) {
        (var sin, var cos) = MathF.SinCos(value.m_real);
        return new IQ((float)(cos * MathF.Cosh(value.m_imaginary)), (float)(-sin * MathF.Sinh(value.m_imaginary)));
    }

    public static IQ Cosh(IQ value) {
        return Cos(new IQ(-value.m_imaginary, value.m_real));
    }

    public static IQ Tan(IQ value) {
        var x2 = 2.0f * value.m_real;
        var y2 = 2.0f * value.m_imaginary;
        (var sin, var cos) = MathF.SinCos(x2);
        var cosh = MathF.Cosh(y2);
        if (MathF.Abs(value.m_imaginary) <= 4.0f) {
            var D = cos + cosh;
            return new IQ((float)sin / (float)D, (float)MathF.Sinh(y2) / (float)D);
        }
        else {
            var D = 1.0f + cos / cosh;
            return new IQ((float)sin / (float)cosh / (float)D, (float)MathF.Tanh(y2) / (float)D);
        }
    }


    //
    // Explicit Conversions To IQ
    //

    public static explicit operator IQ(decimal value) {
        return new IQ((float)value, 0.0f);
    }

    //
    // Implicit Conversions To IQ
    //

    public static implicit operator IQ(byte value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(char value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(float value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(Half value) {
        return new IQ((float)value, 0.0f);
    }

    public static implicit operator IQ(short value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(int value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(long value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(nint value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(sbyte value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(ushort value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(uint value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(ulong value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(nuint value) {
        return new IQ(value, 0.0f);
    }

    public static implicit operator IQ(System.Numerics.Complex value) {
        return new IQ((float)value.Real, (float)value.Imaginary);
    }

    public static implicit operator System.Numerics.Complex(IQ value) {
        return new System.Numerics.Complex(value.I, value.Q);
    }
}