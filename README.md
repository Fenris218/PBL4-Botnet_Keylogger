# PBL4 - Botnet Keylogger (Remote Administration Tool)

## 📋 Tổng Quan

Dự án PBL4 Botnet Keylogger là một hệ thống Remote Administration Tool (RAT) được xây dựng trên kiến trúc Client-Server, cho phép quản lý và điều khiển từ xa các máy tính client thông qua một giao diện server tập trung.

**⚠️ LƯU Ý**: Dự án này chỉ phục vụ mục đích học tập và nghiên cứu. Việc sử dụng công cụ này cho mục đích bất hợp pháp là vi phạm luật pháp.

## 🏗️ Kiến Trúc Hệ Thống

```
┌─────────────────────┐         TCP/IP          ┌─────────────────────┐
│   Client Machine    │ ◄─────────────────────► │   Server Machine    │
│                     │    Port 10000           │                     │
│  ┌───────────────┐  │                         │  ┌───────────────┐  │
│  │  Client.exe   │  │                         │  │  Server.exe   │  │
│  │               │  │                         │  │               │  │
│  │  - Networking │  │                         │  │  - Listen     │  │
│  │  - Keylogger  │  │                         │  │  - UI Forms   │  │
│  │  - Activity   │  │                         │  │  - Client Mgr │  │
│  └───────────────┘  │                         │  └───────────────┘  │
└─────────────────────┘                         └─────────────────────┘
```

## 🎯 Chức Năng Chính

### 1. 🖥️ Remote Desktop
- Xem màn hình từ xa real-time
- Hỗ trợ đa màn hình
- Điều khiển chuột và bàn phím từ xa
- Nén JPEG để tối ưu băng thông

### 2. 📁 File Manager
- Duyệt thư mục và ổ đĩa
- Download/Upload files
- Đổi tên, xóa files/folders
- Transfer với chunk-based protocol (64KB chunks)

### 3. ⌨️ Keylogger
- Ghi lại tất cả phím bấm
- Theo dõi window title
- Lưu logs với timestamp
- Auto-rotate khi file đạt 5MB

### 4. 💻 Remote Shell
- Thực thi lệnh cmd.exe từ xa
- Capture output real-time
- Hỗ trợ multiple commands
- Display trong console-style interface

### 5. 🔧 Task Manager
- Liệt kê processes đang chạy
- Kill/Suspend/Resume process
- Hiển thị PID, tên, memory usage

### 6. ℹ️ System Information
- Thông tin CPU, RAM, OS
- Network adapters
- Installed software
- Hardware details

### 7. 📊 Activity Detection
- Giám sát hoạt động người dùng
- Phát hiện idle (không hoạt động 10 phút)
- Cập nhật trạng thái real-time

### 8. 💬 Message Box
- Hiển thị message box trên client
- Tùy chỉnh caption, text, icon, buttons

## 📚 Tài Liệu

Dự án bao gồm tài liệu chi tiết về kiến trúc và thiết kế:

### [ARCHITECTURE.md](./ARCHITECTURE.md)
Tài liệu chi tiết về:
- ✅ Tổng quan hệ thống
- ✅ Kiến trúc Client (chi tiết từng module)
- ✅ Kiến trúc Server (chi tiết từng module)
- ✅ Giao thức giao tiếp (ProcessStream Protocol)
- ✅ Các chức năng chính và cách hoạt động
- ✅ Luồng hoạt động chi tiết với ASCII diagrams
- ✅ Packet IDs và handlers mapping
- ✅ Sequence flows cho từng tính năng

### [UML_DIAGRAMS.md](./UML_DIAGRAMS.md)
Các biểu đồ UML bằng PlantUML:
- ✅ Class Diagram - Tổng quan hệ thống
- ✅ Sequence Diagram - Connection Establishment
- ✅ Sequence Diagram - Remote Desktop
- ✅ Sequence Diagram - File Transfer
- ✅ Component Diagram
- ✅ State Diagram - Client Connection Lifecycle
- ✅ Activity Diagram - Keylogger Flow
- ✅ Deployment Diagram

