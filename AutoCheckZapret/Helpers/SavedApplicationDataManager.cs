using AutoCheckZapret.Models;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace AutoCheckZapret.Helpers
{
    /// <summary>
    /// Менеджер для работы с сохранёнными данными пользователя
    /// </summary>
    public static class SavedApplicationDataManager
    {
        // Имя файла для сохранения данных приложения (настройки, выбранная версия, подобранные обходы)
        private const string SavedDataFileName = "appdata.json";

        /// <summary>
        /// Загружает данные из файла appdata.json: последнюю выбранную версию,
        /// список скачанных версий и подобранные для них методы обхода.
        /// </summary>
        /// <param name="zapretVersions">Список всех доступных версий Zapret</param>
        /// <returns>Кортеж: (SelectedVersion, DownloadedVersions)</returns>
        public static (ZapretVersion? selectedVersion, List<ZapretVersion>? downloadedVersions) LoadSavedData(List<ZapretVersion> zapretVersions)
        {
            if (!File.Exists(SavedDataFileName) || zapretVersions == null)
                return (null, null);

            string json = File.ReadAllText(SavedDataFileName);
            SavedApplicationData? savedData = null;
            try
            {
                savedData = JsonConvert.DeserializeObject<SavedApplicationData>(json);
            }
            catch (JsonSerializationException)
            {
                MessageBox.Show("Файл с сохранёнными настройками приложения был повреждён. Загружены настройки по умолчанию.", "Файл повреждён", MessageBoxButton.OK, MessageBoxImage.Error);
                return (null, null);
            }

            if (savedData == null)
                return (null, null);

            // Создаём список для восстановленных скачанных версий
            var downloadedVersions = new List<ZapretVersion>();

            // Восстанавливаем информацию о скачанных версиях и их методах обхода
            foreach (var savedVersion in savedData.DownloadedZapretVersions)
            {
                var found = zapretVersions.FirstOrDefault(v => v.Number == savedVersion.Number);
                if (found != null)
                {
                    found.BypassMethodName = savedVersion.BypassMethodName;
                    found.IsDownloaded = true;
                    downloadedVersions.Add(found);
                }
            }

            // Восстанавливаем выбранную версию, если она присутствует в сохранённых данных
            ZapretVersion? selectedVersion = null;
            if (savedData.LastSelectedZapretVersion != null)
            {
                selectedVersion = zapretVersions.FirstOrDefault(v => v.Number == savedData.LastSelectedZapretVersion.Number);
            }

            return (selectedVersion, downloadedVersions);
        }

        /// <summary>
        /// Сохраняет текущее состояние приложения в файл appdata.json:
        /// выбранную версию и список скачанных версий с их методами обхода.
        /// </summary>
        /// <param name="zapretVersions">Список всех версий Zapret</param>
        /// <param name="selectedVersion">Выбранная пользователем версия</param>
        public static void SaveData(List<ZapretVersion> zapretVersions, ZapretVersion? selectedVersion)
        {
            if (zapretVersions == null)
                return;

            var data = new SavedApplicationData
            {
                LastSelectedZapretVersion = selectedVersion,
                DownloadedZapretVersions = zapretVersions
                    .Where(v => v.IsDownloaded)
                    .ToList()
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(SavedDataFileName, json);
        }

        /// <summary>
        /// Проверяет, существует ли файл с сохранёнными данными
        /// </summary>
        public static bool IsSavedDataExists() => File.Exists(SavedDataFileName);

        /// <summary>
        /// Удаляет файл с сохранёнными данными
        /// </summary>
        public static void DeleteSavedData()
        {
            if (File.Exists(SavedDataFileName))
                File.Delete(SavedDataFileName);
        }
    }
}