using AutoCheckZapret.Models;
using AutoCheckZapret.Services;
using System.Reflection;
using System.Windows;

namespace AutoCheckZapret
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ZapretDownloaderService _downloaderService;

        /// <summary>
        /// Конструктор главного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version!;

            // В конце используем Build, потому что в .csproj используем вид Major.Minor.Feature, а не Major.Minor.Feature.Build
            // А VS определяет последнюю цифру как Build
            Title = $"Auto Check Zapret v{version.Major}.{version.Minor}.{version.Build}";

            _downloaderService = new ZapretDownloaderService();

            FillZapretVersionsComboBox();
        }

        private async void FillZapretVersionsComboBox()
        {
            List<ZapretVersion> zapretVersions = await _downloaderService.FetchAvailableVersions();

            ZapretVersionsComboBox.IsEnabled = false;
            ZapretVersionsComboBox.Items.Clear();

            foreach (ZapretVersion version in zapretVersions)
            {
                ZapretVersionsComboBox.Items.Add(version.Number);
            }

            // TODO: В дальнейшем нужно будет устанавливать индекс в зависимости от того, какую версию пользователь сам себе ставил в последний раз.
            // Эти данные будут сохраняться в файлике рядом с прогой ACZ
            ZapretVersionsComboBox.SelectedIndex = 0;
            ZapretVersionsComboBox.IsEnabled = true;
        }
    }
}