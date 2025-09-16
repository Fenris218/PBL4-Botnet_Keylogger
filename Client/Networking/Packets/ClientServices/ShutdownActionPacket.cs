using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;

namespace Client.Networking.Packets.ClientServices
{
    public class ShutdownActionPacket : ResponsePacket
    {
        public override int Id => 0x07;
        public ShutdownAction Action { get; set; }

        public override void Populate(ProcessStream stream)
        {
            Action = (ShutdownAction)stream.ReadUnsignedByte();
        }
    }
}
