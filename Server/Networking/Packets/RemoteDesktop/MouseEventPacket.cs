using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.RemoteDesktop
{
    public class MouseEventPacket : RequestPacket
    {
        public override int Id => 0x72;
        public MouseAction Action { get; set; }
        public bool IsMouseDown { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int MonitorIndex { get; set; }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteShort((short)Action);
            packetStream.WriteBoolean(IsMouseDown);
            packetStream.WriteInt(X);
            packetStream.WriteInt(Y);
            packetStream.WriteInt(MonitorIndex);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
