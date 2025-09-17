using HackRFDotnet.Api.Streams.SignalStreams.Analogue;
using HackRFDotnet.ManagedApi.Streams.SignalProcessing;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.Api.Services {
    public class AnaloguePlayer : IDisposable {
        private readonly WaveSignalStream _sampleDeModulator;
        private WaveOutEvent _waveOut;

        public AnaloguePlayer(WaveSignalStream signalStream) {
            _sampleDeModulator = signalStream;
        }

        public void Dispose() {
            _waveOut.Dispose();
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, Bandwidth bandwidth, int audioRate) {
            _waveOut = new WaveOutEvent {
                Volume = 0.1f,
            };

            _sampleDeModulator.SetBand(centerOffset, bandwidth);

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
            var waveProvider = resampler.ToWaveProvider();

            _waveOut.Init(waveProvider);
            _waveOut.Play();
        }
    }
}