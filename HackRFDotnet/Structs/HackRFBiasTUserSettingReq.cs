using System.Runtime.InteropServices;

namespace HackRFDotnet.Structs {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HackRFBiasTUserSettingReq {
        public HackRFBoolUserSetting tx;
        public HackRFBoolUserSetting rx;
        public HackRFBoolUserSetting off;
    }
}
