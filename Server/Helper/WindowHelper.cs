using Server.Networking;

namespace Server.Helper
{
    public static class WindowHelper
    {
        // "Remote Desktop - hoang@DESKTOP-PC [192.168.1.10:8080]"
        public static string GetWindowTitle(string title, Client c)
        {
            return string.Format("{0} - {1}@{2} [{3}:{4}]", title, c.Value.Username, c.Value.PcName, c.EndPoint.Address.ToString(), c.EndPoint.Port.ToString());
        }
        // "Clients [Selected: 3]"
        public static string GetWindowTitle(string title, int count)
        {
            return string.Format("{0} [Selected: {1}]", title, count);
        }
    }
}
