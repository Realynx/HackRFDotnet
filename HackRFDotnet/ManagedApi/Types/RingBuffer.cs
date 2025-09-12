using System.Runtime.InteropServices;

namespace HackRFDotnet.ManagedApi.Types;

internal sealed class RingBuffer<T> {
    public int Capacity { get; }

    public int Count {
        get {
            if (_start < _end) {
                return _end - _start;
            }

            return Capacity - (_start - _end);
        }
    }

    public bool IsEmpty {
        get {
            return Count == 0;
        }
    }

    private readonly T[] _array;
    private int _start;
    private int _end;

    public RingBuffer(int capacity) {
        Capacity = capacity;
        _array = new T[capacity];
    }

    /// <summary>
    /// Writes items to the ring buffer.
    /// </summary>
    public void Write(ReadOnlySpan<T> buffer) {
        // Buffer larger than capacity - only write end of buffer
        if (buffer.Length >= Capacity) {
            buffer[^Capacity..].CopyTo(_array);

            _start = 0;
            _end = Capacity;
            return;
        }

        // Buffer smaller than capacity at end, no need to wrap around
        var freeUntilWrap = Capacity - _end;
        if (buffer.Length <= freeUntilWrap) {
            buffer.CopyTo(_array.AsSpan(_end));
            _end += buffer.Length;
            _end %= Capacity;
            return;
        }

        // Buffer is large enough that two copies are needed
        buffer[..freeUntilWrap].CopyTo(_array.AsSpan(_end));
        buffer[freeUntilWrap..].CopyTo(_array);

        // Advance pointers
        if (_end < _start && _end + buffer.Length > _start) {
            _end += buffer.Length;
            _end %= Capacity;

            _start = _end;
        }
        else {
            _end += buffer.Length;
            _end %= Capacity;
        }

    }


    /// <summary>
    /// Reads items from the ringer buffer without modifying the start and end pointers.
    /// </summary>
    /// <returns>The number of items read.</returns>
    public int Peek(Span<T> buffer) {
        // [||||||||||||||||||||||||||||]
        //  ^ start                 ^ end
        if (_start < _end) {
            if (buffer.Length <= _end - _start) {
                _array.AsSpan(_start, buffer.Length).CopyTo(buffer);
                return buffer.Length;
            }

            _array.AsSpan(_start, _end - _start).CopyTo(buffer);
            return _end - _start;
        }

        // [||||||||||||||||||||||||||||]
        //  ^ end                 ^ start
        else {
            if (buffer.Length <= Capacity - _start) {
                _array.AsSpan(_start, buffer.Length).CopyTo(buffer);
                return buffer.Length;
            }

            _array.AsSpan(_start, Capacity - _start).CopyTo(buffer);
            _array.AsSpan(0, Math.Min(_end, buffer.Length - (Capacity - _start))).CopyTo(buffer[(Capacity - _start)..]);
            return Count;
        }
    }

    /// <summary>
    /// Consumes items from the ringer buffer.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> pointing to the items in the backing array.</returns>
    /// <remarks>The returned <see cref="ReadOnlySpan{T}"/> should not be read from if <see cref="Write"/> is called after <see cref="DangerousRead"/>.</remarks>
    public ReadOnlySpan<T> DangerousRead(int count, out int read) {
        var sliceStart = _start;

        // [||||||||||||||||||||||||||||]
        //  ^ start                 ^ end
        if (_start < _end) {
            read = Math.Min(count, _end - _start);

            if (_end - _start == read) {
                _start = 0;
                _end = 0;
            }
            else {
                _start += read;
            }
        }
        // [||||||||||||||||||||||||||||]
        //  ^ end                 ^ start
        else {
            read = Math.Min(count, Capacity - _start);

            _start += read;
            _start %= Capacity;
        }

        return _array.AsSpan(sliceStart, read);
    }

    /// <summary>
    /// Consumes items from the ringer buffer.
    /// </summary>
    /// <returns>The number of items read</returns>
    public int Read(Span<T> buffer) {
        var read = Peek(buffer);

        // Advance pointers
        if (_start + read >= _end) {
            // _start = 0;
            // _end = 0;
            _start += read;
            _start %= Capacity;
            _end = _start;
        }
        else {
            _start += read;
        }

        return read;
    }
}