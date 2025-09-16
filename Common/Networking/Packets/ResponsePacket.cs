namespace Common.Networking.Packets
{
    public abstract class ResponsePacket : IResponsePacket
    {
        public virtual int Id => throw new NotImplementedException();

        public void Populate(byte[] data)
        {
            using var stream = new ProcessStream(data);
            Populate(stream);
        }

        public abstract void Populate(ProcessStream stream);

        public static T Deserialize<T>(byte[] data) where T : ResponsePacket, new()
        {
            using var stream = new ProcessStream(data);
            return Deserialize<T>(stream);
        }

        public static T Deserialize<T>(ProcessStream stream) where T : ResponsePacket, new()
        {
            var packet = new T();
            packet.Populate(stream);
            return packet;
        }
    }
}
