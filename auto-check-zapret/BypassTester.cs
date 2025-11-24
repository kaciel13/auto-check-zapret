using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace auto_check_zapret
{
    public class BypassTester
    {
        public async Task<bool> TestBypassAsync(string folderPath, TextBox infoTextBox)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                infoTextBox.AppendText($"❌ Папка не найдена: {folderPath}" + Environment.NewLine);
                return false;
            }

            string serviceBatPath = Path.Combine(folderPath, "service.bat");
            if (!File.Exists(serviceBatPath))
            {
                infoTextBox.AppendText($"❌ Файл service.bat не найден в папке: {folderPath}" + Environment.NewLine);
                return false;
            }

            infoTextBox.AppendText($"🔍 Начинаю тестирование обхода из папки: {Path.GetFileName(folderPath)}" + Environment.NewLine);

            try
            {
                // Запускаем service.bat
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = serviceBatPath,
                        WorkingDirectory = folderPath,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        LoadUserProfile = true
                    };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            infoTextBox.Invoke(new Action(() =>
                                infoTextBox.AppendText($"[BAT] {e.Data}" + Environment.NewLine)));
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            infoTextBox.Invoke(new Action(() =>
                                infoTextBox.AppendText($"[ERROR] {e.Data}" + Environment.NewLine)));
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Имитируем ввод: 1 + Enter, 1 + Enter
                    await Task.Delay(1000);
                    await process.StandardInput.WriteLineAsync("2");
                    await Task.Delay(1000);
                    await process.StandardInput.WriteLineAsync("1");
                    await Task.Delay(500);
                    await process.StandardInput.WriteLineAsync("1");
                    await Task.Delay(500);

                    // Ждем завершения процесса или таймаут
                    bool exited = process.WaitForExit(10000);
                    if (!exited)
                    {
                        process.Kill();
                        infoTextBox.AppendText("⚠ Процесс был завершен по таймауту" + Environment.NewLine);
                    }

                    await Task.Delay(2000); // Даем время для применения настроек
                }

                // Проверяем ping до YouTube и Discord
                bool youtubeSuccess = await TestPingAsync("youtube.com", infoTextBox, "YouTube");
                bool discordSuccess = await TestPingAsync("discord.com", infoTextBox, "Discord");

                if (youtubeSuccess && discordSuccess)
                {
                    infoTextBox.AppendText($"✅ Обход работает корректно! Оба сервиса доступны." + Environment.NewLine);
                    return true;
                }
                else
                {
                    infoTextBox.AppendText($"❌ Обход не работает. Проблемы с доступом к сервисам." + Environment.NewLine);
                    return false;
                }
            }
            catch (Exception ex)
            {
                infoTextBox.AppendText($"❌ Ошибка при тестировании обхода: {ex.Message}" + Environment.NewLine);
                return false;
            }
        }

        private async Task<bool> TestPingAsync(string host, TextBox infoTextBox, string serviceName)
        {
            try
            {
                using (var pingProcess = new Process())
                {
                    pingProcess.StartInfo = new ProcessStartInfo
                    {
                        FileName = "ping",
                        Arguments = $"-n 4 {host}", // 4 пакета для Windows
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    pingProcess.Start();
                    string output = await pingProcess.StandardOutput.ReadToEndAsync();
                    pingProcess.WaitForExit();

                    // Анализируем результат ping
                    if (output.Contains("TTL=") && !output.Contains("Превышен интервал"))
                    {
                        // Извлекаем среднее время задержки
                        var avgDelay = ExtractAveragePing(output);
                        infoTextBox.AppendText($"📡 {serviceName}: доступен, задержка {avgDelay} мс" + Environment.NewLine);

                        // Считаем обход успешным если задержка меньше 500 мс
                        return avgDelay < 500;
                    }
                    else
                    {
                        infoTextBox.AppendText($"❌ {serviceName}: недоступен или большая задержка" + Environment.NewLine);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                infoTextBox.AppendText($"❌ Ошибка ping для {serviceName}: {ex.Message}" + Environment.NewLine);
                return false;
            }
        }

        private int ExtractAveragePing(string pingOutput)
        {
            try
            {
                // Ищем строку с средним временем (для русского и английского Windows)
                string[] lines = pingOutput.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("Average =") || line.Contains("Среднее ="))
                    {
                        var parts = line.Split('=');
                        if (parts.Length > 1)
                        {
                            var timePart = parts[1].Trim();
                            var msIndex = timePart.IndexOf("ms");
                            if (msIndex > 0)
                            {
                                var timeStr = timePart.Substring(0, msIndex).Trim();
                                if (int.TryParse(timeStr, out int result))
                                {
                                    return result;
                                }
                            }
                        }
                    }
                }
                return 999; // Если не удалось извлечь - считаем большой задержкой
            }
            catch
            {
                return 999;
            }
        }

        // Метод для поиска папки с bat файлами в установленной версии
        public string FindBatFolder(string versionFolder)
        {
            if (!Directory.Exists(versionFolder))
                return null;

            // Ищем service.bat в различных возможных местах
            string[] possiblePaths = {
                versionFolder,
                Path.Combine(versionFolder, "bin"),
                Path.Combine(versionFolder, "scripts"),
                Path.Combine(versionFolder, "zapret-discord-youtube", "bin"),
                Path.Combine(versionFolder, "zapret-discord-youtube", "scripts")
            };

            foreach (string path in possiblePaths)
            {
                if (Directory.Exists(path) && File.Exists(Path.Combine(path, "service.bat")))
                {
                    return path;
                }
            }

            // Если не нашли в стандартных путях, ищем рекурсивно
            try
            {
                var batFiles = Directory.GetFiles(versionFolder, "service.bat", SearchOption.AllDirectories);
                if (batFiles.Length > 0)
                {
                    return Path.GetDirectoryName(batFiles[0]);
                }
            }
            catch
            {
                // Игнорируем ошибки доступа
            }

            return null;
        }
    }
}