## How to Test the Application

### Prerequisites
- Visual Studio (recommended: Visual Studio 2019 or later)
- The .NET Framework / .NET version required by the project

### Steps
1. Clone the repository.
2. Open the solution in Visual Studio.
3. Configure the Client:
   - Open `Client/FrmMain.cs`.
   - Replace the server IP address with the IP of the machine running the Server.
4. Build the projects:
   - Build both the **Client** and **Server** projects.
5. Run the application:
   - Start the **Server** first.
   - Then run the **Client** to test the connection.

## How to Send Client.exe (Bypass Window Defender)
1. Package Client.exe into a ZIP archive.
2. Upload Client.zip and Pikachu.zip to cloud storage or Server
3. Replace the URL of Client.Zip and Pikachu.zip in Bypass_WindowDefender.cpp
4. build Bypass_WindowDefender.cpp to .exe, rename it
5. Send it to Victims
