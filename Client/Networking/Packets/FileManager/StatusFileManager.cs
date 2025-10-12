using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.FileManager
{
    public class StatusFileManager : RequestPacket
    {
        public override int Id => 0x40;
        public string Message { get; set; } = string.Empty;
        public bool SetLastDirectorySeen { get; set; } = false;

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteString(Message);
            packetStream.WriteBoolean(SetLastDirectorySeen);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
