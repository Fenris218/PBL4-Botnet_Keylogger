using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.FileManager
{
    public class PathDeletePacket : RequestPacket//Gói tin yêu cầu xóa file hoặc thư mục.
    {
        public override int Id => 0x44;
        public string Path { get; set; }
        public FileType PathType { get; set; }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteString(Path);
            packetStream.WriteUnsignedByte((byte)PathType);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
