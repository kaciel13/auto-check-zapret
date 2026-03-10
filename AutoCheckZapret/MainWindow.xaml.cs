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
        private List<ZapretVersion> _zapretVersions;

        private readonly ZapretDownloaderService _zapretDownloaderService;
        private readonly WindowStateService _windowStateService;
   
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
            lbTitle.Content = $"Auto Check Zapret v{version.Major}.{version.Minor}.{version.Build}";

            _zapretDownloaderService = new ZapretDownloaderService();

            // Инициализируем сервис с передачей ссылок на элементы
            _windowStateService = new WindowStateService(
                this,
                MainGrid,
                DragHeader,
                btnToggleFullscreen
            );

            FillZapretVersionsComboBox();
        }

        private void btnMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            _windowStateService.MinimizeWindow();
        }

        private void btnToggleFullscreen_Click(object sender, RoutedEventArgs e)
        {
            _windowStateService.ToggleFullscreen();
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            _windowStateService.ShutDownApplication();
        }

        private async void FillZapretVersionsComboBox()
        {
            btnDownloadZapretVersion.IsEnabled = false;
            btnDeleteZapretVersion.IsEnabled = false;

            _zapretVersions = await _zapretDownloaderService.FetchAvailableVersions();

            cbxZapretVersions.IsEnabled = false;
            cbxZapretVersions.Items.Clear();

            foreach (ZapretVersion version in _zapretVersions)
            {
                cbxZapretVersions.Items.Add(version.Number);
            }

            // TODO: В дальнейшем нужно будет устанавливать индекс в зависимости от того, какую версию пользователь сам себе ставил в последний раз.
            // Эти данные будут сохраняться в файлике рядом с прогой ACZ
            cbxZapretVersions.SelectedIndex = 0;
            cbxZapretVersions.IsEnabled = true;

            btnDownloadZapretVersion.IsEnabled = true;
            btnDeleteZapretVersion.IsEnabled = true;
        }

        private async void btnDownloadZapretVersion_Click(object sender, RoutedEventArgs e)
        {
            cbxZapretVersions.IsEnabled = false;
            btnDownloadZapretVersion.IsEnabled = false;
            btnDeleteZapretVersion.IsEnabled = false;

            ZapretVersion versionToDownload = _zapretVersions[cbxZapretVersions.SelectedIndex];

            bool isDownloaded = await _zapretDownloaderService.DownloadZapretVersion(versionToDownload);
            if (!isDownloaded)
            {
                // TODO: В консоль нужно что-то выводить, собственно, по поводу возникшей при скачивании ошибки
                MessageBox.Show($"Ошибка скачивания Zapret версии {versionToDownload.Number}. Смотрите детали ошибки в консоли программы.", "Ошибка скачивания версии Zapret", MessageBoxButton.OK, MessageBoxImage.Error);

                cbxZapretVersions.IsEnabled = true;
                btnDownloadZapretVersion.IsEnabled = true;
                btnDeleteZapretVersion.IsEnabled = true;

                return;
            }

            cbxZapretVersions.IsEnabled = true;
            btnDownloadZapretVersion.IsEnabled = true;
            btnDeleteZapretVersion.IsEnabled = true;
        }
    }
}