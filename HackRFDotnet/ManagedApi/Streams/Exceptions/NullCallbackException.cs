namespace HackRFDotnet.ManagedApi.Streams.Exceptions;
public class NullCallbackException : Exception {
    public NullCallbackException(string? message) : base(message) {
    }
}
