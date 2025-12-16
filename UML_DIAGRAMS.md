# UML Diagrams - PBL4 Botnet Keylogger

> **Lưu ý**: File này chứa các biểu đồ UML được viết bằng PlantUML. Để render các diagram, sử dụng:
> - **Online**: http://www.plantuml.com/plantuml/uml/
> - **VS Code**: Extension "PlantUML" 
> - **Command line**: `java -jar plantuml.jar file.puml`

## Mục Lục
1. [Class Diagram - Tổng Quan Hệ Thống](#1-class-diagram-tổng-quan-hệ-thống)
2. [Sequence Diagram - Connection Establishment](#2-sequence-diagram-connection-establishment)
3. [Sequence Diagram - Remote Desktop](#3-sequence-diagram-remote-desktop)
4. [Sequence Diagram - File Transfer](#4-sequence-diagram-file-transfer)
5. [Component Diagram](#5-component-diagram)
6. [State Diagram - Client Connection Lifecycle](#6-state-diagram-client-connection-lifecycle)
7. [Activity Diagram - Keylogger Flow](#7-activity-diagram-keylogger-flow)
8. [Deployment Diagram](#8-deployment-diagram)

---

## 1. Class Diagram - Tổng Quan Hệ Thống

### Mô tả
Biểu đồ lớp này thể hiện cấu trúc tổng quan của hệ thống, bao gồm:
- **Common Library**: Thư viện dùng chung cho cả Client và Server
- **Client Application**: Ứng dụng chạy trên máy tính mục tiêu
- **Server Application**: Ứng dụng điều khiển

### PlantUML Code

```plantuml
@startuml SystemOverview
!theme plain
skinparam classAttributeIconSize 0
skinparam backgroundColor #FFFFFF

package "Common Library" <<Rectangle>> {
    interface IPacket
    interface IRequestPacket extends IPacket
    interface IResponsePacket extends IPacket
    
    class ProcessStream {
        - stream: Stream
        + Write(id: int, data: byte[])
        + ReadAsync(): Task<(int, byte[])>
    }
    
    class RequestPacket implements IRequestPacket {
        + Id: int
        + Serialize(): byte[]
        {static} + Deserialize<T>(): T
    }
    
    class FileTransfer {
        + Id: string
        + Path: string
        + Size: long
        + Type: TransferType
    }
}

package "Client Application" {
    class FrmMain {
        - _connectClient: Client
        - _keyloggerService: KeyloggerService
        - _userActivityDetection: ActivityDetection
        + Run()
    }
    
    class "Client.Client" as ClientClass {
        + Connected: bool
        - _tcpClient: TcpClient
        - _stream: ProcessStream
        - _handler: ClientHandler
        + Connect(ip, port)
        + SendPacket(packet)
    }
    
    class ClientHandler {
        + HandlePackets(id, data)
    }
    
    class KeyloggerService {
        - _keylogger: Keylogger
        + Start()
    }
    
    class ActivityDetection {
        - _client: Client
        + Start()
    }
}

package "Server Application" {
    class "Server.FrmMain" as ServerForm {
        + ListenServer: ListenServer
        - AddClientToListview(client)
    }
    
    class ListenServer {
        + Port: int
        + ConnectedClients: List<Client>
        + RunAsync()
    }
    
    class "Server.Client" as ServerClient {
        + Identified: bool
        + Value: UserState
        + StartConnectionAsync()
    }
}

FrmMain *-- ClientClass
FrmMain *-- KeyloggerService
FrmMain *-- ActivityDetection
ClientClass *-- ProcessStream
ClientClass *-- ClientHandler

ServerForm *-- ListenServer
ListenServer "1" *-- "*" ServerClient
ServerClient *-- ProcessStream

@enduml
```

---

## 2. Sequence Diagram - Connection Establishment

### Mô tả
Biểu đồ tuần tự này mô tả quá trình kết nối và xác thực giữa Client và Server:
1. Client khởi động và kết nối tới Server
2. Server chấp nhận kết nối
3. Client gửi thông tin định danh (IdentifyClientPacket)
4. Server xác thực và thêm client vào danh sách

### PlantUML Code

```plantuml
@startuml ConnectionEstablishment
!theme plain
autonumber

actor "Client App" as ClientApp
participant "Client" as Client
participant "Network" as Net
participant "ListenServer" as Server
participant "Server.Client" as SClient
participant "ClientServicesHandler" as Handler
participant "Server UI" as UI

== Startup ==
ClientApp -> Client: Connect(127.0.0.1, 10000)
activate Client

Client -> Net: TCP Connect
activate Net
Net -> Server: Accept Connection
activate Server

Server -> SClient: Create Client Object
activate SClient
Server -> SClient: Task.Run(StartConnectionAsync)

note over SClient: 15 second timeout\nfor identification

== Identification ==
Client -> Client: ProcessAsync() starts
Client -> Net: Send IdentifyClientPacket (0x01)
note right
{
  OS: "Windows 10"
  Account: "Administrator"
  Country: "Vietnam"
  IP: "192.168.1.100"
  PC: "PC-01"
}
end note

Net -> SClient: Receive Packet
SClient -> Handler: HandlePackets(0x01, data)
activate Handler

Handler -> Handler: Deserialize packet
Handler -> SClient: Set UserState
Handler -> SClient: Identified = true
Handler -> SClient: Init() handlers
Handler -> Server: OnClientConnected(client)
deactivate Handler

Server -> UI: ClientConnected event
activate UI
UI -> UI: AddClientToListview()
note right: Display in ListView:\n192.168.1.100 | user@PC-01 | Windows 10
deactivate UI

== Connected ==
note over Client, SClient: Connection established\nReady for commands

deactivate SClient
deactivate Server
deactivate Net
deactivate Client

@enduml
```

---

## 3. Sequence Diagram - Remote Desktop

### Mô tả
Biểu đồ này hiển thị luồng hoạt động của tính năng Remote Desktop:
- Server yêu cầu screenshot định kỳ (mỗi 100ms)
- Client capture màn hình, nén JPEG và gửi về
- Server hiển thị hình ảnh
- User có thể điều khiển chuột/bàn phím

### PlantUML Code

```plantuml
@startuml RemoteDesktop
!theme plain
autonumber

actor User
participant "FrmRemoteDesktop" as Form
participant "Server.Client" as SClient
participant Network
participant "Client" as Client
participant "RemoteDesktopHandler" as Handler
participant "ScreenHelper" as Screen

User -> Form: Open Remote Desktop
activate Form
Form -> Form: Show & Start Timer (100ms)

loop Every 100ms
    Form -> SClient: QueuePacket(GetDesktopPacket)
    note right: Quality: 75, Monitor: 0
    
    SClient -> Network: Send (0x70)
    Network -> Client: Receive
    Client -> Handler: HandlePackets(0x70)
    activate Handler
    
    Handler -> Screen: CaptureScreen(0)
    activate Screen
    Screen -> Screen: Graphics.CopyFromScreen()
    Screen --> Handler: Bitmap
    deactivate Screen
    
    Handler -> Handler: Compress JPEG
    Handler -> Network: Send Response
    Network -> SClient: Receive
    SClient -> Form: Update Image
    Form -> User: Display Screen
    deactivate Handler
end

User -> Form: Click at (500, 300)
Form -> SClient: QueuePacket(MouseEventPacket)
note right: X:500, Y:300, Action:LeftClick

SClient -> Network: Send (0x72)
Network -> Client: Receive
Client -> Handler: HandlePackets(0x72)
activate Handler
Handler -> Handler: SetCursorPos(500, 300)
Handler -> Handler: mouse_event(LeftClick)
deactivate Handler

@enduml
```

---

## 4. Sequence Diagram - File Transfer

### Mô tả
Biểu đồ mô tả quá trình download file từ Client về Server:
- Server gửi yêu cầu transfer
- Client đọc file và chia thành chunks (64KB mỗi chunk)
- Mỗi chunk được gửi tuần tự
- Server nhận và ghi vào file local

### PlantUML Code

```plantuml
@startuml FileTransfer
!theme plain
autonumber

actor User
participant "FrmFileManager" as Form
participant "Server.Client" as SClient
participant Network
participant "Client" as Client
participant "FileManagerHandler" as Handler
participant FileSystem as FS

User -> Form: Double-click "document.pdf"
activate Form

Form -> SClient: FileTransferRequestPacket
note right
{
  TransferId: "TRANS_001"
  Path: "C:\\Users\\user\\Documents\\document.pdf"
  Type: Download
}
end note

SClient -> Network: Send (0x45)
Network -> Client: Receive
Client -> Handler: HandlePackets(0x45)
activate Handler

Handler -> FS: File.OpenRead(path)
activate FS
FS --> Handler: FileStream (1MB)

loop For each 64KB chunk
    Handler -> FS: Read(64KB)
    FS --> Handler: byte[65536]
    
    Handler -> Network: FileTransferChunkPacket (0x48)
    note right: Seq: N, Data: 64KB
    
    Network -> SClient: Receive
    SClient -> Form: Write chunk to local file
    Form -> User: Update progress: N/16
end

Handler -> Network: FileTransferCompletePacket (0x46)
Handler -> FS: Close()
deactivate FS

Network -> SClient: Receive
SClient -> Form: Transfer complete
Form -> User: "Download complete!"
deactivate Handler
deactivate Form

@enduml
```

---

## 5. Component Diagram

### Mô tả
Biểu đồ thành phần thể hiện các module chính và mối quan hệ giữa chúng.

### PlantUML Code

```plantuml
@startuml Components
!theme plain

package "Client Machine" {
    component "Client App" as ClientApp {
        [FrmMain]
        [Client Networking]
        [Keylogger Service]
        [Activity Detection]
        [Packet Handlers]
    }
    
    component "Windows OS" {
        [Keyboard Hooks]
        [Process API]
        [File System]
        [Screen Capture]
    }
}

package "Network" {
    [TCP/IP] as Network
}

package "Server Machine" {
    component "Server App" as ServerApp {
        [Server.FrmMain]
        [ListenServer]
        [Client Manager]
        [Feature Forms]
        [Packet Handlers]
    }
}

component "Common Library" {
    [ProcessStream]
    [Packet System]
    [Models]
}

ClientApp --> Common: uses
ServerApp --> Common: uses

[Client Networking] --> Network: TCP Socket
Network --> [ListenServer]: TCP Socket

[Keylogger Service] --> [Keyboard Hooks]
[Activity Detection] --> [Keyboard Hooks]
[Packet Handlers] --> [Process API]
[Packet Handlers] --> [File System]
[Packet Handlers] --> [Screen Capture]

@enduml
```

---

## 6. State Diagram - Client Connection Lifecycle

### Mô tả
Biểu đồ trạng thái mô tả vòng đời kết nối của Client.

### PlantUML Code

```plantuml
@startuml ClientStateMachine
!theme plain

[*] --> Disconnected

Disconnected --> Connecting : Connect(IP, Port)
Connecting --> Connected : TCP Success
Connecting --> Disconnected : Timeout/Error

Connected --> Identifying : Send IdentifyClientPacket
Identifying --> Identified : Server Accepts
Identifying --> Disconnected : 15s Timeout

Identified --> Active : Init Handlers

Active --> Idle : No Activity (600s)
Idle --> Active : User Activity
Active --> Disconnected : Disconnect/Error
Idle --> Disconnected : Disconnect/Error

Active --> Reconnecting : Reconnect Command
Reconnecting --> Disconnected
Disconnected --> Connecting : Auto-reconnect

Disconnected --> [*]

note right of Active
  Substates:
  - Keylogger Running
  - Processing Commands
  - Transferring Files
  - Streaming Desktop
end note

@enduml
```

---

## 7. Activity Diagram - Keylogger Flow

### Mô tả
Biểu đồ hoạt động mô tả quy trình hoạt động của Keylogger.

### PlantUML Code

```plantuml
@startuml KeyloggerFlow
!theme plain

start
:Application Starts;
:Create KeyloggerService;
:Start KeyloggerService;

fork
    :Set Windows Hook\n(SetWindowsHookEx);
    
    while (Application Running?) is (Yes)
        :Wait for Key Press;
        :Hook Callback Triggered;
        
        if (Valid Key?) then (Yes)
            :Get Virtual Key Code;
            :Get Active Window;
            
            if (Window Changed?) then (Yes)
                :Write Window Header;
            endif
            
            if (Special Key?) then (Yes)
                :Format Special Key\n([Enter], [Tab], etc.);
            else (No)
                :Convert to Character;
            endif
            
            :Append to Buffer;
            
            if (Flush Condition?) then (Yes)
                :Write Buffer to File;
                note right
                    AppData\\KeyLogs\\
                    Log_2024-01-15.txt
                end note
                :Clear Buffer;
            endif
        endif
        
        :CallNextHookEx();
    endwhile (No)
    
    :Unhook;
fork end

if (Server Requests Logs?) then (Yes)
    :Get Logs Directory;
    :Send File List;
    :Transfer Log Files;
endif

stop

@enduml
```

---

## 8. Deployment Diagram

### Mô tả
Biểu đồ triển khai hiển thị cách các thành phần được deploy trên các node vật lý.

### PlantUML Code

```plantuml
@startuml Deployment
!theme plain

node "Target Machine (Client)" {
    artifact "Client.exe" {
        component "Client Application"
        component "Keylogger Service"
        component "Activity Monitor"
    }
    
    database "Log Files" {
        file "Log_2024-01-15.txt"
        file "Log_2024-01-16.txt"
    }
    
    [Client Application] --> [Log Files]: writes
}

cloud "Network" {
    protocol "TCP/IP\nPort 10000"
}

node "Control Machine (Server)" {
    artifact "Server.exe" {
        component "Server Application"
        component "ListenServer"
        component "Feature Forms"
    }
    
    database "Downloads" {
        folder "Client_192.168.1.100"
    }
    
    [Server Application] --> [Downloads]: writes
}

[Client.exe] ..> [TCP/IP\nPort 10000]: Socket
[TCP/IP\nPort 10000] ..> [Server.exe]: Socket

note right of "Client.exe"
    Deployment:
    - AppData location
    - Runs hidden
    - Auto-start
    - Single instance
end note

note left of "Server.exe"
    Requirements:
    - Admin rights
    - Port forwarding
    - .NET Runtime
end note

@enduml
```

---

## Hướng Dẫn Sử Dụng

### Render PlantUML Diagrams

#### 1. Online (Nhanh nhất)
```
1. Truy cập: http://www.plantuml.com/plantuml/uml/
2. Copy code PlantUML từ file này
3. Paste vào editor
4. Click "Submit" để xem diagram
5. Download as PNG/SVG
```

#### 2. Visual Studio Code
```
1. Install extension: "PlantUML" by jebbs
2. Open file .puml
3. Press Alt+D để preview
4. Right-click → Export để save image
```

#### 3. Command Line
```bash
# Download PlantUML
wget http://sourceforge.net/projects/plantuml/files/plantuml.jar/download -O plantuml.jar

# Render diagram
java -jar plantuml.jar diagram.puml

# Render as SVG
java -jar plantuml.jar -tsvg diagram.puml
```

### Tích Hợp Với GitHub

Để hiển thị diagram trực tiếp trên GitHub README:

```markdown
![Diagram Name](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/USER/REPO/main/diagram.puml)
```

### Giải Thích Các Ký Hiệu

#### Class Diagram
- `+` : Public
- `-` : Private
- `#` : Protected
- `~` : Package
- `{static}` : Static member
- `{abstract}` : Abstract member

#### Relationships
- `-->` : Association
- `..>` : Dependency
- `--|>` : Inheritance
- `..|>` : Realization
- `*--` : Composition
- `o--` : Aggregation

#### Sequence Diagram
- `->` : Synchronous message
- `->>` : Asynchronous message
- `-->` : Return message
- `activate`/`deactivate` : Lifeline
- `note` : Comment/explanation

---

## Tổng Kết

File này cung cấp các UML diagrams chính cho hệ thống PBL4 Botnet Keylogger:

1. **Class Diagram**: Cấu trúc classes và relationships
2. **Sequence Diagrams**: Luồng tương tác cho các chức năng chính
3. **Component Diagram**: Kiến trúc module
4. **State Diagram**: Vòng đời kết nối
5. **Activity Diagram**: Flow của Keylogger
6. **Deployment Diagram**: Triển khai vật lý

Sử dụng kết hợp với file `ARCHITECTURE.md` để có cái nhìn toàn diện về hệ thống.
