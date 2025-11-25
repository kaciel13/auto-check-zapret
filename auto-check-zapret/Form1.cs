using System;
using System.Numerics;

namespace auto_check_zapret
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> versions;
        ZapretParser parser = new ZapretParser();
        public Form1()
        {
            InitializeComponent();
            progressBar.Value = 0;
            infoTextBox.AppendText("Загрузка приложения..." + Environment.NewLine);
            progressBar.Value = 10;
            LoadZapretVersions();
        }

        private async void LoadZapretVersions()
        {
            try
            {
                infoTextBox.AppendText("Получение версий zapret на Github" + Environment.NewLine);
                versions = await parser.FetchReleasesAsync();
                progressBar.Value = 90;
                foreach (var version in versions)
                {

                    choiceVersionComboBox.Items.Add("Zapret " + version.Key);

                }
                progressBar.Value = 100;

                infoTextBox.AppendText("Версий получены" + Environment.NewLine);
                choiceVersionComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                infoTextBox.AppendText($"Ошибка получения версий c Github: {ex.Message}" + Environment.NewLine);
                infoTextBox.AppendText($"Получение версий zapret из папки zaprets" + Environment.NewLine);
                progressBar.Value = 50;

                string zapretsPath = Path.Combine(Application.StartupPath, "zaprets");
                string[] enableVersions = Directory.GetDirectories("zaprets", "*", SearchOption.TopDirectoryOnly);
                progressBar.Value = 80;
                foreach (var version in enableVersions)
                {
                    choiceVersionComboBox.Items.Add("Zapret " + version.Replace("zaprets\\zapret-discord-youtube-", ""));
                }
                infoTextBox.AppendText("Версий получены" + Environment.NewLine);
                choiceVersionComboBox.SelectedIndex = 0;
                progressBar.Value = 100;
            }


        }

        private void choiceVersionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (choiceVersionComboBox.SelectedIndex != -1)
            {
                string selectedText = choiceVersionComboBox.Text;
                string formattedVersion = selectedText.Replace("Zapret ", "");

                // Путь к папке zaprets
                string zapretsPath = Path.Combine(Application.StartupPath, "zaprets");

                // Проверяем и создаем папку если нужно
                if (!Directory.Exists(zapretsPath))
                {
                    try
                    {
                        Directory.CreateDirectory(zapretsPath);
                        infoTextBox.AppendText($"Папка zaprets создана: {zapretsPath}" + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        infoTextBox.AppendText($"Ошибка при создании папки zaprets: {ex.Message}" + Environment.NewLine);
                        return;
                    }
                }

                // Ищем папки с formattedVersion в названии
                string[] matchingFolders = Directory.GetDirectories(zapretsPath, $"*zapret-discord-youtube-{formattedVersion}*", SearchOption.TopDirectoryOnly);

                if (matchingFolders.Length > 0)
                {
                    downloadButton.Enabled = false;
                    downloadButton.Text = "Уже установлено";
                    infoTextBox.AppendText($"Найдена папка для версии {formattedVersion}:\n" + Environment.NewLine);
                    foreach (string folder in matchingFolders)
                    {
                        infoTextBox.AppendText($"- {Path.GetFileName(folder)}" + Environment.NewLine);
                    }
                }
                else
                {

                    downloadButton.Enabled = true;
                    downloadButton.Text = "Установить";
                }
            }
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            string version = choiceVersionComboBox.Text.Replace("Zapret ", "");
            if (versions.TryGetValue(version, out string downloadUrl))
            {
                // URL найден, используем downloadUrl
                infoTextBox.AppendText($"Найден URL: {downloadUrl}" + Environment.NewLine);
                // Дальнейшие действия с downloadUrl
            }
            else
            {
                // URL не найден
                infoTextBox.AppendText($"URL для версии {version} не найден" + Environment.NewLine);
            }

            infoTextBox.AppendText($"Скачиваю версию {version}..." + Environment.NewLine);

            await parser.DownloadAndExtractAsync(version, downloadUrl, infoTextBox, progressBar);

            infoTextBox.AppendText($"Скачивание завершено" + Environment.NewLine);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            BypassTester tester = new BypassTester(infoTextBox, progressBar);
            string version = choiceVersionComboBox.Text.Replace("Zapret ", "");
            string path = Path.Combine("zaprets", $"zapret-discord-youtube-{version}");

            List<int> trueChoice = await tester.BypassTest(path, version);

            if (trueChoice.Count > 0)
            {
                foreach (int choice in trueChoice)
                {
                    trueChoiceComboBox.Items.Add($"Пункт №{choice}");
                }
            }
        }

        private void removeZapretButton_Click(object sender, EventArgs e)
        {
            string version = choiceVersionComboBox.Text.Replace("Zapret ", "");
            string path = Path.Combine("zaprets", $"zapret-discord-youtube-{version}");
            ZapretService zapretService = new ZapretService(infoTextBox);
            zapretService.ZapretRemove(path, version);

        }

        private void installZapretButton_Click(object sender, EventArgs e)
        {
            ZapretService zapret = new ZapretService(infoTextBox);
            string version = choiceVersionComboBox.Text.Replace("Zapret ", "");
            int choice = Convert.ToInt32(trueChoiceComboBox.Text.Replace("Пункт №", ""));
            string path = Path.Combine("zaprets", $"zapret-discord-youtube-{version}");
            zapret.BypassStart(path, choice);
        }
    }
}
