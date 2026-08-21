using System.Diagnostics;
using System.IO;

namespace AutoCheckZapret.Services
{
    public class ZapretService
    {
        private string _folderPath;
        private string _listsPath;
        private string _winsPath;
        private string _binPath;             
        private string _gameFilterStatus = "disabled";
        private string _gameFilter = "12";
        private string _gameFilterUDP = "12";
        private string _gameFilterTCP = "12";
        private string _serviceName = "zapret";
        private string _description = "Zapret DPI bypass software";

        public ZapretService(string folderPath)
        {
            _folderPath = folderPath;
            _listsPath = GetListsPath();
            _winsPath = GetWinsPath();
            _binPath = string.IsNullOrEmpty(_winsPath) ? "" : Path.GetDirectoryName(_winsPath);
            if (string.IsNullOrEmpty(_binPath))
                _binPath = Path.Combine(_folderPath, "bin");
        }

        public List<string> GetBypassFilesFromFolder()
        {
            string[] files = Directory.GetFiles(_folderPath);
            List<string> bypassFiles = new List<string>();
            foreach (string file in files)
            {
                if (!file.Contains(".bat")) continue;
                string content = File.ReadAllText(file);
                if (content.Contains("start \"zapret"))
                {
                    bypassFiles.Add(file);
                }
            }
            return bypassFiles;
        }

        private string GetBypassArg(string filePath)
        {
            string content = File.ReadAllText(filePath);
            int index = content.IndexOf("--");
            if (index == -1)
                return "Аргументы не найдены";

            string rawArg = content.Substring(index);
            rawArg = rawArg.Replace("^", " ").Replace("\r", " ").Replace("\n", " ");
            //while (rawArg.Contains("  "))
                //rawArg = rawArg.Replace("  ", " ");

            string listsPath = _listsPath.EndsWith("\\") ? _listsPath : _listsPath + "\\";
            string binPath = _binPath.EndsWith("\\") ? _binPath : _binPath + "\\";

            // Замена плейсхолдеров
            rawArg = rawArg.Replace("%LISTS%", listsPath)
                           .Replace("%BIN%", binPath)
                           .Replace("%GameFilterStatus%", _gameFilterStatus)
                           .Replace("%GameFilter%", _gameFilter)
                           .Replace("%GameFilterTCP%", _gameFilterTCP)
                           .Replace("%GameFilterUDP%", _gameFilterUDP);

            // Замена '=' на пробел (разделитель ключ-значение)
            rawArg = rawArg.Replace('=', ' ');

            // Экранирование кавычек
            rawArg = rawArg.Replace("\"", "\\\"");

            return rawArg.Trim();
        }

        private string GetListsPath()
        {
            string[] allFiles = Directory.GetFiles(_folderPath, "*", SearchOption.AllDirectories);
            foreach (string file in allFiles)
            {
                var name = Path.GetFileName(file);
                if (name.Contains("list"))
                {
                    string listsPath = Path.GetDirectoryName(file);
                    return listsPath;
                }
            }
            return "";
        }

        private string GetWinsPath()
        {
            string[] allFiles = Directory.GetFiles(_folderPath, "*", SearchOption.AllDirectories);
            foreach (string file in allFiles)
            {
                var name = Path.GetFileName(file);
                if (name.Contains("winws.exe"))
                {
                    return Path.GetFullPath(file);
                }
            }
            return "";
        }

        private void EnableTcpTimestamps()
        {
            try
            {
                var psi = new ProcessStartInfo("netsh", "interface tcp show global")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                if (!output.Contains("timestamps") || !output.Contains("enabled"))
                {
                    Process.Start("netsh", "interface tcp set global timestamps=enabled")?.WaitForExit();
                    Console.WriteLine("TCP timestamps включены.");
                }
                else
                {
                    Console.WriteLine("TCP timestamps уже включены.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось проверить/включить TCP timestamps: {ex.Message}");
            }
        }

        public bool InstallService(string strategyFilePath, bool enableTcp = true)
        {
            if (enableTcp)
                EnableTcpTimestamps();

            if (!File.Exists(strategyFilePath))
            {
                Console.WriteLine($"Файл стратегии не найден: {strategyFilePath}");
                return false;
            }

            string args = GetBypassArg(strategyFilePath);
            if (string.IsNullOrEmpty(args) || args == "Аргументы не найдены")
            {
                Console.WriteLine("Не удалось извлечь аргументы.");
                return false;
            }

            if (!File.Exists(_winsPath))
            {
                Console.WriteLine($"winws.exe не найден: {_winsPath}");
                return false;
            }

            string command = $"create {_serviceName} binPath= \"\\\"{_winsPath}\\\" {args}\" DisplayName= \"{_serviceName}\" start= auto";
            RunSc($"stop \"{_serviceName}\"", true);
            RunSc($"delete \"{_serviceName}\"", true);
            Console.WriteLine(command);
            int exitCode = RunSc(command);

            if (exitCode != 0)
            {
                Console.WriteLine($"Ошибка создания службы, код {exitCode}");
                return false;
            }

            RunSc($"description \"{_serviceName}\" \"{_description}\"");
            RunSc($"start \"{_serviceName}\"");

            string strategyName = Path.GetFileNameWithoutExtension(strategyFilePath);
            string regKey = $@"HKLM\System\CurrentControlSet\Services\{_serviceName}";

            try
            {
                Process.Start("reg", $"add \"{regKey}\" /v zapret-discord-youtube /t REG_SZ /d \"{strategyName}\" /f")?.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи в реестр: {ex.Message}");
            }

            Console.WriteLine("Служба успешно установлена и запущена.");
            return true;
        }

        public void RemoveService()
        {
            RunSc($"stop \"{_serviceName}\"", true);
            RunSc($"delete \"{_serviceName}\"", true);

            try
            {
                Process.Start("taskkill", "/IM winws.exe /F")?.WaitForExit();
            }
            catch { }

            string[] divertServices = { "WinDivert", "WinDivert14" };
            foreach (var svc in divertServices)
            {
                RunSc($"stop \"{svc}\"", true);
                RunSc($"delete \"{svc}\"", true);
            }

            Console.WriteLine("Служба zapret и связанные драйверы удалены.");
        }

        private int RunSc(string arguments, bool ignoreErrors = false)
        {
            try
            {
                var psi = new ProcessStartInfo("sc", arguments)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using var proc = Process.Start(psi);
                proc.WaitForExit();
                if (!ignoreErrors && proc.ExitCode != 0)
                {
                    string error = proc.StandardError.ReadToEnd();
                    Console.WriteLine($"sc ошибка: {error}");
                }
                return proc.ExitCode;
            }
            catch (Exception ex)
            {
                if (!ignoreErrors) Console.WriteLine($"Ошибка sc: {ex.Message}");
                return -1;
            }
        }
    }
}