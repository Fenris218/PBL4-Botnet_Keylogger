using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.SystemInfo
{
    public class SystemInfoPacket : RequestPacket
    {
        public override int Id => 0xca;
        public List<Tuple<string, string>> SystemInfos { get; set; } = new();

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteInt(SystemInfos.Count);
            for (var i = 0; i < SystemInfos.Count; i++)
            {
                packetStream.WriteString(SystemInfos[i].Item1);
                packetStream.WriteString(SystemInfos[i].Item2);
            }

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
