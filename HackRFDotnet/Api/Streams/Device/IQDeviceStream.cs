using System.Diagnostics;

using HackRFDotnet.Api.Streams.Buffers;
using HackRFDotnet.Api.Streams.Exceptions;
using HackRFDotnet.Api.Streams.Interfaces;
using HackRFDotnet.Api.Streams.SignalStreams;
using HackRFDotnet.NativeApi.Lib;
using HackRFDotnet.NativeApi.Structs;

namespace HackRFDotnet.Api.Streams.Device {
    /// <summary>
    /// IQ Stream from the Rf Device.
    /// This stream is the root of all data processed with the library.
    /// This stream must remain immutable from all other <see cref="SignalStream{T}"/>.
    /// </summary>
    public unsafe class IQDeviceStream : IIQStream {
        public readonly DigitalRadioDevice RfDevice;

        private ThreadedRingBuffer<IQ>? _iqBuffer = null;
        private IQ[] _sampleConvertBuffer = [];
        private readonly int _transferBufferSize = 0;

        public SampleRate SampleRate {
            get {
                return RfDevice.DeviceSamplingRate;
            }
        }

        public int BufferLength {
            get {
                return _iqBuffer?.BytesAvailable() ?? 0;
            }
        }

        public IQDeviceStream(DigitalRadioDevice rfDevice) {
            RfDevice = rfDevice;

            _transferBufferSize = (int)HackRfNativeLib.DeviceStreaming.GetTransferBufferSize(rfDevice.DevicePtr);
        }

        public void OpenRx(SampleRate? sampleRate = null) {
            if (sampleRate is null) {
                sampleRate = SampleRate;
            }

            SetSampleRate(sampleRate ?? throw new ZeroSampleRateException("Cannot have a 0 sample rate"));
            RfDevice.StartRx(BufferTransferChunk);
        }

        public void Close() {
            RfDevice.StopRx();
        }

        public void SetSampleRate(SampleRate sampleRate) {
            var bufferSize = (int)(TimeSpan.FromMilliseconds(64).TotalSeconds * SampleRate.Sps);
            RfDevice.SetSampleRate(sampleRate);

            // TODO: Fix thread multiwrite (because hack rf seems to use multiple threads sometimes)
            _iqBuffer = new ThreadedRingBuffer<IQ>(bufferSize, true);

            var transferSize = HackRfNativeLib.DeviceStreaming.GetTransferBufferSize(RfDevice.DevicePtr) / 2;
            _sampleConvertBuffer = new IQ[transferSize];
        }

        public int TxBuffer(Span<IQ> iqFrame) {
            throw new NotImplementedException();
        }

        public int ReadBuffer(Span<IQ> iqBuffer) {
            Debug.Assert(_iqBuffer is null || _iqBuffer.Capacity >= iqBuffer.Length);

            // Stop CPU pinning
            while (iqBuffer.Length > _iqBuffer?.BytesAvailable()) {
                Thread.Sleep(1);
            }

            var readSamples = _iqBuffer?.ReadSpan(iqBuffer) ?? throw new Exception("Empty Buffer");
            return readSamples;
        }

        private int BufferTransferChunk(HackrfTransfer* hackrfTransfer) {
            if (hackrfTransfer is null) {
                return 1;
            }

            // Each callback takes the same amount of time to happen as the frame size is in the time domain
            // 1MSPS = [131ms frame] 131072 / 131ms
            // 3MSPS = [43ms frame]  131072 / 43ms
            // 5MSPS = [26ms frame]  131072 / 26ms
            // 8MSPS = [16ms frame]  131072 / 16ms
            // 16MSPS =[8.2ms frame]  131072 / 8ms
            // 20MSPS =[6.5ms frame]  131072 / 6ms

            var iqSize = hackrfTransfer->valid_length / 2;
            var interleavedSamples = new Span<InterleavedSample>(hackrfTransfer->buffer, iqSize);
            fixed (InterleavedSample* packedSamples = interleavedSamples) {
                fixed (IQ* iq = _sampleConvertBuffer) {
                    for (var x = 0; x < iqSize; x++) {
                        iq[x].I = packedSamples[x].I;
                        iq[x].Q = packedSamples[x].Q;
                    }
                }
            }

            _iqBuffer?.WriteSpan(_sampleConvertBuffer);
            return 0;
        }

        public void Dispose() {
            Close();
        }
    }
}
