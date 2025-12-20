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
