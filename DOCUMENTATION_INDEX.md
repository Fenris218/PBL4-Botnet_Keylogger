# 📚 Chỉ Mục Tài Liệu - PBL4 Botnet Keylogger

> **Tổng hợp tất cả tài liệu và hướng dẫn cho dự án PBL4 Botnet Keylogger**

---

## 🎯 Mục Đích Tài Liệu

Bộ tài liệu này được tạo ra để giải thích chi tiết **cách hoạt động, logic và flow của chương trình ở cả 2 phía Client và Server** theo các chức năng chính, đồng thời cung cấp **bản thiết kế và code UML một cách chi tiết**.

---

## 📖 Danh Sách Tài Liệu

### 1. README.md (425 dòng, 16KB)
**🎯 Mục đích**: Điểm bắt đầu - Overview tổng quan của project

**📋 Nội dung**:
- Giới thiệu hệ thống và kiến trúc
- Các chức năng chính (Remote Desktop, File Manager, Keylogger, etc.)
- Công nghệ sử dụng
- Cấu trúc project
- Giao thức giao tiếp cơ bản
- Hướng dẫn build và chạy
- Screenshots và diagrams ASCII

**👥 Dành cho**: Tất cả mọi người muốn hiểu tổng quan về project

**⏱️ Thời gian đọc**: 30-45 phút

**🔗 Link**: [README.md](./README.md)

---

### 2. ARCHITECTURE.md (945 dòng, 35KB)
**🎯 Mục đích**: Tài liệu chi tiết nhất về kiến trúc và logic hoạt động

**📋 Nội dung**:
1. **Tổng Quan Hệ Thống**
   - Mô tả chung
   - Công nghệ sử dụng
   - Các thành phần chính

2. **Kiến Trúc Client** (Chi tiết)
   - Cấu trúc tổng quan
   - Luồng khởi động Client
   - Quá trình kết nối
   - Xử lý Packets tại Client (bảng Packet ID mapping)
   - 7 Handler modules chi tiết:
     - ClientServicesHandler
     - SystemInfoHandler
     - RemoteShellHandler
     - TaskManagerHandler
     - FileManagerHandler
     - KeyloggerHandler
     - RemoteDesktopHandler

3. **Kiến Trúc Server** (Chi tiết)
   - Cấu trúc tổng quan
   - Luồng khởi động Server
   - Quá trình chấp nhận kết nối
   - Quá trình Identification
   - Xử lý Packets tại Server
   - 7 Feature Forms chi tiết

4. **Giao Thức Giao Tiếp**
   - ProcessStream Protocol (binary)
   - Packet Types (IPacket, IRequestPacket, IResponsePacket)
   - Packet Flow (gửi và nhận)
   - Packet Queue System

5. **Các Chức Năng Chính** (Chi tiết từng function)
   - Remote Desktop (với flow diagram)
   - File Manager & File Transfer
   - Keylogger
   - Task Manager
   - Remote Shell
   - Activity Detection

6. **Luồng Hoạt Động Chi Tiết**
   - Toàn bộ quá trình từ Startup đến Feature Usage (ASCII diagram)
   - Sequence: Remote Desktop Session
   - State Machine: Client Connection Lifecycle
   - Component Interaction: File Transfer
   - Data Flow: Keylogger

**👥 Dành cho**: Developers muốn hiểu sâu về cách hệ thống hoạt động

**⏱️ Thời gian đọc**: 3-4 giờ (đọc kỹ)

**🔗 Link**: [ARCHITECTURE.md](./ARCHITECTURE.md)

**💡 Highlight**:
- Giải thích logic từng dòng code
- Flow diagrams bằng ASCII art
- Packet ID reference table đầy đủ
- Chi tiết implementation của từng chức năng

---

### 3. UML_DIAGRAMS.md (675 dòng, 16KB)
**🎯 Mục đích**: Cung cấp các biểu đồ UML thiết kế hệ thống

**📋 Nội dung**:
1. **Class Diagram - Tổng Quan Hệ Thống**
   - Common Library
   - Client Application
   - Server Application
   - Relationships

