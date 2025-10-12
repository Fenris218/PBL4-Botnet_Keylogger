using Common.Networking;
using Common.Networking.Packets;

namespace Server.Networking.Packets.FileManager
{
    public class StatusFileManager : ResponsePacket//Có thể chứa trạng thái chung của File Manager, như tiến độ, kết nối, hoặc xử lý phản hồi từ server.
    {
        public override int Id => 0x40;
        public string Message { get; private set; }
        public bool SetLastDirectorySeen { get; private set; }

        public override void Populate(ProcessStream stream)
        {
            Message = stream.ReadString();
            SetLastDirectorySeen = stream.ReadBoolean();
        }
    }
}
