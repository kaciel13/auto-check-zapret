using AutoCheckZapret.Models;
using AutoCheckZapret.Services;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AutoCheckZapret
{
    /// <summary>
    /// Главное окно приложения. Содержит всю логику взаимодействия с пользователем,
    /// управление версиями Zapret, их скачивание, удаление, подбор обходов и запуск/остановку.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Логгер для вывода сообщений в консоль приложения
        private Logger _logger;

        // Сервис для получения списка версий, скачивания и удаления
        private ZapretVersionsService _versionsService;

        // Сервис проверки обновлений приложения
        private Updater _updater;

        // Токен отмены для процесса подбора обхода (позволяет прервать операцию)
        private CancellationTokenSource _bypassCheckerCtSource;

        // Список всех полученных версий Zapret
        private List<ZapretVersion> _zapretVersions;

        // Текущая выбранная пользователем версия
        private ZapretVersion _selectedVersion;

        // Флаг, указывающий, запущен ли в данный момент Zapret как служба
        private bool _isZapretRunning;

        // Флаг, указывающий, идёт ли в данный момент процесс подбора обхода
        private bool _isChoosingBypassMethod;

        // Имя файла для сохранения данных приложения (настройки, выбранная версия, подобранные обходы)
        private const string SavedDataFileName = "appdata.json";

        /// <summary>
        /// Конструктор главного окна. Инициализирует компоненты, логгер, сервисы,
        /// устанавливает заголовок окна и запускает асинхронную загрузку версий.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Создаём экземпляр логгера и подписываемся на событие добавления сообщения
            _logger = new Logger();
            _logger.MessageAdded += OnMessageAdded;
            // Привязываем документ логгера к элементу FlowDocumentScrollViewer
            fdsViewerConsole.Document = _logger.LogDocument;

            // Инициализация сервисов
            _versionsService = new ZapretVersionsService();
            _updater = new Updater();
            _bypassCheckerCtSource = new CancellationTokenSource();

            // Определяем версию приложения из сборки и отображаем в заголовке
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version!;
            lbTitle.Content = $"Auto Check Zapret v{version.Major}.{version.Minor}.{version.Build}";

            // Проверяем наличие обновлений (асинхронно, но не блокируем)
            _updater.CheckUpdate();

            // Запускаем асинхронную загрузку списка доступных версий Zapret
            _ = FetchAvailableVersions();
            UpdateUI();
        }

        /// <summary>
        /// Обработчик события добавления нового сообщения в лог.
        /// Выполняет прокрутку окна консоли к последнему сообщению.
        /// </summary>
        private void OnMessageAdded(object sender, EventArgs e)
        {
            // Используем Dispatcher для выполнения прокрутки после обновления UI
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Ищем ScrollViewer внутри FlowDocumentScrollViewer и прокручиваем вниз
                var scrollViewer = FindVisualChild<ScrollViewer>(fdsViewerConsole);
                scrollViewer?.ScrollToEnd();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        /// <summary>
        /// Рекурсивный поиск дочернего элемента указанного типа в визуальном дереве.
        /// </summary>
        /// <typeparam name="T">Тип искомого элемента (например, ScrollViewer)</typeparam>
        /// <param name="parent">Родительский DependencyObject, с которого начинается поиск</param>
        /// <returns>Найденный элемент или null, если элемент не найден</returns>
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                var subResult = FindVisualChild<T>(child);
                if (subResult != null)
                    return subResult;
            }
            return null;
        }

        // ===== Обработчики кнопок управления окном =====

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveData();             // Сохраняем текущие настройки
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => SaveData();

        // ===== Загрузка и сохранение данных приложения =====

        /// <summary>
        /// Асинхронно получает список доступных версий Zapret из репозитория,
        /// обновляет UI и загружает сохранённые пользовательские данные.
        /// </summary>
        private async Task FetchAvailableVersions()
        {
            _logger.AddInfo("Получение версий Zapret...");
            try
            {
                var models = await _versionsService.FetchAvailableVersions();
                _zapretVersions = models.ToList();

                // Проверяем, какие версии уже скачаны на диск
                foreach (var v in _zapretVersions)
                    v.IsDownloaded = _versionsService.IsZapretVersionDownloaded(v);

                // Передаём список в ComboBox
                cbVersions.ItemsSource = _zapretVersions;
                _logger.AddSuccess($"Получено {_zapretVersions.Count} версий.", false);
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка получения версий: {ex.Message}");
                UpdateUI();
                return;
            }

            // После успешной загрузки списка восстанавливаем сохранённое состояние
            LoadSavedData();
            UpdateUI();
        }

        /// <summary>
        /// Загружает данные из файла appdata.json: последнюю выбранную версию,
        /// список скачанных версий и подобранные для них методы обхода.
        /// </summary>
        private void LoadSavedData()
        {
            if (!File.Exists(SavedDataFileName) || _zapretVersions == null)
                return;

            string json = File.ReadAllText(SavedDataFileName);
            SavedApplicationData savedData = null;
            try
            {
                savedData = JsonConvert.DeserializeObject<SavedApplicationData>(json);
            }
            catch (JsonSerializationException)
            {
                MessageBox.Show("Файл с сохранёнными настройками приложения был повреждён. Загружены настройки по умолчанию.",
                                "Файл повреждён", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (savedData == null)
                return;

            // Восстанавливаем информацию о скачанных версиях и их методах обхода
            foreach (var savedVersion in savedData.DownloadedZapretVersions)
            {
                var found = _zapretVersions.FirstOrDefault(v => v.Number == savedVersion.Number);
                if (found != null)
                {
                    found.BypassMethodName = savedVersion.BypassMethodName;
                    found.IsDownloaded = true;
                }
            }

            // Восстанавливаем выбранную версию, если она присутствует в сохранённых данных
            if (savedData.LastSelectedZapretVersion != null)
            {
                var last = _zapretVersions.FirstOrDefault(v => v.Number == savedData.LastSelectedZapretVersion.Number);
                if (last != null)
                {
                    _selectedVersion = last;
                    cbVersions.SelectedItem = last;
                }
            }
        }

        /// <summary>
        /// Сохраняет текущее состояние приложения в файл appdata.json:
        /// выбранную версию и список скачанных версий с их методами обхода.
        /// </summary>
        private void SaveData()
        {
            if (_zapretVersions == null)
                return;

            var data = new SavedApplicationData
            {
                LastSelectedZapretVersion = _selectedVersion,
                DownloadedZapretVersions = _zapretVersions.Where(v => v.IsDownloaded).ToList()
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(SavedDataFileName, json);
        }

        // ===== Обработчики событий элементов управления =====

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
            UpdateUI();
        }

        /// <summary>
        /// Основная кнопка управления: запускает подбор обхода, отменяет подбор,
        /// запускает или останавливает Zapret в зависимости от текущего состояния.
        /// </summary>
        private async void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVersion == null) return;

            string versionPath = AppDomain.CurrentDomain.BaseDirectory + $"versions\\{_selectedVersion.Number}";
            var zapretService = new ZapretService(versionPath);

            // Если метод обхода ещё не подобран – переходим в режим подбора
            if (string.IsNullOrWhiteSpace(_selectedVersion.BypassMethodName))
            {
                // Если подбор уже идёт – отменяем его
                if (_isChoosingBypassMethod)
                {
                    _bypassCheckerCtSource.Cancel();
                    _isChoosingBypassMethod = false;
                    _logger.AddInfo("Отмена процесса подбора обхода...");
                    _logger.AddInfo("");
                    UpdateUI();
                    return;
                }

                // Начинаем подбор
                _isChoosingBypassMethod = true;
                UpdateUI();

                _logger.AddInfo("");
                _logger.AddInfo($"Запущен процесс подбора обхода для Zapret v{_selectedVersion.Number}.");

                bool found = false;
                string methodName = string.Empty;
                try
                {
                    (found, methodName) = await BypassCheckerService.FindBypassMethodAsync(
                        zapretService, _logger, _bypassCheckerCtSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // Подбор был отменён пользователем – создаём новый токен для будущих операций
                    _bypassCheckerCtSource = new CancellationTokenSource();
                    _logger.AddInfo("Процесс подбора обхода отменён.");
                    _isChoosingBypassMethod = false;
                    UpdateUI();
                    return;
                }

                if (found)
                {
                    _logger.AddInfo("");
                    _logger.AddSuccess("Найден подходящий обход!");
                    _selectedVersion.BypassMethodName = methodName;
                }
                else
                {
                    _logger.AddError($"Не удалось подобрать подходящий обход для версии Zapret {_selectedVersion.Number}...");
                }

                _isChoosingBypassMethod = false;
                UpdateUI();
            }
            else
            {
                // Метод обхода уже подобран – запускаем или останавливаем службу
                if (!_isZapretRunning)
                {
                    await zapretService.InstallServiceAsync(_selectedVersion.BypassMethodName);
                    _isZapretRunning = true;
                    _logger.AddInfo($"Zapret v{_selectedVersion.Number} запущен. Приятного пользования!");
                }
                else
                {
                    await zapretService.RemoveServiceAsync();
                    _isZapretRunning = false;
                    _logger.AddInfo($"Zapret v{_selectedVersion.Number} остановлен.");
                }
                UpdateUI();
            }
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
            btnStartStop.IsEnabled = _selectedVersion != null && _selectedVersion.IsDownloaded;

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