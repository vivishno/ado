
cd "%~dp0"

set SOLUTION_FILE="%~dp0..\src\BasicSupportAcisExtension\BasicSupportAcisExtension.sln"

call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd" -arch=amd64 -host_arch=amd64 -winsdk=10.0.16299.0

:: Build the solution
msbuild.exe /t:build %SOLUTION_FILE%
if not %ERRORLEVEL%==0 (
echo 'Build the solution failed'
exit /B 1
)
