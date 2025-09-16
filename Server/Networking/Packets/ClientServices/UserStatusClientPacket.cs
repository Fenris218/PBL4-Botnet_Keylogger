using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;

namespace Server.Networking.Packets.ClientServices
{
    public class UserStatusClientPacket : ResponsePacket
    {
        public override int Id => 0x03;
        public UserStatus Message { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Message = (UserStatus)stream.ReadUnsignedByte();
        }
    }
}
