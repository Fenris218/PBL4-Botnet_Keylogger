using Common.Enums;
using Common.Helper;
using Common.Models;
using Server.Extensions;
using Server.Helper;
using Server.Networking;
using Server.Networking.Handlers;
using Server.Utilities;
using System.Diagnostics;

namespace Server.Forms
{
    public partial class FrmFileManager : Form
    {
        private static readonly Dictionary<Client, FrmFileManager> OpenedForms = new Dictionary<Client, FrmFileManager>();

        public static FrmFileManager CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmFileManager f = new FrmFileManager(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        private readonly Client _connectClient;
        private readonly FileManagerHandler _fileManagerHandler;

        private string _currentDir;
        private enum TransferColumn
        {
            Id,
            Type,
            Status,
        }

        public FrmFileManager(Client client)
        {
            _connectClient = client;
            _fileManagerHandler = client.ClientHandler.FileManagerHandler;

            RegisterMessageHandler();
            InitializeComponent();
        }

        private void RegisterMessageHandler()
        {
            _connectClient.Disconnected += ClientDisconnected;
            _fileManagerHandler.StatusMessageChanged += SetStatusMessage;
            _fileManagerHandler.DrivesChanged += DrivesChanged;
            _fileManagerHandler.DirectoryChanged += DirectoryChanged;
            _fileManagerHandler.FileTransferUpdated += FileTransferUpdated;
        }

        private void UnregisterMessageHandler()
        {
            _connectClient.Disconnected -= ClientDisconnected;
            _fileManagerHandler.StatusMessageChanged -= SetStatusMessage;
            _fileManagerHandler.DrivesChanged -= DrivesChanged;
            _fileManagerHandler.DirectoryChanged -= DirectoryChanged;
            _fileManagerHandler.FileTransferUpdated -= FileTransferUpdated;
        }

        private void ClientDisconnected(Client client)
        {
            this.Invoke((MethodInvoker)this.Close);
        }

        private void SetStatusMessage(object sender, string message)
        {
            stripLblStatus.Text = $"Status: {message}";
        }

        private void DrivesChanged(object sender, List<Drive> drives)
        {
            cmbDrives.Invoke(() =>
            {
                cmbDrives.Items.Clear();
                cmbDrives.DisplayMember = "DisplayName";
                cmbDrives.ValueMember = "RootDirectory";
                cmbDrives.DataSource = new BindingSource(drives, null);
            });

            SetStatusMessage(this, "Ready");
        }

        private void DirectoryChanged(object sender, string remotePath, List<FileSystemEntry> items)
        {
            txtPath.Invoke(() =>
            {
                txtPath.Text = remotePath;
                _currentDir = remotePath;
            });

            lstDirectory.Invoke(() =>
            {
                lstDirectory.Items.Clear();
            });

            AddItemToFileBrowser("..", 0, FileType.Back);
            foreach (var item in items)
            {
                switch (item.EntryType)
                {
                    case FileType.Directory:
                        AddItemToFileBrowser(item.Name, 0, item.EntryType);
                        break;
                    case FileType.File:
                        AddItemToFileBrowser(item.Name, item.Size, item.EntryType);
                        break;
                }
            }

            SetStatusMessage(this, "Ready");
        }

        private void FileTransferUpdated(object sender, FileTransfer transfer)
        {
            lstTransfers.Invoke(() =>
            {
                for (var i = 0; i < lstTransfers.Items.Count; i++)
                {
                    if (lstTransfers.Items[i].SubItems[(int)TransferColumn.Id].Text == transfer.Id.ToString())
                    {
                        lstTransfers.Items[i].SubItems[(int)TransferColumn.Status].Text = transfer.Status;
                        lstTransfers.Items[i].ImageIndex = GetTransferImageIndex(transfer.Status);
                        return;
                    }
                }

                var lvi = new ListViewItem(new[]
                        {transfer.Id.ToString(), transfer.Type.ToString(), transfer.Status, transfer.RemotePath})
                { Tag = transfer, ImageIndex = GetTransferImageIndex(transfer.Status) };

                lstTransfers.Items.Add(lvi);
            });
        }

        private int GetTransferImageIndex(string status)
        {
            int imageIndex = -1;
            switch (status)
            {
                case "Completed":
                    imageIndex = 1;
                    break;
                case "Canceled":
                    imageIndex = 0;
                    break;
            }

            return imageIndex;
        }

        private string GetAbsolutePath(string path)
        {
            if (!string.IsNullOrEmpty(_currentDir) && _currentDir[0] == '/') // support forward slashes
            {
                if (_currentDir.Length == 1)
                    return Path.Combine(_currentDir, path);
                else
                    return Path.Combine(_currentDir + '/', path);
            }

            return Path.GetFullPath(Path.Combine(_currentDir, path));
        }

        private string NavigateUp()
        {
            if (!string.IsNullOrEmpty(_currentDir) && _currentDir[0] == '/') // support forward slashes
            {
                if (_currentDir.LastIndexOf('/') > 0)
                {
                    _currentDir = _currentDir.Remove(_currentDir.LastIndexOf('/') + 1);
                    _currentDir = _currentDir.TrimEnd('/');
                }
                else
                    _currentDir = "/";

                return _currentDir;
            }
            else
                return GetAbsolutePath(@"..\");
        }

        private void AddItemToFileBrowser(string name, long size, FileType type)
        {
            lstDirectory.Invoke(() =>
            {
                ListViewItem lvi = new ListViewItem(new string[]
                {
                name,
                (type == FileType.File) ? StringHelper.GetHumanReadableFileSize(size) : string.Empty,
                (type != FileType.Back) ? type.ToString() : string.Empty
                })
                {
                    Tag = new FileManagerListTag(type, size)
                };

                lstDirectory.Items.Add(lvi);
            });
        }

        private void RefreshDirectory()
        {
            SwitchDirectory(_currentDir);
        }

        private void SwitchDirectory(string remotePath)
        {
            _fileManagerHandler.GetDirectoryContents(remotePath);
            SetStatusMessage(this, "Loading directory content...");
        }

        private void FrmFileManager_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("File Manager", _connectClient);
            _fileManagerHandler.RefreshDrives();
        }

        private void FrmFileManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
        }

