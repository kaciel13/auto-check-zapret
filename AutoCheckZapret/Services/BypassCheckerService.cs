using AutoCheckZapret.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace AutoCheckZapret.Services
{
    public static class BypassCheckerService
    {
        /// <summary>
        /// Найти подходящий обход из списка обходов в версии Zapret.
        /// Использует <see cref="TestSingleBypassAsync"/> для проверки каждого файла.
        /// </summary>
        public static async Task<(bool Success, string BypassName)> FindBypassMethodAsync(
            ZapretService zapretService,
            Logger logger,
            CancellationToken ct)
        {
            List<string> bypassFiles = zapretService.GetBypassFilesFromFolder();
            await zapretService.RemoveServiceAsync(ct);

            for (int i = 0; i < bypassFiles.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                string fullPath = bypassFiles[i];
                string fileName = Path.GetFileName(fullPath);
                logger.AddInfo($"Тест обхода \"{fileName}\" ({i + 1}/{bypassFiles.Count})...");

                (bool success, string _) = await TestSingleBypassAsync(
                    zapretService,
                    fileName, 
                    logger,
                    ct);

                if (success)
                    return (true, fileName);

                await zapretService.RemoveServiceAsync(ct);
                logger.AddInfo("");
            }

            return (false, string.Empty);
        }

        /// <summary>
        /// Проверить работоспособность одного указанного обхода.
        /// </summary>
        /// <param name="zapretService">Сервис для установки/удаления служб</param>
        /// <param name="bypassMethodName">Имя .bat-файла обхода (без пути)</param>
        /// <param name="logger">Логгер</param>
        /// <param name="ct">Токен отмены</param>
        public static async Task<(bool Success, string BypassName)> TestSingleBypassAsync(
            ZapretService zapretService,
            string bypassMethodName,
            Logger logger,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(bypassMethodName))
            {
                logger.AddError("Имя файла обхода не указано.");
                return (false, string.Empty);
            }

            logger.AddInfo($"Тестирование обхода \"{bypassMethodName}\"...");

            await zapretService.RemoveServiceAsync(ct);
            ct.ThrowIfCancellationRequested();

            bool serviceInstalled = false;
            bool success = false;
            try
            {
                serviceInstalled = await zapretService.InstallServiceAsync(bypassMethodName, cancellationToken: ct);
                if (!serviceInstalled)
                {
                    logger.AddError("Не удалось запустить службу Zapret для данного обхода.", false);
                    return (false, bypassMethodName);
                }

                string[] urlsToCheck = new[]
                {
                    "https://discord.com",
                    "https://gateway.discord.gg",
                    "https://cdn.discordapp.com",
                    "https://updates.discord.com",
                    "https://www.youtube.com",
                    "https://youtu.be",
                    "https://i.ytimg.com",
                    "https://redirector.googlevideo.com",
                    "https://www.google.com",
                };

                logger.AddInfo("   Проверка доступности сервисов...");
                var tasks = urlsToCheck.Select(url => UrlChecker.IsUrlRespondingAsync(url, 10)).ToArray();
                bool[] results = await Task.WhenAll(tasks);

                bool allOk = true;
                for (int i = 0; i < urlsToCheck.Length; i++)
                {
                    bool ok = results[i];
                    string host = new Uri(urlsToCheck[i]).Host;
                    if (ok)
                        logger.AddSuccess($"   {host} работает!");
                    else
                    {
                        logger.AddError($"   {host} не отвечает...");
                        allOk = false;
                    }
                }
                logger.AddInfo("");
                success = allOk;
                if (success)
                    logger.AddSuccess($"Обход \"{bypassMethodName}\" подходит.");
                else
                    logger.AddError($"Обход \"{bypassMethodName}\" не подходит.");
            }
            finally
            {
                if (!success)
                    await zapretService.RemoveServiceAsync(ct);
            }

            return (success, bypassMethodName);
        }
    }
}