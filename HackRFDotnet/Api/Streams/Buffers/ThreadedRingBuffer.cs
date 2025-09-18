using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using HackRFDotnet.Api.Streams.Exceptions;

namespace HackRFDotnet.Api.Streams.Buffers;

/// <summary>
/// <see cref="ThreadedRingBuffer{T}"/> allows for multithreaded access to a single ring buffer. Writing is not thread synced.
/// </summary>
internal class ThreadedRingBuffer<T> : UnsafeRingBuffer<T> {
    private struct ThreadPointer() {
        public int ReadStart;
        public bool Empty = true;
        public bool Full = false;
    }

    private int? _writerId = null;
    private int _writerStart = 0;

    private readonly Dictionary<int, ThreadPointer> _threadIdPointers = [];
    private readonly bool _multiWrite;
    private readonly Lock _writeLock = new();

    public ThreadedRingBuffer(int capacity, bool multiWrite = false) : base(capacity) {
        _multiWrite = multiWrite;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BytesAvailable() {
        var threadId = Environment.CurrentManagedThreadId;
        ref var state = ref CollectionsMarshal.GetValueRefOrAddDefault(_threadIdPointers, threadId, out _);

        if (state.Full) {
            return Capacity;
        }

        if (state.Empty) {
            return 0;
        }

        var difference = _writerStart - state.ReadStart;
        return difference < 0 ? Capacity + difference : difference;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSpan(Span<T> emptyMemory) {
        return ReadSpan(emptyMemory, emptyMemory.Length);
    }

    public int ReadSpan(Span<T> emptyMemory, int count) {
        var threadId = Environment.CurrentManagedThreadId;
        ref var state = ref CollectionsMarshal.GetValueRefOrAddDefault(_threadIdPointers, threadId, out _);

        var readBytes = ReadSpan(emptyMemory, state.ReadStart, count);
        var newReadPoint = (state.ReadStart + readBytes) % Capacity;

        var looped = (state.ReadStart + readBytes) >= Capacity;
        if ((looped && _writerStart > state.ReadStart) || (newReadPoint > _writerStart && state.ReadStart < _writerStart)) {
            state.ReadStart = _writerStart;
            state.Empty = true;

            return readBytes;
        }

        state.ReadStart = newReadPoint;
        return readBytes;
    }

    public void WriteSpan(Span<T> inputData, int? threadId = null) {
        threadId ??= Environment.CurrentManagedThreadId;

        if (_writerId is null) {
            _writerId = threadId;
        }
        else if (threadId != _writerId && !_multiWrite) {
            throw new BufferConcurrencyException("Cannot have more than one writing thread.");
        }

        lock (_writeLock) {
            WriteSpan(inputData, inputData.Length);
        }
    }

    public void WriteSpan(Span<T> inputData, int count) {
        Write(inputData, _writerStart, count);

        var newWritePoint = (_writerStart + count) % Capacity;
        var looped = (_writerStart + count) >= Capacity;

        lock (_writeLock) {
            foreach (var thread in _threadIdPointers) {
                var existingValue = thread.Value;
                if (newWritePoint == existingValue.ReadStart ||
                    (looped && existingValue.ReadStart > _writerStart) ||
                    (newWritePoint > existingValue.ReadStart && _writerStart < existingValue.ReadStart)) {
                    existingValue.ReadStart = newWritePoint;
                    existingValue.Full = true;
                }
                else {
                    existingValue.Full = false;
                }

                existingValue.Empty = false;
                _threadIdPointers[thread.Key] = existingValue;
            }

            _writerStart = newWritePoint;
        }
    }
}
