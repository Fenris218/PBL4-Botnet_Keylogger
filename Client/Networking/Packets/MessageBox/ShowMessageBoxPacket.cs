using Common.Networking;
using Common.Networking.Packets;

namespace Client.Networking.Packets.MessageBox
{
    public class ShowMessageBoxPacket : ResponsePacket
    {
        public override int Id => 0x50;
        public string Caption { get; private set; }
        public string Text { get; private set; }
        public string Button { get; private set; }
        public string Icon { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Caption = stream.ReadString();
            Text = stream.ReadString();
            Button = stream.ReadString();
            Icon = stream.ReadString();
        }
    }
}
