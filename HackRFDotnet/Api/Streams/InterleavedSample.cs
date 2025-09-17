namespace HackRFDotnet.Api.Streams;
/// <summary>
/// <see cref="InterleavedSample"/> comes directly from the HackRF device in transfer chunks.
/// Memory alignment allows us to ready and copy them into objects very quickly.
/// </summary>
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