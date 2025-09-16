using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.ClientServices
{
    public class ClientDisconnectPacket : RequestPacket
    {
        public override int Id => 0x05;

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
