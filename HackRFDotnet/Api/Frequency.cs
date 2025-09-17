namespace HackRFDotnet.Api {
    /// <summary>
    /// Frequency of a signal.
    /// </summary>
    public class Frequency : Hertz {
        public Frequency(long hz) : base(hz) {
        }

        public Frequency(Hertz hz) : base(hz.Hz) {
        }

        public static new Frequency FromHz(long hz) {
            return new Frequency(hz);
        }

        public static new Frequency FromKHz(double khz) {
            return new Frequency((long)(khz * 1_000f));
        }

        public static new Frequency FromMHz(double mhz) {
            return new Frequency((long)(mhz * 1_000_000f));
        }

        public static new Frequency FromGHz(double ghz) {
            return new Frequency((long)(ghz * 1_000_000_000f));
        }

        public static bool operator >(Frequency a, Frequency b) {
            return a.Hz > b.Hz;
        }

        public static bool operator <(Frequency a, Frequency b) {
            return a.Hz < b.Hz;
        }

        public static bool operator >=(Frequency a, Frequency b) {
            return a.Hz >= b.Hz;
        }

        public static bool operator <=(Frequency a, Frequency b) {
            return a.Hz <= b.Hz;
        }

        public static bool operator ==(Frequency a, Frequency b) {
            return a.Hz == b.Hz;
        }

        public static bool operator !=(Frequency a, Frequency b) {
            return a.Hz != b.Hz;
        }

        public static Frequency operator %(Frequency a, Frequency b) {
            return new Frequency(a.Hz % b.Hz);
        }

        public static Frequency operator -(Frequency a) {
            return new Frequency(-a.Hz);
        }

        public static Frequency operator /(Frequency a, int b) {
            return new Frequency(a.Hz / b);
        }

        public static Frequency operator -(Frequency a, Frequency b) {
            return new Frequency(a.Hz - b.Hz);
        }

        public static Frequency operator +(Frequency a, Frequency b) {
            return new Frequency(a.Hz + b.Hz);
        }
    }
}
