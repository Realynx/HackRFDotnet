using System;
using System.Collections;

namespace HackRFDotnet.ManagedApi.Types;
public class CircularBuffer<T> : IEnumerable<T> {
    private readonly T[] _buffer;
    private int _head;   // Points to the oldest element
    private int _tail;   // Points to the next free slot
    private int _count;

    public int Capacity { get; }
    public int Count => _count;
    public bool IsFull => _count == Capacity;
    public bool IsEmpty => _count == 0;

    public CircularBuffer(int capacity) {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));

        Capacity = capacity;
        _buffer = new T[capacity];
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    public void Enqueue(T item) {
        _buffer[_tail] = item;
        _tail = (_tail + 1) % Capacity;

        if (IsFull) {
            // Overwrite the oldest element
            _head = (_head + 1) % Capacity;
        }
        else {
            _count++;
        }
    }

    public T Dequeue() {
        if (IsEmpty)
            throw new InvalidOperationException("The buffer is empty.");

        var item = _buffer[_head];
        _buffer[_head] = default!; // optional: clear reference for GC
        _head = (_head + 1) % Capacity;
        _count--;

        return item;
    }

    public T Peek() {
        if (IsEmpty)
            throw new InvalidOperationException("The buffer is empty.");
        return _buffer[_head];
    }

    public IEnumerator<T> GetEnumerator() {
        for (int i = 0; i < _count; i++)
            yield return _buffer[(_head + i) % Capacity];
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    internal int Read(Span<T> iqBuffer) {
        for (var x = 0; x < iqBuffer.Length; x++) {
            iqBuffer[x] = Dequeue();
        }

        return iqBuffer.Length;
    }

    internal void Write(Span<T> iqBuffer) {
        for (var x = 0; x < iqBuffer.Length; x++) {
            Enqueue(iqBuffer[x]);
        }
    }
}
