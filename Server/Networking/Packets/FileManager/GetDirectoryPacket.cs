using Common.Models;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.FileManager
{
    public class GetDirectoryPacket : DuplexPacket//Gói tin yêu cầu lấy danh sách file/thư mục trong một đường dẫn.
    {
        public override int Id => 0x42;
        public string RemotePath { get; set; }
        public List<FileSystemEntry> Items { get; set; }

        public override void Populate(ProcessStream stream)
        {
            Items = new();

            RemotePath = stream.ReadString();

            int length = stream.ReadInt();
            for (int i = 0; i < length; i++)
            {
                Items.Add(new FileSystemEntry
                {
                    EntryType = (Common.Enums.FileType)stream.ReadUnsignedByte(),
                    Name = stream.ReadString(),
                    Size = stream.ReadLong(),
                    LastAccessTimeUtc = DateTime.Parse(stream.ReadString()),
                    ContentType = (Common.Enums.ContentType)stream.ReadShort()
                });
            }
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteString(RemotePath);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
