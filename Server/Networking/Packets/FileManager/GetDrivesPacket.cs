using Common.Models;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.FileManager
{
    public class GetDrivesPacket : DuplexPacket//Gói tin yêu cầu lấy danh sách ổ đĩa trên máy
    {
        public override int Id => 0x41;
        public List<Drive> Drives { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Drives = new();
            int length = stream.ReadInt();
            for (int i = 0; i < length; i++)
            {
                Drives.Add(new Drive
                {
                    DisplayName = stream.ReadString(),
                    RootDirectory = stream.ReadString()
                });
            }
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
