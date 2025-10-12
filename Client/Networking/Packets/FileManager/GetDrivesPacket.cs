using Common.Models;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.FileManager
{
    public class GetDrivesPacket : RequestPacket
    {
        public override int Id => 0x41;
        public List<Drive> Drives { get; set; }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteInt(Drives.Count());
            foreach (var item in Drives)
            {
                packetStream.WriteString(item.DisplayName);
                packetStream.WriteString(item.RootDirectory);
            }

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
