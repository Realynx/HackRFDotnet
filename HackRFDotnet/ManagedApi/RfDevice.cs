using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Types;
using HackRFDotnet.NativeApi.Enums.Peripherals;
using HackRFDotnet.NativeApi.Lib;
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
            RfDeviceStream.Open(20_000_000);
        }

        public void StartRecordingToFile(string fileName) {
            _recordingFile = new RfFileStream(fileName);
            _recordingFile.Open(0);
        }

        public void StopRecording() {
            _recordingFile?.Close();
        }

        public void Dispose() {
            HackRfNativeLib.Devices.CloseDevice(_devicePtr);
        }

        public void SetAmplifications(uint lna, uint vga, bool internalAmp) {
            HackRfNativeLib.DeviceStreaming.SetLnaGain(_devicePtr, lna);
            HackRfNativeLib.DeviceStreaming.SetVgaGain(_devicePtr, vga);
            HackRfNativeLib.DeviceStreaming.EnableAmp(_devicePtr, (byte)(internalAmp ? 1 : 0));
        }

        public bool SetFrequency(RadioBand radioFrequency) {
            return SetFrequency(radioFrequency, RadioBand.FromMHz(10));
        }

        public bool SetFrequency(RadioBand radioFrequency, RadioBand bandwidth) {
            Frequency = radioFrequency;
            Bandwidth = bandwidth;

            var tuningOffset = radioFrequency;


            var baseBandFilter = HackRfNativeLib.DeviceStreaming.ComputeBasebandFilterBandWith((uint)tuningOffset.Hz);
            var setFilter = HackRfNativeLib.DeviceStreaming.SetBasebandFilterBandith(_devicePtr, baseBandFilter) != 0;

            return HackRfNativeLib.DeviceStreaming.SetFrequency(_devicePtr, (uint)tuningOffset.Hz) == 0 && setFilter;
        }

        public bool SetSampleRate(double sampleRate) {
            return HackRfNativeLib.DeviceStreaming.SetSampleRate(_devicePtr, sampleRate) != 0;
        }

        public bool StartRx() {
            HackRfNativeLib.Devices.SetDeviceLeds(_devicePtr, (byte)LedState.RxLight);

            var result = HackRfNativeLib.DeviceStreaming.StartRx(
                _devicePtr,
                _rxCallback,
                null
            );

            return result == 0;
        }

        public bool StopRx() {
            return HackRfNativeLib.DeviceStreaming.StopRx(_devicePtr) != 0;
        }

        private int HandleTransferChunk(HackrfTransfer* transferStruct) {
            if (transferStruct == null) {
                return -1;
            }

            RfDeviceStream.BufferTransferChunk(*transferStruct);

            // HackRF Expects 0 = success.
            return 0;
        }
    }
}
