using HackRFDotnet.Api.Streams.Exceptions;
using HackRFDotnet.NativeApi.Enums.Peripherals;
using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.Api {
    public unsafe class DigitalRadioDevice : IDisposable {
        public Frequency Frequency { get; set; } = Frequency.FromHz(0);
        public Bandwidth Bandwidth { get; set; } = Bandwidth.FromHz(0);
        public SampleRate DeviceSamplingRate { get; set; } = SampleRate.FromMsps(10);

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

        public bool SetFrequency(Frequency radioFrequency) {
            return SetFrequency(radioFrequency, Bandwidth.FromMHz(200));
        }

        public bool SetFrequency(Frequency radioFrequency, Bandwidth bandwidth) {
            Frequency = radioFrequency;
            Bandwidth = bandwidth;

            // This shifts the signal to offset the 0 dc spike away from the signal we want.
            radioFrequency -= new Frequency(DeviceSamplingRate.NyquistFrequencyBandwidth / 2);
            return HackRfNativeLib.DeviceStreaming.SetFrequency(DevicePtr, (uint)radioFrequency.Hz) == 0;
        }

        public void SetSampleRate(SampleRate sampleRate) {
            DeviceSamplingRate = sampleRate;
            HackRfNativeLib.DeviceStreaming.SetSampleRate(DevicePtr, sampleRate.Sps);

            var baseBandFilter = HackRfNativeLib.DeviceStreaming.ComputeBasebandFilterBandWidth((uint)sampleRate.NyquistFrequencyBandwidth.Hz);
            var setFilter = HackRfNativeLib.DeviceStreaming.SetBasebandFilterBandwidth(DevicePtr, baseBandFilter) != 0;
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
