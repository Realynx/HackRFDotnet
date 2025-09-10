using System.Collections.Concurrent;

using HackRFDotnet.ManagedApi.Streams;

using MathNet.Filtering.FIR;

using NAudio.Wave;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public abstract class BasePlayer : IDisposable {
        private readonly ConcurrentQueue<float[]> _audioQueue = new();

        private BufferedWaveProvider _waveProvider;
        private WaveOutEvent _waveOut;

        private double[] _firCoefficients;
        private OnlineFirFilter _lowPassFilter;

        private readonly Thread _playbackThread;
        protected readonly RfDeviceStream _iQStream;


        private bool _running = true;

        protected BasePlayer(RfDeviceStream iQStream) {
            _iQStream = iQStream;

            _playbackThread = new Thread(AudioPlaybackThread) { IsBackground = true };
        }

        public virtual void PlayStreamAsync(int audioRate = 44100) {
            _waveProvider = new BufferedWaveProvider(new WaveFormat(audioRate, 16, 1));

            _waveOut = new WaveOutEvent { Volume = 0.05f };
            _waveOut.Init(_waveProvider);
            _waveOut.Play();

            var halfOrder = 100;
            _firCoefficients = FirCoefficients.LowPass(
                samplingRate: _iQStream.SampleRate,
                cutoff: audioRate / 2,
                dcGain: 2.0,
                halforder: halfOrder
            );

            _lowPassFilter = new OnlineFirFilter(_firCoefficients);
            _playbackThread.Start();
        }

        protected void EnqueueAudio(float[] audioChunk) {
            _audioQueue.Enqueue(audioChunk);
        }

        private void AudioPlaybackThread() {
            while (_running) {
                if (_audioQueue.TryDequeue(out var chunk)) {
                    // Convert entire chunk to PCM
                    byte[] buffer = new byte[chunk.Length * 2];
                    for (int i = 0; i < chunk.Length; i++) {
                        short val = (short)(Math.Clamp(chunk[i], -1f, 1f) * short.MaxValue);
                        buffer[i * 2] = (byte)(val & 0xFF);
                        buffer[i * 2 + 1] = (byte)((val >> 8) & 0xFF);
                    }

                    _waveProvider.AddSamples(buffer, 0, buffer.Length);
                }
                else {
                    Thread.Sleep(1); // prevent CPU spin
                }
            }
        }


        protected void PlayDownsampled(float[] audio, uint iqSampleRate, int audioSampleRate = 44100) {
            // 2️⃣ Filter the signal
            for (var x = 0; x < audio.Length; x++) {
                audio[x] = (float)_lowPassFilter.ProcessSample(audio[x]);
            }

            // 3️⃣ Decimate
            var decimation = (int)(iqSampleRate / audioSampleRate);
            float[] audioOut = new float[audio.Length / decimation];
            for (int i = 0; i < audioOut.Length; i++)
                audioOut[i] = audio[i * decimation];

            EnqueueAudio(audioOut);
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
