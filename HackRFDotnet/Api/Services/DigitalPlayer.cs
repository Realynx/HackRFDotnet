//using HackRFDotnet.Api.Streams.SignalStreams.Digital;

//using NAudio.Wave;
//using NAudio.Wave.SampleProviders;

//namespace HackRFDotnet.Api.Services;
//public class DigitalPlayer : IDisposable {
//    private readonly HdRadioSignalStream _sampleDeModulator;
//    private WaveOutEvent _waveOut;

//    public DigitalPlayer(HdRadioSignalStream sampleDeModulator) {
//        _sampleDeModulator = sampleDeModulator;
//    }

//    public void Dispose() {
//        _waveOut.Dispose();
//    }

//    public virtual void PlayStreamAsync(Frequency centerOffset, Bandwidth bandwidth, int audioRate) {
//        _waveOut = new WaveOutEvent {
//            Volume = 0.6f,
//        };

//        _sampleDeModulator.SetBand(centerOffset, bandwidth);

//        var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
//        var waveProvider = resampler.ToWaveProvider();

//        _waveOut.Init(waveProvider);
//        _waveOut.Play();
//    }
//}
