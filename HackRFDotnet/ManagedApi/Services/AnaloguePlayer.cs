using HackRFDotnet.ManagedApi.Streams.SignalProcessing;
using HackRFDotnet.ManagedApi.Streams.SignalStreams.Analogue;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.Services {
    public class AnaloguePlayer : IDisposable {
        private readonly WaveSignalStream _sampleDeModulator;
        private WaveOutEvent _waveOut;

        public AnaloguePlayer(WaveSignalStream signalStream) {
            _sampleDeModulator = signalStream;
        }

        public void Dispose() {
            _waveOut.Dispose();
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate) {
            _waveOut = new WaveOutEvent {
                Volume = 0.5f,
            };

            _sampleDeModulator.SetBand(centerOffset, bandwith);

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
            var waveProvider = resampler.ToWaveProvider();

            _waveOut.Init(waveProvider);
            _waveOut.Play();
        }
    }
}