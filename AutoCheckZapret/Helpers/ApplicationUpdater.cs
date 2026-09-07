using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace AutoCheckZapret.Helpers
{
    /// <summary>
    /// Статический класс для проверки и установки обновлений приложения
    /// </summary>
    public static class ApplicationUpdater
    {
        /// <summary>
        /// Проверяет наличие новой версии приложения на GitHub и предлагает обновиться
        /// </summary>
        public static async void CheckForUpdatesAsync()
        {
            if (!await IsGitHubAccessibleAsync())
            {
                MessageBox.Show("Нет подключения к GitHub", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version!;
            string currentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, $"AutoCheckZapret/{currentVersion}");

                    string jsonString = await wc.DownloadStringTaskAsync("https://api.github.com/repos/kaciel13/auto-check-zapret/releases/latest");

                    JsonDocument json = JsonDocument.Parse(jsonString);
                    string latestVersion = json.RootElement.GetProperty("tag_name").GetString();

                    if (IsNewVersionAvailable(currentVersion, latestVersion))
                    {
                        if (MessageBox.Show($"Вышло новое обновление.\n{currentVersion} => {latestVersion}\nОбновить?",
                        "Обновление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            await DownloadAndInstallUpdateAsync(latestVersion);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Проверяет доступность GitHub
        /// </summary>
        /// <returns>True, если GitHub доступен, иначе False</returns>
        public static async Task<bool> IsGitHubAccessibleAsync()
        {
            try
            {
                await Dns.GetHostEntryAsync("github.com");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет, доступна ли новая версия
        /// </summary>
        /// <param name="currentVersion">Текущая версия приложения</param>
        /// <param name="latestVersion">Последняя доступная версия</param>
        /// <returns>True, если новая версия доступна, иначе False</returns>
        private static bool IsNewVersionAvailable(string currentVersion, string latestVersion)
        {
            try
            {
                return Version.Parse(currentVersion) < Version.Parse(latestVersion);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Скачивает и устанавливает обновление
        /// </summary>
        /// <param name="version">Версия для установки</param>
        private static async Task DownloadAndInstallUpdateAsync(string version)
        {
            try
            {
                string downloadUrl = $"https://github.com/kaciel13/auto-check-zapret/releases/download/{version}/ACZ.zip";
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string updateScriptPath = Path.Combine(appDir, "Helpers", "update.bat");

                if (!File.Exists(updateScriptPath))
                {
                    MessageBox.Show($"Файл обновления не найден: {updateScriptPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = updateScriptPath,
                    Arguments = $"\"{downloadUrl}\"",
                    WorkingDirectory = appDir,
                    WindowStyle = ProcessWindowStyle.Normal,
                    CreateNoWindow = false,
                    UseShellExecute = true
                };

                Process.Start(processInfo);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}