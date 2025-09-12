namespace HackRFDotnet.ManagedApi.Streams.Exceptions;
internal class ZeroSampleRateException : Exception {
    public ZeroSampleRateException(string? message) : base(message) {
    }
}
