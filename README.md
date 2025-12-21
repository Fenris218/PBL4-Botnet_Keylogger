# PBL4 - Botnet Keylogger

## Giao thức mạng / Network Protocol

Dự án này sử dụng **TCP/IP** (Transmission Control Protocol over Internet Protocol) để thiết lập kết nối giữa Client và Server.

This project uses **TCP/IP** (Transmission Control Protocol over Internet Protocol) for establishing connections between Client and Server.

### Tại sao chọn TCP/IP? / Why TCP/IP?

TCP/IP được chọn vì các lý do sau:

1. **Độ tin cậy cao** - TCP đảm bảo dữ liệu được truyền đầy đủ và đúng thứ tự
2. **Kiểm soát lỗi** - TCP tự động kiểm tra và sửa lỗi trong quá trình truyền
3. **Kiểm soát luồng** - TCP quản lý tốc độ truyền dữ liệu giữa client và server
4. **Kết nối hướng liên kết** - Đảm bảo kết nối ổn định giữa client và server

TCP/IP is chosen for the following reasons:

1. **High Reliability** - TCP ensures data is transmitted completely and in order
2. **Error Control** - TCP automatically detects and corrects errors during transmission
3. **Flow Control** - TCP manages data transmission speed between client and server
4. **Connection-Oriented** - Ensures stable connection between client and server

### Triển khai kỹ thuật / Technical Implementation

#### Server Side
- Sử dụng `IConnectionListener` với Kestrel Transport (ASP.NET Core)
- Lắng nghe kết nối TCP trên cổng được chỉ định
- File: `Server/Networking/ListenServer.cs`

```csharp
// Tạo TCP Listener
_tcpListener = await CreateListenerAsync(
    new IPEndPoint(IPAddress.Any, Port), 
    token: _cancelTokenSource.Token);
```

#### Client Side
- Sử dụng `TcpClient` class từ `System.Net.Sockets`
- Kết nối đến server qua địa chỉ IP và cổng
- File: `Client/Networking/Client.cs`

```csharp
// Tạo và kết nối TCP Client
_tcpClient = new TcpClient();
_tcpClient.Connect(ip, port); // Kết nối TCP/IP
```

### So sánh TCP vs UDP / TCP vs UDP Comparison

| Tiêu chí / Criteria | TCP | UDP |
|---------------------|-----|-----|
| Độ tin cậy / Reliability | ✅ Cao / High | ❌ Thấp / Low |
| Tốc độ / Speed | Chậm hơn / Slower | Nhanh hơn / Faster |
| Kết nối / Connection | Có / Yes | Không / No |
| Thứ tự gói tin / Packet Order | Đảm bảo / Guaranteed | Không đảm bảo / Not Guaranteed |
| Kiểm tra lỗi / Error Checking | Có / Yes | Cơ bản / Basic |
| Phù hợp cho / Suitable for | Keylogger, Remote Control | Streaming, Gaming |

**Kết luận:** TCP/IP phù hợp hơn cho ứng dụng Botnet Keylogger vì cần đảm bảo dữ liệu keylog được truyền đầy đủ và chính xác.

**Conclusion:** TCP/IP is more suitable for Botnet Keylogger application because it needs to ensure keylog data is transmitted completely and accurately.

### Các tầng trong mô hình TCP/IP / TCP/IP Model Layers

Mô hình TCP/IP gồm 4 tầng chính / TCP/IP model consists of 4 main layers:

#### 1. Tầng Ứng dụng (Application Layer)
**Chức năng của dự án hoạt động ở tầng này:**
- ✅ **Remote Desktop**: Chụp màn hình, mã hóa hình ảnh, gửi/nhận dữ liệu hình ảnh
- ✅ **Keylogger**: Thu thập và truyền dữ liệu phím nhấn
- ✅ **File Manager**: Quản lý và truyền file
- ✅ **Remote Shell**: Thực thi lệnh từ xa
- ✅ **Mouse/Keyboard Control**: Điều khiển chuột và bàn phím từ xa

**Project features operating at this layer:**
- ✅ **Remote Desktop**: Screen capture, image encoding, sending/receiving image data
- ✅ **Keylogger**: Collecting and transmitting keystroke data
- ✅ **File Manager**: Managing and transferring files
- ✅ **Remote Shell**: Remote command execution
- ✅ **Mouse/Keyboard Control**: Remote mouse and keyboard control

