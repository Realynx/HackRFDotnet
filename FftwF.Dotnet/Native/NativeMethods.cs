using System.Runtime.InteropServices;

using MathNet.Numerics;

namespace FftwF.Dotnet.Native;

/// <summary>
///* https://www.fftw.org/fftw3_doc/Complex-One_002dDimensional-DFTs.html#Complex-One_002dDimensional-DFTs
///#include <fftw3.h>
///   ...
///   {
///       fftw_complex *in, *out;
///       fftw_plan p;
///       ...
///       in = (fftw_complex*) fftw_malloc(sizeof(fftw_complex) * N);
///       out = (fftw_complex*) fftw_malloc(sizeof(fftw_complex) * N);
///       p = fftw_plan_dft_1d(N, in, out, FFTW_FORWARD, FFTW_ESTIMATE);
///       ...
///       fftw_execute(p); //  repeat as needed
///       ...
///       fftw_destroy_plan(p);
///       fftw_free(in); fftw_free(out);
///   }
/// </summary>
public unsafe class NativeMethods {
    private const string LIBFFTW_DLL = "libfftw3f-3.dll";

    [DllImport(LIBFFTW_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_1d")]
    public static extern void* GetSingleDimensionDftPlan(int n, Complex32* input, Complex32* output, int sign, FftwFlags flags);

    [DllImport(LIBFFTW_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_execute")]
    public static extern void ExecutePlan(void* plan);

    [DllImport(LIBFFTW_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_destroy_plan")]
    public static extern void DestroyPlan(void* plan);

    public const int FFTW_FORWARD = -1;
    public const int FFTW_BACKWARD = 1;

    public const float FFTW_NO_TIMELIMIT = -1.0f;
}
