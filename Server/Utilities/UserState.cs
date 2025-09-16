using Common.Helper;

namespace Server.Utilities
{
    public class UserState
    {
        private string _downloadDirectory;

        public string OperatingSystem { get; set; }
        public string AccountType { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Username { get; set; }
        public string PcName { get; set; }
        public string UserAtPc => $"{Username}@{PcName}";
        public string CountryWithCode => $"{Country} [{CountryCode}]";
        public string HardwareId { get; set; }
        public string IpAddress { get; set; }

        public string DownloadDirectory => _downloadDirectory ?? (_downloadDirectory = (!FileHelper.HasIllegalCharacters(UserAtPc))
                                               ? Path.Combine(Application.StartupPath, $"Clients\\{UserAtPc}_{HardwareId.Substring(0, 7)}\\")
                                               : Path.Combine(Application.StartupPath, $"Clients\\{HardwareId}\\"));
    }
}
