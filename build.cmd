@echo off

if not defined CI_PIPELINE_IID (
	set CI_PIPELINE_IID=0
)

if [%1]==[] (
	set CONFIGURATION=Release
) else set CONFIGURATION=%1

set VERSION=1.0.0.%CI_PIPELINE_IID%

call setup.cmd || goto error 

dotnet build --no-restore --configuration %CONFIGURATION% -p:Version=%VERSION% || goto error
REM dotnet publish --configuration %CONFIGURATION% -p:Version=%VERSION% --self-contained -r rhel.6-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true || goto error
dotnet publish --configuration %CONFIGURATION% -p:Version=%VERSION% --self-contained -r rhel.6-x64 /p:PublishSingleFile=true || goto error

goto success

:error
echo build failed
exit 1

:success
echo build done
exit 0