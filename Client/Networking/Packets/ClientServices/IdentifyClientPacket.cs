using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;

namespace Client.Networking.Packets.ClientServices
{
    public class IdentifyClientPacket : RequestPacket
    {
        public override int Id => 0x01;
        public string OperatingSystem { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PcName { get; set; } = string.Empty;
        public string HardwareId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;

        public override void Serialize(ProcessStream stream)
        {
            using var packetStream = new ProcessStream();

            packetStream.WriteString(OperatingSystem);
            packetStream.WriteString(AccountType);
            packetStream.WriteString(Country);
            packetStream.WriteString(CountryCode);
            packetStream.WriteString(Username);
            packetStream.WriteString(PcName);
            packetStream.WriteString(HardwareId);
            packetStream.WriteString(IpAddress);

            stream.Lock.Wait();
            stream.WriteInt(4 + (int)packetStream.Length);
            stream.WriteInt(Id);
            packetStream.Position = 0;
            packetStream.CopyTo(stream);
            stream.Lock.Release();
        }
    }
}
