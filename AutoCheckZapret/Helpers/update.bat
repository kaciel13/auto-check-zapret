@echo off
chcp 65001 > nul

if "%~1"== "" (
    echo Error: You did not specify a download link!
    pause
    exit /b
)

for %%i in ("%~1") do (
    set "FILENAME=%%~nxi"
    set "FOLDERNAME=%%~ni"
)

echo.
echo === 1. DOWNLOADING ===
echo Downloading archive: %FILENAME%
curl -f -L -O -J "%~1"

if %errorlevel% neq 0 (
    echo [ERROR] Failed to download the file.
    pause
    exit /b
)
echo [SUCCESS] File downloaded.

echo.
echo === 2. EXTRACTING ===
echo Extracting to a temporary folder...
mkdir "%FOLDERNAME%" 2>nul
tar -xf "%FILENAME%" -C "%FOLDERNAME%"

if %errorlevel% neq 0 (
    echo [ERROR] Failed to extract the archive.
    pause
    exit /b
)
echo [SUCCESS] Archive extracted.

echo.
echo === 3. MOVING AND REPLACING FILES ===
echo Moving files to the current directory with replacement...

set "SOURCE_DIR=%FOLDERNAME%"
for /d %%d in ("%FOLDERNAME%\*") do (
    set "SOURCE_DIR=%%d"
)

robocopy "%SOURCE_DIR%" . /E /MOVE /IS /R:0 /W:0 > nul

echo [SUCCESS] All files successfully moved to the current directory!

:: Cleanup
if exist "%FOLDERNAME%" rmdir /s /q "%FOLDERNAME%"
if exist "%FILENAME%" del "%FILENAME%"

echo.
echo === ALL OPERATIONS COMPLETED SUCCESSFULLY ===
echo Press any key to launch the application...
pause

:: === 4. LAUNCHING THE APPLICATION ===
:: Пытаемся найти главный .exe файл на основе имени архива
set "EXE_NAME=%FOLDERNAME%"
:: Если в имени была точка (как в AutoCheckZapret.x64), берем часть до первой точки
for /f "delims=." %%a in ("%FOLDERNAME%") do set "EXE_NAME=%%a"

if exist "%EXE_NAME%.exe" (
    start "" "%EXE_NAME%.exe"
) else (
    :: Если имя не совпало, ищем любой доступный .exe в текущей папке
    for %%f in (*.exe) do (
        if not "%%f"=="curl.exe" (
            start "" "%%f"
            goto :eof
        )
    )
    echo [WARNING] Could not find any executable file to launch.
)
