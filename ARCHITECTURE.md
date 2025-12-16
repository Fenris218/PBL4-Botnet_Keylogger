# Tài Liệu Kiến Trúc và Luồng Hoạt Động - PBL4 Botnet Keylogger

## Mục Lục
1. [Tổng Quan Hệ Thống](#1-tổng-quan-hệ-thống)
2. [Kiến Trúc Client](#2-kiến-trúc-client)
3. [Kiến Trúc Server](#3-kiến-trúc-server)
4. [Giao Thức Giao Tiếp](#4-giao-thức-giao-tiếp)
5. [Các Chức Năng Chính](#5-các-chức-năng-chính)
6. [Luồng Hoạt Động Chi Tiết](#6-luồng-hoạt-động-chi-tiết)

---

## 1. Tổng Quan Hệ Thống

### 1.1 Mô Tả Chung
Hệ thống PBL4 Botnet Keylogger là một ứng dụng Remote Administration Tool (RAT) được xây dựng trên kiến trúc Client-Server, cho phép quản lý và điều khiển từ xa các máy tính client thông qua một giao diện server tập trung.

### 1.2 Công Nghệ Sử Dụng
- **Ngôn ngữ**: C# (.NET)
- **Framework**: .NET (Windows Forms cho GUI)
- **Transport**: TCP/IP Socket
- **Networking Library**: Kestrel Transport (ASP.NET Core) cho Server
- **Serialization**: Custom binary protocol với ProcessStream

### 1.3 Các Thành Phần Chính
- **Client Application**: Ứng dụng chạy trên máy tính được giám sát
- **Server Application**: Ứng dụng trung tâm để điều khiển các client
- **Common Library**: Thư viện chia sẻ giữa Client và Server (models, networking, utilities)

---

## 2. Kiến Trúc Client

### 2.1 Cấu Trúc Tổng Quan

Client được thiết kế để chạy ẩn (hidden) trên máy tính mục tiêu và duy trì kết nối liên tục với Server.

#### Các Module Chính:

**A. FrmMain (Form Chính)**
- Entry point của ứng dụng client
- Chạy ẩn ngay khi khởi động (WindowState.Minimized, Visible = false)
- Khởi tạo và quản lý các service chính

**B. Client Networking**
```
Client.Networking.Client
├── Quản lý kết nối TCP tới Server
├── ProcessStream: Xử lý serialization/deserialization packets
├── BufferBlock: Hàng đợi packet để gửi
└── ClientHandler: Định tuyến và xử lý packets nhận được
```

**C. KeyloggerService**
```
KeyloggerService
├── Chạy trên thread riêng với ApplicationContext
├── Keylogger: Capture phím bấm của người dùng
├── Lưu logs vào file với giới hạn kích thước (5MB)
└── Flush interval: 15 giây
```

**D. ActivityDetection**
```
ActivityDetection
├── Giám sát hoạt động của người dùng
├── Kiểm tra idle time (thời gian không hoạt động)
├── Ngưỡng idle: 600 giây (10 phút)
└── Gửi UserStatus packet khi trạng thái thay đổi
```

### 2.2 Luồng Khởi Động Client

```
1. Main() → Application.Run(new FrmMain())
   ↓
2. FrmMain_Load
   ↓
3. Run() method
   ├── Tạo SingleInstanceMutex (đảm bảo chỉ 1 instance chạy)
   ├── Khởi tạo Client Networking
   ├── Khởi động KeyloggerService
   ├── Khởi động ActivityDetection
   └── Connect tới Server (127.0.0.1:10000)
```

### 2.3 Quá Trình Kết Nối

```
Client.Connect(IPAddress, Port)
   ↓
1. Tạo TcpClient và kết nối
   ↓
2. Tạo ProcessStream từ NetworkStream
   ↓
3. Gọi ProcessAsync() để bắt đầu nhận packets (chạy async)
   ↓
4. Gửi IdentifyClientPacket (0x01) với thông tin:
   - Operating System
   - Account Type
   - Country & CountryCode (từ IP Geolocation)
   - Hardware ID
   - Username & PC Name
   - IP Address
   ↓
5. Vào vòng lặp nhận và xử lý packets
```

### 2.4 Xử Lý Packets Tại Client

Client sử dụng **ClientHandler** để định tuyến packets dựa trên Packet ID:

| Packet ID | Chức Năng | Handler |
|-----------|-----------|---------|
| 0x04 | Reconnect | ClientServicesHandler |
| 0x05 | Disconnect | ClientServicesHandler |
| 0x06 | Ask Elevate | ClientServicesHandler |
| 0x07 | Shutdown Action | ClientServicesHandler |
| 0x10 | Get System Info | SystemInfoHandler |
| 0x20 | Remote Shell Command | RemoteShellHandler |
| 0x30 | Get Processes | TaskManagerHandler |
| 0x31 | Process Action | TaskManagerHandler |
| 0x41-0x48 | File Manager Operations | FileManagerHandler |
| 0x50 | Show Message Box | MessageBoxHandler |
| 0x60 | Keylogger Logs | KeyloggerHandler |
| 0x70-0x73 | Remote Desktop | RemoteDesktopHandler |

### 2.5 Các Handler Chi Tiết

#### A. ClientServicesHandler
- **Reconnect**: Ngắt kết nối hiện tại và kết nối lại
- **Disconnect**: Đóng kết nối và thoát ứng dụng
- **AskElevate**: Khởi động lại với quyền Administrator
- **ShutdownAction**: Thực hiện Shutdown/Restart/Standby

#### B. SystemInfoHandler
- Thu thập thông tin hệ thống (CPU, RAM, OS, etc.)
- Gửi về Server qua SystemInfoPacket

#### C. RemoteShellHandler
- Thực thi lệnh shell (cmd.exe)
- Capture output và gửi về Server

#### D. TaskManagerHandler
- **GetProcesses**: Lấy danh sách process đang chạy
- **ProcessAction**: Kill/Suspend/Resume process

#### E. FileManagerHandler
- **GetDrives**: Lấy danh sách ổ đĩa
- **GetDirectory**: Lấy nội dung thư mục
- **PathRename**: Đổi tên file/folder
- **PathDelete**: Xóa file/folder
- **FileTransfer**: Upload/Download file (chunk-based)

#### F. KeyloggerHandler
- Gửi đường dẫn thư mục chứa keylogger logs

#### G. RemoteDesktopHandler
- **GetDesktop**: Capture màn hình và gửi về (JPEG compression)
- **GetMonitors**: Lấy danh sách màn hình
- **MouseEvent**: Xử lý sự kiện chuột từ xa
- **KeyboardEvent**: Xử lý sự kiện bàn phím từ xa

---

## 3. Kiến Trúc Server

### 3.1 Cấu Trúc Tổng Quan

Server là ứng dụng Windows Forms cung cấp giao diện để quản lý nhiều client đồng thời.

#### Các Module Chính:

**A. FrmMain (Form Chính)**
- Hiển thị danh sách clients đã kết nối
- Context menu để thực hiện các thao tác trên client
- Khởi động ListenServer

**B. ListenServer**
```
ListenServer
├── Sử dụng Kestrel SocketTransportFactory
├── Lắng nghe trên Port 10000
├── Chấp nhận kết nối từ Clients
├── Quản lý ConcurrentHashSet<Client>
└── Raise events: ClientConnected/ClientDisconnected
```

**C. Server.Networking.Client**
```
Server.Networking.Client
├── Đại diện cho 1 client kết nối
├── ConnectionContext từ Kestrel
├── DuplexPipeStream cho bidirectional communication
├── ProcessStream cho packet handling
├── ClientHandler cho packet routing
└── BufferBlock packetQueue cho gửi packets
```

**D. ClientHandler**
- Định tuyến packets từ Client dựa trên ID
- Khởi tạo các Handler modules cho từng chức năng

**E. Feature Forms**
- FrmSystemInformation
- FrmRemoteShell
- FrmTaskManager
- FrmFileManager
- FrmKeylogger
- FrmRemoteDesktop
- FrmShowMessagebox

### 3.2 Luồng Khởi Động Server

```
1. Main() → Application.Run(new FrmMain())
   ↓
2. FrmMain Constructor
   ├── InitializeServer()
   │   ├── Tạo ListenServer(port: 10000)
   │   └── Subscribe events:
   │       ├── ClientConnected
   │       ├── ClientDisconnected
   │       ├── StatusUpdated
   │       └── UserStatusUpdated
   └── InitializeComponent()
   ↓
3. FrmMain_Load
   └── StartConnectionListener()
       ├── ListenServer.RunAsync()
       └── ThreadPool.QueueUserWorkItem(ProcessClientConnections)
```

### 3.3 Quá Trình Chấp Nhận Kết Nối

```
ListenServer.RunAsync()
   ↓
1. Tạo IConnectionListener với SocketTransportFactory
   ↓
2. Vòng lặp while (!cancellationToken.IsCancellationRequested)
   ↓
3. await _tcpListener.AcceptAsync()
   ↓
4. Tạo Server.Networking.Client từ ConnectionContext
   ↓
5. Thêm client vào ConcurrentHashSet
   ↓
6. Subscribe event client.Disconnected
   ↓
7. Task.Delay(15 seconds) - Timeout cho identification
   ├── Nếu sau 15s client chưa Identified
   └── → client.Disconnect()
   ↓
8. Task.Run(client.StartConnectionAsync) - Bắt đầu nhận packets
```

### 3.4 Quá Trình Identification

```
Client gửi IdentifyClientPacket (0x01)
   ↓
ClientHandler.HandlePackets(0x01, data)
   ↓
ClientServicesHandler.Handler(IdentifyClientPacket)
   ↓
1. Deserialize packet data
2. Tạo UserState với thông tin:
   - IpAddress
   - Country & CountryCode
   - OperatingSystem
   - AccountType
   - UserAtPc (Username@PcName)
   - HardwareId
3. Set client.Value = userState
4. Set client.Identified = true
5. Init các Handler modules
6. Raise server.OnClientConnected(client)
   ↓
FrmMain.ClientConnected(client)
   ↓
AddClientToListview(client)
   - Tạo ListViewItem với các columns:
     [IP, User@PC, OS, Country, Status, UserStatus, AccountType]
```

### 3.5 Xử Lý Packets Tại Server

Server sử dụng **ClientHandler** để định tuyến packets từ Client:

| Packet ID | Packet Type | Handler |
|-----------|-------------|---------|
| 0x01 | IdentifyClientPacket | ClientServicesHandler |
| 0x02 | StatusClientPacket | ClientServicesHandler |
| 0x03 | UserStatusClientPacket | ClientServicesHandler |
| 0x10 | SystemInfoPacket | SystemInformationHandler |
| 0x20 | RemoteShellPacket | RemoteShellHandler |
| 0x30 | GetProcessesPacket | TaskManagerHandler |
| 0x31 | ProcessActionPacket | TaskManagerHandler |
| 0x40-0x48 | File Manager Packets | FileManagerHandler |
| 0x60 | KeyloggerLogsDirectory | KeyloggerHandler |
| 0x70-0x71 | Remote Desktop Packets | RemoteDesktopHandler |

### 3.6 Các Feature Forms Chi Tiết

#### A. FrmSystemInformation
- Request thông tin hệ thống từ client (0x10)
- Hiển thị: CPU, RAM, OS, Network adapters, etc.

#### B. FrmRemoteShell
- Gửi command tới client (0x20)
- Nhận và hiển thị output từ cmd.exe

#### C. FrmTaskManager
- Request danh sách processes (0x30)
- Hiển thị trong ListView
- Kill/Suspend/Resume process (0x31)

#### D. FrmFileManager
- Tree view cho drives và directories
- ListView cho files
- Các thao tác:
  - Navigate directories (0x42)
  - Rename (0x43)
  - Delete (0x44)
  - Upload/Download files (0x45-0x48)

#### E. FrmKeylogger
- Request logs directory từ client (0x60)
- Download log files
- Hiển thị nội dung logs

#### F. FrmRemoteDesktop
- Request desktop screenshot (0x70)
- Hiển thị real-time (streaming)
- Gửi mouse/keyboard events (0x72, 0x73)
- Hỗ trợ multi-monitor (0x71)

#### G. FrmShowMessagebox
- Hiển thị dialog để nhập:
  - Caption
  - Text
  - Button type
  - Icon type
- Gửi ShowMessageBoxPacket (0x50) tới client

---

## 4. Giao Thức Giao Tiếp

### 4.1 ProcessStream Protocol

ProcessStream là custom binary protocol để serialize/deserialize packets:

```
Packet Structure:
┌─────────────┬──────────────┬─────────────┐
│ Packet ID   │ Data Length  │ Data        │
│ (1 byte)    │ (4 bytes)    │ (N bytes)   │
└─────────────┴──────────────┴─────────────┘
```

### 4.2 Packet Types

#### IPacket (Interface chung)
- Base interface cho tất cả packets

#### IRequestPacket (Client → Server hoặc Server → Client)
```csharp
interface IRequestPacket : IPacket
{
    int Id { get; }  // Packet identifier
}
```

#### IResponsePacket (Response packets)
```csharp
interface IResponsePacket : IPacket
{
    // Chứa data response
}
```

#### RequestPacket & ResponsePacket
- Base classes implement serialization logic
- Sử dụng JsonHelper để serialize/deserialize

### 4.3 Packet Flow

**Gửi Packet:**
```
Client/Server
   ↓
QueuePacketAsync(packet)
   ↓
BufferBlock.Post(packet)
   ↓
ActionBlock processes packet
   ↓
SendPacket(packet)
   ↓
RequestPacket.Serialize(packet)
   ↓
ProcessStream.Write(id, data)
   ↓
NetworkStream sends data
```

**Nhận Packet:**
```
NetworkStream has data
   ↓
ProcessStream.Read()
   ↓
Read packet ID (1 byte)
   ↓
Read data length (4 bytes)
   ↓
Read data (N bytes)
   ↓
(id, data) tuple
   ↓
ClientHandler.HandlePackets(id, data)
   ↓
ResponsePacket.Deserialize<T>(data)
   ↓
Appropriate Handler processes packet
```

### 4.4 Packet Queue System

Cả Client và Server đều sử dụng **DataFlow BufferBlock** để quản lý packet queue:

```csharp
BufferBlock<IRequestPacket> packetQueue
   ↓
LinkTo(ActionBlock<IRequestPacket>)
   ↓
ActionBlock xử lý sequentially (EnsureOrdered = true)
```

**Lợi ích:**
- Thread-safe
- Ordered delivery
- Backpressure handling
- Cancellation support

---

## 5. Các Chức Năng Chính

### 5.1 Remote Desktop

**Client Side:**
```
RemoteDesktopHandler.GetDesktop(packet)
   ↓
1. Lấy monitor được chỉ định (hoặc primary)
2. Capture screen sử dụng ScreenHelper
3. Nén ảnh với JpgCompression (quality từ packet)
4. Chia thành chunks nếu cần
5. Gửi GetDesktopPacket về Server
```

**Server Side:**
```
FrmRemoteDesktop
   ↓
1. Gửi GetDesktopPacket request với interval (timer)
2. Nhận GetDesktopPacket response
3. Deserialize image data
4. Hiển thị trong PictureBox
5. Xử lý mouse/keyboard events từ user
6. Gửi MouseEventPacket/KeyboardEventPacket tới Client
```

### 5.2 File Manager

**File Transfer Flow (Download từ Client):**
```
Server: FileTransferRequestPacket (0x45)
   ↓
Client: FileManagerHandler.FileTransferRequest
   ├── Mở file để đọc
   ├── Tạo FileTransfer object
   └── Bắt đầu đọc chunks
   ↓
Client: Gửi FileTransferChunkPacket (0x48) cho từng chunk
   ├── Chunk size: 64KB
   ├── Sequence number
   └── Chunk data
   ↓
Server: FileManagerHandler.Handler(FileTransferChunkPacket)
   ├── Ghi chunk vào file
   └── Nếu là chunk cuối → Complete
   ↓
Client: Gửi FileTransferCompletePacket (0x46)
```

**Cancel Transfer:**
```
Server: FileTransferCancelPacket (0x47)
   ↓
Client: Đóng file streams và cleanup
```

### 5.3 Keylogger

**Client Side:**
```
KeyloggerService.Start()
   ↓
Keylogger hooks keyboard với SetWindowsHookEx
   ↓
Mỗi phím bấm:
   1. Capture key code
   2. Convert sang character
   3. Append vào buffer
   4. Nếu buffer đầy (flush interval) → ghi vào file
   ↓
Files được lưu trong AppData với timestamp
Format: [Timestamp] window_title
        key_strokes
```

**Server Side:**
```
FrmKeylogger
   ↓
Request logs directory (0x60)
   ↓
Client: Gửi GetKeyloggerLogsDirectory với danh sách files
   ↓
Server: Hiển thị danh sách files
   ↓
User select file → Download qua FileTransfer
   ↓
Server: Hiển thị nội dung log
```

### 5.4 Task Manager

**Get Processes:**
```
Server: GetProcessesPacket (0x30)
   ↓
Client: TaskManagerHandler.GetProcesses
   ├── Lấy Process.GetProcesses()
   ├── Convert sang Common.Models.Process
   └── Gửi GetProcessesPacket về Server
   ↓
Server: TaskManagerHandler.Handler
   └── Update ListView trong FrmTaskManager
```

**Process Actions:**
```
Server: ProcessActionPacket (0x31)
   ↓
Client: TaskManagerHandler.ActionProcess
   ├── Kill: Process.Kill()
   ├── Suspend: NativeMethods.SuspendProcess()
   └── Resume: NativeMethods.ResumeProcess()
   ↓
Client: Gửi status packet về Server
```

### 5.5 Remote Shell

**Command Execution:**
```
Server: RemoteShellPacket (0x20) with command
   ↓
Client: RemoteShellHandler.SendCommand
   ├── Tạo Process với cmd.exe
   ├── Redirect StandardInput/Output/Error
   ├── Execute command
   ├── Capture output
   └── Gửi RemoteShellPacket với output về Server
   ↓
Server: RemoteShellHandler.Handler
   └── Append output vào RichTextBox trong FrmRemoteShell
```

### 5.6 Activity Detection

**Client Side:**
```
ActivityDetection.UserActivityThread
   ↓
Loop mỗi giây:
   1. GetLastInputInfoTickCount() - lấy thời điểm input cuối
   2. Tính idle time = current - last input
   3. Nếu idle > 600 giây:
      └── Gửi UserStatusClientPacket (Idle)
   4. Nếu có hoạt động:
      └── Gửi UserStatusClientPacket (Active)
```

**Server Side:**
```
ClientHandler.HandlePackets(0x03)
   ↓
ClientServicesHandler.Handler(UserStatusClientPacket)
   ↓
server.OnUserStatusUpdated(client, userStatus)
   ↓
FrmMain.SetUserStatusByClient
   └── Update column "UserStatus" trong ListView
```

---

## 6. Luồng Hoạt Động Chi Tiết

### 6.1 Toàn Bộ Quá Trình Từ Startup Đến Feature Usage

```
┌─────────────────────────────────────────────────────────────┐
│                    CLIENT STARTUP                            │
└─────────────────────────────────────────────────────────────┘
                            ↓
    FrmMain.Load → Run() → Initialize Components
                            ↓
    ┌──────────────────┬──────────────────┬─────────────────┐
    ↓                  ↓                  ↓                 ↓
SingleInstance    NetworkClient    KeyloggerService  ActivityDetection
  Mutex             .Connect()         .Start()          .Start()
    ↓                  ↓                  ↓                 ↓
    └──────────────────┴──────────────────┴─────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    CONNECTION PHASE                          │
└─────────────────────────────────────────────────────────────┘
                            ↓
    Client: Connect to 127.0.0.1:10000
                            ↓
    Server: Accept connection → Create Client object
                            ↓
    Client: Send IdentifyClientPacket (0x01)
                            ↓
    Server: Receive → ClientServicesHandler
                            ↓
    Server: Set client.Identified = true
                            ↓
    Server: OnClientConnected → Add to ListView
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    IDLE STATE                                │
└─────────────────────────────────────────────────────────────┘
    Client:                          Server:
    - Keylogger capturing            - Displaying client list
    - Activity monitoring            - Waiting for user action
    - Listening for packets          - Ready to send commands
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              USER INITIATES FEATURE (Example: Remote Shell) │
└─────────────────────────────────────────────────────────────┘
                            ↓
    Server: User right-clicks client → Remote Shell
                            ↓
    Server: FrmRemoteShell.CreateNewOrGetExisting(client)
                            ↓
    Server: Show form
                            ↓
    User types command: "dir"
                            ↓
    Server: btnSend_Click
                            ↓
    Server: client.QueuePacketAsync(RemoteShellPacket)
          - Command: "dir"
          - ID: 0x20
                            ↓
    Network: Packet sent via TCP
                            ↓
    Client: ProcessAsync receives packet
                            ↓
    Client: ClientHandler.HandlePackets(0x20, data)
                            ↓
    Client: RemoteShellHandler.SendCommand(packet)
          - Create Process(cmd.exe)
          - Execute "dir"
          - Capture output
                            ↓
    Client: QueuePacketAsync(RemoteShellPacket)
          - Output: "Directory listing..."
          - ID: 0x20
                            ↓
    Network: Packet sent back
                            ↓
    Server: client.StartConnectionAsync receives packet
                            ↓
    Server: ClientHandler.HandlePackets(0x20, data)
                            ↓
    Server: RemoteShellHandler.Handler(packet)
                            ↓
    Server: FrmRemoteShell.txtConsoleOutput.AppendText(output)
                            ↓
    User sees output in Remote Shell window
```

### 6.2 Sequence: Remote Desktop Session

```
User                FrmRemoteDesktop        Server.Client       Network     Client.Client    RemoteDesktopHandler
  |                        |                      |                |              |                    |
  |----Open Remote Desktop-|                      |                |              |                    |
  |                        |                      |                |              |                    |
  |                        |--Show Form---------->|                |              |                    |
  |                        |                      |                |              |                    |
  |                        |--Start Timer-------->|                |              |                    |
  |                        |                      |                |              |                    |
  |                        |          [Every 100ms]                |              |                    |
  |                        |                      |                |              |                    |
  |                        |--QueuePacket-------->|                |              |                    |
  |                        | (GetDesktopPacket)   |                |              |                    |
  |                        |  - MonitorIndex      |                |              |                    |
  |                        |  - Quality           |                |              |                    |
  |                        |                      |                |              |                    |
  |                        |                      |--Send-------->|              |                    |
  |                        |                      | (Packet 0x70)  |              |                    |
  |                        |                      |                |--Receive---->|                    |
  |                        |                      |                |              |                    |
  |                        |                      |                |              |--HandlePackets---->|
  |                        |                      |                |              |                    |
  |                        |                      |                |              |                    |<--.
  |                        |                      |                |              |                    |   |
  |                        |                      |                |              |                    |  Capture Screen
  |                        |                      |                |              |                    |  Compress JPEG
  |                        |                      |                |              |                    |   |
  |                        |                      |                |              |                    |<--'
  |                        |                      |                |              |<--Send Response----|
  |                        |                      |                |<--Receive----|                    |
  |                        |                      |<--Receive------|              |                    |
  |                        |                      | (Image Data)   |              |                    |
  |                        |<--Handler Callback---|                |              |                    |
  |                        |                      |                |              |                    |
  |<---Display Image-------|                      |                |              |                    |
  |                        |                      |                |              |                    |
  |                        |                      |                |              |                    |
  |--Mouse Click---------->|                      |                |              |                    |
  |                        |                      |                |              |                    |
  |                        |--QueuePacket-------->|                |              |                    |
  |                        | (MouseEventPacket)   |                |              |                    |
  |                        |  - X, Y              |                |              |                    |
  |                        |  - Action            |                |              |                    |
  |                        |                      |                |              |                    |
  |                        |                      |--Send-------->|              |                    |
  |                        |                      | (Packet 0x72)  |              |                    |
  |                        |                      |                |--Receive---->|                    |
  |                        |                      |                |              |--HandlePackets---->|
  |                        |                      |                |              |                    |
  |                        |                      |                |              |                    |<--.
  |                        |                      |                |              |                    |   |
  |                        |                      |                |              |                    | Simulate
  |                        |                      |                |              |                    | Mouse Click
  |                        |                      |                |              |                    |   |
  |                        |                      |                |              |                    |<--'
```

### 6.3 State Machine: Client Connection Lifecycle

```
┌─────────────┐
│ Disconnected│
└──────┬──────┘
       │
       │ Connect()
       │
       ↓
┌─────────────┐
│ Connecting  │
└──────┬──────┘
       │
       │ TcpClient.Connect success
       │
       ↓
┌─────────────┐
│  Connected  │
└──────┬──────┘
       │
       │ Send IdentifyClientPacket
       │
       ↓
┌──────────────┐          15s timeout
│ Identifying  ├────────────────────┐
└──────┬───────┘                    │
       │                            │
       │ Server accepts ID          │
       │                            ↓
       ↓                     ┌─────────────┐
┌──────────────┐            │ Disconnected│
│  Identified  │            └─────────────┘
└──────┬───────┘
       │
       │ ┌───────────────────────────────┐
       │ │                               │
       ↓ │                               │
┌──────────────┐                         │
│    Active    │                         │
│              │                         │
│ - Receiving  │                         │
│   packets    │                         │
│ - Sending    │                         │
│   packets    │                         │
│ - Keylogging │                         │
│ - Monitoring │                         │
└──────┬───────┘                         │
       │                                 │
       │ Activity Detection              │
       │                                 │
       ↓                                 │
┌──────────────┐     Activity            │
│     Idle     │─────────────────────────┘
│ (600s no     │
│  activity)   │
└──────┬───────┘
       │
       │ Disconnect/Error
       │
       ↓
┌─────────────┐
│ Disconnected│
└─────────────┘
```

### 6.4 Component Interaction: File Transfer

```
Server                        Network                     Client
FrmFileManager               (TCP/IP)            FileManagerHandler
     │                           │                           │
     │ User double-click file    │                           │
     │                           │                           │
     ├──FileTransferRequest──────►──────────────────────────►│
     │  - File path              │                           │
     │  - Transfer ID            │                           │
     │  - Type: Download         │                           │
     │                           │                           ├─Open file
     │                           │                           │ Create chunks
     │                           │                           │
     │                           │◄──────────────────────────┤
     │◄──────────────────────────┤  FileTransferChunk #1     │
     │  - Transfer ID            │  - Sequence: 0            │
     │  - Data (64KB)            │  - Data                   │
     │                           │                           │
     ├─Write to file             │                           │
     │                           │                           │
     │                           │◄──────────────────────────┤
     │◄──────────────────────────┤  FileTransferChunk #2     │
     │                           │                           │
     ├─Write to file             │                           │
     │                           │                           │
     │         ...               │           ...             │
     │                           │                           │
     │                           │◄──────────────────────────┤
     │◄──────────────────────────┤  FileTransferChunk #N     │
     │                           │  - IsLastChunk: true      │
     │                           │                           │
     ├─Write to file             │                           │
     │                           │                           │
     │                           │◄──────────────────────────┤
     │◄──────────────────────────┤  FileTransferComplete     │
     │                           │                           │
     ├─Close file                │                           ├─Close file
     ├─Update UI                 │                           │
     │  "Transfer complete"      │                           │
```

### 6.5 Data Flow: Keylogger

```
Keyboard Input
      ↓
Windows Hook (SetWindowsHookEx)
      ↓
Keylogger.HookCallback
      ↓
┌───────────────────────────────┐
│ Process Key Press             │
│ - Get key code                │
│ - Get window title            │
│ - Convert to character        │
│ - Handle special keys         │
│   (Enter, Backspace, etc.)    │
└───────────────────────────────┘
      ↓
Append to internal buffer
      ↓
┌───────────────────────────────┐
│ Check flush conditions:       │
│ - Time interval (15s) OR      │
│ - Buffer size (5MB)           │
└───────────────────────────────┘
      ↓
Write to log file
Format:
[2024-01-15 10:30:45] Notepad
Hello world
[Enter]
[2024-01-15 10:31:00] Chrome - Google
search query
[Tab] [Enter]
      ↓
Save in AppData\KeyLogs\
Filename: Log_2024-01-15.txt
      ↓
Server can request files (0x60)
      ↓
Download via FileTransfer
      ↓
Display in FrmKeylogger
```

---

## Tổng Kết

Hệ thống PBL4 Botnet Keylogger là một RAT đầy đủ chức năng với kiến trúc Client-Server rõ ràng:

**Điểm Mạnh:**
- Kiến trúc module hóa tốt
- Sử dụng async/await cho performance
- Custom binary protocol hiệu quả
- Hỗ trợ nhiều client đồng thời
- UI trực quan với Windows Forms

**Các Chức Năng Chính:**
1. **Keylogger**: Ghi lại phím bấm
2. **Remote Desktop**: Điều khiển màn hình từ xa
3. **File Manager**: Quản lý file/folder
4. **Task Manager**: Quản lý process
5. **Remote Shell**: Thực thi lệnh cmd
6. **System Info**: Thu thập thông tin hệ thống
7. **Activity Detection**: Giám sát hoạt động user

**Flow Chính:**
```
Client ←──TCP/IP──→ Server
   ├── Keylogger (background)
   ├── Activity Detection (background)
   └── Packet Handler (handles commands)
```

Tất cả giao tiếp đều thông qua **ProcessStream** với custom binary protocol, đảm bảo hiệu suất cao và bảo mật cơ bản.
