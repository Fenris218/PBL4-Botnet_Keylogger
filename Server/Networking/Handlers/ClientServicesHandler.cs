using Server.Networking.Packets.ClientServices;

namespace Server.Networking.Handlers
{
    public class ClientServicesHandler : IDisposable
    {
        private readonly Client _client;

        public ClientServicesHandler(Client client)
        {
            _client = client;
        }

        public void Handler(IdentifyClientPacket packet)
        {
            _client.Value = packet.GetUserState(); // lấy thông tin user từ gói tin
            _client.Identified = true;
            _client.server.OnClientConnected(_client); // thông báo sự kiện client đã kết nối
            _client.ClientHandler.Init();
        }

        public void Handler(StatusClientPacket packet)
        {
            _client.server.OnStatusUpdated(_client, packet.Message);
        }

        public void Handler(UserStatusClientPacket packet)
        {
            _client.server.OnUserStatusUpdated(_client, packet.Message);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
