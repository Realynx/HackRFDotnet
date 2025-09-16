namespace HackRFDotnet.ManagedApi.Streams.SignalProcessing;
public class SampleRate {
    private readonly int _sps;

    public int Sps {
        get {
            return _sps;
        }
    }

    public float Msps {
        get {
            return _sps / 1000000f;
        }
    }

    public float Ksps {
        get {
            return _sps / 1000f;
        }
    }

    public RadioBand NyquistFrequencyRange {
        get {
            return new RadioBand(_sps / 2);
        }
    }

    public SampleRate(int rate) {
        _sps = rate;
    }

    public static SampleRate FromSps(int sps) {
        return new SampleRate(sps);
    }

    public static SampleRate FromKSps(float ksps) {
        return new SampleRate((int)(ksps * 1000f));
    }

    public static SampleRate FromMSps(float msps) {
        return new SampleRate((int)(msps * 1000000f));
    }


    public static SampleRate operator +(SampleRate a, SampleRate b) {
        return new SampleRate(a._sps + b._sps);
    }
    public static SampleRate operator -(SampleRate a, SampleRate b) {
        return new SampleRate(a._sps - b._sps);
    }

    public static bool operator >(SampleRate a, SampleRate b) {
        return a._sps > b._sps;
    }

    public static bool operator <(SampleRate a, SampleRate b) {
        return a._sps < b._sps;
    }

    public static bool operator >=(SampleRate a, SampleRate b) {
        return a._sps >= b._sps;
    }

    public static bool operator <=(SampleRate a, SampleRate b) {
        return a._sps <= b._sps;
    }

    public static bool operator ==(SampleRate a, SampleRate b) {
        return a._sps == b._sps;
    }

    public static bool operator !=(SampleRate a, SampleRate b) {
        return a._sps != b._sps;
    }

    public static SampleRate operator %(SampleRate a, SampleRate b) {
        return new SampleRate(a._sps % b._sps);
    }

    public static SampleRate operator -(SampleRate a) {
        return new SampleRate(-a._sps);
    }
}
