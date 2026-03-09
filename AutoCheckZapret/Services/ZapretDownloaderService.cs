using AutoCheckZapret.Models;
using HtmlAgilityPack;
using System.Net.Http;

namespace AutoCheckZapret.Services
{
    /// <summary>
    /// Сервис для получения информации о доступных версиях
    /// и скачивания выбранной версии Zapret из официального репозитория
    /// </summary>
    public class ZapretDownloaderService
    {
        private const string BaseUrl = "https://github.com/Flowseal/zapret-discord-youtube";

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

                // Если не нашли версий, значит они закончились и парсинг можно прекращать
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

        private string GetNextVersionsPageUrl(string lastVersionNumber)
        {
            return $"{BaseUrl}/tags?after={lastVersionNumber}";
        }
    }
}
