using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;
using Common.Video;

namespace Client.Networking.Packets.RemoteDesktop
{
    public class GetDesktopPacket : DuplexPacket
    {
        public override int Id => 0x70;
        public bool CreateNew { get; set; }
        public int Quality { get; set; }
        public int DisplayIndex { get; set; }
        public byte[] Image { get; set; }
        public int Monitor { get; set; }
        public Resolution Resolution { get; set; }

        public override void Populate(ProcessStream stream)
        {
            CreateNew = stream.ReadBoolean();
            Quality = stream.ReadInt();
            DisplayIndex = stream.ReadInt();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteByteArray(Image);
            packetStream.WriteInt(Quality);
            packetStream.WriteInt(Monitor);
            packetStream.WriteInt(Resolution.Width);
            packetStream.WriteInt(Resolution.Height);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
