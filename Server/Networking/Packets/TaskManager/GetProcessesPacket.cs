using Common.Models;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.TaskManager
{
    public class GetProcessesPacket : DuplexPacket
    {
        public override int Id => 0x30;
        public List<Process> Processes { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Processes = new();
            int length = stream.ReadInt();
            for (int i = 0; i < length; i++)
            {
                Processes.Add(new Process
                {
                    Name = stream.ReadString(),
                    Id = stream.ReadInt(),
                    MainWindowTitle = stream.ReadString()
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
