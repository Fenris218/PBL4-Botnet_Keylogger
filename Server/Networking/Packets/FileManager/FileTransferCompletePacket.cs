using Common.Networking;
using Common.Networking.Packets;

namespace Server.Networking.Packets.FileManager
{
    public class FileTransferCompletePacket : ResponsePacket//Gói tin báo truyền file xong.
    {
        public override int Id => 0x46;
        public int IdRequest { get; private set; }
        public string FilePath { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            IdRequest = stream.ReadInt();
            FilePath = stream.ReadString();
        }
    }
}
