using AutoCheckZapret.Helpers;
using System.Diagnostics;

namespace AutoCheckZapret.Services
{
    /// <summary>
    /// Сервис для подбора (проверки работоспособности) обходов для выбранной версии Zapret
    /// </summary>
    public static class BypassCheckerService
    {
        /// <summary>
        /// Найти подходящий обход из списка обходов в версии Zapret
        /// </summary>
        /// <param name="bypassMethodsPaths">Список относительных адресов (от версии Zapret) до .bat-файлов с обходами</param>
        /// <param name="logger">Объект логгера</param>
        /// <param name="versionPath">Относительный (от ACZ) путь до версии Zapret</param>
        /// <param name="ct">Токен отмены. Используется для остановки процесса подбора обхода</param>
        /// <returns>bool - удалось ли подобрать подходящий обход из списка, string - название .bat-файла с подходящим обходом (если подходящий найден)</returns>
        public static async Task<ValueTuple<bool, string>> FindBypassMethodAsync(List<string> bypassMethodsPaths, Logger logger, string versionPath, CancellationToken ct)
        {
            for (int i = 0; i < bypassMethodsPaths.Count; i++)
            {
                // Перед началом тестирования каждого обхода проверяем, не отменил ли пользователь подбор обходов
                ct.ThrowIfCancellationRequested();

                string bypassMethodName = string.Empty;
                int index = bypassMethodsPaths[i].LastIndexOf("\\");
                bypassMethodName = bypassMethodsPaths[i].Substring(index + 1);

                logger.AddInfo($"Тест обхода \"{bypassMethodName}\" ({i + 1}/{bypassMethodsPaths.Count})...");
                using (Process mainProcess = new Process())
                {
                    mainProcess.StartInfo = new ProcessStartInfo()
                    {
                        FileName = "cmd.exe",
                        WorkingDirectory = versionPath,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = "/k" + bypassMethodName
                    };
                    mainProcess.Start();

                    logger.AddInfo("    Проверка Discord...");
                    bool isDiscordResponding = await UrlChecker.IsUrlRespondingAsync("http://discord.com");
                    if (isDiscordResponding)
                    {
                        logger.AddSuccess("Работает!", false);
                    }
                    else
                    {
                        logger.AddError("Не отвечает...", false);
                    }

                    logger.AddInfo("    Проверка YouTube...");
                    bool isYouTubeResponding = await UrlChecker.IsUrlRespondingAsync("https://www.youtube.com");
                    if (isYouTubeResponding)
                    {
                        logger.AddSuccess("Работает!", false);
                    }
                    else
                    {
                        logger.AddError("Не отвечает...", false);
                    }

                    mainProcess.CloseMainWindow(); // Закрываем открытую ранее консольку
                    // Убиваем процесс, который был открыт консолькой
                    Process[] processes = Process.GetProcessesByName("winws");
                    foreach (Process process in processes)
                    {
                        process.Kill();
                        process.WaitForExit();
                        process.Dispose();
                    }

                    if (isDiscordResponding && isYouTubeResponding)
                    {
                        return (true, bypassMethodName);
                    }
                }

                logger.AddError($"Обход \"{bypassMethodName}\" не подходит...");
                logger.AddInfo("");
            }

            return (false, string.Empty);
        }
    }
}