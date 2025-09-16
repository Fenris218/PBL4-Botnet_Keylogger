using Common.Helper;
using Common.Utilities;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Client.IO
{
    public static class HardwareDevices
    {
        public static string HardwareId => _hardwareId ?? (_hardwareId = Sha256.ComputeHash(CpuName + MainboardName + BiosManufacturer));
        private static string _hardwareId;

        public static string CpuName => _cpuName ?? (_cpuName = GetCpuName());
        private static string _cpuName;

        public static string GpuName => _gpuName ?? (_gpuName = GetGpuName());
        private static string _gpuName;

        public static string BiosManufacturer => _biosManufacturer ?? (_biosManufacturer = GetBiosManufacturer());
        private static string _biosManufacturer;

        public static string MainboardName => _mainboardName ?? (_mainboardName = GetMainboardName());
        private static string _mainboardName;

        public static int? TotalPhysicalMemory => _totalPhysicalMemory ?? (_totalPhysicalMemory = GetTotalPhysicalMemoryInMb());
        private static int? _totalPhysicalMemory;

        public static string LanIpAddress => GetLanIpAddress();
        public static string MacAddress => GetMacAddress();

        private static string GetBiosManufacturer()
        {
            try
            {
                string biosIdentifier = string.Empty;
                string query = "SELECT * FROM Win32_BIOS";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        biosIdentifier = mObject["Manufacturer"].ToString();
                        break;
                    }
                }

                return (!string.IsNullOrEmpty(biosIdentifier)) ? biosIdentifier : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        private static string GetMainboardName()
        {
            try
            {
                string mainboardIdentifier = string.Empty;
                string query = "SELECT * FROM Win32_BaseBoard";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        mainboardIdentifier = mObject["Manufacturer"].ToString() + " " + mObject["Product"].ToString();
                        break;
                    }
                }

                return (!string.IsNullOrEmpty(mainboardIdentifier)) ? mainboardIdentifier : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        private static string GetCpuName()
        {
            try
            {
                string cpuName = string.Empty;
                string query = "SELECT * FROM Win32_Processor";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        cpuName += mObject["Name"].ToString() + "; ";
                    }
                }
                cpuName = StringHelper.RemoveLastChars(cpuName);

                return (!string.IsNullOrEmpty(cpuName)) ? cpuName : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        private static int GetTotalPhysicalMemoryInMb()
        {
            try
            {
                int installedRAM = 0;
                string query = "Select * From Win32_ComputerSystem";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        double bytes = (Convert.ToDouble(mObject["TotalPhysicalMemory"]));
                        installedRAM = (int)(bytes / 1048576); // bytes to MB
                        break;
                    }
                }

                return installedRAM;
            }
            catch
            {
                return -1;
            }
        }

        private static string GetGpuName()
        {
            try
            {
                string gpuName = string.Empty;
                string query = "SELECT * FROM Win32_DisplayConfiguration";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        gpuName += mObject["Description"].ToString() + "; ";
                    }
                }
                gpuName = StringHelper.RemoveLastChars(gpuName);

                return (!string.IsNullOrEmpty(gpuName)) ? gpuName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static string GetLanIpAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                GatewayIPAddressInformation gatewayAddress = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (gatewayAddress != null)
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                                ip.AddressPreferredLifetime == UInt32.MaxValue)
                                continue;

                            return ip.Address.ToString();
                        }
                    }
                }
            }

            return "-";
        }

        private static string GetMacAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    ni.OperationalStatus == OperationalStatus.Up)
                {
                    bool foundCorrect = false;
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                            ip.AddressPreferredLifetime == UInt32.MaxValue) // exclude virtual network addresses
                            continue;

                        foundCorrect = (ip.Address.ToString() == GetLanIpAddress());
                    }

                    if (foundCorrect)
                        return StringHelper.GetFormattedMacAddress(ni.GetPhysicalAddress().ToString());
                }
            }

            return "-";
        }
    }
}
