using Common.Enums;
using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.TaskManager
{
    public class ProcessActionPacket : DuplexPacket
    {
        public override int Id => 0x31;
        public ProcessAction ProcessAction { get; private set; }
        public string FilePath { get; private set; }
        public int Pid { get; private set; }
        public bool Result { get; set; } = false;

        public override void Populate(ProcessStream stream)
        {
            ProcessAction = (ProcessAction)stream.ReadUnsignedByte();
            FilePath = stream.ReadString();
            Pid = stream.ReadInt();
        }

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteUnsignedByte((byte)ProcessAction);
            packetStream.WriteBoolean(Result);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
