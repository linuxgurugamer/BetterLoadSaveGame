@echo off
setlocal
set KSPDIR=G:\KSP\1.4.4_test
cd %~dp0
rd KSP
rd %KSPDIR%\GameData\BetterLoadSaveGame
mklink /J KSP %KSPDIR%
mklink /J %KSPDIR%\GameData\BetterLoadSaveGame GameData\BetterLoadSaveGame
endlocal
