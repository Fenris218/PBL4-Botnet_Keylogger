using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;

namespace Client.Networking.Packets.FileManager
{
    public class PathRenamePacket : ResponsePacket
    {
        public override int Id => 0x43;
        public string Path { get; private set; }
        public string NewPath { get; private set; }
        public FileType PathType { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Path = stream.ReadString();
            NewPath = stream.ReadString();
            PathType = (FileType)stream.ReadUnsignedByte();
        }
    }
}
