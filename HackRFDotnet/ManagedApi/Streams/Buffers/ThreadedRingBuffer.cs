using System.Runtime.CompilerServices;

namespace HackRFDotnet.ManagedApi.Streams.Buffers;
/// <summary>
/// <see cref="ThreadedRingBuffer"/> allows for multi threadded access to a single ring buffer. Writing is not thread synced.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ThreadedRingBuffer<T> : UnsafeRingBuffer<T> {
    private int? _writerId = null;
    private int _writerStart = 0;

    private readonly Dictionary<int, int> _threadIdPointers = [];
    private readonly bool _multiWrite;

    public ThreadedRingBuffer(int capacity, bool multiWrite = false) : base(capacity) {
        _multiWrite = multiWrite;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSpan(Span<T> emptyMemory) {
        return ReadSpan(emptyMemory, emptyMemory.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSpan(Span<T> emptyMemory, int count) {
        var threadId = Environment.CurrentManagedThreadId;
        _threadIdPointers.TryGetValue(threadId, out var start);

        var readBytes = ReadSpan(emptyMemory, start, count);
        _threadIdPointers[threadId] = (count + start) % Length;

        return readBytes;
    }

    public int BytesAvailable() {
        var threadId = Environment.CurrentManagedThreadId;
        if (!_threadIdPointers.TryGetValue(threadId, out var start)) {
            _threadIdPointers.Add(threadId, 0);
        }

        var difference = _writerStart - start;
        return difference > 0 ? difference : difference * -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan(Span<T> emptyMemory, int? threadId = null) {
        threadId ??= Environment.CurrentManagedThreadId;

        if (_writerId is null) {
            _writerId = threadId;
        }
        else if (threadId != _writerId && !_multiWrite) {
            throw new BufferConcurrencyException("Cannot have more than one writing thread.");
        }

        WriteSpan(emptyMemory, emptyMemory.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan(Span<T> emptyMemory, int count) {
        Write(emptyMemory, _writerStart, count);
        _writerStart = (_writerStart + count) % Length;
    }
}
