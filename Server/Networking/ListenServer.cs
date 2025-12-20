using Common.Enums;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Server.Utilities;
using System.Net;

namespace Server.Networking
{
    /// <summary>
    /// Server lắng nghe và chấp nhận kết nối TCP/IP từ các Client
    /// Server listens and accepts TCP/IP connections from Clients
    /// </summary>
    public class ListenServer
    {
        public int Port { get; private set; } //cổng sever lắng nghe
        public bool Listening { get; private set; }
        public List<Client> ConnectedClients //danh sách client đã indentified
        {
            get { return _clients.Where(c => c != null && c.Identified).ToList(); }
        }
        private readonly CancellationTokenSource _cancelTokenSource = new();
        private IConnectionListener? _tcpListener; //TCP Listener - lắng nghe kết nối TCP/IP
        private readonly ConcurrentHashSet<Client> _clients = new();// tập các clinet đang trong danh sách

        #region Event
        public delegate void ClientConnectedEventHandler(Client client);
        public event ClientConnectedEventHandler ClientConnected;
        public void OnClientConnected(Client client)
        {
            if (_cancelTokenSource.IsCancellationRequested || !Listening) return;
            var handler = ClientConnected;
            handler?.Invoke(client);
        }

        public delegate void ClientDisconnectedEventHandler(Client client);
        public event ClientDisconnectedEventHandler ClientDisconnected;
        public void OnClientDisconnected(Client client) // ngắt kết nối client chỉ định ( gọi event ClientDisconnected )
        {
            if (_cancelTokenSource.IsCancellationRequested || !Listening) return;
            var handler = ClientDisconnected;
            handler?.Invoke(client);
        }
        public delegate void StatusUpdatedEventHandler(object sender, Client client, string statusMessage);
        public event StatusUpdatedEventHandler StatusUpdated;
        public void OnStatusUpdated(Client client, string statusMessage)
        {
            StatusUpdated?.Invoke(this, client, statusMessage);
        }

        public delegate void UserStatusUpdatedEventHandler(object sender, Client client, UserStatus userStatusMessage);
        public event UserStatusUpdatedEventHandler UserStatusUpdated;
        public void OnUserStatusUpdated(Client client, UserStatus userStatusMessage)
        {
            UserStatusUpdated?.Invoke(this, client, userStatusMessage);
        }
        #endregion

        public ListenServer(int port)
        {
            Port = port;
        }

        public async Task RunAsync()
        {
            // Tạo TCP Listener để lắng nghe kết nối TCP/IP từ client
            // Create TCP Listener to listen for TCP/IP connections from clients
            _tcpListener = await CreateListenerAsync(new IPEndPoint(IPAddress.Any, Port), token: _cancelTokenSource.Token);//Tạo TCP Listener
            Listening = true;
            while (!_cancelTokenSource.Token.IsCancellationRequested)//Vòng lặp chấp nhận kết nối
            {
                ConnectionContext connection;
                try
                {
                    var acceptedConnection = await _tcpListener.AcceptAsync(_cancelTokenSource.Token);
                    if (acceptedConnection is null)
                    {
                        break;
                    }

                    connection = acceptedConnection;// chấp nhận kết nối mới
                }
                catch (Exception e)
                {
                    break;
                }

                var client = new Client(connection, this);// tạo client mới từ kết nối vừa chấp nhận

                _clients.Add(client);

                client.Disconnected += client =>
                {
                    _clients.TryRemove(client);
                    OnClientDisconnected(client);
                };
                _ = Task.Delay(new TimeSpan(0, 0, 15)).ContinueWith(o =>
                {
                    try
                    {
                        if (!client.Identified)
                        {
                            client.Disconnect();
                        }
                    }
                    catch (Exception)
                    {

                    }
                });
                _ = Task.Run(client.StartConnectionAsync);
            }

            Listening = false;
            await _tcpListener.UnbindAsync();
        }

        public async Task StopAsync()
        {
            _cancelTokenSource.Cancel();

            if (_tcpListener is not null)
            {
                await _tcpListener.UnbindAsync();
            }

            foreach (var client in _clients)
            {
                client.Disconnect();
                client.Dispose();
            }
        }

        //Tạo TCP Listener sử dụng Kestrel Transport (ASP.NET Core) cho kết nối TCP/IP
        //Create TCP Listener using Kestrel Transport (ASP.NET Core) for TCP/IP connections
        public async Task<IConnectionListener> CreateListenerAsync(IPEndPoint endPoint, SocketTransportOptions? options = null, ILoggerFactory? loggerFactory = null, CancellationToken token = default)
        {
            options ??= new SocketTransportOptions();
            loggerFactory ??= NullLoggerFactory.Instance;

            var factory = new SocketTransportFactory(Options.Create(options), loggerFactory);
            return await factory.BindAsync(endPoint, token);
        }
    }
}
