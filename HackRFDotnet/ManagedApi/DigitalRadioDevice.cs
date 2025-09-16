using HackRFDotnet.ManagedApi.Streams.Exceptions;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.NativeApi.Enums.Peripherals;
using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.ManagedApi {
    public unsafe class DigitalRadioDevice : IDisposable {
        public RadioBand Frequency { get; set; } = RadioBand.FromHz(0);
        public RadioBand Bandwidth { get; set; } = RadioBand.FromHz(0);

        public readonly HackRFDevice* DevicePtr;
        private HackRFSampleBlockCallback _rxCallback;
        public bool IsConnected {
            get {
                return DevicePtr is not null;
            }
        }

        internal DigitalRadioDevice(HackRFDevice* devicePtr) {
            DevicePtr = devicePtr;
        }

        public void SetAmplifications(uint lna, uint vga, bool internalAmp) {
            HackRfNativeLib.DeviceStreaming.SetLnaGain(DevicePtr, lna);
            HackRfNativeLib.DeviceStreaming.SetVgaGain(DevicePtr, vga);
            HackRfNativeLib.DeviceStreaming.EnableAmp(DevicePtr, (byte)(internalAmp ? 1 : 0));
        }

        public bool SetFrequency(RadioBand radioFrequency) {
            return SetFrequency(radioFrequency, RadioBand.FromMHz(200));
        }

        public bool SetFrequency(RadioBand radioFrequency, RadioBand bandwidth) {
            Frequency = radioFrequency;
            Bandwidth = bandwidth;

            radioFrequency -= RadioBand.FromKHz(1);
            //var baseBandFilter = HackRfNativeLib.DeviceStreaming.ComputeBasebandFilterBandWith((uint)radioFrequency.Hz);
            //var setFilter = HackRfNativeLib.DeviceStreaming.SetBasebandFilterBandith(DevicePtr, baseBandFilter) != 0;

            return HackRfNativeLib.DeviceStreaming.SetFrequency(DevicePtr, (uint)radioFrequency.Hz) == 0;
        }

        public bool StartRx(HackRFSampleBlockCallback rxCallback) {
            _rxCallback = rxCallback;
            if (_rxCallback is null) {
                throw new NullCallbackException($"You cannot start Rx without a transfer callback '{nameof(_rxCallback)}'");
            }

            HackRfNativeLib.Devices.SetDeviceLeds(DevicePtr, (byte)LedState.RxLight);
            var result = HackRfNativeLib.DeviceStreaming.StartRx(
                DevicePtr,
                _rxCallback,
                null
            );

            return result == 0;
        }

        public bool StopRx() {
            HackRfNativeLib.Devices.SetDeviceLeds(DevicePtr, (byte)LedState.UsbLight);
            return HackRfNativeLib.DeviceStreaming.StopRx(DevicePtr) != 0;
        }

        public void Dispose() {
            HackRfNativeLib.Devices.CloseDevice(DevicePtr);
        }
    }
}
