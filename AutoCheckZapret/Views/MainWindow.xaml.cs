using AutoCheckZapret.Helpers;
using AutoCheckZapret.Models;
using AutoCheckZapret.Services;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace AutoCheckZapret
{
    /// <summary>
    /// Главное окно приложения. Содержит всю логику взаимодействия с пользователем,
    /// управление версиями Zapret, их скачивание, удаление, подбор обходов и запуск/остановку.
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logger _logger;
        private ZapretVersionsService _versionsService;
        private CancellationTokenSource _bypassCheckerCtSource;

        private List<ZapretVersion> _zapretVersions;
        private ZapretVersion? _selectedVersion;

        private bool _isZapretRunning;
        private bool _isChoosingBypassMethod;
        private bool _isCancellingChoosingBypassMethod;

        /// <summary>
        /// Конструктор главного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Создаём экземпляр логгера и передаём ему FlowDocumentScrollViewer
            _logger = new Logger(fdsViewerConsole);
            // Привязываем документ логгера к элементу FlowDocumentScrollViewer
            fdsViewerConsole.Document = _logger.LogDocument;

            // Инициализируем сервисы и даём полям значения по умолчанию
            _versionsService = new ZapretVersionsService();

            _bypassCheckerCtSource = new CancellationTokenSource();

            _zapretVersions = new List<ZapretVersion>();
            _selectedVersion = null;

            // Определяем версию приложения из сборки и отображаем в хэдере приложения
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version!;
            lbTitle.Content = $"Auto Check Zapret v{version.Major}.{version.Minor}.{version.Build}";

            // Проверяем наличие обновлений
            ApplicationUpdater.CheckForUpdatesAsync();

            // Запускаем асинхронную загрузку списка доступных версий Zapret
            _ = FetchAvailableZapretVersionsVersions();
            UpdateUI();
        }

        #region Обработчики кнопок управления окном

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => SavedApplicationDataManager.SaveData(_zapretVersions, _selectedVersion);

        #endregion

        #region Обработчики событий элементов управления

        /// <summary>
        /// Вызывается при изменении выбранного элемента в ComboBox с версиями.
        /// Обновляет выбранную версию и перерисовывает состояние кнопок.
        /// </summary>
        private void VersionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedVersion = cbVersions.SelectedItem as ZapretVersion;
            UpdateUI();
        }

        /// <summary>
        /// Обработчик кнопки "Скачать": загружает выбранную версию Zapret,
        /// распаковывает и модифицирует служебные файлы для отключения проверки обновлений.
        /// </summary>
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVersion == null) return;

            // Блокируем UI на время операции
            btnDownload.IsEnabled = false;
            cbVersions.IsEnabled = false;

            _logger.AddInfo($"Cкачивание версии Zapret {_selectedVersion.Number}...");
            try
            {
                await _versionsService.DownloadZapretVersion(_selectedVersion);
                _selectedVersion.IsDownloaded = true;
                _logger.AddSuccess("Скачивание завершено!", false);

                // Обновляем ComboBox для отображения изменений
                RefreshComboBox();
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка скачивания Zapret: {ex.Message}", false);
            }
            finally
            {
                cbVersions.IsEnabled = true;
                UpdateUI();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Удалить": удаляет папку с выбранной версией Zapret.
        /// Если файлы заняты службой, предварительно останавливает и удаляет службу.
        /// </summary>
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVersion == null) return;

            // Подтверждение удаления
            if (MessageBox.Show($"Вы уверены, что хотите удалить Zapret версии {_selectedVersion.Number}?",
                                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            _logger.AddInfo($"Удаление версии Zapret {_selectedVersion.Number}...");

            // Пытаемся удалить папку. Если не удаётся (файлы заблокированы), останавливаем службу и повторяем
            bool deleted = _versionsService.DeleteZapretVersion(_selectedVersion);
            if (!deleted)
            {
                string versionPath = AppDomain.CurrentDomain.BaseDirectory + $"versions\\{_selectedVersion.Number}";
                var zapretService = new ZapretService(versionPath);
                await zapretService.RemoveServiceAsync();
                _versionsService.DeleteZapretVersion(_selectedVersion);
            }

            // Обновляем состояние модели
            _selectedVersion.IsDownloaded = false;
            _selectedVersion.BypassMethodName = string.Empty;
            _logger.AddSuccess("Версия удалена.", false);

            // Обновляем ComboBox для отображения изменений
            RefreshComboBox();
            UpdateUI();
        }

        /// <summary>
        /// Основная кнопка управления: запускает подбор обхода, отменяет подбор,
        /// запускает или останавливает Zapret в зависимости от текущего состояния.
        /// </summary>
        private async void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVersion == null) return;

            if (_isChoosingBypassMethod)
            {
                _logger.AddInfo("Отмена процесса подбора...");
                _isCancellingChoosingBypassMethod = true;
                UpdateUI();
                _bypassCheckerCtSource?.Cancel();
                return;
            }

            string versionPath = AppDomain.CurrentDomain.BaseDirectory + $"versions\\{_selectedVersion.Number}";
            var zapretService = new ZapretService(versionPath);

            // 1. Если служба уже запущена – останавливаем
            if (_isZapretRunning)
            {
                await zapretService.RemoveServiceAsync();
                _logger.AddInfo($"Zapret v{_selectedVersion.Number} остановлен.");
                _isZapretRunning = false;
                UpdateUI();
                return;
            }

            // 2. Служба не запущена – пытаемся запустить
            // Проверяем, выбран ли метод обхода
            if (!string.IsNullOrWhiteSpace(_selectedVersion.BypassMethodName))
            {
                // Проверяем работоспособность выбранного метода
                UpdateUI();

                _logger.AddInfo($"Проверка обхода \"{_selectedVersion.BypassMethodName}\"...");

                (bool success, string _) = await ZapretBypassTester.TestBypassMethodAsync(
                    zapretService,
                    _selectedVersion.BypassMethodName,
                    _logger,
                    _bypassCheckerCtSource.Token);

                if (success)
                {
                    // Служба уже установлена и запущена (TestSingleBypassAsync оставляет её активной)
                    _isZapretRunning = true;
                    _logger.AddInfo($"Zapret v{_selectedVersion.Number} запущен. Приятного пользования!");
                    UpdateUI();
                    return;
                }

                // Обход не работает – сбрасываем имя и переходим к подбору
                _logger.AddError($"Обход \"{_selectedVersion.BypassMethodName}\" не работает. Будет выполнен автоматический подбор.");
                _selectedVersion.BypassMethodName = null;
                RefreshComboBox();
            }

            // 3. Общий блок подбора (выполняется, если метод не выбран или был сброшен)
            _isChoosingBypassMethod = true;
            UpdateUI();

            _logger.AddInfo("");
            _logger.AddInfo($"Запущен процесс подбора обхода для Zapret v{_selectedVersion.Number}.");

            bool found = false;
            string methodName = string.Empty;
            try
            {
                (found, methodName) = await ZapretBypassTester.FindBypassMethodAsync(
                    zapretService, _logger, _bypassCheckerCtSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Отмена подбора пользователем
                _bypassCheckerCtSource = new CancellationTokenSource(); // сброс токена
                _logger.AddInfo("Процесс подбора обхода отменён.");
                _isChoosingBypassMethod = false;
                _isCancellingChoosingBypassMethod = false;
                UpdateUI();
                return;
            }

            if (found)
            {
                _logger.AddInfo("");
                _logger.AddSuccess("Найден подходящий обход!");
                _selectedVersion.BypassMethodName = methodName;
                RefreshComboBox();
                // Служба уже запущена (FindBypassMethodAsync оставляет её активной)
                _isZapretRunning = true;
                _logger.AddInfo($"Zapret v{_selectedVersion.Number} запущен. Приятного пользования!");
            }
            else
            {
                _logger.AddError($"Не удалось подобрать подходящий обход для версии Zapret {_selectedVersion.Number}...");
            }

            _isChoosingBypassMethod = false;
            UpdateUI();
        }

        #endregion

        /// <summary>
        /// Асинхронно получает список доступных версий Zapret из репозитория,
        /// обновляет UI и загружает сохранённые пользовательские данные.
        /// </summary>
        private async Task FetchAvailableZapretVersionsVersions()
        {
            _logger.AddInfo("Получение версий Zapret...");
            try
            {
                _zapretVersions = await _versionsService.FetchAvailableVersions();

                // Проверяем, какие версии уже скачаны
                foreach (var version in _zapretVersions)
                    version.IsDownloaded = _versionsService.IsZapretVersionDownloaded(version);

                // Передаём список в ComboBox
                cbVersions.ItemsSource = _zapretVersions;
                _logger.AddSuccess($"Получено {_zapretVersions.Count} версий.", false);

                LoadSavedData();
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка получения версий: {ex.Message}");
                UpdateUI();
                return;
            }

            UpdateUI();
        }

        /// <summary>
        /// Загружает сохранённые данные приложения из файла
        /// </summary>
        private void LoadSavedData()
        {
            // Загружаем сохранённые данные
            var (selectedVersion, downloadedVersions) = SavedApplicationDataManager.LoadSavedData(_zapretVersions);

            // Применяем восстановленные данные
            if (selectedVersion != null)
            {
                _selectedVersion = selectedVersion;
                cbVersions.SelectedItem = selectedVersion;
            }
            else
            {
                cbVersions.SelectedIndex = 0; // По умолчанию выбираем последнюю (самую новую) версию
            }
        }

        /// <summary>
        /// Обновляет отображение ComboBox без изменения выбранного элемента
        /// </summary>
        private void RefreshComboBox()
        {
            // Сохраняем текущий выбранный элемент
            ZapretVersion? selected = _selectedVersion;

            // Обновляем ItemsSource
            cbVersions.ItemsSource = null;
            cbVersions.ItemsSource = _zapretVersions;

            // Восстанавливаем выбранный элемент
            if (selected != null)
                cbVersions.SelectedItem = selected;
        }

        /// <summary>
        /// Обновляет состояние всех элементов управления в зависимости от текущего состояния:
        /// доступность ComboBox, кнопок Download, Delete, StartStop и их текстовое содержимое.
        /// </summary>
        private void UpdateUI()
        {
            // ComboBox доступен, если есть версии или не идёт подбор и Zapret не запущен
            cbVersions.IsEnabled = _zapretVersions != null && !_isChoosingBypassMethod && !_isZapretRunning;

            // Кнопка скачивания доступна, если выбрана версия, она не скачана, и нет активных процессов
            btnDownload.IsEnabled = _selectedVersion != null && !_selectedVersion.IsDownloaded && !_isChoosingBypassMethod && !_isZapretRunning;

            // Кнопка удаления доступна, если версия скачана и нет активных процессов
            btnDelete.IsEnabled = _selectedVersion != null && _selectedVersion.IsDownloaded && !_isChoosingBypassMethod && !_isZapretRunning;

            // Кнопка StartStop активна, если выбрана версия и она скачана (подбор или запуск/остановка)
            btnStartStop.IsEnabled = _selectedVersion != null && _selectedVersion.IsDownloaded && !_isCancellingChoosingBypassMethod;

            // Определяем текст на кнопке в зависимости от состояния
            if (_selectedVersion == null)
            {
                btnStartStop.Content = "Не выбрана версия Zapret";
                return;
            }

            if (_isChoosingBypassMethod)
            {
                btnStartStop.Content = "Остановить подбор обхода";
                return;
            }

            if (!_selectedVersion.IsDownloaded)
            {
                btnStartStop.Content = $"Скачайте Zapret v{_selectedVersion.Number}, чтобы начать работу";
                btnStartStop.IsEnabled = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedVersion.BypassMethodName))
                btnStartStop.Content = $"Подобрать обход для Zapret v{_selectedVersion.Number}";
            else
                btnStartStop.Content = _isZapretRunning
                    ? $"Остановить Zapret v{_selectedVersion.Number}"
                    : $"Запустить Zapret v{_selectedVersion.Number}";
        }
    }
}