## 🔧 Công Nghệ Sử Dụng

### Core Technologies
- **Language**: C# (.NET)
- **Framework**: .NET 6.0+
- **UI Framework**: Windows Forms
- **Transport**: TCP/IP Sockets

### Networking
- **Server**: Kestrel Transport (ASP.NET Core)
- **Protocol**: Custom binary protocol với ProcessStream
- **Packet Queue**: DataFlow BufferBlock
- **Serialization**: JSON (via JsonHelper)

### Key Libraries
- `System.Threading.Tasks.Dataflow` - Packet queue management
- `Microsoft.AspNetCore.Connections` - Server networking
- `System.Net.Sockets` - Client networking
- `System.Drawing` - Screen capture
- `System.Diagnostics` - Process management

## 📦 Cấu Trúc Project

```
PBL4-Botnet_Keylogger/
├── Client/                      # Client application
│   ├── FrmMain.cs              # Entry point, hidden form
│   ├── Networking/             # Network communication
│   │   ├── Client.cs           # TCP client, connection management
│   │   ├── ClientHandler.cs   # Packet routing
│   │   └── Handlers/           # Feature handlers
│   ├── Logging/                # Keylogger implementation
│   │   ├── KeyloggerService.cs
│   │   └── Keylogger.cs
│   └── User/                   # User activity detection
│       └── ActivityDetection.cs
│
├── Server/                      # Server application
│   ├── Forms/                  # UI forms
│   │   ├── FrmMain.cs          # Main window with client list
│   │   ├── FrmRemoteDesktop.cs
│   │   ├── FrmFileManager.cs
│   │   ├── FrmTaskManager.cs
│   │   ├── FrmRemoteShell.cs
│   │   └── FrmKeylogger.cs
│   └── Networking/             # Network communication
│       ├── ListenServer.cs     # TCP listener
│       ├── Client.cs           # Connected client
│       ├── ClientHandler.cs    # Packet routing
│       └── Handlers/           # Feature handlers
│
└── Common/                      # Shared library
    ├── Networking/             # Protocol implementation
    │   ├── ProcessStream.cs    # Binary protocol
    │   ├── IPacket.cs
    │   ├── RequestPacket.cs
    │   └── ResponsePacket.cs
    ├── Models/                 # Data models
    │   ├── FileTransfer.cs
    │   ├── Process.cs
    │   └── Drive.cs
    └── Enums/                  # Enumerations
        ├── UserStatus.cs
        ├── TransferType.cs
        └── ProcessAction.cs
```

## 🔐 Giao Thức Giao Tiếp

### Packet Structure
```
┌─────────────┬──────────────┬─────────────┐
│ Packet ID   │ Data Length  │ Data        │
│ (1 byte)    │ (4 bytes)    │ (N bytes)   │
└─────────────┴──────────────┴─────────────┘
```

### Packet IDs

| ID | Direction | Packet Type | Mô tả |
|----|-----------|-------------|-------|
| 0x01 | C→S | IdentifyClientPacket | Client identification |
| 0x02 | C→S | StatusClientPacket | Status update |
| 0x03 | C→S | UserStatusClientPacket | User activity status |
| 0x04 | S→C | ClientReconnectPacket | Request reconnect |
| 0x05 | S→C | ClientDisconnectPacket | Request disconnect |
| 0x06 | S→C | AskElevatePacket | Request elevation |
| 0x07 | S→C | ShutdownActionPacket | Shutdown/Restart/Standby |
| 0x10 | S→C→S | SystemInfoPacket | System information |
| 0x20 | S→C→S | RemoteShellPacket | Shell command & output |
| 0x30 | S→C→S | GetProcessesPacket | Process list |
| 0x31 | S→C→S | ProcessActionPacket | Process action |
| 0x41-0x48 | Duplex | File Manager Packets | File operations |
| 0x50 | S→C | ShowMessageBoxPacket | Show message box |
| 0x60 | S→C→S | KeyloggerLogsPacket | Keylogger logs |
| 0x70-0x73 | Duplex | Remote Desktop Packets | Desktop control |

