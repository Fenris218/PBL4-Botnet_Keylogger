using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.FileManager
{
    public class FileTransferRequest : RequestPacket//Gói tin yêu cầu truyền file
    {
        public override int Id => 0x45;
        public int IdRequest { get; set; }
        public string RemotePath { get; set; }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteInt(IdRequest);
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
