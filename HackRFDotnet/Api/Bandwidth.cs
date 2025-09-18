namespace HackRFDotnet.Api;

/// <summary>
/// Bandwidth of a signal sample.
/// </summary>
public readonly record struct Bandwidth {
    private readonly Hertz _hertz;

    /// <summary>
    /// https://en.wikipedia.org/wiki/Nyquist_rate
    /// The smallest sample rate that can be used to represent the bandwidth.
    /// </summary>
    public SampleRate NyquistSampleRate {
        get {
            return new SampleRate(_hertz * 2);
        }
    }

    public Bandwidth(long hz) : this(new Hertz(hz)) { }

    public Bandwidth(Hertz hertz) {
        _hertz = hertz;
    }

    public long Hz {
        get {
            return _hertz.Hz;
        }
    }

    public static Bandwidth FromHz(Hertz hz) {
        return new Bandwidth(hz);
    }

    public static Bandwidth FromHz(long hz) {
        return new Bandwidth(hz);
    }

    public static Bandwidth FromKHz(double khz) {
        return new Bandwidth(Hertz.FromKHz(khz));
    }

    public static Bandwidth FromMHz(double mhz) {
        return new Bandwidth(Hertz.FromMHz(mhz));
    }

    public static Bandwidth FromGHz(double ghz) {
        return new Bandwidth(Hertz.FromGHz(ghz));
    }

    public static bool operator >(Bandwidth a, Bandwidth b) {
        return a._hertz > b._hertz;
    }

    public static bool operator <(Bandwidth a, Bandwidth b) {
        return a._hertz < b._hertz;
    }

    public static bool operator >=(Bandwidth a, Bandwidth b) {
        return a._hertz >= b._hertz;
    }

    public static bool operator <=(Bandwidth a, Bandwidth b) {
        return a._hertz <= b._hertz;
    }

    public static Bandwidth operator %(Bandwidth a, Bandwidth b) {
        return new Bandwidth(a._hertz % b._hertz);
    }

    public static Bandwidth operator -(Bandwidth a) {
        return new Bandwidth(-a._hertz);
    }

    public static Bandwidth operator /(Bandwidth a, int b) {
        return new Bandwidth(a._hertz / b);
    }

    public static Bandwidth operator /(Bandwidth a, double b) {
        return new Bandwidth(a._hertz / b);
    }

    public static Bandwidth operator -(Bandwidth a, Bandwidth b) {
        return new Bandwidth(a._hertz - b._hertz);
    }

    public static Bandwidth operator +(Bandwidth a, Bandwidth b) {
        return new Bandwidth(a._hertz + b._hertz);
    }

    public static Bandwidth operator *(Bandwidth a, int b) {
        return new Bandwidth(a._hertz * b);
    }

    public static Bandwidth operator *(Bandwidth a, double b) {
        return new Bandwidth(a._hertz * b);
    }

    public static implicit operator Frequency(Bandwidth b) {
        return new Frequency(b._hertz);
    }

    public static implicit operator Hertz(Bandwidth b) {
        return b._hertz;
    }
}