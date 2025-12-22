using Common.Networking;
using Common.Networking.Packets;
using Common.Utilities;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Server.Utilities;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;

namespace Server.Networking
{
    public class Client : IDisposable
    {
        public bool Identified { get; set; }
        public IPEndPoint EndPoint { get; private set; }
        public UserState Value { get; set; }  // Thông tin trạng thái user
        public ClientHandler ClientHandler { get; private set; } // Quản lý xử lý gói tin từ client
        private readonly BufferBlock<IRequestPacket> packetQueue; // Hàng đợi để gửi packet ra client

        public static bool operator ==(Client c1, Client c2)
        {
            if (ReferenceEquals(c1, null))
                return ReferenceEquals(c2, null);

            return c1.Equals(c2);
        }

        public static bool operator !=(Client c1, Client c2)
        {
            return !(c1 == c2);
        }

        public bool Equals(Client other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            try
            {
                return EndPoint.Port.Equals(other.EndPoint.Port);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Client);
        }
        public override int GetHashCode()
        {
            return EndPoint.GetHashCode();
        }

        internal readonly ListenServer server;

        private bool disposed;
        private ProcessStream serverStream;
        private readonly DuplexPipeStream networkStream;
        private readonly ConnectionContext connectionContext;
        private readonly CancellationTokenSource cancellationSource = new();
        public event Action<Client>? Disconnected;

        public Client(ConnectionContext connectionContext, ListenServer server)
        {
            this.connectionContext = connectionContext;
            EndPoint = connectionContext.RemoteEndPoint as IPEndPoint;
            ClientHandler = new(this);
            networkStream = new(connectionContext.Transport);
            serverStream = new(networkStream);

            this.server = server;

            // *** Thiết lập hàng đợi gửi packet thread-safe ***
            // Sử dụng TPL Dataflow để đảm bảo packet gửi đi theo đúng thứ tự
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            
            // EnsureOrdered = true: Đảm bảo packet được xử lý theo thứ tự vào hàng đợi
            // Điều này quan trọng để tránh packet sau được gửi trước packet trước
            var blockOptions = new ExecutionDataflowBlockOptions { CancellationToken = cancellationSource.Token, EnsureOrdered = true };
            
            // ActionBlock: Xử lý các packet từ hàng đợi một cách tuần tự
            // Mỗi packet được gửi đi sau khi packet trước đã được gửi xong
            var sendPacketBlock = new ActionBlock<IRequestPacket>(packet =>
            {
                Thread.Sleep(2);
                if (!connectionContext.ConnectionClosed.IsCancellationRequested)
                    SendPacket(packet);
            }, blockOptions);

            // BufferBlock: Hàng đợi thread-safe để lưu trữ packet cần gửi
            // Nhiều thread có thể thêm packet vào hàng đợi đồng thời
            packetQueue = new BufferBlock<IRequestPacket>(blockOptions);
            _ = packetQueue.LinkTo(sendPacketBlock, linkOptions);
        }

        // *** Vòng lặp chính xử lý packet từ client ***
        // Mỗi client có task riêng chạy hàm này
        // Nhiều client = nhiều task chạy song song
        public async Task StartConnectionAsync()//Nếu còn kết nối thì đợi và xử lý gói tin
        {
            while (!cancellationSource.IsCancellationRequested && !connectionContext.ConnectionClosed.IsCancellationRequested)
            {
                try
                {
                    // Đọc packet tiếp theo từ network stream (blocking I/O)
                    // Trong khi chờ packet, task này bị block nhưng KHÔNG ảnh hưởng đến client khác
                    (var id, var data) = await GetNextPacketAsync();
                    
                    // *** ĐIỂM QUAN TRỌNG: Fire-and-forget packet handling ***
                    // Gọi HandlePackets KHÔNG chờ đợi (no await)
                    // Điều này có nghĩa:
                    // 1. Việc xử lý packet không làm chậm việc đọc packet tiếp theo
                    // 2. Các packet có thể được xử lý SONG SONG (concurrent)
                    // 3. Thứ tự xử lý KHÔNG được đảm bảo (packet sau có thể xử lý xong trước packet trước)
                    // 
                    // Giải pháp nếu cần đảm bảo thứ tự:
                    // - Option 1: await ClientHandler.HandlePackets(id, data); (xử lý tuần tự, chậm hơn)
                    // - Option 2: Implement hàng đợi có thứ tự với ActionBlock (tương tự packetQueue cho sending)
                    // - Option 3: Chỉ apply thứ tự cho một số loại packet quan trọng
                    _ = ClientHandler.HandlePackets(id, data);
                }
                catch (Exception)
                {

                }
            }

            Disconnected?.Invoke(this);
            Dispose();
        }

        private async Task<(int id, byte[] data)> GetNextPacketAsync()
        {
            var length = await serverStream.ReadIntAsync();

            if (length <= 0)
                return (0, new byte[0]);

            var receivedData = new byte[length];

            Thread.Sleep(10);
            _ = await serverStream.ReadAsync(receivedData.AsMemory(0, length));

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

            return (packetId, packetData);
        }

        internal void SendPacket(IRequestPacket packet)
        {
            try
            {
                packet.Serialize(serverStream);
            }
            catch (SocketException)
            {
                if (connectionContext.ConnectionClosed.IsCancellationRequested)
                {
                    Disconnect();
                }
            }
        }
        internal async Task QueuePacketAsync(IRequestPacket packet)
        {
            _ = await packetQueue.SendAsync(packet);
        }

        public void Disconnect()
        {
            cancellationSource.Cancel();
            Disconnected?.Invoke(this);

            Dispose();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            serverStream.Dispose();
            connectionContext.Abort();
            cancellationSource?.Dispose();
            ClientHandler.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
