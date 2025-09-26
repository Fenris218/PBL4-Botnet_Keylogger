using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.RemoteDesktop
{
    public class GetMonitorsPacket : DuplexPacket
    {
        public override int Id => 0x71;
        public int Number { get; set; }


        public override void Populate(ProcessStream stream)
        {
            Number = stream.ReadInt();
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
