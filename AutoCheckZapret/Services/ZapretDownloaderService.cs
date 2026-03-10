using AutoCheckZapret.Models;
using HtmlAgilityPack;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;

namespace AutoCheckZapret.Services
{
    /// <summary>
    /// Сервис для получения информации о доступных версиях
    /// и скачивания выбранной версии Zapret из официального репозитория
    /// </summary>
    public class ZapretDownloaderService
    {
        private const string BaseUrl = "https://github.com/Flowseal/zapret-discord-youtube";
        private const string DownloadPath = "versions";

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ZapretDownloaderService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Получение информации о всех доступных версиях Zapret
        /// </summary>
        /// <returns>Список доступных версий в виде ZapretVersion</returns>
        public async Task<List<ZapretVersion>> FetchAvailableVersions()
        {
            List<ZapretVersion> versions = new List<ZapretVersion>();
            string url = $"{BaseUrl}/tags";

            // Проходимся по всем страницам с версиями Zapret
            while (true)
            {
                string response = await _httpClient.GetStringAsync(url);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(response);

                // Получаем все теги (версии)
                HtmlNodeCollection versionsNodes = document.DocumentNode.SelectNodes("//h2/a[@data-view-component='true']");
                // Получаем ссылки на скачивание версий
                HtmlNodeCollection downloadNodes = document.DocumentNode.SelectNodes("//li/a[contains(@href, 'zip')]");

                // Если не нашли версий на текущей странице, значит страницы и версии закончились и парсинг можно прекращать
                if (versionsNodes == null || downloadNodes == null) { break; }

                for (int i = 0; i < versionsNodes.Count; i++)
                {
                    string tag = versionsNodes[i].InnerText.Trim(); // Номер версии
                    string archiveUrl = "https://github.com" + downloadNodes[i].GetAttributeValue("href", ""); // Ссылка для скачивания архива с версией

                    versions.Add(new ZapretVersion(tag, archiveUrl));
                }

                url = GetNextVersionsPageUrl(versions[versions.Count - 1].Number);
            }

            return versions;
        }

        /// <summary>
        /// Скачивание архива с версией Zapret и распаковывание его в папку с версиями
        /// </summary>
        /// <param name="version">Версия Zapret для скачивания</param>
        /// <returns>true - скачивание и распаковка успешны, false - ошибка при скачивании и распаковывании версии Zapret</returns>
        public async Task<bool> DownloadZapretVersion(ZapretVersion version)
        {
            if (!Directory.Exists(DownloadPath)) { Directory.CreateDirectory(DownloadPath); }

            string versionPath = Path.Combine(DownloadPath, version.Number);

            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(version.DownloadUrl))
                {
                    response.EnsureSuccessStatusCode();
                    await using Stream stream = await response.Content.ReadAsStreamAsync();

                    await using (FileStream fileStream = new FileStream(versionPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                try
                {
                    // Попытка 1: с кодировкой 866 (Windows-1251)
                    ZipFile.ExtractToDirectory(versionPath, DownloadPath, Encoding.GetEncoding(866), true);
                }
                catch
                {
                    try
                    {
                        // Попытка 2: с UTF-8
                        ZipFile.ExtractToDirectory(versionPath, DownloadPath, Encoding.UTF8, true);
                    }
                    catch
                    {
                        // Попытка 3: с кодировкой по умолчанию
                        ZipFile.ExtractToDirectory(versionPath, DownloadPath, true);
                    }
                }

                File.Delete(versionPath);
            }
            catch
            {
                if (File.Exists(versionPath)) { File.Delete(versionPath); }
            }

            return true;
        }

        private string GetNextVersionsPageUrl(string lastVersionNumber)
        {
            return $"{BaseUrl}/tags?after={lastVersionNumber}";
        }
    }
}
