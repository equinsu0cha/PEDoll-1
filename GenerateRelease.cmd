@echo off

set "RELEASEDIR=%tmp%\PEDollRelease"


:main
	pushd %~dp0
	msbuild -ver >nul 2>nul
	if %ERRORLEVEL% neq 0 (
		echo Run this batch file from a Developer Command Prompt!
		goto :eof
	)

	if exist "%RELEASEDIR%" (
		rd /s /q "%RELEASEDIR%"
	)

	md %RELEASEDIR%

	call :buildController Debug ^
	&& call :buildMonitor x86 Debug ^
	&& call :buildMonitor x64 Debug

	start explorer %RELEASEDIR%
	popd
goto :eof


:: call :buildController Debug
:buildController
	set PLATFORMDIR=PEDollController\bin

	msbuild PEDoll.sln -p:Platform="Any CPU";Configuration=%1
	if %ERRORLEVEL% neq 0 (
		goto :eof
	)

	if not exist "%RELEASEDIR%\Controller" (
		md "%RELEASEDIR%\Controller"
	)

	md "%RELEASEDIR%\Controller\%1"

	xcopy /e %PLATFORMDIR%\%1 "%RELEASEDIR%\Controller\%1\"
	xcopy /e /i Scripts "%RELEASEDIR%\Controller\%1\Scripts"

	wsl ./GenerateAPIx64.sh "%RELEASEDIR%\Controller\%1\Scripts\API"
goto :eof


:: call :buildMonitor x64 Debug
:buildMonitor

	if %1 equ x86 (
		set PLATFORMDIR=.
	) else (
		set PLATFORMDIR=x64
	)

	msbuild PEDoll.sln -p:Platform=%1;Configuration=%2
	if %ERRORLEVEL% neq 0 (
		goto :eof
	)

	if not exist "%RELEASEDIR%\Monitor_%1" (
		md "%RELEASEDIR%\Monitor_%1"
	)

	md "%RELEASEDIR%\Monitor_%1\%2"

	copy %PLATFORMDIR%\%2\PEDollMonitor.exe "%RELEASEDIR%\Monitor_%1\%2\"
	copy %PLATFORMDIR%\%2\libDoll.dll "%RELEASEDIR%\Monitor_%1\%2\"

	if %2 equ Debug (
		copy %PLATFORMDIR%\%2\PEDollMonitor.pdb "%RELEASEDIR%\Monitor_%1\%2\"
		copy %PLATFORMDIR%\%2\libDoll.pdb "%RELEASEDIR%\Monitor_%1\%2\"
	)

goto :eof