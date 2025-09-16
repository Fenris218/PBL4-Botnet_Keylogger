using Common.Networking;
using Common.Networking.Packets;
using Server.Utilities;

namespace Server.Networking.Packets.ClientServices
{
    public class IdentifyClientPacket : ResponsePacket
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

        public override void Populate(ProcessStream stream)
        {
            OperatingSystem = stream.ReadString();
            AccountType = stream.ReadString();
            Country = stream.ReadString();
            CountryCode = stream.ReadString();
            Username = stream.ReadString();
            PcName = stream.ReadString();
            HardwareId = stream.ReadString();
            IpAddress = stream.ReadString();
        }

        public UserState GetUserState()
        {
            return new UserState
            {
                AccountType = AccountType,
                Country = Country,
                CountryCode = CountryCode,
                HardwareId = HardwareId,
                OperatingSystem = OperatingSystem,
                PcName = PcName,
                Username = Username,
                IpAddress = IpAddress
            };
        }
    }
}
