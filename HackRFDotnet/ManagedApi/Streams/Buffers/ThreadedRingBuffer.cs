using System.Runtime.CompilerServices;
using System.Threading;

namespace HackRFDotnet.ManagedApi.Streams.Buffers;
internal struct ThreadPointer {
    public int readStart;
    public bool empty = true;
    public bool full = false;

    public ThreadPointer() {
    }
}

/// <summary>
/// <see cref="ThreadedRingBuffer"/> allows for multi threadded access to a single ring buffer. Writing is not thread synced.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ThreadedRingBuffer<T> : UnsafeRingBuffer<T> {
    private int? _writerId = null;
    private int _writerStart = 0;

    private readonly Dictionary<int, ThreadPointer> _threadIdPointers = [];
    private readonly bool _multiWrite;

    public ThreadedRingBuffer(int capacity, bool multiWrite = false) : base(capacity) {
        _multiWrite = multiWrite;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BytesAvailable() {
        var threadId = Environment.CurrentManagedThreadId;
        if (!_threadIdPointers.TryGetValue(threadId, out var state)) {
            _threadIdPointers.Add(threadId, state);
        }

        if (state.full) {
            return Length;
        }

        if (state.empty) {
            return 0;
        }

        var difference = _writerStart - state.readStart;
        return difference < 0 ? Length + difference : difference;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSpan(Span<T> emptyMemory) {
        return ReadSpan(emptyMemory, emptyMemory.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSpan(Span<T> emptyMemory, int count) {
        var threadId = Environment.CurrentManagedThreadId;
        _threadIdPointers.TryGetValue(threadId, out var state);

        var readBytes = ReadSpan(emptyMemory, state.readStart, count);
        var newReadPoint = (state.readStart + readBytes) % Length;

        var looped = (state.readStart + readBytes) >= Length;
        if ((looped && _writerStart > state.readStart) || (newReadPoint > _writerStart && state.readStart < _writerStart)) {
            state.readStart = _writerStart;
            state.empty = true;

            _threadIdPointers[threadId] = state;
            return readBytes;
        }

        state.readStart = newReadPoint;
        _threadIdPointers[threadId] = state;
        return readBytes;
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

        var newWritePoint = (_writerStart + count) % Length;
        var looped = (_writerStart + count) >= Length;

        foreach (var thread in _threadIdPointers) {
            var existingValue = thread.Value;
            if ((looped && existingValue.readStart > _writerStart) ||
                (newWritePoint > existingValue.readStart && _writerStart < existingValue.readStart)) {
                existingValue.readStart = newWritePoint;
                existingValue.full = true;
            }
            else {
                existingValue.full = false;
            }

            existingValue.empty = false;
            _threadIdPointers[thread.Key] = existingValue;
        }

        _writerStart = newWritePoint;
    }
}
