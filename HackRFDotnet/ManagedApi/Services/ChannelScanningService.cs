using System.IO;

using HackRFDotnet.ManagedApi.Types;

namespace HackRFDotnet.ManagedApi.Services {
    public class ChannelScanningService {
        private readonly IQStream _iQStream;
        private readonly RfDevice _rfDevice;

        private DateTime _lastTxDetected = DateTime.MinValue;
        private TimeSpan _nextChannelDelay = TimeSpan.FromMilliseconds(200);

        public ChannelScanningService(IQStream iQStream, RfDevice rfDevice) {
            _iQStream = iQStream;
            _rfDevice = rfDevice;
        }

        public void StartScanning(params RadioBand[] scanBands) {
            new Thread(() => Scan(scanBands)).Start();
        }

        private void Scan(params RadioBand[] scanBands) {
            Console.Clear();
            Console.CursorVisible = false;
            var scanIndex = 0;

            while (true) {
                Console.CursorLeft = 0;
                _iQStream.WaitForFrame.WaitOne();

                Console.Title = $"Floor: {_iQStream.GetNoiseFloorDb()} Db";

                if (DateTime.Now - _lastTxDetected > _nextChannelDelay) {
                    scanIndex++;
                    scanIndex %= scanBands.Length;

                    _rfDevice.SetFrequency(scanBands[scanIndex], _iQStream.Bandwith);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"[{_rfDevice.Frequency.Mhz}] Mhz @ {_iQStream.GetLastLevelDb()} Db                ");

                    _lastTxDetected = DateTime.Now;
                    continue;
                }

                if (_iQStream.GetLastLevelDb() > _iQStream.GetNoiseFloorDb() - 16.5) {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"[{_rfDevice.Frequency.Mhz}] Mhz @ {_iQStream.GetLastLevelDb()} Db                ");
                    _lastTxDetected = DateTime.Now;
                }

                Thread.Sleep(5);
            }
        }
    }
}
