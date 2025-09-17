namespace HackRFDotnet.Api.Streams;
public struct InterleavedSample {
    public sbyte I;
    public sbyte Q;

    public InterleavedSample Clone() {
        return new InterleavedSample {
            I = I,
            Q = Q
        };
    }
}