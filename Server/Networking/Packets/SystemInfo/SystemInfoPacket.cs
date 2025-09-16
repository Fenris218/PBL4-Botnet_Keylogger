using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.SystemInfo
{
    public class SystemInfoPacket : DuplexPacket
    {
        public override int Id => 0x10;
        public List<Tuple<string, string>> SystemInfos { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            SystemInfos = new List<Tuple<string, string>>();
            var count = stream.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var key = stream.ReadString();
                var value = stream.ReadString();
                SystemInfos.Add(new Tuple<string, string>(key, value));
            }
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
