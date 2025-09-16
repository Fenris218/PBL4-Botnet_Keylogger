namespace Common.Networking.Packets
{
    public abstract class DuplexPacket : ResponsePacket, IRequestPacket, IResponsePacket
    {
        public abstract void Serialize(ProcessStream stream);
    }
}
