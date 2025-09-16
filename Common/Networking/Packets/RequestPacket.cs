namespace Common.Networking.Packets
{
    public abstract class RequestPacket : IRequestPacket
    {
        public virtual int Id => throw new NotImplementedException();
        public abstract void Serialize(ProcessStream stream);
    }
}
