using HackRFDotnet.Api.Interfaces;
using HackRFDotnet.Api.Streams.Exceptions;
using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.NativeApi.Enums.Peripherals;
using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs;
using HackRFDotnet.NativeApi.Structs.Devices;

namespace HackRFDotnet.Api {
    /// <summary>
    /// Radio Device to receive IQ Samples with.
    /// </summary>
    public unsafe class DigitalRadioDevice : IDigitalRadioDevice, IDisposable {
        /// <summary>
        /// Current frequency tuned to.
        /// </summary>
        public Frequency Frequency { get; set; } = Frequency.FromHz(0);

        /// <summary>
        /// Current capture bandwidth.
        /// </summary>
        public Bandwidth Bandwidth { get; set; } = Bandwidth.FromHz(0);

        /// <summary>
        /// Current capture sample rate.
        /// </summary>
        public SampleRate DeviceSamplingRate { get; set; } = SampleRate.FromMsps(10);

        /// <summary>
        /// The immutable stream running on this device.
        /// </summary>
        public IIQStream? DeviceStream { get; internal set; } = null;

        /// <summary>
        /// Is the device connected to the usb host in the native library?
        /// </summary>
        public bool IsConnected {
            get {
                return DevicePtr is not null;
            }
        }

        public readonly HackRFDevice* DevicePtr;
        private HackRFSampleBlockCallback _rxCallback;

        internal DigitalRadioDevice(HackRFDevice* devicePtr) {
            DevicePtr = devicePtr;
        }

        /// <summary>
        /// Set the Lna, Vga, and Internal amp settings for the Rf Device.
        /// </summary>
        /// <param name="lna"></param>
        /// <param name="vga"></param>
        /// <param name="internalAmp"></param>
        public void SetAmplifications(uint lna, uint vga, bool internalAmp) {
            HackRfNativeLib.DeviceStreaming.SetLnaGain(DevicePtr, lna);
            HackRfNativeLib.DeviceStreaming.SetVgaGain(DevicePtr, vga);
            HackRfNativeLib.DeviceStreaming.EnableAmp(DevicePtr, (byte)(internalAmp ? 1 : 0));
        }

        /// <summary>
        /// Set the tuning frequency and bandwidth for the Rf Device.
        /// </summary>
        /// <param name="radioFrequency"></param>
        /// <param name="bandwidth"></param>
        /// <returns></returns>
        public bool SetFrequency(Frequency radioFrequency, Bandwidth bandwidth) {
            Frequency = radioFrequency;
            Bandwidth = bandwidth;

            // This shifts the signal to offset the 0 dc spike away from the signal we want.
            radioFrequency -= (Frequency)(DeviceSamplingRate.NyquistFrequencyBandwidth / 2);
            return HackRfNativeLib.DeviceStreaming.SetFrequency(DevicePtr, (uint)radioFrequency.Hz) == 0;
        }

        /// <summary>
        /// Set the sample rate for the radio device to capture data at.
        /// This will also set the baseband filter the smallest filter 
        /// that fits the sample rate's Nyquist frequency cutoff.
        /// </summary>
        /// <param name="sampleRate"></param>
        public void SetSampleRate(SampleRate sampleRate) {
            DeviceSamplingRate = sampleRate;
            HackRfNativeLib.DeviceStreaming.SetSampleRate(DevicePtr, sampleRate.Sps);

            var baseBandFilter = HackRfNativeLib.DeviceStreaming
                .ComputeBasebandFilterBandWidth((uint)sampleRate.NyquistFrequencyBandwidth.Hz);

            HackRfNativeLib.DeviceStreaming.SetBasebandFilterBandwidth(DevicePtr, baseBandFilter);
        }

        /// <summary>
        /// Start receiving data from the RfDevice.
        /// </summary>
        /// <param name="rxCallback"></param>
        /// <returns></returns>
        /// <exception cref="NullCallbackException"></exception>
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

        /// <summary>
        /// Stop receiving data from the Rf Device.
        /// </summary>
        /// <returns></returns>
        public bool StopRx() {
            HackRfNativeLib.Devices.SetDeviceLeds(DevicePtr, (byte)LedState.UsbLight);
            return HackRfNativeLib.DeviceStreaming.StopRx(DevicePtr) != 0;
        }

        /// <summary>
        /// Dispose the Rf Device from the library.
        /// </summary>
        public void Dispose() {
            HackRfNativeLib.Devices.CloseDevice(DevicePtr);
        }
    }
}