2. **Sequence Diagram - Connection Establishment**
   - Client startup
   - Server listening
   - Connection và Identification
   - Update Server UI

3. **Sequence Diagram - Remote Desktop**
   - Continuous screen capture loop
   - User mouse interaction
   - User keyboard interaction

4. **Sequence Diagram - File Transfer**
   - Browse directory
   - User initiates download
   - Chunked transfer
   - Cancel transfer

5. **Component Diagram**
   - Client Machine components
   - Server Machine components
   - Network layer
   - Common Library

6. **State Diagram - Client Connection Lifecycle**
   - States: Disconnected → Connecting → Connected → Identifying → Identified → Active/Idle
   - Transitions và conditions

7. **Activity Diagram - Keylogger Flow**
   - Application starts
   - Keyboard hook
   - Key processing
   - Write to file
   - Server requests logs

8. **Deployment Diagram**
   - Physical nodes
   - Artifacts deployment
   - Network connections

**👥 Dành cho**: Architects, designers, và developers muốn xem thiết kế visual

**⏱️ Thời gian đọc**: 1-2 giờ

**🔗 Link**: [UML_DIAGRAMS.md](./UML_DIAGRAMS.md)

**💡 Highlight**:
- Tất cả diagrams đều bằng PlantUML (có thể render)
- Hướng dẫn sử dụng PlantUML (online, VS Code, CLI)
- 8 loại diagram khác nhau
- Có thể export sang PNG/SVG

**🛠️ Cách sử dụng**:
```bash
# Online
http://www.plantuml.com/plantuml/uml/

# VS Code
Extension: "PlantUML" → Alt+D

# CLI
java -jar plantuml.jar diagram.puml
```

---

### 4. QUICK_REFERENCE.md (504 dòng, 13KB)
**🎯 Mục đích**: Tài liệu tham khảo nhanh cho developers

**📋 Nội dung**:
1. **Roadmap Tài Liệu**
   - Thứ tự đọc tài liệu đề xuất

2. **Packet ID Reference**
   - Bảng Client → Server Packets (13 packets)
   - Bảng Server → Client Packets (18 packets)
   - Mô tả data của từng packet

3. **Key Classes Reference**
   - Client.Networking.Client
   - ClientHandler
   - KeyloggerService
   - ActivityDetection
   - ListenServer
   - Server.Client
   - ProcessStream
   - Packet Base Classes

4. **Common Patterns**
   - Sending a Packet (async/sync)
   - Handling a Packet
   - Creating a New Packet Type
   - File Transfer Pattern
   - Remote Desktop Streaming

5. **Debugging Tips**
   - Connection Issues
   - Packet Tracing
   - Network Monitoring
   - Common Errors

6. **Code Navigation**
   - Find Handler for Packet ID
   - Find Packet Definition
   - Find Form for Feature
   - Trace Packet Flow

7. **Performance Metrics**
   - Typical Values
   - Memory Usage

8. **Learning Resources**
   - Recommended Reading Order
   - Key Concepts to Understand
   - Related Topics

9. **Quick Help / FAQ**

**👥 Dành cho**: Developers đang code và cần tra cứu nhanh

**⏱️ Thời gian đọc**: 30-60 phút (hoặc dùng như reference)

**🔗 Link**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)

**💡 Highlight**:
- Packet ID lookup table đầy đủ
- Code examples thực tế
- Copy-paste ready code snippets
- Debugging checklist
- FAQ với câu trả lời cụ thể

---

## 🗺️ Lộ Trình Đọc Tài Liệu

### 🎓 Cho người mới (New to project)
```
1. README.md (30 phút)
   ↓
2. UML_DIAGRAMS.md - Component Diagram (10 phút)
   ↓
3. ARCHITECTURE.md - Section 1: Tổng Quan (20 phút)
   ↓
4. UML_DIAGRAMS.md - Sequence Diagrams (30 phút)
   ↓
5. Khám phá code với QUICK_REFERENCE.md bên cạnh
```

