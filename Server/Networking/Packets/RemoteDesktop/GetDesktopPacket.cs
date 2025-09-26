using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;
using Common.Video;

namespace Server.Networking.Packets.RemoteDesktop
{
    public class GetDesktopPacket : DuplexPacket
    {
        public override int Id => 0x70;
        public bool CreateNew { get; set; }
        public int Quality { get; set; }
        public int DisplayIndex { get; set; }
        public byte[] Image { get; set; }
        public int Monitor { get; set; }
        public Resolution Resolution { get; set; } = new();


        public override void Populate(ProcessStream stream)
        {
            Image = stream.ReadByteArray();
            Quality = stream.ReadInt();
            Monitor = stream.ReadInt();
            Resolution.Width = stream.ReadInt();
            Resolution.Height = stream.ReadInt();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteBoolean(CreateNew);
            packetStream.WriteInt(Quality);
            packetStream.WriteInt(DisplayIndex);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
