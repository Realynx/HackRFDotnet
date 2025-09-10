using HackRFDotnet.Enums;
using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.NativeApi;
using HackRFDotnet.Structs;

namespace HackRFDotnet.ManagedApi {
    public unsafe class RfDevice : IDisposable {
        public RadioBand AntennaTuneOffset { get; set; } = RadioBand.FromHz(0);
        public RadioBand Frequency { get; set; } = RadioBand.FromHz(0);
        public RadioBand Bandwidth { get; set; } = RadioBand.FromHz(0);

        private HackRFDevice* _devicePtr;

        public bool IsConnected {
            get {
                return _devicePtr is not null;
            }
        }

        private HackRFSampleBlockCallback? _rxCallback;
        private Action<HackrfTransfer>? _userCallback;

        internal RfDevice() {
            ConnectToFirstDevice();
        }

        private bool ConnectToFirstDevice() {
            HackRFDevice* localDevice = null;
            HackRfNativeFunctions.hackrf_open(&localDevice);

            if (localDevice is null) {
                return false;
            }


            _devicePtr = localDevice;
            return true;
        }

        public bool SetFrequency(RadioBand radioFrequency) {
            return SetFrequency(radioFrequency, RadioBand.FromMHz(10));
        }

        public bool SetFrequency(RadioBand radioFrequency, RadioBand bandwidth) {
            Frequency = radioFrequency;
            Bandwidth = bandwidth;

            var tuningOffset = radioFrequency + AntennaTuneOffset;


            var baseBandFilter = HackRfNativeFunctions.hackrf_compute_baseband_filter_bw((uint)tuningOffset.Hz);
            var setFilter = HackRfNativeFunctions.hackrf_set_baseband_filter_bandwidth(_devicePtr, baseBandFilter) != 0;

            return HackRfNativeFunctions.hackrf_set_freq(_devicePtr, (uint)tuningOffset.Hz) == 0 && setFilter;
        }

        public bool SetSampleRate(double sampleRate) {
            return HackRfNativeFunctions.hackrf_set_sample_rate(_devicePtr, sampleRate) != 0;
        }

        public void OnSample(Action<HackrfTransfer> blockCallback) {
            _userCallback = blockCallback;
        }

        public bool StartRx() {
            _rxCallback = HandleTransferSample;
            HackRfNativeFunctions.hackrf_set_leds(_devicePtr, (byte)LedState.RxLight);

            HackRfNativeFunctions.hackrf_set_lna_gain(_devicePtr, 32);
            HackRfNativeFunctions.hackrf_set_vga_gain(_devicePtr, 24);
            HackRfNativeFunctions.hackrf_set_amp_enable(_devicePtr, 0);

            var result = HackRfNativeFunctions.hackrf_start_rx(
                _devicePtr,
                _rxCallback,
                null
            );

            return result == 0;
        }

        public bool StopRx() {
            return HackRfNativeFunctions.hackrf_stop_rx(_devicePtr) != 0;
        }

        private int HandleTransferSample(HackrfTransfer* transferStruct) {
            if (transferStruct == null) {
                return -1;
            }
            // Invoke user callback

            _userCallback?.Invoke(*transferStruct);
            return 0;
        }

        public void Dispose() {
            HackRfNativeFunctions.hackrf_close(_devicePtr);
            HackRfNativeFunctions.hackrf_exit();
        }
    }
}
