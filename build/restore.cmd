setlocal
pushd %~dp0
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd" -arch=amd64 -host_arch=amd64 -winsdk=10.0.16299.0

set SOLUTION_FILE="%~dp0..\src\BasicSupportAcisExtension\BasicSupportAcisExtension.sln"

:: Restore NuGet packages for solution. 
nuget.exe restore %SOLUTION_FILE%

:: Restore NuGet packages for solution.
dotnet.exe restore %SOLUTION_FILE%

dotnet.exe restore "%~dp0..\src\BasicSupportAcisExtension\BasicSupportAcisExtension.csproj"

popd
endlocal