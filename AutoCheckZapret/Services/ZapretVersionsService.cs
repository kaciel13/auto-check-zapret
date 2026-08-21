using AutoCheckZapret.Models;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;

namespace AutoCheckZapret.Services
{
    /// <summary>
    /// Сервис для получения информации о доступных версиях Zapret из официального репозитория,
    /// их скачивания и удаления
    /// </summary>
    public class ZapretVersionsService
    {
        private const string BaseUrl = "https://github.com/Flowseal/zapret-discord-youtube";
        private const string DownloadPath = "versions";

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ZapretVersionsService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Получение информации о всех доступных версиях Zapret
        /// </summary>
        /// <returns>Список доступных версий в виде ZapretVersion</returns>
        public async Task<ObservableCollection<ZapretVersion>> FetchAvailableVersions()
        {
            ObservableCollection<ZapretVersion> versions = new ObservableCollection<ZapretVersion>();
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

            ZipFile.ExtractToDirectory(versionPath, DownloadPath, true);

            // Удаляем распакованный архив
            File.Delete(versionPath);

            // Пользователь может скачать не самую новую версию Zapret
            // В таком случае при запуске любого обхода Zapret будет проверять себя на наличие обновлений
            // И будет открываться страница в браузере с новейшей версией, чего нам не надо
            // Поэтому обрубаем Zapret возможность провериться на обновления
            string servicePath = Path.Combine(DownloadPath, $"zapret-discord-youtube-{version.Number}", "service.bat");
            string targetText = ":service_check_updates";
            string textToInsert = "goto menu";

            // Лезем в service.bat скачанной версии Zapret
            List<string> allLines;
            try
            {
                // Если тут ловим, что файл не найден, то говорим, что версия успешно скачана и всё
                // Старые версии Zapret не имеют единого service.bat
                allLines = File.ReadAllLines(servicePath).ToList();
            }
            catch (FileNotFoundException) { return true; }

            int targetIndex = allLines.IndexOf(targetText);
            if (targetIndex == -1) { return false; } // Будем считать, что версия не поддерживается, если мы не можем убрать возможность проверки на обновления

            allLines.Insert(targetIndex + 1, textToInsert);

            // Можем позволить себе перезаписать весь файл полностью, потому что service.bat весит немного
            File.WriteAllLines(servicePath, allLines);

            return true;
        }

        /// <summary>
        /// Удаление папки с выбранной версией Zapret
        /// </summary>
        /// <param name="version">Версия Zapret для удаления</param>
        public bool DeleteZapretVersion(ZapretVersion version)
        {
            string versionFolderPath = GetVersionFolderPath(version);
            if (!Directory.Exists(versionFolderPath)) { return true; }

            try
            {
                Directory.Delete(versionFolderPath, true);
            }
            catch (IOException ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Скачана ли у пользователя выбранная версия Zapret
        /// </summary>
        /// <param name="version">Версия Zapret для проверки</param>
        /// <returns>true - версия скачана, false - версия не скачана</returns>
        public bool IsZapretVersionDownloaded(ZapretVersion version)
        {
            string versionPath = GetVersionFolderPath(version);

            // Смотрим следующее:
            // 1. Папки нет вообще
            // 2. Папка пустая, а также вложенные в неё папки пустые
            // 3. В папке нет ни одного .bat-файла
            if (string.IsNullOrEmpty(versionPath) ||
                !Directory.EnumerateFiles(versionPath, "*", SearchOption.AllDirectories).Any() ||
                !Directory.EnumerateFiles(versionPath, "*.bat").Any())
            { 
                return false; 
            }

            return true;
        }

        /// <summary>
        /// Получить URL страницы с версиями Zapret, идущей после последней версии программы на текущей странице
        /// </summary>
        /// <param name="lastVersionNumber">Название последней версии Zapret на текущей странице</param>
        private string GetNextVersionsPageUrl(string lastVersionNumber)
        {
            return $"{BaseUrl}/tags?after={lastVersionNumber}";
        }

        /// <summary>
        /// Получить путь до папки с указанной установленной версией Zapret
        /// </summary>
        /// <param name="version">Версия Zapret, папку которой нужно найти</param>
        /// <returns>Путь до папки установленной версии Zapret или пустую строку в случае, если версия не скачана</returns>
        private string GetVersionFolderPath(ZapretVersion version)
        {
            if (!Directory.Exists(DownloadPath)) { return string.Empty; }

            // Использование звёздочек в номере версии нужно, чтобы метод искал совпадение в любом месте названия папки
            string? versionDirectoryPath = Directory.GetDirectories(DownloadPath, "*", SearchOption.TopDirectoryOnly)
                                           .Where(dir => Path.GetFileName(dir).Contains(version.Number))
                                           .Where(dir => Path.GetFileName(dir).EndsWith(version.Number)) // Добавляем точное совпадение окончания номера версии в названии папки
                                           .FirstOrDefault();

            if (versionDirectoryPath == null)
            {
                return string.Empty;
            }
            else
            {
                return versionDirectoryPath;
            }
        }
    }
}
