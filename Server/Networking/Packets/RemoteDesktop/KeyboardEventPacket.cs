using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.RemoteDesktop
{
    public class KeyboardEventPacket : RequestPacket
    {
        public override int Id => 0x73;
        public byte Key { get; set; }
        public bool KeyDown { get; set; }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteByte(Key);
            packetStream.WriteBoolean(KeyDown);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
