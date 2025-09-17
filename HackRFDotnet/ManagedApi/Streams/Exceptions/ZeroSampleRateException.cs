namespace HackRFDotnet.Api.Streams.Exceptions;
internal class ZeroSampleRateException : Exception {
    public ZeroSampleRateException(string? message) : base(message) {
    }
}
