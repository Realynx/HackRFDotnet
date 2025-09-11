using System.Collections.Concurrent;
using System.Reflection.PortableExecutable;

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

            _playbackThread = new Thread(AudioPlaybackThread) { IsBackground = true };
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
        protected void PlayDownsampled(float[] audio, uint iqSampleRate, int audioSampleRate = 44_100) {
            // var sampleProvider = new SampleToWaveProvider16(ISampleProvider);

            var resampler = new NAudio.Dsp.WdlResampler();
            resampler.SetFeedMode(true); // input driven
            resampler.SetRates(iqSampleRate, audioSampleRate);

            var bytesWritten = resampler.ResamplePrepare(audio.Length, 1, out var inBuffer, out var inBufferOffset);
            Array.Copy(audio, 0, inBuffer, inBufferOffset, bytesWritten * 1);

            var outputSamples = (int)Math.Ceiling((double)(audio.Length * audioSampleRate / iqSampleRate));

            var outAudio = new float[outputSamples];
            var outAvailable = resampler.ResampleOut(outAudio, 0, bytesWritten, outputSamples, 1);

            // 2️⃣ Filter the signal
            for (var x = 0; x < outAudio.Length; x++) {
                outAudio[x] = (float)_lowPassFilter.ProcessSample(outAudio[x]);
            }

            EnqueueAudio(outAudio);
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
