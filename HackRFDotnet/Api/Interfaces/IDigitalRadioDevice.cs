using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.Api.Interfaces;
public interface IDigitalRadioDevice {
    Bandwidth Bandwidth { get; set; }
    SampleRate DeviceSamplingRate { get; set; }
    IIQStream? DeviceStream { get; }
    Frequency Frequency { get; set; }
    bool IsConnected { get; }

    void Dispose();
    void SetAmplifications(uint lna, uint vga, bool internalAmp);
    bool SetFrequency(Frequency radioFrequency, Bandwidth bandwidth);
    void SetSampleRate(SampleRate sampleRate);
    bool StartRx(HackRFSampleBlockCallback rxCallback);
    bool StopRx();
}