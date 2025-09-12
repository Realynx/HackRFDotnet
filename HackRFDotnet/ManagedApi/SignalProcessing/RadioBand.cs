namespace HackRFDotnet.ManagedApi.SignalProcessing {
    public class RadioBand {

        private readonly int _hz;
        public int Hz {
            get {
                return _hz;
            }
        }

        public float Mhz {
            get {
                return Hz / 1000000f;
            }
        }
        public float Khz {
            get {
                return Hz / 1000f;
            }
        }

        public RadioBand(int hz) {
            _hz = hz;
        }

        public static RadioBand FromHz(int hz) {
            return new RadioBand(hz);
        }

        public static RadioBand FromKHz(float khz) {
            return new RadioBand((int)(khz * 1000f));
        }

        public static RadioBand FromMHz(float mhz) {
            return new RadioBand((int)(mhz * 1000000f));
        }

        public static RadioBand operator +(RadioBand a, RadioBand b) {
            return new RadioBand(a.Hz + b.Hz);
        }
        public static RadioBand operator -(RadioBand a, RadioBand b) {
            return new RadioBand(a.Hz - b.Hz);
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
    }
}
