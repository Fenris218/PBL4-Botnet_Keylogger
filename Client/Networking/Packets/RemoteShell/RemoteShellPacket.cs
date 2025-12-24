using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.RemoteShell
{
    public class RemoteShellPacket : DuplexPacket
    {
        public override int Id => 0x20;
        public string Command { get; private set; }
        public string Output { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;

        public override void Populate(ProcessStream stream)
        {
            Command = stream.ReadString();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteBoolean(IsError);
            packetStream.WriteString(Output);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
