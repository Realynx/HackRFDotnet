using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public class AnaloguePlayer {
        private readonly SignalStream _sampleDeModulator;
        private IWaveProvider _sampleToWaveProvider16;
        private BufferedWaveProvider _waveProvider;
        private WaveOutEvent _waveOut;
        private Thread _playbackThread;
        private int _audioRate;
        public AnaloguePlayer(SignalStream signalStream) {
            _sampleDeModulator = signalStream;
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate) {
            _audioRate = audioRate;

            _waveProvider = new BufferedWaveProvider(new WaveFormat(audioRate, 16, 1)) {
                BufferDuration = TimeSpan.FromMilliseconds(50),
                DiscardOnBufferOverflow = true,
            };

            _waveOut = new WaveOutEvent {
                Volume = 0.5f,
            };

            _waveOut.Init(_waveProvider);
            _waveOut.Play();

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
            _sampleToWaveProvider16 = resampler.ToWaveProvider16();

            _playbackThread = new Thread(() => StreamAudioFrames(centerOffset, bandwith));
            _playbackThread.Start();
        }

        private void StreamAudioFrames(RadioBand centerOffset, RadioBand bandwith) {
            _sampleDeModulator.SetBand(centerOffset, bandwith);

            var bufferSize = (int)(TimeSpan.FromMilliseconds(10).TotalSeconds * _audioRate);
            var waveProviderBuffer = new byte[bufferSize];

            int read;
            while (true) {
                if (_waveProvider.BufferedBytes > 512) {
                    continue;
                }

                read = _sampleToWaveProvider16.Read(waveProviderBuffer, 0, waveProviderBuffer.Length);
                if (read == 0) {
                    continue;
                }

                _waveProvider.AddSamples(waveProviderBuffer, 0, read);
            }
        }
    }
}