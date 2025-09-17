namespace HackRFDotnet.Api;
public class SampleRate : Hertz {
    /// <summary>
    /// https://en.wikipedia.org/wiki/Nyquist_frequency
    /// The largest bandwidth this sample rate can represent.
    /// </summary>
    public Bandwidth NyquistFrequencyBandwidth {
        get {
            return new Bandwidth(Hz / 2);
        }
    }

    public SampleRate(long sps) : base(sps) {
    }

    public SampleRate(Hertz hz) : base(hz.Hz) {
    }

    public long Sps {
        get {
            return Hz;
        }
    }

    public double Msps {
        get {
            return Hz / 1000000f;
        }
    }

    public double Ksps {
        get {
            return Hz / 1000f;
        }
    }

    public static SampleRate FromKsps(double khz) {
        return new SampleRate((long)(khz * 1_000f));
    }

    public static SampleRate FromMsps(double mhz) {
        return new SampleRate((long)(mhz * 1_000_000f));
    }

    public static SampleRate FromGsps(double ghz) {
        return new SampleRate((long)(ghz * 1_000_000_000f));
    }

    public static bool operator >(SampleRate a, SampleRate b) {
        return a.Hz > b.Hz;
    }

    public static bool operator <(SampleRate a, SampleRate b) {
        return a.Hz < b.Hz;
    }

    public static bool operator >=(SampleRate a, SampleRate b) {
        return a.Hz >= b.Hz;
    }

    public static bool operator <=(SampleRate a, SampleRate b) {
        return a.Hz <= b.Hz;
    }

    public static bool operator ==(SampleRate a, SampleRate b) {
        return a.Hz == b.Hz;
    }

    public static bool operator !=(SampleRate a, SampleRate b) {
        return a.Hz != b.Hz;
    }

    public static SampleRate operator %(SampleRate a, SampleRate b) {
        return new SampleRate(a.Hz % b.Hz);
    }

    public static SampleRate operator -(SampleRate a) {
        return new SampleRate(-a.Hz);
    }

    public static SampleRate operator /(SampleRate a, int b) {
        return new SampleRate(a.Hz / b);
    }

    public static SampleRate operator -(SampleRate a, SampleRate b) {
        return new SampleRate(a.Hz - b.Hz);
    }

    public static SampleRate operator +(SampleRate a, SampleRate b) {
        return new SampleRate(a.Hz + b.Hz);
    }
}
