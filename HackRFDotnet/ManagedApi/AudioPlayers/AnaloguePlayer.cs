using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public class AnaloguePlayer {
        private readonly SignalStream _sampleDeModulator;
        protected readonly RfDeviceStream _rfDeviceStream;

        private BufferedWaveProvider _waveProvider;
        private WaveOutEvent _waveOut;

        public AnaloguePlayer(RfDeviceStream rfDeviceStream, SignalStream signalStream) {
            _rfDeviceStream = rfDeviceStream;
            _sampleDeModulator = signalStream;
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            _waveProvider = new BufferedWaveProvider(new WaveFormat(audioRate, 16, 1)) {
                DiscardOnBufferOverflow = true
            };

            _waveOut = new WaveOutEvent { Volume = 0.05f };
            _waveOut.Init(_waveProvider);
            _waveOut.Play();

            new Thread(() => StreamIqFrames(centerOffset, bandwith, audioRate)).Start();
        }

        private void StreamIqFrames(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            _sampleDeModulator.SetBand(centerOffset, bandwith);

            var resampler = new WdlResamplingSampleProvider(_sampleDeModulator, audioRate);
            var pcmProvider = new SampleToWaveProvider16(resampler);

            var waveProviderBuffer = new byte[4096];

            int read;
            while (true) {
                while ((read = pcmProvider.Read(waveProviderBuffer, 0, waveProviderBuffer.Length)) > 0) {
                    _waveProvider.AddSamples(waveProviderBuffer, 0, read);
                }
            }
        }
    }
}
