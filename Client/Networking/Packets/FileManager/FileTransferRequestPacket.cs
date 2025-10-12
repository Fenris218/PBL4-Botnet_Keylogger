using Common.Networking;
using Common.Networking.Packets;

namespace Client.Networking.Packets.FileManager
{
    public class FileTransferRequestPacket : ResponsePacket
    {
        public override int Id => 0x45;
        public int IdRequest { get; private set; }
        public string RemotePath { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            IdRequest = stream.ReadInt();
            RemotePath = stream.ReadString();
        }
    }
}
