using System.Runtime.CompilerServices;

namespace HackRFDotnet.Api.Streams.Buffers;
/// <summary>
/// <see cref="UnsafeRingBuffer{T}"/> does not store any pointers to offset read or write.
/// You are meant to track your own pointers when using <see cref="UnsafeRingBuffer{T}"/>.
/// </summary>
internal class UnsafeRingBuffer<T> {
    public int Length { get; init; }
    private readonly T[] _array;

    public UnsafeRingBuffer(int capacity) {
        Length = capacity;
        _array = new T[capacity];
    }

    /// <summary>
    /// Write memory to the buffer, accounts for overlapping.
    /// This gets called in loops a lot so we inline it.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<T> bufferData, int offset, int count) {
        if (count == 0 || count > Length) {
            return;
        }

        var writeTo = (offset + count) % _array.Length;
        if (writeTo > offset) {
            bufferData.Slice(0, count).CopyTo(_array.AsSpan(offset, count));
        }
        else {
            var lastHalfSize = _array.Length - offset;
            bufferData.Slice(0, lastHalfSize).CopyTo(_array.AsSpan(offset, lastHalfSize));
            bufferData.Slice(lastHalfSize, writeTo).CopyTo(_array.AsSpan(0, writeTo));
        }
    }

    /// <summary>Read memory from the buffer into another buffer.</summary>
    /// <param name="emptyMemory">The memory to be filled.</param>
    /// <param name="offset">The offset from 0 in the ring buffer.</param>
    /// <param name="count">The number of bytes to be read. If larger than the buffer size, you will get duplicate data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSpan(Span<T> emptyMemory, int offset, int count) {
        if (count == 0) {
            return 0;
        }

        if (emptyMemory.Length < count) {
            count = emptyMemory.Length;
        }

        var readTo = (offset + count) % _array.Length;
        if (readTo > offset) {
            _array.AsSpan(offset, count).CopyTo(emptyMemory);
        }
        else {
            var lastHalfSize = _array.Length - offset;
            _array.AsSpan(offset, lastHalfSize).CopyTo(emptyMemory.Slice(0, lastHalfSize));
            _array.AsSpan(0, readTo).CopyTo(emptyMemory.Slice(lastHalfSize, readTo));
        }

        return count;
    }

    /// <summary>Copy the ring buffer into a new array. Accounts for overlapping.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray(int offset, int count) {
        var array = new T[count];
        _ = ReadSpan(array, offset, count);

        return array;
    }
}
