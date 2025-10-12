using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;

namespace Client.Networking.Packets.FileManager
{
    public class PathDeletePacket : ResponsePacket
    {
        public override int Id => 0x44;
        public string Path { get; private set; }
        public FileType PathType { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Path = stream.ReadString();
            PathType = (FileType)stream.ReadUnsignedByte();
        }
    }
}