#### 2. Tầng Giao vận (Transport Layer)
- Sử dụng **TCP** để đảm bảo truyền dữ liệu tin cậy
- Quản lý kết nối giữa Client và Server
- Files: `Client/Networking/Client.cs`, `Server/Networking/ListenServer.cs`

**Transport Layer:**
- Uses **TCP** to ensure reliable data transmission
- Manages connection between Client and Server
- Files: `Client/Networking/Client.cs`, `Server/Networking/ListenServer.cs`

#### 3. Tầng Mạng (Internet Layer)
- Sử dụng **IP** để định tuyến gói tin giữa Client và Server
- Xác định địa chỉ IP của Client và Server

**Internet Layer:**
- Uses **IP** for routing packets between Client and Server
- Identifies IP addresses of Client and Server

#### 4. Tầng Liên kết dữ liệu (Network Access Layer)
- Xử lý truyền dữ liệu vật lý qua mạng (Ethernet, WiFi, etc.)

**Network Access Layer:**
- Handles physical data transmission over network (Ethernet, WiFi, etc.)

---

**Lưu ý quan trọng:** Chức năng Remote Desktop hoạt động ở **Tầng Ứng dụng (Application Layer)** vì nó xử lý logic nghiệp vụ như chụp màn hình, mã hóa/giải mã hình ảnh, và điều khiển thiết bị đầu vào. Các tầng bên dưới (Transport, Internet, Network Access) chỉ đảm nhiệm việc truyền tải dữ liệu một cách tin cậy.

**Important Note:** The Remote Desktop function operates at the **Application Layer** because it handles business logic such as screen capture, image encoding/decoding, and input device control. The lower layers (Transport, Internet, Network Access) are only responsible for reliably transmitting the data.

## Cách hoạt động Remote Desktop / How Remote Desktop Works

### Luồng dữ liệu Remote Desktop / Remote Desktop Data Flow

Remote Desktop hoạt động theo mô hình Client-Server với luồng dữ liệu hai chiều:

Remote Desktop operates on a Client-Server model with bidirectional data flow:

#### 1️⃣ Server → Client: Yêu cầu chụp màn hình / Screen Capture Request

**Bước 1: Server gửi yêu cầu**
1. File: `Server/Networking/Handlers/RemoteDesktopHandler.cs`
2. Function: `BeginReceiveFrames(int quality, int display)` (line 64)
3. Tạo packet: `new GetDesktopPacket { CreateNew = true, Quality = quality, DisplayIndex = display }`
4. Gửi qua: `_client.QueuePacketAsync()` (line 71)

**Bước 2: Packet được serialize**
1. File: `Server/Networking/Packets/RemoteDesktop/GetDesktopPacket.cs`
2. Function: `Serialize(ProcessStream stream)` (line 28)
3. Ghi dữ liệu: `WriteBoolean()`, `WriteInt()` vào stream
4. Stream gửi qua TCP/IP connection

**Bước 3: Client nhận và xử lý**
1. File: `Client/Networking/Client.cs`
2. Function: `GetNextPacketAsync()` đọc packet từ TCP stream (line 150)
3. Function: `ProcessAsync()` gọi handler (line 133-140)
4. File: `Client/Networking/ClientHandler.cs`
5. Function: `HandlePackets(int id, byte[] data)` (line 42)
6. Case `0x70`: Deserialize packet và gọi `_remoteDesktopHandler.GetDesktop()` (line 109)

#### 2️⃣ Client → Server: Gửi hình ảnh màn hình / Send Screen Image

**Bước 1: Client chụp và mã hóa màn hình**
1. File: `Client/Networking/Handlers/RemoteDesktopHandler.cs`
2. Function: `GetDesktop(GetDesktopPacket packet)` (line 22)
3. Chụp màn hình: `ScreenHelper.CaptureScreen(packet.DisplayIndex)` (line 44)
4. Mã hóa: `_streamCodec.CodeImage()` (line 51-54)
5. Tạo packet với image data: `new GetDesktopPacket { Image = stream.ToArray(), ... }` (line 56-62)
6. Gửi: `_client.SendPacket()` (line 56)

