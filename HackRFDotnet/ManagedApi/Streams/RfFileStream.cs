using System.Buffers;
using System.Runtime.InteropServices;

using HackRFDotnet.ManagedApi.Streams.Interfaces;
using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Streams;
public class RfFileStream : IRfDeviceStream, IDisposable {
    private readonly string _fileName;
    private FileStream? _stream = null;

    public RfFileStream(string fileName) {
        _fileName = fileName;
    }

    public int BufferLength {
        get {
            return (int)(_stream?.Length ?? 0);
        }
    }

    public RadioBand Frequency { get; set; }

    public double SampleRate { get; set; }

    public void Close() {
        _stream?.Close();
    }

    public void Dispose() {
        _stream?.Dispose();
    }

    public void Open(double sampleRate) {
        SampleRate = sampleRate;
        _stream = File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public int WriteBuffer(Span<byte> iqFrame) {
        _stream?.Write(iqFrame.ToArray(), 0, iqFrame.Length);
        return iqFrame.Length;
    }

    public int TxBuffer(Span<IQ> iqFrame) {
        var iqBytes = ArrayPool<byte>.Shared.Rent(iqFrame.Length);
        try {
            for (var x = 0; x < iqFrame.Length; x++) {
                iqBytes[x * 2] = (byte)(iqFrame[x].I * 128);
                iqBytes[(x * 2) + 1] = (byte)(iqFrame[x].Q * 128);
            }

            _stream?.Write(iqBytes, 0, iqBytes.Length);
            return iqBytes.Length;
        }
        finally {
            ArrayPool<byte>.Shared.Return(iqBytes);
        }
    }

    public int ReadBuffer(Span<IQ> iqFrame) {
        var fileBytes = ArrayPool<byte>.Shared.Rent(iqFrame.Length);
        _stream?.Read(fileBytes, 0, iqFrame.Length);

        var sampleRateDelay = TimeSpan.FromSeconds(iqFrame.Length / SampleRate);
        Thread.Sleep(sampleRateDelay);

        try {
            var interleavedTransferFrame = MemoryMarshal.Cast<byte, InterleavedSample>(fileBytes);
            var iqSamples = ArrayPool<IQ>.Shared.Rent(interleavedTransferFrame.Length);

            try {
                for (var x = 0; x < interleavedTransferFrame.Length; x++) {
                    iqSamples[x] = new IQ(interleavedTransferFrame[x]);
                }

                if (iqSamples.Length == 0) {
                    return 0;
                }

                lock (this) {
                    iqSamples.AsSpan(0, interleavedTransferFrame.Length).CopyTo(iqFrame);
                    return iqSamples.Length;
                }
            }
            finally {
                ArrayPool<IQ>.Shared.Return(iqSamples);
            }
        }
        finally {
            ArrayPool<byte>.Shared.Return(fileBytes);
        }
    }

    public void SetSampleRate(double sampleRate) {
        SampleRate = sampleRate;
    }
}
