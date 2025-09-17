namespace HackRFDotnet.Api;
public class Bandwidth : Hertz {
    public SampleRate NyquistSampleRate {
        get {
            return new SampleRate(Hz * 2);
        }
    }
    public Bandwidth(long hz) : base(hz) {
    }

    public Bandwidth(Hertz hz) : base(hz.Hz) {
    }

    public static new Bandwidth FromHz(long hz) {
        return new Bandwidth(hz);
    }

    public static new Bandwidth FromKHz(double khz) {
        return new Bandwidth((long)(khz * 1_000f));
    }

    public static new Bandwidth FromMHz(double mhz) {
        return new Bandwidth((long)(mhz * 1_000_000f));
    }

    public static new Bandwidth FromGHz(double ghz) {
        return new Bandwidth((long)(ghz * 1_000_000_000f));
    }

    public static bool operator >(Bandwidth a, Bandwidth b) {
        return a.Hz > b.Hz;
    }

    public static bool operator <(Bandwidth a, Bandwidth b) {
        return a.Hz < b.Hz;
    }

    public static bool operator >=(Bandwidth a, Bandwidth b) {
        return a.Hz >= b.Hz;
    }

    public static bool operator <=(Bandwidth a, Bandwidth b) {
        return a.Hz <= b.Hz;
    }

    public static bool operator ==(Bandwidth a, Bandwidth b) {
        return a.Hz == b.Hz;
    }

    public static bool operator !=(Bandwidth a, Bandwidth b) {
        return a.Hz != b.Hz;
    }

    public static Bandwidth operator %(Bandwidth a, Bandwidth b) {
        return new Bandwidth(a.Hz % b.Hz);
    }

    public static Bandwidth operator -(Bandwidth a) {
        return new Bandwidth(-a.Hz);
    }

    public static Bandwidth operator /(Bandwidth a, int b) {
        return new Bandwidth(a.Hz / b);
    }

    public static Bandwidth operator -(Bandwidth a, Bandwidth b) {
        return new Bandwidth(a.Hz - b.Hz);
    }

    public static Bandwidth operator +(Bandwidth a, Bandwidth b) {
        return new Bandwidth(a.Hz + b.Hz);
    }
}
