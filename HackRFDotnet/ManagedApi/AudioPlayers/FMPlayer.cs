using System.Numerics;

using HackRFDotnet.ManagedApi.Services;
using HackRFDotnet.ManagedApi.Streams;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public unsafe class FMPlayer : BasePlayer {
        public FMPlayer(IQStream iQStream) : base(iQStream) {

        }

        public override void PlayStreamAsync(int audioRate = 44100) {
            base.PlayStreamAsync(audioRate);

            new Thread(() => StreamIqFrames(audioRate)).Start();
        }

        private void StreamIqFrames(int audioRate = 44100) {
            using var filterService = new ChannelFilteringService(_iQStream);

            while (true) {
                var iqFrame = _iQStream.WaitAndDequeue();

                //Simple arctangent FM demod
                var audio = new float[iqFrame.Length - 1];
                if (_iQStream.GetLastLevelDb() <= _iQStream.GetNoiseFloorDb() - 5) {
                    continue;
                }

                for (var x = 1; x < iqFrame.Length; x++) {
                    var delta = iqFrame[x] * Complex.Conjugate(iqFrame[x - 1]);
                    var fmSample = Math.Atan2(delta.Imaginary, delta.Real);
                    audio[x - 1] = (float)fmSample;
                }

                // 4) Downsample and play
                PlayDownsampled(audio, (uint)_iQStream.SampleRate, audioRate);
            }
        }
    }
}