### 🔧 Cho developer (Want to understand implementation)
```
1. README.md - Overview (15 phút)
   ↓
2. ARCHITECTURE.md - Sections 2-3 (2 giờ)
   - Kiến trúc Client
   - Kiến trúc Server
   ↓
3. QUICK_REFERENCE.md - Packet ID Reference (20 phút)
   ↓
4. ARCHITECTURE.md - Sections 4-5 (1.5 giờ)
   - Giao thức giao tiếp
   - Các chức năng chính
   ↓
5. Source code exploration với QUICK_REFERENCE.md
```

### 🏗️ Cho architect (Want to understand design)
```
1. README.md (20 phút)
   ↓
2. UML_DIAGRAMS.md - All diagrams (1.5 giờ)
   ↓
3. ARCHITECTURE.md - Full read (3 giờ)
   ↓
4. Review source code structure
```

### 🐛 Cho debugging (Need to fix issues)
```
1. QUICK_REFERENCE.md - Debugging Tips (10 phút)
   ↓
2. QUICK_REFERENCE.md - Packet ID Reference (5 phút)
   ↓
3. ARCHITECTURE.md - Relevant section cho feature bạn đang debug (30 phút)
   ↓
4. UML_DIAGRAMS.md - Sequence diagram cho feature đó (15 phút)
```

### ✨ Cho thêm feature mới (Want to add feature)
```
1. QUICK_REFERENCE.md - Common Patterns (20 phút)
   ↓
2. ARCHITECTURE.md - Section 4: Giao thức giao tiếp (30 phút)
   ↓
3. Xem code của feature tương tự (1 giờ)
   ↓
4. QUICK_REFERENCE.md - "Creating a New Packet Type" (10 phút)
```

---

## 📊 Thống Kê Tài Liệu

### Tổng Quan
- **Tổng số files**: 4 files
- **Tổng số dòng**: 2,549 dòng
- **Tổng dung lượng**: ~80KB
- **Số UML diagrams**: 8 diagrams
- **Thời gian tạo**: ~3 giờ

### Phân Bổ Nội Dung
```
ARCHITECTURE.md   37% (945 dòng)  - Chi tiết nhất
UML_DIAGRAMS.md   26% (675 dòng)  - Visual design
QUICK_REFERENCE.md 20% (504 dòng) - Reference nhanh
README.md         17% (425 dòng)  - Overview
```

### Coverage
- ✅ **Client Architecture**: 100% documented
- ✅ **Server Architecture**: 100% documented
- ✅ **Protocol**: 100% documented
- ✅ **All 8 Features**: 100% documented
- ✅ **UML Diagrams**: 8 types completed
- ✅ **Code Examples**: 15+ examples
- ✅ **Packet Reference**: 31 packets documented

---

## 🎯 Câu Hỏi Thường Gặp

### Q1: Tôi nên đọc file nào trước?
**A**: Bắt đầu với README.md để có overview, sau đó đọc ARCHITECTURE.md cho chi tiết.

### Q2: Tôi muốn hiểu một chức năng cụ thể (ví dụ: Remote Desktop)?
**A**: 
1. ARCHITECTURE.md → Section 5.1 (Remote Desktop logic)
2. UML_DIAGRAMS.md → Section 3 (Remote Desktop sequence)
3. QUICK_REFERENCE.md → Remote Desktop Streaming pattern
4. Source code: `Client/Networking/Handlers/RemoteDesktopHandler.cs`

### Q3: Làm sao render UML diagrams?
**A**: 
- Nhanh nhất: Copy code vào http://www.plantuml.com/plantuml/uml/
- Hoặc dùng VS Code extension "PlantUML"
- Chi tiết trong UML_DIAGRAMS.md

### Q4: Packet ID 0x70 là gì?
**A**: Tra trong QUICK_REFERENCE.md → Packet ID Reference
- 0x70 = GetDesktopPacket (Request/Response cho Remote Desktop)

### Q5: Tôi muốn thêm feature mới, làm thế nào?
**A**: Đọc QUICK_REFERENCE.md → "Creating a New Packet Type" section