## 🚀 Cách Sử Dụng

### Biên Dịch Project

#### Yêu Cầu
- Visual Studio 2022 hoặc mới hơn
- .NET 6.0 SDK hoặc mới hơn
- Windows OS (Windows 10/11)

#### Build Steps
```bash
# Clone repository
git clone https://github.com/Fenris218/PBL4-Botnet_Keylogger.git
cd PBL4-Botnet_Keylogger

# Build Server
cd Server
dotnet build --configuration Release

# Build Client
cd ../Client
dotnet build --configuration Release
```

### Chạy Ứng Dụng

#### 1. Chạy Server
```bash
cd Server/bin/Release/net6.0-windows
./Server.exe
```
- Server sẽ lắng nghe trên port **10000**
- Giao diện hiển thị danh sách clients kết nối

#### 2. Chạy Client
```bash
cd Client/bin/Release/net6.0-windows
./Client.exe
```
- Client sẽ tự động kết nối tới `127.0.0.1:10000`
- Chạy ẩn (hidden), không hiển thị cửa sổ
- Để test trên mạng LAN, sửa IP trong `Client/FrmMain.cs`

#### 3. Sử Dụng Features
1. Trong Server UI, click chuột phải vào client trong danh sách
2. Chọn tính năng muốn sử dụng (Remote Desktop, File Manager, etc.)
3. Cửa sổ tính năng sẽ mở ra
4. Tương tác với client thông qua UI

## 📖 Luồng Hoạt Động Cơ Bản

### 1. Connection Flow
```
Client Start → Connect to Server → Send IdentifyClientPacket
  ↓
Server Accept → Create Client Object → Set Identified = true
  ↓
Server UI → Add to ListView → Show client info
  ↓
Connection Established → Ready for commands
```

### 2. Feature Usage Flow
```
User → Right-click client → Select feature (e.g., Remote Shell)
  ↓
Server → Open FrmRemoteShell → User types command
  ↓
Server → Send RemoteShellPacket (0x20) → Network → Client
  ↓
Client → Execute cmd.exe → Capture output → Send back
  ↓
Server → Receive output → Display in UI
```

### 3. Keylogger Flow
```
Client → KeyloggerService starts → Set keyboard hook
  ↓
User types on client → Hook captures keys → Append to buffer
  ↓
Every 15s OR 5MB → Write to log file → Save in AppData
  ↓
Server requests logs → Client sends file list → Transfer files
  ↓
Server downloads → Display logs in FrmKeylogger
```

## 🎨 Screenshots

### Server Main Window
```
┌─────────────────────────────────────────────────────────────┐
│ Số Lượng Kết Nối: 3 [Đã Lựa Chọn: 1]                        │
├─────────────────────────────────────────────────────────────┤
│ IP Address     │ User@PC      │ OS        │ Country  │ ...  │
├─────────────────────────────────────────────────────────────┤
│ 192.168.1.100  │ john@PC-01   │ Windows 10│ Vietnam  │ ...  │
│ 192.168.1.101  │ mary@PC-02   │ Windows 11│ USA      │ ...  │
│ 192.168.1.102  │ bob@PC-03    │ Windows 10│ Japan    │ ...  │
└─────────────────────────────────────────────────────────────┘
```

### Context Menu
```
Right-click → ┌──────────────────────┐
              │ System Information   │
              │ Remote Shell         │
              │ Task Manager         │
              │ File Manager         │
              │ Keylogger            │
              │ Remote Desktop       │
              ├──────────────────────┤
              │ Show Message Box     │
              ├──────────────────────┤
              │ Reconnect            │
              │ Disconnect           │
              │ Shutdown/Restart     │
              └──────────────────────┘
```

## 🔒 Security Considerations

### ⚠️ Lưu Ý Bảo Mật

