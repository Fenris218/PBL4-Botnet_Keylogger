using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.RemoteShell
{
    public class RemoteShellPacket : DuplexPacket
    {
        public override int Id => 0x20;
        public string Command { get; set; } = string.Empty;
        public string Output { get; private set; }
        public bool IsError { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            IsError = stream.ReadBoolean();
            Output = stream.ReadString();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();
            packetStream.WriteString(Command);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
