@echo off
setlocal
set KSPDIR=G:\KSP\1.7.0
cd %~dp0
echo Remove old root link
rd KSP
echo Remove old output link
rd %KSPDIR%\GameData\BetterLoadSaveGame
echo Create new root link
mklink /J KSP %KSPDIR%
echo Create new output link
mklink /J %KSPDIR%\GameData\BetterLoadSaveGame GameData\BetterLoadSaveGame
endlocal
