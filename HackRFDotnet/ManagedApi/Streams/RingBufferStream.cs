using HackRFDotnet.ManagedApi.Streams.Buffers;

namespace HackRFDotnet.ManagedApi.Streams;

internal sealed class RingBufferStream : Stream
{
    private readonly RingBuffer<byte> _ringBuffer;

    public RingBufferStream(RingBuffer<byte> ringBuffer)
    {
        _ringBuffer = ringBuffer;
    }

    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _ringBuffer.Read(buffer.AsSpan(offset, count));
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _ringBuffer.Write(buffer.AsSpan(offset, count));
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => _ringBuffer.AvailableBytes;

    public override long Position
    {
        get => 0;
        set => throw new NotSupportedException();
    }
}