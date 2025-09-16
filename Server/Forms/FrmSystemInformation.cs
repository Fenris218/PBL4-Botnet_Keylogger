using Server.Extensions;
using Server.Helper;
using Server.Networking;
using Server.Networking.Handlers;
using System.Data;

namespace Server.Forms
{
    public partial class FrmSystemInformation : Form
    {
        private static readonly Dictionary<Client, FrmSystemInformation> OpenedForms = new Dictionary<Client, FrmSystemInformation>();

        public static FrmSystemInformation CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmSystemInformation f = new FrmSystemInformation(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        private readonly Client _connectClient;
        private readonly SystemInformationHandler _sysInfoHandler;

        public FrmSystemInformation(Client client)
        {
            _connectClient = client;
            _sysInfoHandler = client.ClientHandler.SystemInformationHandler;

            RegisterMessageHandler();
            InitializeComponent();
        }

        private void RegisterMessageHandler()
        {
            _connectClient.Disconnected += ClientDisconnected;
            _sysInfoHandler.SystemInformationEvent += SystemInformationChanged;
        }

        private void UnregisterMessageHandler()
        {
            _connectClient.Disconnected -= ClientDisconnected;
            _sysInfoHandler.SystemInformationEvent -= SystemInformationChanged;
        }

        private void ClientDisconnected(Client client)
        {
            this.Invoke((MethodInvoker)this.Close);
        }

        //Event SystemInformationEvent bắn → gọi SystemInformationChanged → update lại ListView bằng thông tin mới.
        private void SystemInformationChanged(object sender, List<Tuple<string, string>> infos)
        {
            lstSystem.Invoke(() =>
            {
                lstSystem.Items.Clear();

                foreach (var info in infos)
                {
                    lstSystem.Items.Add(new ListViewItem(new[] { info.Item1, info.Item2 }));
                }

                lstSystem.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            });
        }

        private void FrmSystemInformation_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("System Information", _connectClient);
            _sysInfoHandler.RefreshSystemInformation();//Gửi yêu cầu tới server (RefreshSystemInformation) để lấy dữ liệu hệ thống.
            AddBasicSystemInformation();
        }

        private void FrmSystemInformation_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSystem.Items.Count == 0) return;

            string output = string.Empty;

            foreach (ListViewItem lvi in lstSystem.Items)
            {
                output = lvi.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate(output, (current, lvs) => current + (lvs.Text + " : "));
                output = output.Remove(output.Length - 3);
                output = output + "\r\n";
            }

            try
            {
                Clipboard.SetText(output);
            }
            catch (Exception)
            {
            }
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSystem.SelectedItems.Count == 0) return;

            string output = string.Empty;

            foreach (ListViewItem lvi in lstSystem.SelectedItems)
            {
                output = lvi.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate(output, (current, lvs) => current + (lvs.Text + " : "));
                output = output.Remove(output.Length - 3);
                output = output + "\r\n";
            }

            try
            {
                Clipboard.SetText(output);
            }
            catch (Exception)
            {
            }
        }

        
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstSystem.Items.Clear();
            _sysInfoHandler.RefreshSystemInformation();
            AddBasicSystemInformation();
        }

        private void AddBasicSystemInformation()
        {
            ListViewItem lvi = new ListViewItem(new[] { "", "Loading Data..." });
            lstSystem.Items.Add(lvi);
        }
    }
}
