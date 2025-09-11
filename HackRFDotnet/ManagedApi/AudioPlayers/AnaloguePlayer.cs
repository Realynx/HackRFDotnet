using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public class AnaloguePlayer {
        private readonly SignalStream _sampleDeModulator;
        private SampleToWaveProvider16 _sampleToWaveProvider16;
        private BufferedWaveProvider _waveProvider;
        private WaveOutEvent _waveOut;
        private Thread _playbackThread;

        public AnaloguePlayer(SignalStream signalStream) {
            _sampleDeModulator = signalStream;
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            _waveProvider = new BufferedWaveProvider(new WaveFormat(audioRate, 16, 1)) {
                DiscardOnBufferOverflow = true
            };

            _waveOut = new WaveOutEvent { Volume = 0.5f };
            _waveOut.Init(_waveProvider);
            _waveOut.Play();

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
            _sampleToWaveProvider16 = new SampleToWaveProvider16(resampler);

            _playbackThread = new Thread(() => StreamIqFrames(centerOffset, bandwith));
            _playbackThread.Start();
        }

        private void StreamIqFrames(RadioBand centerOffset, RadioBand bandwith) {
            _sampleDeModulator.SetBand(centerOffset, bandwith);
            var waveProviderBuffer = new byte[10_000];

            int read;
            while (true) {
                while ((read = _sampleToWaveProvider16.Read(waveProviderBuffer, 0, waveProviderBuffer.Length)) > 0) {
                    _waveProvider.AddSamples(waveProviderBuffer, 0, read);
                }
            }
        }
    }
}
