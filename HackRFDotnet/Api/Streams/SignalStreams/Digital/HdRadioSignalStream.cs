//using FFmpeg.AutoGen;

//using HackRFDotnet.Api.Streams.Interfaces;
//using HackRFDotnet.Api.Streams.SignalProcessing;

//using NAudio.Wave;

//namespace HackRFDotnet.Api.Streams.SignalStreams.Digital;
//public unsafe class HdRadioSignalStream : QpskSignalStream, ISampleProvider {
//    public WaveFormat? WaveFormat { get; protected set; }

//    private AVCodecContext* _codecCtx;
//    private AVCodecParserContext* _parser;
//    private AVPacket* _packet;
//    private AVFrame* _frame;

//    public HdRadioSignalStream(IIQStream iQStream, SampleRate sampleRate, bool stereo = true,
//        SignalProcessingPipeline<IQ>? processingPipeline = null, bool keepOpen = true)
//        : base(iQStream, processingPipeline, keepOpen) {

//        var rate = sampleRate.Sps;
//        if (stereo) {
//            rate /= 2;
//        }

//        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)rate, stereo ? 2 : 1);
//    }

//    public int Read(float[] buffer, int offset, int count) {
//        var bytesNeeded = count;

//        Span<byte> qpskBytes = stackalloc byte[bytesNeeded];

//        // Step 2: Read QPSK bytes
//        var read = Read(qpskBytes, bytesNeeded);
//        if (read == 0) {
//            return 0;
//        }


//        for (var x = 0; x < read && x < count; x++) {
//            buffer[offset + x] = 0;
//        }

//        return read;
//    }
//}
