using Common.Models;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.FileManager
{
    public class GetDirectoryPacket : DuplexPacket
    {
        public override int Id => 0x42;
        public string RemotePath { get; set; }
        public List<FileSystemEntry> Items { get; set; }

        public override void Populate(ProcessStream stream)
        {
            RemotePath = stream.ReadString();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteString(RemotePath);
            packetStream.WriteInt(Items.Count());
            foreach (var item in Items)
            {
                packetStream.WriteUnsignedByte((byte)item.EntryType);
                packetStream.WriteString(item.Name);
                packetStream.WriteLong(item.Size);
                packetStream.WriteString(item.LastAccessTimeUtc.ToString());
                packetStream.WriteShort((short)item.ContentType);
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
