namespace HackRFDotnet.ManagedApi.Utilities;
public static class BinaryUtilities {
    public static int NextPowerOfTwo(int n) {
        if (n < 1) {
            return 1;
        }

        n--;
        n |= n >> 1;
        n |= n >> 2;
        n |= n >> 4;
        n |= n >> 8;
        n |= n >> 16;
        return n + 1;
    }
}
