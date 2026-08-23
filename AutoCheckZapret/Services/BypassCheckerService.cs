using AutoCheckZapret.Helpers;

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
        /// <param name="zapretService">Сервис для нахождения .bat-файлов с обходами, их запуска (установки служб Zapret) и закрытия (удаления служб Zapret)</param>
        /// <param name="logger">Объект логгера</param>
        /// <param name="ct">Токен отмены. Используется для остановки процесса подбора обхода</param>
        /// <returns>bool - удалось ли подобрать подходящий обход из списка, string - название .bat-файла с подходящим обходом (если подходящий найден)</returns>
        public static async Task<ValueTuple<bool, string>> FindBypassMethodAsync(ZapretService zapretService, Logger logger, CancellationToken ct)
        {
            List<string> bypassFiles = zapretService.GetBypassFilesFromFolder();

            for (int i = 0; i < bypassFiles.Count; i++)
            {
                // Перед началом тестирования каждого обхода проверяем, не отменил ли пользователь подбор обходов
                await zapretService.RemoveServiceAsync(ct);
                ct.ThrowIfCancellationRequested();

                string bypassMethodName = string.Empty;
                int index = bypassFiles[i].LastIndexOf("\\");
                bypassMethodName = bypassFiles[i].Substring(index + 1);
                logger.AddInfo($"Тест обхода \"{bypassMethodName}\" ({i + 1}/{bypassFiles.Count})...");

                bool hasStartedZapret = await zapretService.InstallServiceAsync(bypassFiles[i], true, ct);
                if (!hasStartedZapret)
                {
                    logger.AddError("Не удалось запустить службу Zapret...", false);
                    continue;
                }

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

                if (isDiscordResponding && isYouTubeResponding)
                {
                    return (true, bypassMethodName);
                }

                logger.AddError($"Обход \"{bypassMethodName}\" не подходит...");
                logger.AddInfo("");
            }

            return (false, string.Empty);
        }
    }
}