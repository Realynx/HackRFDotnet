//using FFmpeg.AutoGen;

//namespace HackRFDotnet.ManagedApi.Streams.SignalStreams.Digital.Covnert;
//unsafe class FfmpegDecoder : IDisposable {
//    private AVCodecContext* _codecCtx;
//    private AVCodecParserContext* _parser;
//    private AVPacket* _packet;
//    private AVFrame* _frame;

//    static FfmpegDecoder() {
//        ffmpeg.avdevice_register_all();
//        ffmpeg.avformat_network_init();
//    }

//    public FfmpegDecoder() {
//        var codec = ffmpeg.avcodec_find_decoder(AVCodecID.AV_CODEC_ID_AAC);
//        if (codec == null)
//            throw new Exception("AAC codec not found");

//        _codecCtx = ffmpeg.avcodec_alloc_context3(codec);
//        if (_codecCtx == null)
//            throw new Exception("Could not allocate codec context");

//        ffmpeg.avcodec_open2(_codecCtx, codec, null);

//        _parser = ffmpeg.av_parser_init((int)AVCodecID.AV_CODEC_ID_AAC);
//        _packet = ffmpeg.av_packet_alloc();
//        _frame = ffmpeg.av_frame_alloc();
//    }

//    public int Decode(byte[] input, float[] output, int offset) {
//        int outputOffset = offset;
//        int inputOffset = 0;
//        int inputLength = input.Length;

//        while (inputLength > 0) {
//            int ret = ffmpeg.av_parser_parse2(
//                _parser, _codecCtx, &_packet->data, &_packet->size,
//                (byte*)input.AsSpan(inputOffset, inputLength).GetPinnableReference(),
//                inputLength, ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE, 0);

//            if (ret < 0)
//                break;
//            inputOffset += ret;
//            inputLength -= ret;

//            if (_packet->size > 0) {
//                ffmpeg.avcodec_send_packet(_codecCtx, _packet);

//                while (ffmpeg.avcodec_receive_frame(_codecCtx, _frame) == 0) {
//                    int nbSamples = _frame->nb_samples;
//                    int channels = _codecCtx->;

//                    for (int i = 0; i < nbSamples * channels && outputOffset < output.Length; i++) {
//                        output[outputOffset++] = ((float*)_frame->extended_data[0])[i];
//                    }
//                }
//            }
//        }

//        return outputOffset - offset;
//    }

//    public void Dispose() {
//        ffmpeg.av_parser_close(_parser);
//        ffmpeg.avcodec_free_context(&_codecCtx);
//        ffmpeg.av_packet_free(&_packet);
//        ffmpeg.av_frame_free(&_frame);
//    }
//}

