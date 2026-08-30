using AutoCheckZapret.Models;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
                Debug.WriteLine($"Загрузка страницы: {url}");

                string response = await _httpClient.GetStringAsync(url);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(response);

                // Получаем все теги (версии)
                HtmlNodeCollection versionsNodes = document.DocumentNode.SelectNodes("//h2/a[@data-view-component='true']");
                // Получаем ссылки на скачивание версий
                HtmlNodeCollection downloadNodes = document.DocumentNode.SelectNodes("//li/a[contains(@href, 'zip')]");

                // Если не нашли версий на текущей странице, значит страницы и версии закончились и парсинг можно прекращать
                if (versionsNodes == null || downloadNodes == null)
                {
                    Debug.WriteLine("Версии или ссылки на скачивание не найдены, завершаем парсинг");
                    break;
                }

                Debug.WriteLine($"Найдено {versionsNodes.Count} версий на странице");  

                for (int i = 0; i < versionsNodes.Count; i++)
                {
                    string tag = versionsNodes[i].InnerText.Trim(); // Номер версии
                    string archiveUrl = "https://github.com" + downloadNodes[i].GetAttributeValue("href", ""); // Ссылка для скачивания архива с версией

                    versions.Add(new ZapretVersion(tag, archiveUrl));
                    Debug.WriteLine($"Добавлена версия: {tag}");  
                }

                url = GetNextVersionsPageUrl(versions[versions.Count - 1].Number);
            }

            Debug.WriteLine($"Всего найдено версий: {versions.Count}");  
            return versions;
        }

        /// <summary>
        /// Скачивание архива с версией Zapret и распаковывание его в папку с версиями
        /// </summary>
        /// <param name="version">Версия Zapret для скачивания</param>
        /// <returns>true - скачивание и распаковка успешны, false - ошибка при скачивании и распаковывании версии Zapret</returns>
        public async Task<bool> DownloadZapretVersion(ZapretVersion version)
        {
            Debug.WriteLine($"Начинаем скачивание версии: {version.Number}");  

            if (!Directory.Exists(DownloadPath))
            {
                Directory.CreateDirectory(DownloadPath);
                Debug.WriteLine($"Создана директория: {DownloadPath}");  
            }

            string versionPath = Path.Combine(DownloadPath, version.Number);
            Debug.WriteLine($"Путь к архиву: {versionPath}");  

            using (HttpResponseMessage response = await _httpClient.GetAsync(version.DownloadUrl))
            {
                response.EnsureSuccessStatusCode();
                await using Stream stream = await response.Content.ReadAsStreamAsync();

                await using (FileStream fileStream = new FileStream(versionPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }

            Debug.WriteLine($"Архив скачан, начинаем распаковку");  

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Получаем все папки с версиями Zapret до распаковки архива
            HashSet<string?>? foldersBefore = Directory.GetDirectories(DownloadPath).Select(Path.GetFileName).ToHashSet();

            ZipFile.ExtractToDirectory(versionPath, DownloadPath, true);

            HashSet<string?>? foldersAfter = Directory.GetDirectories(DownloadPath).Select(Path.GetFileName).ToHashSet();
            // Смотрим, какая папка только что была распакована
            List<string?>? newFolders = foldersAfter.Except(foldersBefore).ToList();

            // Удаляем распакованный архив
            File.Delete(versionPath);
            Debug.WriteLine($"Архив удален");

            string oldFolderPath = Path.Combine(DownloadPath, newFolders[0]!);
            string newFolderPath = Path.Combine(DownloadPath, version.Number);

            // Переименовываем папку с версией из zapret-discord-youtube-V в просто V, то есть номер версии, например 1.0.2
            Directory.Move(oldFolderPath, newFolderPath);

            // Пользователь может скачать не самую новую версию Zapret
            // В таком случае при запуске любого обхода Zapret будет проверять себя на наличие обновлений
            // И будет открываться страница в браузере с новейшей версией, чего нам не надо
            // Поэтому обрубаем Zapret возможность провериться на обновления
            string servicePath = Path.Combine(DownloadPath, $"{version.Number}", "service.bat");
            Debug.WriteLine($"Путь к service.bat: {servicePath}");  

            string targetText = ":service_check_updates";
            string textToInsert = "goto menu";

            // Лезем в service.bat скачанной версии Zapret
            List<string> allLines;
            try
            {
                // Если тут ловим, что файл не найден, то говорим, что версия успешно скачана и всё
                // Старые версии Zapret не имеют единого service.bat
                allLines = File.ReadAllLines(servicePath).ToList();
                Debug.WriteLine($"service.bat найден, начинаем модификацию");  
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine($"service.bat не найден для версии {version.Number}, пропускаем модификацию");  
                return true;
            }

            int targetIndex = allLines.IndexOf(targetText);
            if (targetIndex == -1)
            {
                Debug.WriteLine($"Строка '{targetText}' не найдена в service.bat для версии {version.Number}");  
                return false;
            }

            allLines.Insert(targetIndex + 1, textToInsert);

            // Можем позволить себе перезаписать весь файл полностью, потому что service.bat весит немного
            File.WriteAllLines(servicePath, allLines);
            Debug.WriteLine($"service.bat успешно модифицирован для версии {version.Number}");  

            return true;
        }

        /// <summary>
        /// Удаление папки с выбранной версией Zapret
        /// </summary>
        /// <param name="version">Версия Zapret для удаления</param>
        public bool DeleteZapretVersion(ZapretVersion version)
        {
            Debug.WriteLine($"Удаление версии: {version.Number}");  

            string versionFolderPath = GetVersionFolderPath(version);
            if (!Directory.Exists(versionFolderPath))
            {
                Debug.WriteLine($"Папка с версией {version.Number} не найдена!");  
                return true;
            }

            try
            {
                Directory.Delete(versionFolderPath, true);
                Debug.WriteLine($"Папка с версией {version.Number} успешно удалена!");  
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при удалении версии {version.Number}: {ex.Message}");  
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
                Debug.WriteLine($"Версия {version.Number} не скачана");  
                return false;
            }

            Debug.WriteLine($"Версия {version.Number} скачана");  
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