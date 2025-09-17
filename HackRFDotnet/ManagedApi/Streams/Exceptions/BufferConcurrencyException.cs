
namespace HackRFDotnet.Api.Streams.Buffers;

[Serializable]
internal class BufferConcurrencyException : Exception {
    public BufferConcurrencyException() {
    }

    public BufferConcurrencyException(string? message) : base(message) {
    }

    public BufferConcurrencyException(string? message, Exception? innerException) : base(message, innerException) {
    }
}