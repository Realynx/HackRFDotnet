namespace HackRFDotnet.Api {
    public class RadioBand : Hertz {
        public RadioBand(long hz) : base(hz) {
        }

        public RadioBand(Hertz hz) : base(hz.Hz) {
        }

        public static new RadioBand FromHz(long hz) {
            return new RadioBand(hz);
        }

        public static new RadioBand FromKHz(double khz) {
            return new RadioBand((long)(khz * 1_000f));
        }

        public static new RadioBand FromMHz(double mhz) {
            return new RadioBand((long)(mhz * 1_000_000f));
        }

        public static new RadioBand FromGHz(double ghz) {
            return new RadioBand((long)(ghz * 1_000_000_000f));
        }

        public static bool operator >(RadioBand a, RadioBand b) {
            return a.Hz > b.Hz;
        }

        public static bool operator <(RadioBand a, RadioBand b) {
            return a.Hz < b.Hz;
        }

        public static bool operator >=(RadioBand a, RadioBand b) {
            return a.Hz >= b.Hz;
        }

        public static bool operator <=(RadioBand a, RadioBand b) {
            return a.Hz <= b.Hz;
        }

        public static bool operator ==(RadioBand a, RadioBand b) {
            return a.Hz == b.Hz;
        }

        public static bool operator !=(RadioBand a, RadioBand b) {
            return a.Hz != b.Hz;
        }

        public static RadioBand operator %(RadioBand a, RadioBand b) {
            return new RadioBand(a.Hz % b.Hz);
        }

        public static RadioBand operator -(RadioBand a) {
            return new RadioBand(-a.Hz);
        }

        public static RadioBand operator /(RadioBand a, int b) {
            return new RadioBand(a.Hz / b);
        }

        public static RadioBand operator -(RadioBand a, RadioBand b) {
            return new RadioBand(a.Hz - b.Hz);
        }

        public static RadioBand operator +(RadioBand a, RadioBand b) {
            return new RadioBand(a.Hz + b.Hz);
        }
    }
}
