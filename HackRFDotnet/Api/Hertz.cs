namespace HackRFDotnet.Api;
public class Hertz {
    public long Hz { get; set; }

    public double Mhz {
        get {
            return Hz / 1000000f;
        }
    }
    public double Khz {
        get {
            return Hz / 1000f;
        }
    }

    public Hertz(long hz) {
        Hz = hz;
    }

    public static Hertz FromHz(long hz) {
        return new Hertz(hz);
    }

    public static Hertz FromKHz(double khz) {
        return new Hertz((long)(khz * 1_000f));
    }

    public static Hertz FromMHz(double mhz) {
        return new Hertz((long)(mhz * 1_000_000f));
    }

    public static Hertz FromGHz(double ghz) {
        return new Hertz((long)(ghz * 1_000_000_000f));
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

    public static bool operator ==(Hertz a, Hertz b) {
        return a.Hz == b.Hz;
    }

    public static bool operator !=(Hertz a, Hertz b) {
        return a.Hz != b.Hz;
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

    public override bool Equals(object obj) {
        if (ReferenceEquals(this, obj)) {
            return true;
        }

        if (obj is null) {
            return false;
        }

        return obj is Hertz hertz && hertz.Hz == Hz;
    }
}
