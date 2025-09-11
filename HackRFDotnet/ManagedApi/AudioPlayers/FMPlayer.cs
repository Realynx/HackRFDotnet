using HackRFDotnet.ManagedApi.Streams;
using HackRFDotnet.ManagedApi.Streams.SignalStreams;
using HackRFDotnet.ManagedApi.Types;

using NAudio.Wave.SampleProviders;

namespace HackRFDotnet.ManagedApi.AudioPlayers {
    public unsafe class FMPlayer : BasePlayer {
        private readonly FmSignalStream _fmSignalStream;

        public FMPlayer(RfDeviceStream rfDeviceStream) : base(rfDeviceStream) {
            _fmSignalStream = new FmSignalStream(rfDeviceStream);
        }

        public override void PlayStreamAsync(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            base.PlayStreamAsync(centerOffset, bandwith, audioRate);

            new Thread(() => StreamIqFrames(centerOffset, bandwith, audioRate)).Start();
        }

        private void StreamIqFrames(RadioBand centerOffset, RadioBand bandwith, int audioRate = 44100) {
            _fmSignalStream.SetBand(centerOffset, bandwith);

            var resampler = new WdlResamplingSampleProvider(_fmSignalStream, audioRate);
            var pcmProvider = new SampleToWaveProvider16(resampler);

            var waveProviderBuffer = new byte[4096];

            int read;
            while (true) {
                while ((read = pcmProvider.Read(waveProviderBuffer, 0, waveProviderBuffer.Length)) > 0) {
                    PlaySamples(waveProviderBuffer, read);
                }
            }
        }
    }
}
