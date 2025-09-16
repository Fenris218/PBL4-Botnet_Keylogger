namespace Common.Networking
{
    public interface IRequestPacket : IPacket
    {
        public void Serialize(ProcessStream stream); // Chuyển đổi dữ liệu thành dạng byte và ghi vào stream
    }
}
