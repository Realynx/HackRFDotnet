namespace HackRFDotnet.Api;
/// <summary>
/// Number of oscillations per second.
/// </summary>
public readonly record struct Hertz {
    /// <summary>
    /// Number of oscillations per second.
    /// </summary>
    public readonly long Hz;

    /// <summary>
    /// Number of oscillations per second divided by 1,000,000
    /// </summary>
    public double Mhz {
        get {
            return Hz / 1_000_000d;
        }
    }

    /// <summary>
    /// Number of oscillations per second divided by 1,000
    /// </summary>
    public double Khz {
        get {
            return Hz / 1_000d;
        }
    }

    public Hertz(long hz) {
        Hz = hz;
    }

    public static Hertz FromHz(long hz) {
        return new Hertz(hz);
    }

    public static Hertz FromKHz(double khz) {
        return new Hertz((long)(khz * 1_000d));
    }

    public static Hertz FromMHz(double mhz) {
        return new Hertz((long)(mhz * 1_000_000d));
    }

    public static Hertz FromGHz(double ghz) {
        return new Hertz((long)(ghz * 1_000_000_000d));
    }

    public static bool operator >(Hertz a, Hertz b) {
        return a.Hz > b.Hz;
    }

    public static bool operator <(Hertz a, Hertz b) {
        return a.Hz < b.Hz;
    }

    public static bool operator >=(Hertz a, Hertz b) {
        return a.Hz >= b.Hz;
    }

    public static bool operator <=(Hertz a, Hertz b) {
        return a.Hz <= b.Hz;
    }

    public static Hertz operator %(Hertz a, Hertz b) {
        return new Hertz(a.Hz % b.Hz);
    }

    public static Hertz operator -(Hertz a) {
        return new Hertz(-a.Hz);
    }

    public static Hertz operator /(Hertz a, int b) {
        return new Hertz(a.Hz / b);
    }

    public static Hertz operator -(Hertz a, Hertz b) {
        return new Hertz(a.Hz - b.Hz);
    }

    public static Hertz operator +(Hertz a, Hertz b) {
        return new Hertz(a.Hz + b.Hz);
    }

    public static Hertz operator *(Hertz a, int b) {
        return new Hertz(a.Hz * b);
    }

    public static Hertz operator *(Hertz a, double b) {
        return new Hertz((long)(a.Hz * b));
    }
}
