using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace AutoCheckZapret.Helpers
{
    public static class Updater
    {
        public static async void CheckUpdate()
        {
            if (!await OKAsync())
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
                    string lastVersion = json.RootElement.GetProperty("tag_name").GetString();

                    if (Version.Parse(currentVersion) < Version.Parse(lastVersion))
                    {
                        if (MessageBox.Show($"Вышло новое обновление.\n{currentVersion} => {lastVersion}\nОбновить?",
                        "Обновление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string downloadUrl = $"https://github.com/kaciel13/auto-check-zapret/releases/download/{lastVersion}/ACZ.zip";
                                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                                string batPath = Path.Combine(appDir, "Helpers", "update.bat");

                                if (!File.Exists(batPath))
                                {
                                    MessageBox.Show($"Файл обновления не найден: {batPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = batPath,
                                    Arguments = $"\"{downloadUrl}\"",
                                    WorkingDirectory = appDir,
                                    WindowStyle = ProcessWindowStyle.Normal,
                                    CreateNoWindow = false,
                                    UseShellExecute = true
                                };
                                Process.Start(psi);
                                Environment.Exit(0);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static async Task<bool> OKAsync()
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
    }
}