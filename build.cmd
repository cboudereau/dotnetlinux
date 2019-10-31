if not defined CI_PIPELINE_IID (
	set CI_PIPELINE_IID=0
)

set VERSION=1.0.0.%CI_PIPELINE_IID%

SET CONFIGURATION=Release

dotnet build --configuration %CONFIGURATION% -p:Version=%VERSION% || goto error
REM dotnet publish --configuration %CONFIGURATION% -p:Version=%VERSION% --self-contained -r rhel.6-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true || goto error
dotnet publish --configuration %CONFIGURATION% -p:Version=%VERSION% --self-contained -r rhel.6-x64 /p:PublishSingleFile=true || goto error

goto success

:error
echo build failed
exit 1

:success
echo build done
exit 0