### Q6: Code ở đâu xử lý kết nối Client?
**A**: 
- Client side: `Client/Networking/Client.cs` → `Connect()` method
- Đọc ARCHITECTURE.md Section 2.3 để hiểu flow

### Q7: Tài liệu có giải thích Keylogger hoạt động như thế nào không?
**A**: Có! Xem:
- ARCHITECTURE.md → Section 5.3 (Keylogger)
- UML_DIAGRAMS.md → Section 7 (Activity Diagram - Keylogger Flow)
- Source: `Client/Logging/Keylogger.cs`

---

## 🔗 Quick Links

### Concepts
- [Kiến trúc Client](./ARCHITECTURE.md#2-kiến-trúc-client)
- [Kiến trúc Server](./ARCHITECTURE.md#3-kiến-trúc-server)
- [Giao thức ProcessStream](./ARCHITECTURE.md#4-giao-thức-giao-tiếp)
- [Packet IDs](./QUICK_REFERENCE.md#-packet-id-reference)

### Features
- [Remote Desktop](./ARCHITECTURE.md#51-remote-desktop)
- [File Manager](./ARCHITECTURE.md#52-file-manager)
- [Keylogger](./ARCHITECTURE.md#53-keylogger)
- [Task Manager](./ARCHITECTURE.md#54-task-manager)
- [Remote Shell](./ARCHITECTURE.md#55-remote-shell)

### UML Diagrams
- [Class Diagram](./UML_DIAGRAMS.md#1-class-diagram-tổng-quan-hệ-thống)
- [Sequence Diagrams](./UML_DIAGRAMS.md#2-sequence-diagram-connection-establishment)
- [State Diagram](./UML_DIAGRAMS.md#6-state-diagram-client-connection-lifecycle)

### Development
- [Common Patterns](./QUICK_REFERENCE.md#-common-patterns)
- [Debugging Tips](./QUICK_REFERENCE.md#-debugging-tips)
- [Code Navigation](./QUICK_REFERENCE.md#-code-navigation)

---

## ✅ Checklist Hoàn Thành

### Tài Liệu Tổng Quan
- [x] README.md - Overview và getting started
- [x] DOCUMENTATION_INDEX.md - Chỉ mục này

### Tài Liệu Kiến Trúc
- [x] Kiến trúc Client (chi tiết đầy đủ)
- [x] Kiến trúc Server (chi tiết đầy đủ)
- [x] Giao thức giao tiếp (ProcessStream)
- [x] Tất cả 8 chức năng chính

### UML Diagrams (PlantUML)
- [x] Class Diagram (System Overview)
- [x] Sequence Diagram - Connection
- [x] Sequence Diagram - Remote Desktop
- [x] Sequence Diagram - File Transfer
- [x] Component Diagram
- [x] State Diagram
- [x] Activity Diagram
- [x] Deployment Diagram

### Developer Resources
- [x] Packet ID Reference (31 packets)
- [x] Key Classes Reference
- [x] Common Patterns (5 patterns)
- [x] Code Examples (15+ examples)
- [x] Debugging Guide
- [x] FAQ

---

## 🎉 Kết Luận

Bộ tài liệu này cung cấp **giải thích chi tiết về cách hoạt động, logic và flow của chương trình ở cả 2 phía Client và Server** theo yêu cầu:

✅ **Logic và Flow**: Mô tả chi tiết trong ARCHITECTURE.md với ASCII diagrams

✅ **Thiết kế UML**: 8 loại UML diagrams đầy đủ trong UML_DIAGRAMS.md với PlantUML code

✅ **Chi tiết các chức năng**: Mỗi chức năng có section riêng giải thích cặn kẽ

✅ **Reference nhanh**: QUICK_REFERENCE.md cho developers

✅ **Dễ tiếp cận**: README.md làm điểm khởi đầu

Tổng cộng **2,549 dòng tài liệu** và **8 UML diagrams** để giải thích toàn bộ hệ thống!

---

**📅 Ngày tạo**: December 16, 2024  
**✍️ Tác giả**: GitHub Copilot  
**🎯 Mục đích**: Giải thích chi tiết kiến trúc và thiết kế PBL4 Botnet Keylogger
