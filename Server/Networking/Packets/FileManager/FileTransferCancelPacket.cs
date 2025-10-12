using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.FileManager
{
    public class FileTransferCancelPacket : DuplexPacket//Gói tin để hủy quá trình truyền file đang diễn ra.
    {
        public override int Id => 0x47;
        public int IdRequest { get; set; }
        public string Reason { get; set; } = string.Empty;

        public override void Populate(ProcessStream stream)
        {
            IdRequest = stream.ReadInt();
            Reason = stream.ReadString();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteInt(IdRequest);
            packetStream.WriteString(Reason);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
