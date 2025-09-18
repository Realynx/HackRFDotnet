namespace HackRFDotnet.Api;
public record struct SampleRate {
    private readonly Hertz _hertz;

    public long Hz {
        get {
            return _hertz.Hz;
        }
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Nyquist_frequency
    /// The largest bandwidth this sample rate can represent.
    /// </summary>
    public Bandwidth NyquistFrequencyBandwidth {
        get {
            return new Bandwidth(_hertz / 2);
        }
    }

    public SampleRate(long sps) : this(new Hertz(sps)) { }

    public SampleRate(Hertz hertz) {
        _hertz = hertz;
    }

    public long Sps {
        get {
            return _hertz.Hz;
        }
    }

    public double Msps {
        get {
            return _hertz.Mhz;
        }
    }

    public double Ksps {
        get {
            return _hertz.Khz;
        }
    }

    public static SampleRate FromKsps(double khz) {
        return new SampleRate(Hertz.FromKHz(khz));
    }

    public static SampleRate FromMsps(double mhz) {
        return new SampleRate(Hertz.FromMHz(mhz));
    }

    public static SampleRate FromGsps(double ghz) {
        return new SampleRate(Hertz.FromGHz(ghz));
    }

    public static bool operator >(SampleRate a, SampleRate b) {
        return a._hertz > b._hertz;
    }

    public static bool operator <(SampleRate a, SampleRate b) {
        return a._hertz < b._hertz;
    }

    public static bool operator >=(SampleRate a, SampleRate b) {
        return a._hertz >= b._hertz;
    }

    public static bool operator <=(SampleRate a, SampleRate b) {
        return a._hertz <= b._hertz;
    }

    public static SampleRate operator %(SampleRate a, SampleRate b) {
        return new SampleRate(a._hertz % b._hertz);
    }

    public static SampleRate operator -(SampleRate a) {
        return new SampleRate(-a._hertz);
    }

    public static SampleRate operator /(SampleRate a, int b) {
        return new SampleRate(a._hertz / b);
    }

    public static SampleRate operator -(SampleRate a, SampleRate b) {
        return new SampleRate(a._hertz - b._hertz);
    }

    public static SampleRate operator +(SampleRate a, SampleRate b) {
        return new SampleRate(a._hertz + b._hertz);
    }

    public static SampleRate operator *(SampleRate a, int b) {
        return new SampleRate(a._hertz * b);
    }

    public static SampleRate operator *(SampleRate a, double b) {
        return new SampleRate(a._hertz * b);
    }
}
