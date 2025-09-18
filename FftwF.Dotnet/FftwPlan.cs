using FftwF.Dotnet.Native;

using MathNet.Numerics;

namespace FftwF.Dotnet;
public unsafe class FftwPlan : IDisposable {
    private readonly void* _fftwPlan;

    public FftwPlan(int length, Complex32* complexIn, Complex32* complexOut, bool forward = true, FftwFlags flags = FftwFlags.Measure) {
        if (length % 2 != 0) {
            throw new Exception("You should not run FFT on lengths not multiples of 2");
        }

        _fftwPlan = NativeMethods.GetSingleDimensionDftPlan(length, complexIn, complexOut,
            forward ? NativeMethods.FFTW_FORWARD : NativeMethods.FFTW_BACKWARD, flags);
    }

    public void Execute() {
        NativeMethods.ExecutePlan(_fftwPlan);
    }

    public void Dispose() {
        if (_fftwPlan is not null) {
            NativeMethods.DestroyPlan(_fftwPlan);
        }
    }
}
