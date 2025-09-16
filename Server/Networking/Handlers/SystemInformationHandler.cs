using Server.Networking.Packets.SystemInfo;

namespace Server.Networking.Handlers
{
    public class SystemInformationHandler : IDisposable
    {
        #region Event
        public delegate void SystemInformationEventHandler(object sender, List<Tuple<string, string>> value);
        public event SystemInformationEventHandler SystemInformationEvent;

        private void OnSystemInformationEvent(List<Tuple<string, string>> value)
        {
            // Gọi trực tiếp event, không cần SynchronizationContext
            SystemInformationEvent?.Invoke(this, value);
        }
        #endregion

        private readonly Client _client;

        public SystemInformationHandler(Client client)
        {
            _client = client;
        }

        public void RefreshSystemInformation()
        {
            _ = _client.QueuePacketAsync(new SystemInfoPacket());
        }

        public void Handler(SystemInfoPacket packet)
        {
            OnSystemInformationEvent(packet.SystemInfos);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
