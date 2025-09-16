namespace FftwF.Dotnet;
[Flags]
public enum FftwFlags : uint {
    // Documented flags
    Measure = 0,
    DestroyInput = 1U << 0,
    Unaligned = 1U << 1,
    ConserveMemory = 1U << 2,
    Exhaustive = 1U << 3,
    PreserveInput = 1U << 4,
    Patient = 1U << 5,
    Estimate = 1U << 6,
    WisdomOnly = 1U << 21,

    // Undocumented beyond-guru flags
    EstimatePatient = 1U << 7,
    BelievePcost = 1U << 8,
    NoDftR2Hc = 1U << 9,
    NoNonthreaded = 1U << 10,
    NoBuffering = 1U << 11,
    NoIndirectOp = 1U << 12,
    AllowLargeGeneric = 1U << 13,
    NoRankSplits = 1U << 14,
    NoVrankSplits = 1U << 15,
    NoVrecourse = 1U << 16,
    NoSimd = 1U << 17,
    NoSlow = 1U << 18,
    NoFixedRadixLargeN = 1U << 19,
    AllowPruning = 1U << 20
}

public static class Direction {
    public const int Forward = -1; // FFTW_FORWARD
    public const int Backward = +1; // FFTW_BACKWARD
}

public static class Limits {
    public const double NoTimeLimit = -1.0; // FFTW_NO_TIMELIMIT
}