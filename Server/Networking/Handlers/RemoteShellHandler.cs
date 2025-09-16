using Server.Networking.Packets.RemoteShell;

namespace Server.Networking.Handlers
{
    public class RemoteShellHandler : IDisposable
    {
        #region Event
        protected readonly SynchronizationContext SynchronizationContext;
        public delegate void CommandSuccessEventHandler(object sender, string message);
        public event CommandSuccessEventHandler CommandSuccessEvent;
        private void OnCommandSuccessEvent(string message)
        {
            SynchronizationContext.Post(d =>
            {
                CommandSuccessEvent?.Invoke(this, (string)d);
            }, message);
        }

        public delegate void CommandErrorEventHandler(object sender, string message);
        public event CommandErrorEventHandler CommandErrorEvent;
        private void OnCommandErrorEvent(string message)
        {
            SynchronizationContext.Post(d =>
            {
                CommandErrorEvent?.Invoke(this, (string)d);
            }, message);
        }
        #endregion

        private readonly Client _client;

        public RemoteShellHandler(Client client)
        {
            SynchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _client = client;
        }

        public void SendCommand(string command)
        {
            _ = _client.QueuePacketAsync(new RemoteShellPacket { Command = command });
        }

        public void Handler(RemoteShellPacket packet)
        {
            if (packet.IsError)
                OnCommandErrorEvent(packet.Output);
            else
                OnCommandSuccessEvent(packet.Output);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