Hệ thống hiện tại **KHÔNG** có các tính năng bảo mật sau:
- ❌ Không mã hóa dữ liệu (plaintext communication)
- ❌ Không xác thực (authentication)
- ❌ Không ủy quyền (authorization)
- ❌ Không bảo vệ khỏi reverse engineering
- ❌ Không obfuscation

### Khuyến Nghị Cải Tiến Bảo Mật
1. ✅ Thêm TLS/SSL encryption cho network communication
2. ✅ Implement authentication mechanism (token-based, certificate-based)
3. ✅ Mã hóa sensitive data (passwords, logs)
4. ✅ Code obfuscation để bảo vệ binary
5. ✅ Add integrity checks (digital signatures)
6. ✅ Implement rate limiting và anti-detection

## 🧪 Testing

### Unit Testing
```bash
# Chạy tests (nếu có)
dotnet test
```

### Manual Testing
1. Start Server trước
2. Start Client
3. Verify client xuất hiện trong Server UI
4. Test từng feature một
5. Monitor network traffic với Wireshark (optional)

### Test Scenarios
- ✅ Connection establishment và reconnection
- ✅ Multiple clients đồng thời
- ✅ File transfer với files lớn
- ✅ Remote Desktop với resolutions khác nhau
- ✅ Keylogger với special characters
- ✅ Process actions (kill, suspend, resume)
- ✅ Network interruption handling

## 🐛 Known Issues

1. **File Transfer**: Large files (>100MB) có thể chậm
2. **Remote Desktop**: Frame rate giảm với resolution cao
3. **Keylogger**: Một số special keys có thể không capture đúng
4. **Reconnection**: Đôi khi cần manual restart client

## 🔮 Future Enhancements

### Planned Features
- [ ] Encryption (AES-256 cho data, TLS cho transport)
- [ ] Multi-server support (load balancing)
- [ ] Web-based control panel
- [ ] Plugin system cho extensibility
- [ ] Audio recording và streaming
- [ ] Webcam capture
- [ ] Clipboard synchronization
- [ ] Registry editor
- [ ] Network packet sniffer

### Performance Improvements
- [ ] Optimize image compression (WebP, H.264)
- [ ] Implement delta updates cho Remote Desktop
- [ ] Add caching layer
- [ ] Reduce memory footprint
- [ ] Multi-threading optimization

## 📄 License

This project is for educational purposes only. 

**⚠️ DISCLAIMER**: The developers assume no liability and are not responsible for any misuse or damage caused by this program. Use at your own risk. Unauthorized access to computer systems is illegal.

## 👥 Contributors

- **Fenris218** - Initial work and architecture

## 📞 Contact

For questions or discussions about this educational project:
- GitHub Issues: [Create an issue](https://github.com/Fenris218/PBL4-Botnet_Keylogger/issues)
- Email: (Add your email if appropriate)

## 🙏 Acknowledgments

- ASP.NET Core Team - Kestrel Transport
- PlantUML Community - UML diagram tools
- Windows Forms Team - UI framework

---

## 📚 Đọc Thêm

Để hiểu sâu hơn về cách hệ thống hoạt động:

1. **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Đọc trước để hiểu kiến trúc tổng quan, logic và flow chi tiết
2. **[UML_DIAGRAMS.md](./UML_DIAGRAMS.md)** - Xem các biểu đồ UML để hiểu thiết kế hệ thống

### Quick Links
- [Kiến trúc Client](./ARCHITECTURE.md#2-kiến-trúc-client)
- [Kiến trúc Server](./ARCHITECTURE.md#3-kiến-trúc-server)
- [Giao thức giao tiếp](./ARCHITECTURE.md#4-giao-thức-giao-tiếp)
- [Class Diagrams](./UML_DIAGRAMS.md#1-class-diagram-tổng-quan-hệ-thống)
- [Sequence Diagrams](./UML_DIAGRAMS.md#2-sequence-diagram-connection-establishment)

---

**Made with ❤️ for educational purposes**
