using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace AutoCheckZapret
{
    /// <summary>
    /// Сервис для установки, запуска и удаления службы Zapret.
    /// </summary>
    public class ZapretService
    {
        private readonly string _folderPath;
        private readonly string _listsPath;
        private readonly string _winsPath;
        private readonly string _binPath;

        private readonly string _gameFilterStatus = "disabled";
        private readonly string _gameFilter = "12";
        private readonly string _gameFilterUDP = "12";
        private readonly string _gameFilterTCP = "12";

        private readonly string _serviceName = "zapret";
        private readonly string _description =
            "Zapret DPI bypass software";

        /// <summary>
        /// Инициализирует сервис управления Zapret.
        /// </summary>
        /// <param name="folderPath">
        /// Путь к основной папке Zapret.
        /// </param>
        public ZapretService(string folderPath)
        {
            _folderPath = folderPath;

            _listsPath = GetListsPath();
            _winsPath = GetWinsPath();
            Debug.WriteLine("Получен путь до lists: " + _listsPath);
            Debug.WriteLine("Получен путь до winws.exe: " + _winsPath);

            _binPath = string.IsNullOrEmpty(_winsPath)
                ? ""
                : Path.GetDirectoryName(_winsPath) ?? "";

            if (string.IsNullOrEmpty(_binPath))
            {
                _binPath = Path.Combine(_folderPath, "bin");
            }
        }

        /// <summary>
        /// Возвращает список BAT-файлов,
        /// содержащих команды запуска Zapret.
        /// </summary>
        /// <returns>
        /// Список путей к найденным файлам стратегий.
        /// </returns>
        public List<string> GetBypassFilesFromFolder()
        {
            string[] files = Directory.GetFiles(_folderPath);

            List<string> bypassFiles = new();

            foreach (string file in files)
            {
                if (!file.EndsWith(
                        ".bat",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string content = File.ReadAllText(file);

                if (content.Contains(
                        "start \"zapret",
                        StringComparison.OrdinalIgnoreCase))
                {
                    bypassFiles.Add(Path.GetFileName(file));
                }
            }

            return bypassFiles;
        }

        /// <summary>
        /// Извлекает аргументы запуска из файла стратегии.
        /// </summary>
        /// <param name="filePath">
        /// Путь к BAT-файлу со стратегией.
        /// </param>
        /// <returns>
        /// Строка с аргументами запуска.
        /// Если аргументы не найдены, возвращается сообщение
        /// «Аргументы не найдены».
        /// </returns>
        private string GetBypassArg(string filePath)
        {
            string content = File.ReadAllText(filePath);

            int index = content.IndexOf(
                "--",
                StringComparison.Ordinal);

            if (index == -1)
            {
                return "Аргументы не найдены";
            }

            string rawArg = content[index..];
            rawArg = rawArg.Replace("--hostlist=\"%LISTS%list-general-user.txt\"", "")
                           .Replace("--hostlist-exclude=\"%LISTS%list-exclude-user.txt\"", "")
                           .Replace("--ipset-exclude=\"%LISTS%ipset-exclude-user.txt\"", "");
            rawArg = rawArg.Replace("^!", "!")
                .Replace("^", " ")
                .Replace("\r", " ")
                .Replace("\n", " ");

            string listsPath = string.IsNullOrEmpty(_listsPath)
                ? ""
                : _listsPath.TrimEnd('\\') + "\\";

            string binPath = string.IsNullOrEmpty(_binPath)
                ? ""
                : _binPath.TrimEnd('\\') + "\\";

            // В BAT-файлах знак "=" используется
            // как разделитель параметра и значения.

            // Экранирование кавычек для передачи
            // аргументов внешней утилите.

            rawArg = rawArg.Replace("\"", "\\\"");
            rawArg = rawArg.Replace("%LISTS%", listsPath)
                .Replace("%BIN%", binPath).Replace("%~dp0", "")
                .Replace("%GameFilterStatus%", _gameFilterStatus)
                .Replace("%GameFilter%", _gameFilter)
                .Replace("%GameFilterTCP%", _gameFilterTCP)
                .Replace("%GameFilterUDP%", _gameFilterUDP);
            rawArg = System.Text.RegularExpressions.Regex.Replace(rawArg, @"--([a-zA-Z0-9\-]+)=", "--$1 ");
            Debug.WriteLine("Получены аргументы: " + rawArg);
            return rawArg.Trim();
        }

        /// <summary>
        /// Ищет путь к папке со списками адресов.
        /// </summary>
        /// <returns>
        /// Путь к папке, содержащей файл со списком.
        /// Если папка не найдена, возвращается пустая строка.
        /// </returns>
        private string GetListsPath()
        {
            string[] allFiles = Directory.GetFiles(
                _folderPath,
                "*",
                SearchOption.AllDirectories);

            foreach (string file in allFiles)
            {
                string name = Path.GetFileName(file);

                if (name.Contains(
                        "list",
                        StringComparison.OrdinalIgnoreCase))
                {
                    string listPath = Path.GetDirectoryName(file) ?? "";
                    return Path.GetRelativePath(_folderPath, listPath);
                }
            }

            return "";
        }

        /// <summary>
        /// Ищет исполняемый файл winws.exe.
        /// </summary>
        /// <returns>
        /// Полный путь к файлу winws.exe.
        /// Если файл не найден, возвращается пустая строка.
        /// </returns>
        private string GetWinsPath()
        {
            string[] allFiles = Directory.GetFiles(
                _folderPath,
                "*",
                SearchOption.AllDirectories);

            foreach (string file in allFiles)
            {
                string name = Path.GetFileName(file);

                if (name.Equals(
                        "winws.exe",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetRelativePath(_folderPath, file);
                }
            }

            return "";
        }

        /// <summary>
        /// Содержит результат выполнения внешней утилиты.
        /// </summary>
        private sealed class ProcessResult
        {
            /// <summary>
            /// Код завершения процесса.
            /// Нулевое значение обычно означает успешное выполнение.
            /// </summary>
            public int ExitCode { get; init; }

            /// <summary>
            /// Стандартный вывод программы.
            /// </summary>
            public string Output { get; init; } = "";

            /// <summary>
            /// Вывод программы, содержащий сообщения об ошибках.
            /// </summary>
            public string Error { get; init; } = "";
        }

        /// <summary>
        /// Запускает внешнюю консольную утилиту в скрытом режиме асинхронно.
        /// </summary>
        /// <param name="fileName">
        /// Имя или полный путь к исполняемому файлу.
        /// Например: sc.exe, netsh.exe или reg.exe.
        /// </param>
        /// <param name="arguments">
        /// Аргументы командной строки.
        /// </param>
        /// <param name="ignoreErrors">
        /// Если true, сообщения об ошибках
        /// не выводятся в консоль.
        /// </param>
        /// <param name="cancellationToken">
        /// Токен отмены операции.
        /// </param>
        /// <returns>
        /// Объект с кодом завершения,
        /// стандартным выводом и сообщением об ошибке.
        /// </returns>
        private async Task<ProcessResult> RunUtilityAsync(
            string fileName,
            string arguments,
            bool
            ignoreErrors = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,

                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,

                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = new Process
                {
                    StartInfo = psi
                };

                if (!process.Start())
                {
                    string error =
                        $"Не удалось запустить утилиту: {fileName}";

                    if (!ignoreErrors)
                    {
                        Debug.WriteLine(error);
                    }

                    return new ProcessResult
                    {
                        ExitCode = -1,
                        Error = error
                    };
                }

                // Асинхронное чтение вывода
                Task<string> outputTask =
                    process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask =
                    process.StandardError.ReadToEndAsync();

                // Асинхронное ожидание завершения процесса с поддержкой отмены
                await process.WaitForExitAsync(cancellationToken);
                await Task.Delay(100);
                string output = await outputTask;
                string errorOutput = await errorTask;

                if (!ignoreErrors &&
                    process.ExitCode != 0)
                {
                    Debug.WriteLine(
                        $"{fileName} ошибка: {errorOutput.Trim()}");
                }

                return new ProcessResult
                {
                    ExitCode = process.ExitCode,
                    Output = output,
                    Error = errorOutput
                };
            }
            catch (OperationCanceledException)
            {
                if (!ignoreErrors)
                {
                    Debug.WriteLine($"Операция отменена для {fileName}");
                }

                return new ProcessResult
                {
                    ExitCode = -1,
                    Error = "Операция отменена"
                };
            }
            catch (Exception ex)
            {
                if (!ignoreErrors)
                {
                    Debug.WriteLine(
                        $"Ошибка запуска {fileName}: {ex.Message}");
                }

                return new ProcessResult
                {
                    ExitCode = -1,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Проверяет состояние TCP timestamps
        /// и при необходимости включает их асинхронно.
        /// </summary>
        private async Task EnableTcpTimestampsAsync(CancellationToken cancellationToken = default)
        {
            ProcessResult result = await RunUtilityAsync(
                "netsh.exe",
                "interface tcp show global",
                cancellationToken: cancellationToken);

            if (result.ExitCode != 0)
            {
                return;
            }

            string output = result.Output.ToLowerInvariant();

            if (!output.Contains("timestamps") ||
                !output.Contains("enabled"))
            {
                ProcessResult enableResult = await RunUtilityAsync(
                    "netsh.exe",
                    "interface tcp set global timestamps=enabled",
                    cancellationToken: cancellationToken);

                if (enableResult.ExitCode == 0)
                {
                    Debug.WriteLine(
                        "TCP timestamps включены.");
                }
            }
            else
            {
                Debug.WriteLine(
                    "TCP timestamps уже включены.");
            }
        }

        /// <summary>
        /// Создаёт и запускает службу Zapret асинхронно.
        /// </summary>
        /// <param name="fileName">
        /// Имя BAT-файла со стратегией обхода.
        /// </param>
        /// <param name="enableTcp">
        /// Если true, перед установкой службы
        /// включает TCP timestamps.
        /// </param>
        /// <param name="cancellationToken">
        /// Токен отмены операции.
        /// </param>
        /// <returns>
        /// true, если служба успешно создана и запущена;
        /// false, если произошла ошибка.
        /// </returns>
        public async Task<bool> InstallServiceAsync(
            string fileName,
            bool enableTcp = true,
            CancellationToken cancellationToken = default)
        {
            string strategyFilePath = _folderPath + '\\' + fileName;
            Debug.Write(strategyFilePath);
            if (enableTcp)
            {
                await EnableTcpTimestampsAsync(cancellationToken);
            }

            if (!File.Exists(strategyFilePath))
            {
                Debug.WriteLine(
                    $"Файл стратегии не найден: {strategyFilePath}");

                return false;
            }

            string args = GetBypassArg(strategyFilePath);

            if (string.IsNullOrEmpty(args) ||
                args == "Аргументы не найдены")
            {
                Debug.WriteLine(
                    "Не удалось извлечь аргументы.");

                return false;
            }

            if (string.IsNullOrEmpty(_winsPath) ||
                !File.Exists(Path.Combine(_folderPath, _winsPath)))
            {
                Debug.WriteLine(
                    $"winws.exe не найден: {_winsPath}");

                return false;
            }

            string command =
                $"create {_serviceName} " +
                $"binPath= \"cmd.exe /c cd /d \\\"{_folderPath}\\\" && \\\"{_winsPath}\\\" {args}\" " +
                $"DisplayName= \"{_serviceName}\" " +
                "start= auto";

            // Останавливаем и удаляем существующую службу
            await RunUtilityAsync(
                "sc.exe",
                $"stop \"{_serviceName}\"",
                ignoreErrors: true,
                cancellationToken: cancellationToken);

            await RunUtilityAsync(
                "sc.exe",
                $"delete \"{_serviceName}\"",
                ignoreErrors: true,
                cancellationToken: cancellationToken);

            Debug.WriteLine(command);

            ProcessResult createResult = await RunUtilityAsync(
                "sc.exe",
                command,
                cancellationToken: cancellationToken);

            if (createResult.ExitCode != 0)
            {
                Debug.WriteLine(
                    $"Ошибка создания службы, код " +
                    $"{createResult.ExitCode}");

                return false;
            }

            ProcessResult descriptionResult = await RunUtilityAsync(
                "sc.exe",
                $"description \"{_serviceName}\" " +
                $"\"{_description}\"",
                cancellationToken: cancellationToken);

            if (descriptionResult.ExitCode != 0)
            {
                Debug.WriteLine(
                    "Не удалось установить описание службы.");
            }

            ProcessResult startResult = await RunUtilityAsync(
                "sc.exe",
                $"start \"{_serviceName}\"",
                cancellationToken: cancellationToken);

            if (startResult.ExitCode != 0)
            {
                Debug.WriteLine(
                     "Не удалось запустить службу.");

                return false;
            }

            //string strategyName =
            //    Path.GetFileNameWithoutExtension(strategyFilePath);

            //string regKey =
            //    $@"HKLM\System\CurrentControlSet\Services\{_serviceName}";

            //ProcessResult registryResult = await RunUtilityAsync(
            //    "reg.exe",
            //    $"add \"{regKey}\" " +
            //    "/v zapret-discord-youtube " +
            //    "/t REG_SZ " +
            //    $"/d \"{strategyName}\" " +
            //    "/f",
            //    cancellationToken: cancellationToken);

            //if (registryResult.ExitCode != 0)
            //{
            //    Debug.WriteLine(
            //         "Ошибка записи стратегии в реестр.");
            //}
            ProcessResult res = await RunUtilityAsync(
                "sc.exe",
                $"query \"{_serviceName}\"",
                ignoreErrors: true,
                cancellationToken: cancellationToken);

            bool started = await WaitForServiceStatusAsync(_serviceName, ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            await Task.Delay(1000);
            Debug.WriteLine(res.Output, res.Error, res.ExitCode);
            return started;
        }

        /// <summary>
        /// Останавливает и удаляет службу Zapret асинхронно,
        /// завершает процесс winws.exe и удаляет
        /// связанные службы WinDivert.
        /// </summary>
        public async Task RemoveServiceAsync(CancellationToken cancellationToken = default)
        {
            await RunUtilityAsync(
                "sc.exe",
                $"stop \"{_serviceName}\"",
                ignoreErrors: true,
                cancellationToken: cancellationToken);

            await RunUtilityAsync(
                "sc.exe",
                $"delete \"{_serviceName}\"",
                ignoreErrors: true,
                cancellationToken: cancellationToken);

            await RunUtilityAsync(
                "taskkill.exe",
                "/IM winws.exe /F",
                ignoreErrors: true,
                cancellationToken: cancellationToken);

            string[] divertServices =
            {
                "WinDivert",
                "WinDivert14"
            };

            foreach (string service in divertServices)
            {
                await RunUtilityAsync(
                    "sc.exe",
                    $"stop \"{service}\"",
                    ignoreErrors: true,
                    cancellationToken: cancellationToken);

                await RunUtilityAsync(
                    "sc.exe",
                    $"delete \"{service}\"",
                    ignoreErrors: true,
                    cancellationToken: cancellationToken);
            }
            bool removed = await WaitForServiceStatusAsync(_serviceName, ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            await Task.Delay(1000);
            Debug.WriteLineIf(removed,
                "Служба zapret и связанные драйверы удалены.");
            Debug.WriteLineIf(!removed, "Ошибка завершения службы");
        }

        public async Task<bool> WaitForServiceStatusAsync(string serviceName, ServiceControllerStatus desiredStatus, TimeSpan timeout)
        {
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status == desiredStatus)
                        return true;

                    // WaitForStatus — синхронный, поэтому оборачиваем в Task.Run
                    await Task.Run(() => sc.WaitForStatus(desiredStatus, timeout));
                    return sc.Status == desiredStatus;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при ожидании статуса службы '{serviceName}': {ex.Message}");
                return false;
            }
        }
    }
}