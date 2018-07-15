@echo off
c:\Windows\microsoft.net\Framework64\v4.0.30319\MSBuild.exe /t:Build /p:Configuration=Release /p:TargetFramework=v3.5 src\BetterLoadSaveGame.sln
copy /Y src\bin\Release\BetterLoadSaveGame.dll GameData\BetterLoadSaveGame
FOR /F "tokens=* USEBACKQ" %%F IN (`powershell -command "[System.Reflection.Assembly]::LoadFrom('src\bin\Release\BetterLoadSaveGame.dll').GetName().Version.ToString(2)"`) do (
  set ver=%%F
)
del BetterLoadSaveGame.zip
"C:\Program Files\7-Zip\7z.exe" a -tzip BetterLoadSaveGame_v%ver%.zip GameData
