# TÀI LIỆU CHI TIẾT CHỨC NĂNG FILEMANAGER

## MỤC LỤC
1. [Tổng quan](#1-tổng-quan)
2. [Kiến trúc và các file quan trọng](#2-kiến-trúc-và-các-file-quan-trọng)
3. [Các gói tin (Packets)](#3-các-gói-tin-packets)
4. [Luồng hoạt động chi tiết](#4-luồng-hoạt-động-chi-tiết)
5. [Tương tác qua UI](#5-tương-tác-qua-ui)

---

## 1. TỔNG QUAN

FileManager là chức năng cho phép Server điều khiển và quản lý hệ thống file trên máy Client từ xa thông qua giao diện UI. Server có thể:
- Xem danh sách ổ đĩa
- Duyệt thư mục và file
- Tải file từ Client về Server (Download)
- Tải file từ Server lên Client (Upload)
- Xóa file/thư mục
- Đổi tên file/thư mục
- Thực thi file trên Client

### Cơ chế hoạt động chung:
- **Server**: Gửi yêu cầu và hiển thị kết quả qua UI (Windows Forms)
- **Client**: Nhận yêu cầu, thực hiện thao tác trên hệ thống file, và gửi phản hồi
- **Truyền thông**: Sử dụng các gói tin (packets) được serialize/deserialize qua TCP socket

---

## 2. KIẾN TRÚC VÀ CÁC FILE QUAN TRỌNG

### 2.1. Cấu trúc thư mục

```
PBL4-Botnet_Keylogger/
├── Server/
│   ├── Forms/
│   │   ├── FrmFileManager.cs              # Giao diện UI của File Manager
│   │   └── FrmFileManager.Designer.cs     # UI Designer code
│   ├── Networking/
│   │   ├── Handlers/
│   │   │   └── FileManagerHandler.cs      # Xử lý logic FileManager phía Server
│   │   ├── ClientHandler.cs               # Routing packets đến handler tương ứng
│   │   └── Packets/FileManager/           # Định nghĩa các packets gửi từ Server
│   │       ├── GetDrivesPacket.cs         # Packet yêu cầu danh sách ổ đĩa
│   │       ├── GetDirectoryPacket.cs      # Packet yêu cầu nội dung thư mục
│   │       ├── FileTransferRequest.cs     # Packet yêu cầu download file
│   │       ├── FileTransferChunkPacket.cs # Packet chứa chunk data
│   │       ├── FileTransferCompletePacket.cs # Packet báo hoàn thành
│   │       ├── FileTransferCancelPacket.cs   # Packet hủy transfer
│   │       ├── PathRenamePacket.cs        # Packet đổi tên
│   │       ├── PathDeletePacket.cs        # Packet xóa
│   │       └── StatusFileManager.cs       # Packet thông báo trạng thái
│
├── Client/
│   ├── Networking/
│   │   ├── Handlers/
│   │   │   └── FileManagerHandler.cs      # Xử lý logic FileManager phía Client
│   │   ├── ClientHandler.cs               # Routing packets đến handler tương ứng
│   │   └── Packets/FileManager/           # Định nghĩa các packets gửi từ Client
│   │       ├── GetDrivesPacket.cs
│   │       ├── GetDirectoryPacket.cs
│   │       ├── FileTransferRequestPacket.cs
│   │       ├── FileTransferChunkPacket.cs
│   │       ├── FileTransferCompletePacket.cs
│   │       ├── FileTransferCancelPacket.cs
│   │       ├── PathRenamePacket.cs
│   │       ├── PathDeletePacket.cs
│   │       └── StatusFileManager.cs
│
└── Common/
    ├── Models/
    │   ├── FileTransfer.cs                # Model quản lý thông tin transfer
    │   ├── FileChunk.cs                   # Model đại diện cho một chunk dữ liệu
    │   ├── FileSystemEntry.cs             # Model đại diện file/folder
    │   └── Drive.cs                       # Model đại diện ổ đĩa
    ├── IO/
    │   └── FileSplit.cs                   # Class xử lý chia file thành chunks
    └── Enums/
        └── FileType.cs                    # Enum: Directory, File, Back
```

### 2.2. Các class và chức năng chính

#### **Server/Forms/FrmFileManager.cs**
- **Mục đích**: Giao diện Windows Form để người dùng tương tác
- **Thành phần UI chính**:
  - `cmbDrives`: ComboBox hiển thị danh sách ổ đĩa
  - `lstDirectory`: ListView hiển thị file/folder
  - `lstTransfers`: ListView hiển thị tiến trình download/upload
  - `txtPath`: TextBox hiển thị đường dẫn hiện tại
  
- **Các phương thức quan trọng**:
  - `FrmFileManager(Client client)`: Constructor, khởi tạo handler và đăng ký events
  - `RegisterMessageHandler()`: Đăng ký các event handlers
  - `DrivesChanged()`: Xử lý khi nhận danh sách drives
  - `DirectoryChanged()`: Xử lý khi nhận nội dung thư mục
  - `FileTransferUpdated()`: Cập nhật tiến trình transfer
  - `SwitchDirectory(string remotePath)`: Chuyển thư mục
  - `downloadToolStripMenuItem_Click()`: Xử lý khi user click Download
  - `uploadToolStripMenuItem_Click()`: Xử lý khi user click Upload
  - `deleteToolStripMenuItem_Click()`: Xử lý khi user click Delete
  - `renameToolStripMenuItem_Click()`: Xử lý khi user click Rename
  - `executeToolStripMenuItem_Click()`: Xử lý khi user click Execute

#### **Server/Networking/Handlers/FileManagerHandler.cs**
- **Mục đích**: Xử lý logic nghiệp vụ FileManager phía Server
- **Các thuộc tính quan trọng**:
  - `_activeFileTransfers`: List quản lý các transfer đang hoạt động
  - `_limitThreads`: Semaphore giới hạn 5 transfer đồng thời
  - `_baseDownloadPath`: Đường dẫn lưu file download
  
- **Các phương thức quan trọng**:
  - `RefreshDrives()`: Gửi yêu cầu lấy danh sách drives (line 255-258)
  - `GetDirectoryContents(string remotePath)`: Gửi yêu cầu lấy nội dung thư mục (line 250-253)
  - `BeginDownloadFile(string remotePath, ...)`: Bắt đầu download file (line 78-130)
  - `BeginUploadFile(string localPath, string remotePath)`: Bắt đầu upload file (line 132-223)
  - `CancelFileTransfer(int transferId)`: Hủy transfer (line 225-228)
  - `RenameFile(...)`: Đổi tên file/folder (line 230-238)
  - `DeleteFile(...)`: Xóa file/folder (line 240-243)
  - `StartProcess(string remotePath)`: Thực thi file (line 245-248)
  - `Handler(GetDrivesPacket packet)`: Xử lý phản hồi drives (line 265-270)
  - `Handler(GetDirectoryPacket packet)`: Xử lý phản hồi directory (line 272-279)
  - `Handler(FileTransferChunkPacket packet)`: Xử lý chunk nhận được (line 317-347)
  - `Handler(FileTransferCompletePacket packet)`: Xử lý khi hoàn thành (line 281-296)
  - `Handler(FileTransferCancelPacket packet)`: Xử lý khi hủy (line 298-315)

#### **Client/Networking/Handlers/FileManagerHandler.cs**
- **Mục đích**: Xử lý logic nghiệp vụ FileManager phía Client
- **Các thuộc tính quan trọng**:
  - `_activeTransfers`: Dictionary quản lý các FileSplit đang active
  - `_limitThreads`: Semaphore giới hạn 4 transfer đồng thời
  
- **Các phương thức quan trọng**:
  - `GetDrives()`: Lấy danh sách drives và gửi về Server (line 47-96)
  - `GetDirectory(GetDirectoryPacket packet)`: Lấy nội dung thư mục và gửi về (line 98-177)
  - `PathRename(PathRenamePacket packet)`: Đổi tên file/folder (line 179-239)
  - `PathDelete(PathDeletePacket packet)`: Xóa file/folder (line 241-301)
  - `FileTransferRequest(FileTransferRequestPacket packet)`: Xử lý yêu cầu download, đọc file và gửi chunks (line 303-348)
  - `FileTransferChunk(FileTransferChunkPacket packet)`: Nhận chunk và ghi vào file (upload) (line 363-411)
  - `FileTransferCancel(FileTransferCancelPacket packet)`: Hủy transfer (line 350-361)

#### **Common/IO/FileSplit.cs**
- **Mục đích**: Class xử lý việc chia file thành các chunk nhỏ để truyền
- **Thuộc tính**:
  - `MaxChunkSize = 8KB`: Kích thước tối đa mỗi chunk
  - `FilePath`: Đường dẫn file
  - `FileSize`: Kích thước file
  
- **Các phương thức**:
  - `WriteChunk(FileChunk chunk)`: Ghi một chunk vào file (line 30-37)
  - `ReadChunk(long offset)`: Đọc một chunk từ vị trí offset (line 40-56)
  - `GetEnumerator()`: Hỗ trợ foreach để duyệt qua các chunks (line 59-65)

#### **Common/Models/FileTransfer.cs**
- **Mục đích**: Model lưu thông tin về một transfer đang diễn ra
- **Thuộc tính**:
  - `Id`: ID duy nhất của transfer
  - `Type`: Download hoặc Upload
  - `Size`: Kích thước file
  - `TransferredSize`: Số byte đã truyền
  - `LocalPath`: Đường dẫn local (trên Server)
  - `RemotePath`: Đường dẫn remote (trên Client)
  - `Status`: Trạng thái (Pending, Downloading, Uploading, Completed, Canceled)
  - `FileSplit`: Instance của FileSplit để đọc/ghi file

---

## 3. CÁC GÓI TIN (PACKETS)

### 3.1. Bảng tổng hợp Packet IDs

| ID   | Tên Packet                  | Hướng          | Mục đích |
|------|-----------------------------|----------------|----------|
| 0x40 | StatusFileManager           | Client → Server | Thông báo trạng thái/lỗi |
| 0x41 | GetDrivesPacket             | Bidirectional  | Request drives / Response với danh sách drives |
| 0x42 | GetDirectoryPacket          | Bidirectional  | Request directory / Response với nội dung |
| 0x43 | PathRenamePacket            | Server → Client | Yêu cầu đổi tên |
| 0x44 | PathDeletePacket            | Server → Client | Yêu cầu xóa |
| 0x45 | FileTransferRequest         | Server → Client | Yêu cầu download file |
| 0x46 | FileTransferCompletePacket  | Client → Server | Báo transfer hoàn thành |
| 0x47 | FileTransferCancelPacket    | Bidirectional  | Hủy transfer |
| 0x48 | FileTransferChunkPacket     | Bidirectional  | Truyền chunk dữ liệu |

### 3.2. Chi tiết cấu trúc Packets

#### **GetDrivesPacket (0x41)**
```csharp
// Request: Server → Client (empty payload)
// Response: Client → Server
{
    List<Drive> Drives {
        string DisplayName,  // Ví dụ: "C:\ (Windows) [Fixed, NTFS]"
        string RootDirectory // Ví dụ: "C:\"
    }
}
```

#### **GetDirectoryPacket (0x42)**
```csharp
// Request: Server → Client
{
    string RemotePath  // Đường dẫn thư mục cần xem
}

// Response: Client → Server
{
    string RemotePath,
    List<FileSystemEntry> Items {
        FileType EntryType,      // Directory hoặc File
        string Name,             // Tên file/folder
        long Size,               // Kích thước (byte)
        DateTime LastAccessTimeUtc,
        ContentType ContentType  // Loại file (exe, txt, image, ...)
    }
}
```

#### **PathRenamePacket (0x43)**
```csharp
// Server → Client
{
    string Path,        // Đường dẫn hiện tại
    string NewPath,     // Đường dẫn mới
    FileType PathType   // Directory hoặc File
}
```

#### **PathDeletePacket (0x44)**
```csharp
// Server → Client
{
    string Path,        // Đường dẫn cần xóa
    FileType PathType   // Directory hoặc File
}
```

#### **FileTransferRequest (0x45)**
```csharp
// Server → Client (yêu cầu download)
{
    int IdRequest,      // ID duy nhất của request
    string RemotePath   // Đường dẫn file cần download
}
```

#### **FileTransferChunkPacket (0x48)**
```csharp
// Bidirectional
{
    int IdRequest,          // ID của transfer
    string FilePath,        // Đường dẫn file
    long FileSize,          // Kích thước tổng file
    FileChunk Chunk {
        long Offset,        // Vị trí chunk trong file
        byte[] Data         // Dữ liệu (tối đa 8KB)
    }
}
```

#### **FileTransferCompletePacket (0x46)**
```csharp
// Client → Server
{
    int IdRequest,      // ID của transfer
    string FilePath     // Đường dẫn file
}
```

#### **FileTransferCancelPacket (0x47)**
```csharp
// Bidirectional
{
    int IdRequest,      // ID của transfer
    string Reason       // Lý do hủy: "Canceled", "Error reading file", ...
}
```

#### **StatusFileManager (0x40)**
```csharp
// Client → Server
{
    string Message,              // Thông báo trạng thái
    bool SetLastDirectorySeen    // Có refresh directory không
}
```

---

## 4. LUỒNG HOẠT ĐỘNG CHI TIẾT

### 4.1. Khởi tạo File Manager

#### **Flow:**
```
User (Server) → FrmFileManager.Load
  ↓
Server: FileManagerHandler.RefreshDrives()
  ↓
Server → Client: GetDrivesPacket (0x41) [empty]
  ↓
Client: FileManagerHandler.GetDrives()
  - Gọi DriveInfo.GetDrives() để lấy danh sách ổ đĩa
  - Tạo List<Drive> với thông tin: DisplayName, RootDirectory
  ↓
Client → Server: GetDrivesPacket (0x41) [với Drives data]
  ↓
Server: FileManagerHandler.Handler(GetDrivesPacket)
  - Trigger event DrivesChanged
  ↓
UI: FrmFileManager.DrivesChanged()
  - Cập nhật ComboBox cmbDrives với danh sách drives
  - Hiển thị trong UI
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 218-222, 77-88), `FileManagerHandler.cs` (line 255-258, 265-270)
- **Client**: `FileManagerHandler.cs` (line 47-96)
- **Packet**: `Server/Packets/FileManager/GetDrivesPacket.cs`, `Client/Packets/FileManager/GetDrivesPacket.cs`

---

### 4.2. Duyệt thư mục (Browse Directory)

#### **Flow:**
```
User chọn drive hoặc double-click folder
  ↓
UI: FrmFileManager.cmbDrives_SelectedIndexChanged() hoặc lstDirectory_DoubleClick()
  ↓
UI: FrmFileManager.SwitchDirectory(remotePath)
  ↓
Server: FileManagerHandler.GetDirectoryContents(remotePath)
  ↓
Server → Client: GetDirectoryPacket (0x42) [với RemotePath]
  ↓
Client: FileManagerHandler.GetDirectory(packet)
  - DirectoryInfo.GetFiles() → lấy danh sách files
  - DirectoryInfo.GetDirectories() → lấy danh sách folders
  - Tạo List<FileSystemEntry> với thông tin mỗi item
  ↓
Client → Server: GetDirectoryPacket (0x42) [với RemotePath + Items]
  ↓
Server: FileManagerHandler.Handler(GetDirectoryPacket)
  - Trigger event DirectoryChanged
  ↓
UI: FrmFileManager.DirectoryChanged()
  - Cập nhật txtPath với đường dẫn
  - Clear lstDirectory
  - Add item ".." để quay lại
  - Add từng FileSystemEntry vào ListView
  - Hiển thị tên, kích thước, loại file
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 90-118, 208-216, 229-232, 234-250), `FileManagerHandler.cs` (line 250-253, 272-279)
- **Client**: `FileManagerHandler.cs` (line 98-177)
- **Packet**: `Server/Packets/FileManager/GetDirectoryPacket.cs`, `Client/Packets/FileManager/GetDirectoryPacket.cs`

---

### 4.3. Download File (Client → Server)

#### **Flow:**
```
User chọn file và click "Download" trong context menu
  ↓
UI: FrmFileManager.downloadToolStripMenuItem_Click()
  - Lấy remotePath của file được chọn
  ↓
Server: FileManagerHandler.BeginDownloadFile(remotePath)
  - Tạo ID duy nhất cho transfer
  - Tạo đường dẫn local để lưu file (trong DownloadDirectory)
  - Kiểm tra trùng tên, tự động đổi tên nếu cần: file(1).txt, file(2).txt...
  - Tạo FileTransfer object với Type=Download, Status="Pending"
  - Mở FileSplit để chuẩn bị ghi file (FileAccess.Write)
  - Add vào _activeFileTransfers
  - Trigger FileTransferUpdated để hiển thị trong UI
  ↓
Server → Client: FileTransferRequest (0x45) [IdRequest, RemotePath]
  ↓
Client: FileManagerHandler.FileTransferRequest(packet)
  - Mở file để đọc bằng FileSplit (FileAccess.Read)
  - Lưu vào _activeTransfers
  - Tạo thread mới để xử lý
  - Duyệt qua từng chunk trong FileSplit (foreach)
    ↓
    [Lặp cho mỗi chunk]
    Client → Server: FileTransferChunkPacket (0x48)
      [IdRequest, FilePath, FileSize, Chunk{Offset, Data}]
    ↓
    Server: FileManagerHandler.Handler(FileTransferChunkPacket)
      - Tìm FileTransfer theo IdRequest
      - Cập nhật Size và TransferredSize
      - Ghi chunk vào file: FileSplit.WriteChunk(chunk)
      - Tính progress = (TransferredSize / Size) * 100
      - Cập nhật Status = "Downloading...(X%)"
      - Trigger FileTransferUpdated
      ↓
      UI: FrmFileManager.FileTransferUpdated()
        - Cập nhật progress trong lstTransfers
    ↓
    [Lặp lại cho chunk tiếp theo]
  ↓
  [Khi đã gửi hết chunks]
  Client → Server: FileTransferCompletePacket (0x46) [IdRequest, FilePath]
  ↓
Server: FileManagerHandler.Handler(FileTransferCompletePacket)
  - Tìm FileTransfer theo IdRequest
  - Cập nhật Status = "Completed"
  - Remove khỏi _activeFileTransfers
  - Dispose FileSplit
  - Trigger FileTransferUpdated
  ↓
UI: FrmFileManager.FileTransferUpdated()
  - Hiển thị icon "Completed" và status cuối cùng
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 252-265, 120-140), `FileManagerHandler.cs` (line 78-130, 317-347, 281-296)
- **Client**: `FileManagerHandler.cs` (line 303-348)
- **Packet**: `FileTransferRequest.cs`, `FileTransferChunkPacket.cs`, `FileTransferCompletePacket.cs`
- **Common**: `FileSplit.cs` (line 30-37, 59-65), `FileTransfer.cs`, `FileChunk.cs`

---

### 4.4. Upload File (Server → Client)

#### **Flow:**
```
User click "Upload" trong context menu hoặc drag & drop file
  ↓
UI: FrmFileManager.uploadToolStripMenuItem_Click() hoặc lstDirectory_DragDrop()
  - Mở OpenFileDialog để chọn file (có thể chọn nhiều file)
  - Lấy localFilePath từ dialog
  - Tạo remotePath = currentDir + fileName
  ↓
Server: FileManagerHandler.BeginUploadFile(localPath, remotePath)
  - Tạo thread mới để xử lý
  - Tạo ID duy nhất cho transfer
  - Tạo FileTransfer object với Type=Upload, Status="Pending"
  - Mở file local để đọc bằng FileSplit (FileAccess.Read)
  - Lấy FileSize từ FileSplit
  - Add vào _activeFileTransfers
  - Trigger FileTransferUpdated
  - Đợi Semaphore (_limitThreads) để giới hạn số transfer đồng thời
  - Duyệt qua từng chunk trong FileSplit (foreach)
    ↓
    [Lặp cho mỗi chunk]
    - Cập nhật TransferredSize
    - Tính progress = (TransferredSize / Size) * 100
    - Cập nhật Status = "Uploading...(X%)"
    - Trigger FileTransferUpdated
      ↓
      UI: FrmFileManager.FileTransferUpdated()
        - Hiển thị progress trong lstTransfers
    ↓
    Server → Client: FileTransferChunkPacket (0x48)
      [IdRequest, FilePath=remotePath, FileSize, Chunk{Offset, Data}]
    ↓
    Client: FileManagerHandler.FileTransferChunk(packet)
      - Nếu Offset = 0 (chunk đầu tiên):
        * Xóa file cũ nếu tồn tại
        * Tạo FileSplit mới để ghi (FileAccess.Write)
        * Lưu vào _activeTransfers
      - Lấy FileSplit từ _activeTransfers
      - Ghi chunk vào file: FileSplit.WriteChunk(chunk)
      - Kiểm tra nếu đã nhận đủ: destFile.FileSize == packet.FileSize
    ↓
    [Lặp lại cho chunk tiếp theo]
  ↓
  [Khi Client đã nhận đủ tất cả chunks]
  Client → Server: FileTransferCompletePacket (0x46) [IdRequest, FilePath]
  ↓
Server: FileManagerHandler.Handler(FileTransferCompletePacket)
  - Tìm FileTransfer theo IdRequest
  - Cập nhật Status = "Completed"
  - Remove khỏi _activeFileTransfers
  - Dispose FileSplit
  - Release Semaphore
  - Trigger FileTransferUpdated
  ↓
UI: FrmFileManager.FileTransferUpdated()
  - Hiển thị icon "Completed"
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 267-287, 413-433, 120-140), `FileManagerHandler.cs` (line 132-223, 281-296)
- **Client**: `FileManagerHandler.cs` (line 363-411)
- **Packet**: `FileTransferChunkPacket.cs`, `FileTransferCompletePacket.cs`
- **Common**: `FileSplit.cs`, `FileTransfer.cs`, `FileChunk.cs`

---
### 4.5. Hủy Transfer (Cancel)

#### **Flow:**
```
User chọn transfer đang chạy và click "Cancel"
  ↓
UI: FrmFileManager.cancelToolStripMenuItem_Click()
  - Kiểm tra Status là "Downloading", "Uploading", hoặc "Pending"
  - Lấy IdRequest
  ↓
Server: FileManagerHandler.CancelFileTransfer(transferId)
  ↓
Server → Client: FileTransferCancelPacket (0x47) [IdRequest]
  ↓
Client: FileManagerHandler.FileTransferCancel(packet)
  - Tìm transfer trong _activeTransfers
  - Remove transfer và Dispose FileSplit
  ↓
Client → Server: FileTransferCancelPacket (0x47) [IdRequest, Reason="Canceled"]
  ↓
Server: FileManagerHandler.Handler(FileTransferCancelPacket)
  - Tìm FileTransfer theo IdRequest
  - Cập nhật Status = Reason (Canceled / Error...)
  - Remove khỏi _activeFileTransfers
  - Nếu là Download: Xóa file chưa hoàn thành
  - Trigger FileTransferUpdated
  ↓
UI: FrmFileManager.FileTransferUpdated()
  - Hiển thị icon "Canceled" và trạng thái
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 388-400), `FileManagerHandler.cs` (line 225-228, 298-315)
- **Client**: `FileManagerHandler.cs` (line 350-361)
- **Packet**: `FileTransferCancelPacket.cs`

---

### 4.6. Xóa File/Thư mục (Delete)

#### **Flow:**
```
User chọn file/folder và click "Delete" trong context menu
  ↓
UI: FrmFileManager.deleteToolStripMenuItem_Click()
  - Hiển thị MessageBox xác nhận
  - Nếu Yes:
    * Lấy path và FileType (Directory/File) từ Tag
    * Gọi GetAbsolutePath() để lấy đường dẫn đầy đủ
    ↓
    Server: FileManagerHandler.DeleteFile(path, type)
      ↓
      Server → Client: PathDeletePacket (0x44) [Path, PathType]
      ↓
      Client: FileManagerHandler.PathDelete(packet)
        - Kiểm tra PathType:
          * Nếu Directory: Directory.Delete(path, recursive=true)
          * Nếu File: File.Delete(path)
        ↓
        Client → Server: StatusFileManager (0x40) ["Deleted directory/file"]
        ↓
        Client: Tự động refresh directory:
          - Gọi GetDirectory với Path.GetDirectoryName(packet.Path)
        ↓
        Client → Server: GetDirectoryPacket (0x42) [với nội dung mới]
      ↓
      Server: FileManagerHandler.Handler(StatusFileManager)
        - Hiển thị message trong status bar
      ↓
      Server: FileManagerHandler.Handler(GetDirectoryPacket)
        - Trigger DirectoryChanged
      ↓
      UI: FrmFileManager.DirectoryChanged()
        - Refresh danh sách, file/folder đã bị xóa không còn nữa
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 327-348), `FileManagerHandler.cs` (line 240-243, 260-263)
- **Client**: `FileManagerHandler.cs` (line 241-301)
- **Packet**: `PathDeletePacket.cs`, `StatusFileManager.cs`

---

### 4.7. Đổi tên File/Thư mục (Rename)

#### **Flow:**
```
User chọn file/folder và click "Rename" trong context menu
  ↓
UI: FrmFileManager.renameToolStripMenuItem_Click()
  - Hiển thị InputBox với tên hiện tại
  - User nhập tên mới
  - Nếu OK:
    * Lấy oldPath và FileType từ Tag
    * Tạo newPath = GetAbsolutePath(newName)
    ↓
    Server: FileManagerHandler.RenameFile(oldPath, newPath, type)
      ↓
      Server → Client: PathRenamePacket (0x43) [Path, NewPath, PathType]
      ↓
      Client: FileManagerHandler.PathRename(packet)
        - Kiểm tra PathType:
          * Nếu Directory: Directory.Move(oldPath, newPath)
          * Nếu File: File.Move(oldPath, newPath)
        ↓
        Client → Server: StatusFileManager (0x40) ["Renamed directory/file"]
        ↓
        Client: Tự động refresh directory:
          - Gọi GetDirectory với Path.GetDirectoryName(newPath)
        ↓
        Client → Server: GetDirectoryPacket (0x42) [với nội dung mới]
      ↓
      Server: FileManagerHandler.Handler(StatusFileManager)
        - Hiển thị message trong status bar
      ↓
      Server: FileManagerHandler.Handler(GetDirectoryPacket)
        - Trigger DirectoryChanged
      ↓
      UI: FrmFileManager.DirectoryChanged()
        - Refresh danh sách, file/folder hiển thị với tên mới
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 304-325), `FileManagerHandler.cs` (line 230-238, 260-263)
- **Client**: `FileManagerHandler.cs` (line 179-239)
- **Packet**: `PathRenamePacket.cs`, `StatusFileManager.cs`

---

### 4.8. Thực thi File (Execute)

#### **Flow:**
```
User chọn file .exe và click "Execute" trong context menu
  ↓
UI: FrmFileManager.executeToolStripMenuItem_Click()
  - Lấy remotePath của file
  ↓
Server: FileManagerHandler.StartProcess(remotePath)
  - Delegate cho TaskManagerHandler.StartProcess()
  ↓
Server → Client: ProcessActionPacket (không thuộc FileManager, ID khác)
  ↓
Client: Thực thi file bằng Process.Start()
  ↓
Client → Server: Phản hồi kết quả thực thi
  ↓
Server: ProcessActionPerformed event
  ↓
Server: FileManagerHandler.ProcessActionPerformed()
  - Hiển thị "Process started successfully" hoặc "Process failed to start"
  ↓
UI: FrmFileManager.SetStatusMessage()
  - Hiển thị trong status bar
```

#### **Các file liên quan:**
- **Server**: `FrmFileManager.cs` (line 289-302), `FileManagerHandler.cs` (line 245-248, 68-76)
- **Client**: TaskManagerHandler (không trong scope FileManager)
- **Lưu ý**: Chức năng này sử dụng TaskManager subsystem, không phải FileManager packets

---

## 5. TƯƠNG TÁC QUA UI

### 5.1. Mở File Manager

```
User click chuột phải lên một Client trong danh sách
  ↓
Chọn "File Manager" từ context menu
  ↓
FrmMain.fileManagerToolStripMenuItem_Click() (Server/Forms/FrmMain.cs)
  ↓
FrmFileManager.CreateNewOrGetExisting(client)
  - Kiểm tra xem đã có form mở cho client này chưa
  - Nếu có: trả về form đang mở
  - Nếu chưa: tạo form mới
  ↓
form.Show()
  ↓
FrmFileManager_Load event
  - Set window title với thông tin client
  - Gọi RefreshDrives() để load danh sách ổ đĩa
```

### 5.2. Các thành phần UI chính

#### **ComboBox Drives (cmbDrives)**
- **Mục đích**: Hiển thị và chọn ổ đĩa
- **Event**: `SelectedIndexChanged` → Gọi `SwitchDirectory()` với RootDirectory được chọn
- **Hiển thị**: "C:\\ (Windows) [Fixed, NTFS]"

#### **TextBox Path (txtPath)**
- **Mục đích**: Hiển thị đường dẫn thư mục hiện tại
- **Chỉ đọc**: User không thể edit trực tiếp

#### **ListView Directory (lstDirectory)**
- **Columns**: Name, Size, Type
- **Hiển thị**: Danh sách file và folder trong thư mục hiện tại
- **Tag**: Chứa FileManagerListTag với FileType và Size
- **Events**:
  - `DoubleClick`: Vào thư mục hoặc quay lại (..)
  - `DragEnter` + `DragDrop`: Upload file bằng drag & drop
- **Context Menu**:
  - Download: Tải file về máy Server
  - Upload: Tải file lên Client
  - Execute: Chạy file trên Client
  - Rename: Đổi tên file/folder
  - Delete: Xóa file/folder
  - Refresh: Làm mới danh sách
  - Open Directory: Mở Remote Shell tại thư mục này

#### **ListView Transfers (lstTransfers)**
- **Columns**: Id, Type (Download/Upload), Status, Remote Path
- **Hiển thị**: Danh sách các transfer đang chạy/đã hoàn thành
- **Tag**: Chứa FileTransfer object
- **ImageIndex**: Icon tương ứng (Completed, Canceled, ...)
- **Context Menu**:
  - Cancel: Hủy transfer đang chạy
  - Clear: Xóa các transfer đã hoàn thành khỏi danh sách

#### **Status Bar (stripLblStatus)**
- **Mục đích**: Hiển thị trạng thái hiện tại
- **Messages**: "Ready", "Loading directory content...", "Renamed file", "Deleted directory", etc.

#### **Button Open Download Folder (btnOpenDLFolder)**
- **Mục đích**: Mở Windows Explorer tại folder chứa các file đã download
- **Action**: Gọi `Process.Start("explorer.exe", downloadDirectory)`

### 5.3. Workflow tương tác người dùng

#### **Kịch bản 1: Download một file**
1. User double-click vào các folder để navigate đến file cần download
2. Right-click lên file → chọn "Download"
3. Quan sát progress trong tab "Transfers"
4. Khi Status = "Completed", click "Open Download Folder" để xem file

#### **Kịch bản 2: Upload nhiều file**
1. Navigate đến thư mục đích trên Client
2. Chọn một trong hai cách:
   - Cách 1: Right-click → Upload → Chọn files trong OpenFileDialog
   - Cách 2: Drag & drop files từ Windows Explorer vào lstDirectory
3. Quan sát progress của từng file trong tab "Transfers"

#### **Kịch bản 3: Quản lý file**
1. Navigate đến thư mục chứa file
2. Right-click lên file/folder:
   - Rename: Nhập tên mới trong InputBox
   - Delete: Xác nhận xóa trong MessageBox
3. Danh sách tự động refresh sau khi thao tác

#### **Kịch bản 4: Cancel transfer**
1. Trong tab "Transfers", chọn transfer đang chạy (Status = "Downloading..." hoặc "Uploading...")
2. Right-click → Cancel
3. Status chuyển thành "Canceled"

### 5.4. Event Flow trong UI

```
User Action (Click, DoubleClick, Drag&Drop, ...)
  ↓
Event Handler trong FrmFileManager (UI thread)
  ↓
Gọi method trong FileManagerHandler
  ↓
FileManagerHandler gửi packet qua QueuePacketAsync() (async)
  ↓
[Đợi phản hồi từ Client...]
  ↓
Nhận packet từ Client (network thread)
  ↓
ClientHandler.HandlePacket() route đến FileManagerHandler.Handler()
  ↓
Handler trigger event (StatusMessageChanged, DrivesChanged, DirectoryChanged, FileTransferUpdated)
  ↓
Event được post về UI thread qua SynchronizationContext
  ↓
Event handler trong FrmFileManager (UI thread)
  ↓
Invoke() để cập nhật UI controls (ComboBox, ListView, TextBox, StatusBar)
  ↓
User thấy kết quả trên màn hình
```

### 5.5. Thread Safety

- **UI Updates**: Tất cả cập nhật UI đều sử dụng `Invoke()` hoặc extension method `.Invoke(() => {...})` để đảm bảo chạy trên UI thread
- **Event Posting**: FileManagerHandler sử dụng `SynchronizationContext.Post()` để post events về UI thread
- **Lock**: `_syncLock` object được sử dụng để đồng bộ truy cập `_activeFileTransfers` list
- **Semaphore**: Giới hạn số lượng transfer đồng thời (Server: 5, Client: 4)

---

## 6. SƠ ĐỒ TỔNG QUAN

### 6.1. Sơ đồ kiến trúc

```
┌────────────────────────────── SERVER ──────────────────────────────┐
│                                                                      │
│  ┌──────────────────┐         ┌─────────────────────────────────┐ │
│  │ FrmFileManager   │◄────────│  FileManagerHandler             │ │
│  │ (UI)             │  Events │  (Business Logic)               │ │
│  │ - cmbDrives      │         │  - BeginDownloadFile()          │ │
│  │ - lstDirectory   │         │  - BeginUploadFile()            │ │
│  │ - lstTransfers   │         │  - GetDirectoryContents()       │ │
│  └──────────────────┘         │  - RefreshDrives()              │ │
│                                └───────────────┬─────────────────┘ │
│                                                │                    │
│                                                │ Send/Receive       │
│                                                │ Packets            │
└────────────────────────────────────────────────┼────────────────────┘
                                                 │
                                      ═══════════╪════════════
                                        TCP/IP Socket
                                      ═══════════╪════════════
                                                 │
┌────────────────────────────── CLIENT ─────────┼────────────────────┐
│                                                │                    │
│                                  ┌─────────────▼─────────────────┐ │
│                                  │  ClientHandler.HandlePacket   │ │
│                                  │  (Packet Router)              │ │
│                                  └───────────────┬───────────────┘ │
│                                                  │                 │
│                                  ┌───────────────▼───────────────┐ │
│                                  │  FileManagerHandler           │ │
│                                  │  (Business Logic)             │ │
│                                  │  - GetDrives()                │ │
│                                  │  - GetDirectory()             │ │
│                                  │  - FileTransferRequest()      │ │
│                                  │  - FileTransferChunk()        │ │
│                                  └───────────────┬───────────────┘ │
│                                                  │                 │
│                                                  │ File I/O        │
│                                                  ▼                 │
│                                  ┌───────────────────────────────┐ │
│                                  │  File System Operations       │ │
│                                  │  - DriveInfo.GetDrives()      │ │
│                                  │  - DirectoryInfo.GetFiles()   │ │
│                                  │  - FileSplit Read/Write       │ │
│                                  └───────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

### 6.2. Packet Flow cho Download File

```
SERVER                                           CLIENT
  │                                                │
  │  User clicks "Download"                       │
  ├──► BeginDownloadFile()                        │
  │      - Create FileTransfer (ID=12345)         │
  │      - Open FileSplit for Write               │
  │                                                │
  ├────[FileTransferRequest 0x45]─────────────────►
  │    {IdRequest: 12345, RemotePath: "file.txt"} │
  │                                                │
  │                                                ├──► FileTransferRequest()
  │                                                │      - Open file for Read
  │                                                │      - foreach chunk:
  │◄───[FileTransferChunkPacket 0x48]──────────────┤
  │    Chunk 1: Offset=0, Data[8KB]               │
  │                                                │
  ├──► Handler(FileTransferChunkPacket)           │
  │      - Write chunk to file                    │
  │      - Update progress (8%)                   │
  │      - Update UI                              │
  │                                                │
  │◄───[FileTransferChunkPacket 0x48]──────────────┤
  │    Chunk 2: Offset=8192, Data[8KB]            │
  │                                                │
  │      ... (repeat for all chunks) ...          │
  │                                                │
  │◄───[FileTransferCompletePacket 0x46]───────────┤
  │    {IdRequest: 12345, FilePath: "file.txt"}   │
  │                                                │
  ├──► Handler(FileTransferCompletePacket)        │
  │      - Status = "Completed"                   │
  │      - Close file                             │
  │      - Update UI                              │
```

---

## 7. LƯU Ý VÀ BEST PRACTICES

### 7.1. Xử lý lỗi
- Tất cả các thao tác file I/O đều được bao bọc trong try-catch
- Lỗi được gửi về Server qua `StatusFileManager` packet hoặc `FileTransferCancelPacket`
- UI hiển thị thông báo lỗi rõ ràng: "No permission", "I/O error", "Path not found", etc.

### 7.2. Giới hạn và tối ưu
- **Chunk size**: 8KB - cân bằng giữa hiệu suất và overhead
- **Concurrent transfers**: Server max 5, Client max 4 (Semaphore)
- **Thread.Sleep(20ms)**: Giữa các chunk để tránh làm tắc network
- **File rename**: Tự động đổi tên file(1), file(2)... nếu trùng khi download

### 7.3. Security considerations
- **Path validation**: Cần validate path để tránh path traversal attacks
- **File execution**: Chức năng Execute có thể nguy hiểm, cần cân nhắc
- **Access control**: Client có full access vào file system (by design cho botnet)

### 7.4. UI/UX
- **Progress tracking**: Realtime progress update với phần trăm
- **Drag & drop**: Hỗ trợ drag & drop cho upload thuận tiện
- **Multi-select**: Có thể chọn nhiều file để download cùng lúc
- **Context menu**: Tất cả actions đều accessible qua right-click
- **Auto refresh**: Directory tự động refresh sau delete/rename

---

## KẾT LUẬN

FileManager là một chức năng phức tạp với nhiều tương tác giữa Server-Client, yêu cầu:
- **Đồng bộ hóa**: Thread-safe operations với lock và semaphore
- **Event-driven**: UI updates qua events và synchronization context
- **Chunked transfer**: Chia nhỏ file để truyền hiệu quả
- **Error handling**: Xử lý lỗi toàn diện tại mọi tầng
- **User experience**: Giao diện trực quan, progress tracking, drag & drop

Tài liệu này cung cấp cái nhìn chi tiết về mọi khía cạnh của FileManager, từ kiến trúc code đến flow giao tiếp packets, giúp developers hiểu và maintain hệ thống hiệu quả.
