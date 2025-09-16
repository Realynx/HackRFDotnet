using FftwF.Dotnet.Native;

using MathNet.Numerics;

namespace FftwF.Dotnet;
public unsafe class FftwPlan : IDisposable {
    private void* _fftwPlan;

    public FftwPlan(int length, Span<Complex32> complexIn, Span<Complex32> complexOut, bool forward = true, FftwFlags flags = FftwFlags.Measure) {
        if (length % 2 != 0) {
            throw new Exception("You should not run FFT on lengths not multiples of 2");
        }

        fixed (Complex32* complexInPtr = complexIn) {
            fixed (Complex32* complexOutPtr = complexOut) {
                _fftwPlan = NativeMethods.GetSingleDimensionDftPlan(length, complexInPtr, complexOutPtr,
                    forward ? NativeMethods.FFTW_FORWARD : NativeMethods.FFTW_BACKWARD, flags);
            }
        }
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
