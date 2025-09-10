using HackRFDotnet.ManagedApi.Services;
using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public unsafe class AMPlayer : BasePlayer {

        public AMPlayer(IQStream iQStream) : base(iQStream) {

        }

        public override void PlayStreamAsync(int audioRate = 44100) {
            base.PlayStreamAsync(audioRate);

            new Thread(() => StreamIqFrames(audioRate)).Start();
        }

        private void StreamIqFrames(int audioRate = 44100) {
            using var filterService = new ChannelFilteringService(_iQStream);

            while (true) {
                var iqFrame = _iQStream.WaitAndDequeue();
                var audio = new float[iqFrame.Length];

                if (_iQStream.GetLastLevelDb() <= _iQStream.GetNoiseFloorDb() - 16.5) {
                    // Output silence if squelched
                    for (int i = 0; i < audio.Length; i++)
                        audio[i] = 0f;
                }
                else {
                    // Envelope detection (magnitude of IQ)
                    for (var i = 0; i < iqFrame.Length; i++) {
                        var s = iqFrame[i];
                        audio[i] = (float)Math.Sqrt(s.Real * s.Real + s.Imaginary * s.Imaginary);
                    }

                    // Remove DC offset
                    float avg = 0f;
                    for (int i = 0; i < audio.Length; i++)
                        avg += audio[i];
                    avg /= audio.Length;
                    for (int i = 0; i < audio.Length; i++)
                        audio[i] -= avg;
                }

                // Downsample and play
                PlayDownsampled(audio, (uint)_iQStream.SampleRate, audioRate);
            }
        }


    }
}
