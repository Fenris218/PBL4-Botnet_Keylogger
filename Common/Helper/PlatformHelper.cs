using System.Management;
using System.Text.RegularExpressions;

namespace Common.Helper
{
    public static class PlatformHelper
    {
        static PlatformHelper()
        {
            Win32NT = Environment.OSVersion.Platform == PlatformID.Win32NT;
            XpOrHigher = Win32NT && Environment.OSVersion.Version.Major >= 5;
            VistaOrHigher = Win32NT && Environment.OSVersion.Version.Major >= 6;
            SevenOrHigher = Win32NT && Environment.OSVersion.Version >= new Version(6, 1);
            EightOrHigher = Win32NT && Environment.OSVersion.Version >= new Version(6, 2, 9200);
            EightPointOneOrHigher = Win32NT && Environment.OSVersion.Version >= new Version(6, 3);
            TenOrHigher = Win32NT && Environment.OSVersion.Version >= new Version(10, 0);
            RunningOnMono = Type.GetType("Mono.Runtime") != null;

            Name = "Unknown OS";
            using (var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject os in searcher.Get())
                {
                    Name = os["Caption"].ToString();
                    break;
                }
            }

            Name = Regex.Replace(Name, "^.*(?=Windows)", "").TrimEnd().TrimStart();
            Is64Bit = Environment.Is64BitOperatingSystem;
            FullName = $"{Name} {(Is64Bit ? 64 : 32)} Bit";
        }

        public static string FullName { get; }
        public static string Name { get; }
        public static bool Is64Bit { get; }
        public static bool RunningOnMono { get; }
        public static bool Win32NT { get; }
        public static bool XpOrHigher { get; }
        public static bool VistaOrHigher { get; }
        public static bool SevenOrHigher { get; }
        public static bool EightOrHigher { get; }
        public static bool EightPointOneOrHigher { get; }
        public static bool TenOrHigher { get; }

        private const int ProcessorCountRefreshIntervalMs = 30000;

        private static volatile int _processorCount;
        private static volatile int _lastProcessorCountRefreshTicks;

        public static int ProcessorCount
        {
            get
            {
                var now = Environment.TickCount;
                if (_processorCount == 0 || now - _lastProcessorCountRefreshTicks >= ProcessorCountRefreshIntervalMs)
                {
                    _processorCount = Environment.ProcessorCount;
                    _lastProcessorCountRefreshTicks = now;
                }

                return _processorCount;
            }
        }
    }
}
