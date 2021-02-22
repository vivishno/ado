
cd "%~dp0"

set SOLUTION_FILE="%~dp0..\src\BasicSupportAcisExtension\BasicSupportAcisExtension.sln"
set OUTPUT_FOLDER="%~dp0..\out\BasicSupport\EV2"

call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd" -arch=amd64 -host_arch=amd64 -winsdk=10.0.16299.0

robocopy "%~dp0..\src\BasicSupportAcisExtension\bin\GenevaActionPackages" %OUTPUT_FOLDER% /E

robocopy "%~dp0..\src\Deploy" %OUTPUT_FOLDER% /E

:: Build the solution
dotnet publish /t:publish %SOLUTION_FILE% /p:Configuration=Release /p:UseWPP_CopyWebApplication=true /p:PipelineDependsOnBuild=false /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:NoBuild=true /p:PackageLocation="..out/BasicSupport/EV2/BasicSupportAcisExtension.zip"

if not %ERRORLEVEL%==0 (
echo 'Build the solution failed'
exit /B 1
)