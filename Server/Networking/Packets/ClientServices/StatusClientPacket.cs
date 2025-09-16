using Common.Networking;
using Common.Networking.Packets;

namespace Server.Networking.Packets.ClientServices
{
    public class StatusClientPacket : ResponsePacket
    {
        public override int Id => 0x02;
        public string Message { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Message = stream.ReadString();
        }
    }
}
