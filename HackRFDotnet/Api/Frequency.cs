namespace HackRFDotnet.Api {
    /// <summary>
    /// Frequency of a signal.
    /// </summary>
    public readonly record struct Frequency {
        private readonly Hertz _hertz;

        public long Hz {
            get {
                return _hertz.Hz;
            }
        }

        public Frequency(long hz) : this(new Hertz(hz)) { }

        public Frequency(Hertz hertz) {
            _hertz = hertz;
        }

        public static Frequency FromHz(long hz) {
            return new Frequency(hz);
        }

        public static Frequency FromKHz(double khz) {
            return new Frequency(Hertz.FromKHz(khz));
        }

        public static Frequency FromMHz(double mhz) {
            return new Frequency(Hertz.FromMHz(mhz));
        }

        public static Frequency FromGHz(double ghz) {
            return new Frequency(Hertz.FromGHz(ghz));
        }

        public static bool operator >(Frequency a, Frequency b) {
            return a._hertz > b._hertz;
        }

        public static bool operator <(Frequency a, Frequency b) {
            return a._hertz < b._hertz;
        }

        public static bool operator >=(Frequency a, Frequency b) {
            return a._hertz >= b._hertz;
        }

        public static bool operator <=(Frequency a, Frequency b) {
            return a._hertz <= b._hertz;
        }

        public static Frequency operator %(Frequency a, Frequency b) {
            return new Frequency(a._hertz % b._hertz);
        }

        public static Frequency operator -(Frequency a) {
            return new Frequency(-a._hertz);
        }

        public static Frequency operator /(Frequency a, int b) {
            return new Frequency(a._hertz / b);
        }

        public static Frequency operator -(Frequency a, Frequency b) {
            return new Frequency(a._hertz - b._hertz);
        }

        public static Frequency operator +(Frequency a, Frequency b) {
            return new Frequency(a._hertz + b._hertz);
        }

        public static Frequency operator *(Frequency a, int b) {
            return new Frequency(a._hertz * b);
        }

        public static Frequency operator *(Frequency a, double b) {
            return new Frequency(a._hertz * b);
        }

        public static implicit operator Bandwidth(Frequency f) {
            return new Bandwidth(f._hertz);
        }
    }
}