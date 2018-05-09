@ECHO OFF
:: Install.bat

ECHO.
ECHO =========================i95Dev Connector installation.=========================
ECHO.
SET /p INPUT=Do you updated configuration settings (Y/N):

if "%INPUT%"=="Y" (
   I95DevGPConnector.exe 6
   ECHO Database created successfully

   I95DevGPConnector.exe 7
   ECHO Schedulers created successfully

   I95DevGPConnector.exe 10
   ECHO Categories & mappings imported successfully
) else (
ECHO Please do the changes and run this command again
)
pause

