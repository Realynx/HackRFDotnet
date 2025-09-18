using System.Runtime.InteropServices;

namespace BasicScanner.NativeMethods;

[StructLayout(LayoutKind.Sequential)]
public struct COORD {
    public short X;
    public short Y;

    public COORD(short x, short y) {
        X = x;
        Y = y;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct SMALL_RECT {
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
}

[StructLayout(LayoutKind.Explicit)]
public struct CHAR_INFO {
    [FieldOffset(0)]
    public char UnicodeChar;

    [FieldOffset(2)]
    public short Attributes; // foreground/background color
}

public static class Kernel32Methods {
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern unsafe bool WriteConsoleOutputW(
        IntPtr hConsoleOutput,
        CHAR_INFO* lpBuffer,
        COORD dwBufferSize,
        COORD dwBufferCoord,
        ref SMALL_RECT lpWriteRegion);


    public const int STD_OUTPUT_HANDLE = -11;
}