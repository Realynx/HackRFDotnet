using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.NativeApi;
using HackRFDotnet.NativeApi.Enums;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.ManagedApi {
    public unsafe class RfDevice : IDisposable {
        public RadioBand Frequency { get; set; } = RadioBand.FromHz(0);
        public RadioBand Bandwidth { get; set; } = RadioBand.FromHz(0);

        public RfDeviceStream RfDeviceStream { get; set; }

        private readonly HackRFDevice* _devicePtr;
        private readonly HackRFSampleBlockCallback _rxCallback;

        private RfFileStream? _recordingFile = null;

        public bool IsConnected {
            get {
                return _devicePtr is not null;
            }
        }

        internal RfDevice(HackRFDevice* devicePtr) {
            _rxCallback = HandleTransferChunk;
            _devicePtr = devicePtr;

            RfDeviceStream = new RfDeviceStream(this);
            RfDeviceStream.Open(5_000_000);
        }

        public void StartRecordingToFile(string fileName) {
            _recordingFile = new RfFileStream(fileName);
            _recordingFile.Open(0);
        }

        public void StopRecording() {
            _recordingFile?.Close();
        }

        public void Dispose() {
            HackRfNativeFunctions.hackrf_close(_devicePtr);
        }

        public void SetAmplifications(uint lna, uint vga, bool internalAmp) {
            HackRfNativeFunctions.hackrf_set_lna_gain(_devicePtr, lna);
            HackRfNativeFunctions.hackrf_set_vga_gain(_devicePtr, vga);
            HackRfNativeFunctions.hackrf_set_amp_enable(_devicePtr, (byte)(internalAmp ? 1 : 0));
        }

        public bool SetFrequency(RadioBand radioFrequency) {
            return SetFrequency(radioFrequency, RadioBand.FromMHz(10));
        }

        public bool SetFrequency(RadioBand radioFrequency, RadioBand bandwidth) {
            Frequency = radioFrequency;
            Bandwidth = bandwidth;

            var tuningOffset = radioFrequency;


            var baseBandFilter = HackRfNativeFunctions.hackrf_compute_baseband_filter_bw((uint)tuningOffset.Hz);
            var setFilter = HackRfNativeFunctions.hackrf_set_baseband_filter_bandwidth(_devicePtr, baseBandFilter) != 0;

            return HackRfNativeFunctions.hackrf_set_freq(_devicePtr, (uint)tuningOffset.Hz) == 0 && setFilter;
        }

        public bool SetSampleRate(double sampleRate) {
            return HackRfNativeFunctions.hackrf_set_sample_rate(_devicePtr, sampleRate) != 0;
        }

        public bool StartRx() {
            HackRfNativeFunctions.hackrf_set_leds(_devicePtr, (byte)LedState.RxLight);

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

        private int HandleTransferChunk(HackrfTransfer* transferStruct) {
            if (transferStruct == null) {
                return -1;
            }

            RfDeviceStream.BufferTransferChunk(*transferStruct);

            if (_recordingFile is not null) {
                var chunkBuffer = new Span<byte>(transferStruct->buffer, transferStruct->buffer_length);
                _recordingFile.WriteBuffer(chunkBuffer);
            }

            return 0;
        }
    }
}
