using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public class AnaloguePlayer {
        private readonly SignalStream _sampleDeModulator;
        private IWaveProvider _sampleToWaveProvider16;

        private WaveOutEvent _waveOut;

        public AnaloguePlayer(SignalStream signalStream) {
            _sampleDeModulator = signalStream;
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate) {
            _waveOut = new WaveOutEvent {
                Volume = 0.5f,
            };

            _sampleDeModulator.SetBand(centerOffset, bandwith);

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
            _sampleToWaveProvider16 = resampler.ToWaveProvider16();

            _waveOut.Init(_sampleToWaveProvider16);
            _waveOut.Play();
        }
    }
}