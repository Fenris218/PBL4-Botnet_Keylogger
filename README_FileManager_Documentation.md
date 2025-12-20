# Tài liệu FileManager - PBL4 Botnet Keylogger

## Tổng quan

Đã tạo tài liệu chi tiết và đầy đủ về chức năng **FileManager** trong hệ thống PBL4-Botnet_Keylogger.

## File tài liệu chính

📄 **FileManager_Documentation.md** (915 dòng)

## Nội dung tài liệu

### 1. Tổng quan
- Giới thiệu chức năng FileManager
- 7 tính năng chính: xem drives, duyệt thư mục, download, upload, xóa, đổi tên, thực thi
- Cơ chế hoạt động: Server UI ↔ Packets ↔ Client File System

### 2. Kiến trúc và các file quan trọng
- **Sơ đồ cấu trúc thư mục**: Server/, Client/, Common/
- **Chi tiết 5 class chính** với mục đích, thuộc tính, và phương thức:
  - `FrmFileManager.cs` (UI Form)
  - `FileManagerHandler.cs` (Server & Client)
  - `FileSplit.cs` (Chunking mechanism)
  - `FileTransfer.cs` (Transfer state)

### 3. Các gói tin (Packets)
- **Bảng tổng hợp 9 packets** với ID (0x40-0x48), hướng, và mục đích
- **Cấu trúc chi tiết** từng packet với C# code examples:
  - GetDrivesPacket
  - GetDirectoryPacket
  - FileTransferRequest
  - FileTransferChunkPacket
  - FileTransferCompletePacket
  - FileTransferCancelPacket
  - PathRenamePacket
  - PathDeletePacket
  - StatusFileManager

### 4. Luồng hoạt động chi tiết
Mô tả **flow diagram từng bước** cho 8 chức năng:

#### 4.1. Khởi tạo File Manager
- UI Load → RefreshDrives → GetDrivesPacket → Populate ComboBox

#### 4.2. Duyệt thư mục
- User action → SwitchDirectory → GetDirectoryPacket → Update ListView

#### 4.3. Download File (Client → Server)
- Flow chi tiết 20+ bước:
  - BeginDownloadFile → FileTransferRequest
  - Client reads file và gửi chunks
  - Server receives chunks và updates progress
  - FileTransferCompletePacket → Status "Completed"

#### 4.4. Upload File (Server → Client)
- Flow chi tiết 20+ bước:
  - BeginUploadFile với Semaphore control
  - Server reads file và gửi chunks
  - Client receives chunks và writes to file
  - FileTransferCompletePacket → Status "Completed"

#### 4.5. Hủy Transfer
- Cancel button → FileTransferCancelPacket → Cleanup resources

#### 4.6. Xóa File/Thư mục
- Delete action → PathDeletePacket → File.Delete/Directory.Delete → Auto refresh

#### 4.7. Đổi tên
- Rename action → PathRenamePacket → File.Move/Directory.Move → Auto refresh

#### 4.8. Thực thi File
- Execute action → Delegate to TaskManager → Process.Start

**Mỗi section có**:
- ASCII flow diagram
- Danh sách file liên quan với số dòng code cụ thể

### 5. Tương tác qua UI
#### 5.1. Mở File Manager
- Right-click Client → Select "File Manager" → CreateNewOrGetExisting

#### 5.2. Các thành phần UI
Chi tiết từng component:
- **cmbDrives**: ComboBox cho drives
- **lstDirectory**: ListView hiển thị files/folders với context menu
- **lstTransfers**: ListView theo dõi transfer progress
- **txtPath**: Path hiện tại
- **stripLblStatus**: Status bar
- **btnOpenDLFolder**: Button mở download folder

#### 5.3. Workflow người dùng
4 kịch bản thực tế:
1. Download file
2. Upload nhiều files (drag & drop supported)
3. Quản lý files (rename, delete)
4. Cancel transfer

#### 5.4. Event Flow trong UI
- User action → Event Handler → FileManagerHandler
- QueuePacketAsync → Network → Client response
- Handler trigger event → SynchronizationContext.Post
- UI thread Invoke() → Update controls

#### 5.5. Thread Safety
- `Invoke()` cho UI updates
- `SynchronizationContext.Post()` cho events
- `_syncLock` cho shared collections
- `Semaphore` giới hạn concurrent transfers

### 6. Sơ đồ tổng quan
#### 6.1. Sơ đồ kiến trúc
ASCII art diagram hiển thị:
- Server: FrmFileManager ↔ FileManagerHandler ↔ Network
- TCP/IP Socket
- Client: ClientHandler → FileManagerHandler → File System

#### 6.2. Packet Flow cho Download
Diagram chi tiết từng packet:
- FileTransferRequest (0x45)
- Multiple FileTransferChunkPacket (0x48)
- FileTransferCompletePacket (0x46)
Với progress tracking mỗi bước

### 7. Lưu ý và Best Practices
#### 7.1. Xử lý lỗi
- Try-catch cho tất cả I/O operations
- Error messages qua StatusFileManager
- UI feedback rõ ràng

#### 7.2. Giới hạn và tối ưu
- **Chunk size**: 8KB (balance performance/overhead)
- **Concurrent transfers**: Server max 5, Client max 4
- **Thread.Sleep(20ms)**: Tránh network congestion
- **Auto rename**: file(1), file(2)... nếu duplicate

#### 7.3. Security considerations
- Path validation needed
- File execution risks
- Full file system access by design

#### 7.4. UI/UX
- Realtime progress tracking
- Drag & drop support
- Multi-select files
- Context menu accessibility
- Auto refresh after operations

### Kết luận
Tài liệu cung cấp:
- ✅ Kiến trúc và cấu trúc code
- ✅ Chi tiết từng packet và flow
- ✅ Hướng dẫn tương tác UI
- ✅ Best practices và considerations
- ✅ Diagrams và visualizations

## Cách sử dụng tài liệu

1. **Đọc theo thứ tự**: Section 1 → 7 để hiểu toàn bộ hệ thống
2. **Tìm kiếm nhanh**: Dùng mục lục để jump đến section cụ thể
3. **Tham khảo flow**: Section 4 cho từng chức năng
4. **Debug**: Dùng packet IDs và file references để trace code
5. **Extend**: Dựa vào patterns để thêm tính năng mới

## Thông tin kỹ thuật

- **Ngôn ngữ**: C# (.NET)
- **UI Framework**: Windows Forms
- **Network**: TCP/IP with custom packet protocol
- **File handling**: Stream-based chunking (8KB chunks)
- **Concurrency**: Semaphore-controlled thread pool

## Liên hệ

Tài liệu được tạo để giải thích chi tiết luồng hoạt động FileManager theo yêu cầu.

---

*Tài liệu này được tạo tự động dựa trên phân tích code base PBL4-Botnet_Keylogger*
