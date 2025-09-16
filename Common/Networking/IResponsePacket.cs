namespace Common.Networking
{
    public interface IResponsePacket
    {
        public void Populate(byte[] data);// khôi phục dữ liệu từ mảng byte

        public void Populate(ProcessStream stream);// khôi phục dữ liệu từ ProcessStream
    }
}
