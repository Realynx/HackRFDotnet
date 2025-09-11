using System.Numerics;

using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public unsafe class FMPlayer : BasePlayer {
        public FMPlayer(RfDeviceStream rfDeviceStream) : base(rfDeviceStream) {

        }

        public override void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            base.PlayStreamAsync(centerOffset, bandwith, audioRate);

            new Thread(() => StreamIqFrames(centerOffset, bandwith, audioRate)).Start();
        }

        private void StreamIqFrames(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            using var streamReader = new IQStreamReader(_rfDeviceStream);
            streamReader.SetBand(centerOffset, bandwith);

            while (true) {
                var processingChunk = new Complex[4096];
                streamReader.ReadBuffer(processingChunk);

                //Simple arctangent FM demod
                var audio = new float[processingChunk.Length - 1];
                //if (_rfDeviceStream.GetLastLevelDb() <= _rfDeviceStream.GetNoiseFloorDb() - 5) {
                //    continue;
                //}

                for (var x = 1; x < processingChunk.Length; x++) {
                    var delta = processingChunk[x] * Complex.Conjugate(processingChunk[x - 1]);
                    var fmSample = Math.Atan2(delta.Imaginary, delta.Real);
                    audio[x - 1] = (float)fmSample;
                }

                // 4) Downsample and play
                PlayDownsampled(audio, (uint)_rfDeviceStream.SampleRate, audioRate);
            }
        }
    }
}
