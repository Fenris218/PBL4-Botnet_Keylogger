using Common.Enums;
using Common.Models;
using Server.Networking.Packets.TaskManager;

namespace Server.Networking.Handlers
{
    public class TaskManagerHandler : IDisposable
    {
        #region Event
        protected readonly SynchronizationContext SynchronizationContext;
        public delegate void TaskManagerEventHandler(object sender, List<Process> processes);
        public event TaskManagerEventHandler TaskManagerEvent;
        private void OnTaskManagerEvent(List<Process> processes)
        {
            SynchronizationContext.Post(d =>
            {
                TaskManagerEvent?.Invoke(this, (List<Process>)d);
            }, processes);
        }

        public delegate void ProcessActionPerformedEventHandler(object sender, ProcessAction action, bool result);
        public event ProcessActionPerformedEventHandler ProcessActionPerformed;
        private void OnProcessActionPerformed(ProcessAction action, bool value)
        {
            SynchronizationContext.Post(d =>
            {
                ProcessActionPerformed?.Invoke(this, action, (bool)d);
            }, value);
        }
        #endregion

        private readonly Client _client;

        public TaskManagerHandler(Client client)
        {
            SynchronizationContext = new SynchronizationContext();
            _client = client;
        }

        public void GetProcesses()
        {
            _ = _client.QueuePacketAsync(new GetProcessesPacket());
        }

        public void StartProcess(string remotePath)
        {
            _ = _client.QueuePacketAsync(new ProcessActionPacket { ProcessAction = ProcessAction.Start, FilePath = remotePath });
        }

        public void EndProcess(int pid)
        {
            _ = _client.QueuePacketAsync(new ProcessActionPacket { ProcessAction = ProcessAction.End, Pid = pid });
        }

        public void Handler(GetProcessesPacket packet)
        {
            OnTaskManagerEvent(packet.Processes);
        }

        public void Handler(ProcessActionPacket packet)
        {
            OnProcessActionPerformed(packet.ProcessAction, packet.Result);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
