using HackRFDotnet.Api.Streams.Buffers;

namespace HackRFDotnet.Api.Streams.SignalProcessing.Threading;
internal class ThreadedConvertingBuffer<T> : IDisposable {
    private readonly RingBuffer<T> _ringBuffer;

    private readonly Action<RingBuffer<T>> _bufferMaintainer;
    private readonly CancellationTokenSource _cancelMaintainer;
    private readonly Thread _maintainThread;

    /// <summary>
    /// Convert items in the background in a cycle to keep the buffer topped off.
    /// </summary>
    /// <param name="ringBuffer"></param>
    /// <param name="bufferMaintainer"></param>
    public ThreadedConvertingBuffer(RingBuffer<T> ringBuffer, Action<RingBuffer<T>> bufferMaintainer) {
        _ringBuffer = ringBuffer;
        _bufferMaintainer = bufferMaintainer;

        _cancelMaintainer = new CancellationTokenSource();
        _maintainThread = new Thread(MaintainBuffer);
        _maintainThread.Start();
    }

    public void Dispose() {
        _cancelMaintainer.Cancel();
    }

    public void Write(ReadOnlySpan<T> interleavedSamples) {
        _ringBuffer.Write(interleavedSamples);
    }

    public int Read(Span<T> interleavedSamples) {
        return _ringBuffer.Read(interleavedSamples);
    }

    private void MaintainBuffer() {
        while ((!_cancelMaintainer?.IsCancellationRequested ?? false) && _bufferMaintainer is not null) {
            _bufferMaintainer.Invoke(_ringBuffer);
            Thread.Sleep(1);
        }
    }
}
