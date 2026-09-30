@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

:: Определяем имена файлов
set "SCRIPT_NAME=%~nx0"
set "CLONE_NAME=_updater_backend.bat"

:: === ШАГ 0. ЗАПУСК ЧЕРЕЗ СТАБИЛЬНЫЙ КЛОН ===
:: Если мы запущены как основной скрипт, делаем копию и передаем ей управление
if /i "%~nx0"=="!SCRIPT_NAME!" (
    if /i not "%~nx0"=="!CLONE_NAME!" (
        copy /y "%~f0" "!CLONE_NAME!" > nul
        :: Запускаем клон в новом процессе cmd, передавая ссылку в кавычках
        cmd /c ""!CLONE_NAME!" "%~1""
        exit /b
    )
)

:: --- Код ниже выполняется строго внутри _updater_backend.bat ---

:: Проверка аргумента
if "%~1"== "" (
    echo Error: You did not specify a download link!
    pause
    goto :CLEANUP_AND_EXIT
)

:: Получаем имя файла и папки
for %%i in ("%~1") do (
    set "FILENAME=%%~nxi"
    set "FOLDERNAME=%%~ni"
)

echo.
echo === 1. DOWNLOADING ===
echo Downloading archive: !FILENAME!
curl -f -L -O -J "%~1"

if !errorlevel! neq 0 (
    echo [ERROR] Failed to download the file.
    pause
    goto :CLEANUP_AND_EXIT
)
echo [SUCCESS] File downloaded.

echo.
echo === 2. CLEANING OLD EXECUTABLES ===
echo Removing old .exe files from the current directory...
for %%f in (*.exe) do (
    if /i not "%%f"=="curl.exe" if /i not "%%f"=="tar.exe" if /i not "%%f"=="!CLONE_NAME!" (
        del /f /q "%%f" 2>nul
    )
)
echo [SUCCESS] Old executables removed.

echo.
echo === 3. EXTRACTING ===
echo Extracting to a temporary folder...
mkdir "!FOLDERNAME!" 2>nul
tar -xf "!FILENAME!" -C "!FOLDERNAME!"

if !errorlevel! neq 0 (
    echo [ERROR] Failed to extract the archive.
    pause
    goto :CLEANUP_AND_EXIT
)
echo [SUCCESS] Archive extracted.

echo.
echo === 4. MOVING AND REPLACING FILES ===
echo Moving files to the current directory with replacement...

set "SOURCE_DIR=!FOLDERNAME!"
for /d %%d in ("!FOLDERNAME!\*") do (
    set "SOURCE_DIR=%%d"
)

:: Оригинальный update.bat на диске закрыт, robocopy обновит его без конфликтов
robocopy "!SOURCE_DIR!" . /E /MOVE /IS /R:0 /W:0 > nul

echo [SUCCESS] All files successfully moved to the current directory!

:: Очистка мусора архива
if exist "!FOLDERNAME!" rmdir /s /q "!FOLDERNAME!"
if exist "!FILENAME!" del "!FILENAME!"

echo.
echo === ALL OPERATIONS COMPLETED SUCCESSFULLY ===
echo Press any key to launch the application...
pause

:: === 5. LAUNCHING THE APPLICATION ===
for /f "delims=." %%a in ("!FOLDERNAME!") do set "EXE_NAME=%%a"

if exist "!EXE_NAME!.exe" (
    start "" "!EXE_NAME!.exe"
) else (
    set "FOUND_EXE="
    for %%f in (*.exe) do (
        if /i not "%%f"=="curl.exe" if /i not "%%f"=="tar.exe" if /i not "%%f"=="!CLONE_NAME!" (
            set "FOUND_EXE=%%f"
        )
    )
    
    if defined FOUND_EXE (
        start "" "!FOUND_EXE!"
    ) else (
        echo [WARNING] Could not find any executable file to launch.
    )
)

:CLEANUP_AND_EXIT
:: Надежное фоновое удаление клона после завершения работы скрипта
start /b cmd /c "timeout /t 1 /nobreak >nul && del /f /q "!CLONE_NAME!""
endlocal
exit /b