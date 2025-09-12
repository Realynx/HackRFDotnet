using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public class AnaloguePlayer : IDisposable {
        private readonly SignalStream _sampleDeModulator;
        private WaveOutEvent _waveOut;

        public AnaloguePlayer(SignalStream signalStream) {
            _sampleDeModulator = signalStream;
        }

        public void Dispose() {
            _waveOut.Dispose();
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate) {
            _waveOut = new WaveOutEvent {
                Volume = 0.005f,
            };

            _sampleDeModulator.SetBand(centerOffset, bandwith);

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
            resampler.ToStereo();

            var waveProvider = resampler.ToWaveProvider();

            _waveOut.Init(waveProvider);
            _waveOut.Play();
        }
    }
}