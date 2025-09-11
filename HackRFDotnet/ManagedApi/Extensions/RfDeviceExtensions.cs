using HackRFDotnet.NativeApi;

namespace HackRFDotnet.ManagedApi.Extensions;
public static class RfDeviceExtensions {
    public static void AttenuateAmplification(this RfDevice rfDevice) {
        // TODO: Get the noise floor near a pre confogured db.

        rfDevice.SetAmplifications(32, 24, false);
    }
}
