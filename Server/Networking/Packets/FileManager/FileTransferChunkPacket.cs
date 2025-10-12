using Common.Models;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.FileManager
{
    public class FileTransferChunkPacket : DuplexPacket//Gói tin chứa một phần (chunk) dữ liệu của file trong quá trình truyền.
    {
        public override int Id => 0x48;
        public int IdRequest { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public FileChunk Chunk { get; set; }

        public override void Populate(ProcessStream stream)
        {
            Chunk = new();

            IdRequest = stream.ReadInt();
            FilePath = stream.ReadString();
            FileSize = stream.ReadLong();
            Chunk.Offset = stream.ReadLong();
            Chunk.Data = stream.ReadByteArray();
        }
        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteInt(IdRequest);
            packetStream.WriteString(FilePath);
            packetStream.WriteLong(FileSize);
            packetStream.WriteLong(Chunk.Offset);
            packetStream.WriteByteArray(Chunk.Data);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
