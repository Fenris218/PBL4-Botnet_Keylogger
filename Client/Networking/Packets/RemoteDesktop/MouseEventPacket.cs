using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;

namespace Client.Networking.Packets.RemoteDesktop
{
    public class MouseEventPacket : ResponsePacket
    {
        public override int Id => 0x72;
        public MouseAction Action { get; private set; }
        public bool IsMouseDown { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int MonitorIndex { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Action = (MouseAction)stream.ReadShort();
            IsMouseDown = stream.ReadBoolean();
            X = stream.ReadInt();
            Y = stream.ReadInt();
            MonitorIndex = stream.ReadInt();
        }
    }
}
