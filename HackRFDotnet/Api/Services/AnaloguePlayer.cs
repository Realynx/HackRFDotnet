using HackRFDotnet.Api.Streams.SignalStreams.Analogue;

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

        public virtual void PlayStreamAsync(Frequency centerOffset, Bandwidth bandwidth, SampleRate audioRate) {
            _waveOut = new WaveOutEvent {
                Volume = 0.5f,
            };

            _sampleDeModulator.SetBand(centerOffset, bandwidth);

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, (int)audioRate.Sps);
            var waveProvider = resampler.ToWaveProvider();
            _waveOut.Init(waveProvider);
            _waveOut.Play();
        }
    }
}