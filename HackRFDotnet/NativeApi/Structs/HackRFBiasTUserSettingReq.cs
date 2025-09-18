using System.Runtime.InteropServices;

namespace HackRFDotnet.NativeApi.Structs {
    /// <summary>
    /// User settings for user-supplied bias tee defaults.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HackRFBiasTUserSettingReq {
        public HackRFBoolUserSetting tx;
        public HackRFBoolUserSetting rx;
        public HackRFBoolUserSetting off;
    }
}
