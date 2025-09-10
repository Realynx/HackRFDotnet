namespace HackRFDotnet.Enums {
    public enum LedState : byte {
        UsbLight = (0 << 0),
        RxLight = (1 << 1),
        TxLight = (1 << 2)
    }
}
