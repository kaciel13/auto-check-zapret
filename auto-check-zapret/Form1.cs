using System;
using System.Drawing.Text;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace auto_check_zapret
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> versions;
        ZapretParser parser = new ZapretParser();
        FontManager fontManager = new FontManager();

        public Form1()
        {
            
            fontManager.InstallCustomFont();
            
            InitializeComponent();
            progressBar.Value = 0;
            TesterModuleEnabe(false);
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
                    TesterModuleEnabe(false);
                    startTestButton.Enabled = true;
                    startTestButton.BackColor = Color.FromArgb(110, 110, 100);
                    startTestButton.ForeColor = Color.White;
                    removeZapretButton.Enabled = true;
                    removeZapretButton.BackColor = Color.FromArgb(110, 110, 100);
                    removeZapretButton.ForeColor = Color.White;

                    downloadButton.BackColor = Color.FromArgb(90, 90, 85); // Цвет фона
                    downloadButton.ForeColor = Color.FromArgb(100,100,100); // Цвет текста


                }
                else
                {

                    downloadButton.Enabled = true;
                    downloadButton.BackColor = Color.FromArgb(110, 110, 100); // Цвет фона
                    downloadButton.ForeColor = Color.White; // Цвет текста
                    downloadButton.Text = "Установить";
                    TesterModuleEnabe(false);
                }
                
            }
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            int index = choiceVersionComboBox.SelectedIndex;
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

            
            choiceVersionComboBox.SelectedIndex = 0;
            choiceVersionComboBox.SelectedIndex = index;
            infoTextBox.AppendText($"Скачивание завершено" + Environment.NewLine);
        }

        private async void startTest_Click(object sender, EventArgs e)
        {
            choiceVersionComboBox.Enabled = false;
            BypassTester tester = new BypassTester(infoTextBox, progressBar);
            string version = choiceVersionComboBox.Text.Replace("Zapret ", "");
            string path = Path.Combine("zaprets", $"zapret-discord-youtube-{version}");
            TesterModuleEnabe(false);
            List<int> trueChoice = await tester.BypassTest(path, version);
            trueChoiceComboBox.Items.Clear();
            if (trueChoice.Count > 0)
            {
                infoTextBox.AppendText($"Найдено {trueChoice.Count} рабочих обходов" + Environment.NewLine);
                foreach (int choice in trueChoice)
                {
                    trueChoiceComboBox.Items.Add($"Пункт №{choice}");
                }
                TesterModuleEnabe(true);
            }
            else
            {
                infoTextBox.AppendText("Рабочие обходы не найдены" + Environment.NewLine);
            }
            choiceVersionComboBox.Enabled = true;
        }

        private void removeZapretButton_Click(object sender, EventArgs e)
        {
            string version = choiceVersionComboBox.Text.Replace("Zapret ", "");
            string path = Path.Combine("zaprets", $"zapret-discord-youtube-{version}");
            ZapretService zapretService = new ZapretService(infoTextBox);
            zapretService.ZapretRemove(path, version);

        }

        private void zapretInstallButton_Click(object sender, EventArgs e)
        {
            if (trueChoiceComboBox.SelectedIndex != -1)
            {
                ZapretService zapret = new ZapretService(infoTextBox);
                string version = choiceVersionComboBox.Text.Replace("Zapret ", "");
                int choice = Convert.ToInt32(trueChoiceComboBox.Text.Replace("Пункт №", ""));
                string path = Path.Combine("zaprets", $"zapret-discord-youtube-{version}");
                zapret.BypassStart(path, choice);
            }
            else
            {
                MessageBox.Show("Cначала нужно выбрать желаймый пункт");
            }
        }

        private void TesterModuleEnabe(bool enabel)
        {
            removeZapretButton.Enabled = enabel;
            zapretInstallButton.Enabled = enabel;
            startTestButton.Enabled = enabel;
            trueChoiceComboBox.Enabled = enabel;

            if (enabel)
            {
                // Изменение цвета для включенного состояния
                removeZapretButton.BackColor = Color.FromArgb(110, 110, 100);
                zapretInstallButton.BackColor = Color.FromArgb(110, 110, 100);
                startTestButton.BackColor = Color.FromArgb(110, 110, 100);
                trueChoiceComboBox.BackColor = Color.FromArgb(110, 110, 100);

                removeZapretButton.ForeColor = Color.White;
                zapretInstallButton.ForeColor = Color.White;
                startTestButton.ForeColor = Color.White;
                trueChoiceComboBox.ForeColor = Color.White;
            }
            else
            {
                // Изменение цвета для выключенного состояния
                removeZapretButton.BackColor = Color.FromArgb(90, 90, 85);
                zapretInstallButton.BackColor = Color.FromArgb(90, 90, 85);
                startTestButton.BackColor = Color.FromArgb(90, 90, 85);
                trueChoiceComboBox.BackColor = Color.FromArgb(90, 90, 85);

                removeZapretButton.ForeColor = Color.FromArgb(100, 100, 100);
                zapretInstallButton.ForeColor = Color.FromArgb(100, 100, 100);
                startTestButton.ForeColor = Color.FromArgb(100, 100, 100);
                trueChoiceComboBox.ForeColor = Color.FromArgb(100, 100, 100);
            }

        }
    }
}
