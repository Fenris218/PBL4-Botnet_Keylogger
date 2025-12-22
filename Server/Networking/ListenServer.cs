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
    public class ListenServer
    {
        public int Port { get; private set; } //cổng sever lắng nghe
        public bool Listening { get; private set; }
        public List<Client> ConnectedClients //danh sách client đã indentified
        {
            get { return _clients.Where(c => c != null && c.Identified).ToList(); }
        }
        private readonly CancellationTokenSource _cancelTokenSource = new();
        private IConnectionListener? _tcpListener; //object thật sự lắng nghe kết nối TCP
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
            _tcpListener = await CreateListenerAsync(new IPEndPoint(IPAddress.Any, Port), token: _cancelTokenSource.Token);//Tạo TCP Listener
            Listening = true;
            
            // Vòng lặp chính: Chấp nhận kết nối tuần tự, nhưng xử lý đồng thời
            // Mỗi kết nối được chấp nhận lần lượt, nhưng ngay sau đó được xử lý trong task riêng
            // Đây là điểm bắt đầu của kiến trúc đa luồng (multi-threaded)
            while (!_cancelTokenSource.Token.IsCancellationRequested)//Vòng lặp chấp nhận kết nối
            {
                ConnectionContext connection;
                try
                {
                    // Chờ và chấp nhận kết nối TCP mới (blocking call)
                    // Khi có client mới kết nối, vòng lặp tiếp tục xử lý và chấp nhận client tiếp theo
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

                // Thêm client vào danh sách thread-safe (ConcurrentHashSet)
                // Nhiều thread có thể thêm/xóa client đồng thời mà không gây race condition
                _clients.Add(client);

                client.Disconnected += client =>
                {
                    _clients.TryRemove(client);
                    OnClientDisconnected(client);
                };
                
                // Timeout mechanism: Tự động ngắt kết nối client nếu không identified trong 15 giây
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
                
                // *** ĐIỂM QUAN TRỌNG: Xử lý đa luồng (Multi-threading) ***
                // Task.Run tạo một task mới để xử lý client này
                // Mỗi client chạy trong task riêng biệt, KHÔNG chờ đợi (fire-and-forget)
                // Điều này cho phép vòng lặp chính tiếp tục chấp nhận client mới ngay lập tức
                // Kết quả: Server có thể xử lý NHIỀU client ĐỒNG THỜI (concurrent)
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

        //là một helper method để tạo ra một listener TCP dựa trên Kestrel Transport (của ASP.NET Core).
        public async Task<IConnectionListener> CreateListenerAsync(IPEndPoint endPoint, SocketTransportOptions? options = null, ILoggerFactory? loggerFactory = null, CancellationToken token = default)
        {
            options ??= new SocketTransportOptions();
            loggerFactory ??= NullLoggerFactory.Instance;

            var factory = new SocketTransportFactory(Options.Create(options), loggerFactory);
            return await factory.BindAsync(endPoint, token);
        }
    }
}
