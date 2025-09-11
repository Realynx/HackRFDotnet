using System.Collections.Concurrent;

using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Types;

using MathNet.Filtering.FIR;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public abstract class BasePlayer : IDisposable {
        private readonly ConcurrentQueue<float[]> _audioQueue = new();

        private BufferedWaveProvider _waveProvider;
        private WaveOutEvent _waveOut;

        private double[] _firCoefficients;
        private OnlineFirFilter _lowPassFilter;

        private readonly Thread _playbackThread;
        protected readonly RfDeviceStream _rfDeviceStream;


        private bool _running = true;

        protected BasePlayer(RfDeviceStream rfDeviceStream) {
            _rfDeviceStream = rfDeviceStream;
        }

        public virtual void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            _waveProvider = new BufferedWaveProvider(new WaveFormat(audioRate, 16, 1)) {
                DiscardOnBufferOverflow = true
            };

            _waveOut = new WaveOutEvent { Volume = 0.05f };
            _waveOut.Init(_waveProvider);
            _waveOut.Play();

            var halfOrder = 100;
            _firCoefficients = FirCoefficients.LowPass(
                samplingRate: _rfDeviceStream.SampleRate,
                cutoff: audioRate / 2,
                dcGain: 2.0,
                halforder: halfOrder
            );

            _lowPassFilter = new OnlineFirFilter(_firCoefficients);
        }

        protected void EnqueueAudio(float[] audioChunk) {
            _audioQueue.Enqueue(audioChunk);
        }

        protected void PlaySamples(byte[] waveProviderBuffer, int samples) {
            _waveProvider.AddSamples(waveProviderBuffer, 0, samples);
        }

        public void Stop() {
            _running = false;
            _playbackThread.Join();
            _waveOut.Stop();
            _waveOut.Dispose();
        }

        public void Dispose() => Stop();
    }

}
