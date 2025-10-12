using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Server.Networking.Packets.TaskManager
{
    public class ProcessActionPacket : DuplexPacket
    {
        public override int Id => 0x31;
        public ProcessAction ProcessAction { get; set; } = ProcessAction.Start;
        public string FilePath { get; set; } = string.Empty;
        public int Pid { get; set; } = 0;
        public bool Result { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            ProcessAction = (ProcessAction)stream.ReadUnsignedByte();
            Result = stream.ReadBoolean();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteUnsignedByte((byte)ProcessAction);
            packetStream.WriteString(FilePath);
            packetStream.WriteInt(Pid);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
