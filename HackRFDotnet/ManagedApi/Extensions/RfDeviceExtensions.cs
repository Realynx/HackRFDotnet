namespace HackRFDotnet.ManagedApi.Extensions;
public static class RfDeviceExtensions {
    public static void AttenuateAmplification(this DigitalRadioDevice rfDevice) {
        // TODO: Get the noise floor near a pre confogured db.

        rfDevice.SetAmplifications(24, 15, true);
    }
}
