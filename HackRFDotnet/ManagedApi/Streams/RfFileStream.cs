namespace HackRFDotnet.ManagedApi.Streams;
public class RfFileStream : IDisposable {
    private readonly string _fileName;

    public RfFileStream(string fileName) {
        _fileName = fileName;
    }

    public void Dispose() {
    }
}
