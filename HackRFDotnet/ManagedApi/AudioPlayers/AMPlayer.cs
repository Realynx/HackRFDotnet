using System.Numerics;

using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public unsafe class AMPlayer : BasePlayer {

        public AMPlayer(RfDeviceStream iQStream) : base(iQStream) {

        }

        public override void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            base.PlayStreamAsync(centerOffset, bandwith, audioRate);

            new Thread(() => StreamIqFrames(centerOffset, bandwith, audioRate)).Start();
        }

        private void StreamIqFrames(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            using var streamReader = new FmSignalStream(_rfDeviceStream);
            streamReader.SetBand(centerOffset, bandwith);

            while (true) {
                var processingChunk = new Complex[4096];
                // streamReader.ReadBuffer(processingChunk);

                var audio = new float[processingChunk.Length];

                //if (_rfDeviceStream.GetLastLevelDb() <= _rfDeviceStream.GetNoiseFloorDb() - 16.5) {
                //    // Output silence if squelched
                //    for (int i = 0; i < audio.Length; i++)
                //        audio[i] = 0f;

                //    return;
                //}

                // Envelope detection (magnitude of IQ)
                for (var i = 0; i < processingChunk.Length; i++) {
                    var s = processingChunk[i];
                    audio[i] = (float)Math.Sqrt(s.Real * s.Real + s.Imaginary * s.Imaginary);
                }

                // Remove DC offset
                float avg = 0f;
                for (int i = 0; i < audio.Length; i++)
                    avg += audio[i];
                avg /= audio.Length;
                for (int i = 0; i < audio.Length; i++)
                    audio[i] -= avg;


                // Downsample and play
                // PlayDownsampled(audio, (int)_rfDeviceStream.SampleRate, audioRate);
            }
        }


    }
}
