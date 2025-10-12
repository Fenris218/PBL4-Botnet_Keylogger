using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.FileManager
{
    public class FileTransferCompletePacket : RequestPacket
    {
        public override int Id => 0x46;
        public int IdRequest { get; set; }
        public string FilePath { get; set; }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteInt(IdRequest);
            packetStream.WriteString(FilePath);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