**Bước 2: Packet được serialize**
1. File: `Client/Networking/Packets/RemoteDesktop/GetDesktopPacket.cs`
2. Function: `Serialize(ProcessStream stream)` (line 25)
3. Ghi dữ liệu: `WriteByteArray(Image)`, `WriteInt(Quality)`, etc. (line 29-33)
4. File: `Common/Networking/ProcessStream.Writing.cs`
5. Function: `WriteByteArray(byte[] values)` (line 181) - ghi độ dài + dữ liệu
6. Dữ liệu được gửi qua TCP stream

**Bước 3: Server nhận và hiển thị**
1. File: `Server/Networking/Client.cs`
2. Function: `GetNextPacketAsync()` đọc packet từ TCP stream (line 110)
3. Function: `StartConnectionAsync()` gọi handler (line 91-98)
4. File: `Server/Networking/ClientHandler.cs`
5. Function: `HandlePackets(int id, byte[] data)` (line 41)
6. Case `0x70`: Deserialize và gọi `RemoteDesktopHandler.Handler()` (line 110)
7. File: `Server/Networking/Handlers/RemoteDesktopHandler.cs`
8. Function: `Handler(GetDesktopPacket packet)` (line 109)
9. Giải mã: `_codec.DecodeData(ms)` (line 128)
10. Hiển thị: `OnUpdateImageChanged()` gửi event đến UI (line 128)

#### 3️⃣ Server → Client: Điều khiển chuột/bàn phím / Mouse/Keyboard Control

**Gửi lệnh điều khiển chuột:**
1. File: `Server/Networking/Handlers/RemoteDesktopHandler.cs`
2. Function: `SendMouseEvent()` (line 88)
3. Tạo packet: `new MouseEventPacket { Action, X, Y, ... }` (line 92-100)
4. Gửi qua TCP: `_client.QueuePacketAsync()`

**Client thực thi lệnh:**
1. File: `Client/Networking/ClientHandler.cs`
2. Case `0x72`: Gọi `_remoteDesktopHandler.MouseEvent()` (line 115)
3. File: `Client/Networking/Handlers/RemoteDesktopHandler.cs`
4. Function: `MouseEvent(MouseEventPacket packet)` (line 104)
5. Thực thi: `NativeMethodsHelper.DoMouseLeftClick()` hoặc `DoMouseMove()` (line 130-144)

### Tóm tắt cơ chế gửi dữ liệu trên Tầng Ứng dụng / Summary of Application Layer Data Transfer

1. **Serialize**: Đối tượng packet → byte array (sử dụng `ProcessStream.Write*()`)
2. **Send**: byte array → TCP stream (sử dụng `stream.Write()`)
3. **Transport Layer**: TCP tự động chia nhỏ, đánh số thứ tự, gửi qua IP
4. **Receive**: TCP stream → byte array (sử dụng `stream.Read*()`)
5. **Deserialize**: byte array → đối tượng packet (sử dụng `ProcessStream.Read*()`)
6. **Handle**: Packet được xử lý bởi handler tương ứng

**Chu trình liên tục**: Server request → Client capture & send → Server display → Server request (lặp lại)

## Cấu trúc dự án / Project Structure

- **Client**: Ứng dụng client (keylogger) chạy trên máy mục tiêu
- **Server**: Ứng dụng server nhận và quản lý dữ liệu từ các client
- **Common**: Thư viện chung cho cả client và server (networking, models, utilities)

## Cấu hình / Configuration

### Server
- Cấu hình cổng lắng nghe trong `Server/Program.cs`
- Mặc định sử dụng cổng được chỉ định trong constructor của `ListenServer`

### Client  
- Cấu hình địa chỉ IP và cổng server để kết nối
- Thông tin kết nối được truyền vào method `Connect(IPAddress ip, int port)`

## Lưu ý bảo mật / Security Notes

⚠️ **CẢNH BÁO**: Dự án này chỉ dành cho mục đích học tập và nghiên cứu. Việc sử dụng keylogger hoặc botnet trái phép là bất hợp pháp và vi phạm quyền riêng tư.

⚠️ **WARNING**: This project is for educational and research purposes only. Unauthorized use of keyloggers or botnets is illegal and violates privacy rights.
