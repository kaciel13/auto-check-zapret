using HtmlAgilityPack;

using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ZapretParser
{
    private readonly HttpClient _httpClient;

    public ZapretParser()
    {
        _httpClient = new HttpClient();
    }

    public async Task<Dictionary<string, string>> FetchReleasesAsync()
    {
        var releases = new Dictionary<string, string>();
        string lastVersion = string.Empty;
        string url = "https://github.com/Flowseal/zapret-discord-youtube/tags";

        while (true)
        {
            var response = await _httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Получаем все теги и ссылки на загрузки
            var versionNodes = htmlDoc.DocumentNode.SelectNodes("//h2/a[@data-view-component='true']");
            var downloadNodes = htmlDoc.DocumentNode.SelectNodes("//li/a[contains(@href, 'zip')]");

            if (versionNodes == null || downloadNodes == null)
            {
                break; // Выход из цикла, если узлы не найдены
            }

            bool foundNewVersion = false;

            for (int i = 0; i < versionNodes.Count; i++)
            {
                var tag = versionNodes[i].InnerText.Trim(); // Получаем тег

                var link = "https://github.com" + downloadNodes[i].GetAttributeValue("href", ""); // Получаем ссылку на архив

                if (!releases.ContainsKey(tag))
                {
                    releases[tag] = link; // Добавляем в словарь
                    lastVersion = tag; // Обновляем последнюю версию
                    foundNewVersion = true; // Устанавливаем флаг, что новая версия найдена
                }
            }

            // Выход из цикла, если нет новых версий
            if (!foundNewVersion)
            {
                break;
            }

            // Переход на следующую страницу, если существует
            url = GetNextPageUrl(lastVersion);
        }

        return releases;
    }

    private string GetNextPageUrl(string lastVersion)
    {
        return !string.IsNullOrEmpty(lastVersion)
            ? $"https://github.com/Flowseal/zapret-discord-youtube/tags?after={lastVersion}"
            : string.Empty; // Возвращаем пустую строку, если нет последней версии
    }

    public async Task DownloadAndExtractAsync(string version, string url, TextBox infoTextBox, ProgressBar progressBar)
    {
        progressBar.Value = 0;

        var fileName = Path.GetFileName(url);
        var downloadPath = Path.Combine("zaprets", fileName);
        

        try
        {
            // Создаем папку zaprets если не существует
            if (!Directory.Exists("zaprets"))
            {
                Directory.CreateDirectory("zaprets");
            }

            // Скачивание файла
            progressBar.Value = 30;
            infoTextBox.AppendText($"Скачивание {fileName}..." + Environment.NewLine);

            using (var response = await _httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                await using var stream = await response.Content.ReadAsStreamAsync();

                await using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }

            progressBar.Value = 60;

           

            progressBar.Value = 70;
            infoTextBox.AppendText("Распаковка архива..." + Environment.NewLine);

            // Регистрируем провайдер кодировок
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Пытаемся распаковать с разными кодировками
            try
            {
                // Попытка 1: с кодировкой 866 (Windows-1251)
                ZipFile.ExtractToDirectory(downloadPath, "zaprets",Encoding.GetEncoding(866), true);
            }
            catch
            {
                try
                {
                    // Попытка 2: с UTF-8
                    ZipFile.ExtractToDirectory(downloadPath, "zaprets", Encoding.UTF8, true);
                }
                catch
                {
                    // Попытка 3: с кодировкой по умолчанию
                    ZipFile.ExtractToDirectory(downloadPath, "zaprets", true);
                }
            }

            progressBar.Value = 90;

            // Удаление загруженного файла
            File.Delete(downloadPath);
            progressBar.Value = 100;

            infoTextBox.AppendText($"+ Версия {version} успешно скачана и распакована" + Environment.NewLine);
        }
        catch (Exception ex)
        {
            infoTextBox.AppendText($"- Ошибка при обработке версии {version}: {ex.Message}" + Environment.NewLine);
            progressBar.Value = 0;

            // Очистка в случае ошибки
            if (File.Exists(downloadPath))
                File.Delete(downloadPath);
        }
    }
}
