using Common.Networking;
using Common.Networking.Packets;

namespace Client.Networking.Packets.RemoteDesktop
{
    public class KeyboardEventPacket : ResponsePacket
    {
        public override int Id => 0x73;
        public byte Key { get; set; }
        public bool KeyDown { get; set; }

        public override void Populate(ProcessStream stream)
        {
            Key = (byte)stream.ReadByte();
            KeyDown = stream.ReadBoolean();
        }
    }
}
