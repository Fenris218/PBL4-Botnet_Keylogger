using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.MessageBox
{
    public class ShowMessageBoxPacket : RequestPacket
    {
        public override int Id => 0x50;
        public string Caption { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Button { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteString(Caption);
            packetStream.WriteString(Text);
            packetStream.WriteString(Button);
            packetStream.WriteString(Icon);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