        private void cmbDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchDirectory(cmbDrives.SelectedValue.ToString());
        }

        private void lstDirectory_DoubleClick(object sender, EventArgs e)
        {
            if (lstDirectory.SelectedItems.Count > 0)
            {
                FileManagerListTag tag = (FileManagerListTag)lstDirectory.SelectedItems[0].Tag;

                switch (tag.Type)
                {
                    case FileType.Back:
                        SwitchDirectory(NavigateUp());
                        break;
                    case FileType.Directory:
                        SwitchDirectory(GetAbsolutePath(lstDirectory.SelectedItems[0].SubItems[0].Text));
                        break;
                }
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                FileManagerListTag tag = (FileManagerListTag)files.Tag;

                if (tag.Type == FileType.File)
                {
                    string remotePath = GetAbsolutePath(files.SubItems[0].Text);

                    _fileManagerHandler.BeginDownloadFile(remotePath);
                }
            }
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select files to upload";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var localFilePath in ofd.FileNames)
                    {
                        if (!File.Exists(localFilePath)) continue;

                        string remotePath = GetAbsolutePath(Path.GetFileName(localFilePath));

                        _fileManagerHandler.BeginUploadFile(localFilePath, remotePath);
                    }
                }
            }
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                FileManagerListTag tag = (FileManagerListTag)files.Tag;

                if (tag.Type == FileType.File)
                {
                    string remotePath = GetAbsolutePath(files.SubItems[0].Text);

                    _fileManagerHandler.StartProcess(remotePath);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                FileManagerListTag tag = (FileManagerListTag)files.Tag;

                switch (tag.Type)
                {
                    case FileType.Directory:
                    case FileType.File:
                        string path = GetAbsolutePath(files.SubItems[0].Text);
                        string newName = files.SubItems[0].Text;

                        if (InputBox.Show("New name", "Enter new name:", ref newName) == DialogResult.OK)
                        {
                            newName = GetAbsolutePath(newName);
                            _fileManagerHandler.RenameFile(path, newName, tag.Type);
                        }
                        break;
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = lstDirectory.SelectedItems.Count;
            if (count == 0) return;
            if (MessageBox.Show(string.Format("Are you sure you want to delete {0} file(s)?", count),
                "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (ListViewItem files in lstDirectory.SelectedItems)
                {
                    FileManagerListTag tag = (FileManagerListTag)files.Tag;

                    switch (tag.Type)
                    {
                        case FileType.Directory:
                        case FileType.File:
                            string path = GetAbsolutePath(files.SubItems[0].Text);
                            _fileManagerHandler.DeleteFile(path, tag.Type);
                            break;
                    }
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDirectory();
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = _currentDir;
            if (lstDirectory.SelectedItems.Count == 1)
            {
                var item = lstDirectory.SelectedItems[0];
                FileManagerListTag tag = (FileManagerListTag)item.Tag;

                if (tag.Type == FileType.Directory)
                {
                    path = GetAbsolutePath(item.SubItems[0].Text);
                }
            }

            FrmRemoteShell frmRs = FrmRemoteShell.CreateNewOrGetExisting(_connectClient);
            frmRs.Show();
            frmRs.Focus();
            var driveLetter = Path.GetPathRoot(path);
            frmRs._remoteShellHandler.SendCommand($"{driveLetter.Remove(driveLetter.Length - 1)} && cd \"{path}\"");
        }

        private void btnOpenDLFolder_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(_connectClient.Value.DownloadDirectory))
                Directory.CreateDirectory(_connectClient.Value.DownloadDirectory);

            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                Arguments = _connectClient.Value.DownloadDirectory,
                FileName = "explorer.exe"
            });
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.SelectedItems)
            {
                if (!transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Downloading") &&
                    !transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Uploading") &&
                    !transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Pending")) continue;

                int id = int.Parse(transfer.SubItems[(int)TransferColumn.Id].Text);

                _fileManagerHandler.CancelFileTransfer(id);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.Items)
            {
                if (transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Downloading") ||
                    transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Uploading") ||
                    transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Pending")) continue;
                transfer.Remove();
            }
        }

        private void lstDirectory_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) // allow drag & drop with files
                e.Effect = DragDropEffects.Copy;
        }

        private void lstDirectory_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string localFilePath in files)
                {
                    if (!File.Exists(localFilePath)) continue;

                    string remotePath = GetAbsolutePath(Path.GetFileName(localFilePath));

                    _fileManagerHandler.BeginUploadFile(localFilePath, remotePath);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDirectory();
        }

        private void FrmFileManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && !string.IsNullOrEmpty(_currentDir) && TabControlFileManager.SelectedIndex == 0)
            {
                RefreshDirectory();
                e.Handled = true;
            }
        }
    }
}
