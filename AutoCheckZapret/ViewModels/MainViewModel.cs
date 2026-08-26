using AutoCheckZapret.Helpers;
using AutoCheckZapret.Models;
using AutoCheckZapret.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;

namespace AutoCheckZapret.ViewModels
{
    /// <summary>
    /// Обёртка для модели ZapretVersion с поддержкой INotifyPropertyChanged.
    /// Используется для отслеживания состояния IsDownloaded и BypassMethodName без изменения самой модели.
    /// </summary>
    public class ZapretVersionViewModel : INotifyPropertyChanged
    {
        private readonly ZapretVersion _model;
        /// <summary>
        /// Оригинальная модель версии Zapret.
        /// </summary>
        public ZapretVersion Model => _model;

        /// <summary>
        /// Номер версии 
        /// </summary>
        public string Number => _model.Number;

        /// <summary>
        /// URL для скачивания 
        /// </summary>
        public string DownloadUrl => _model.DownloadUrl;

        private bool _isDownloaded;
        /// <summary>
        /// Скачана ли данная версия на компьютер
        /// </summary>
        public bool IsDownloaded
        {
            get => _isDownloaded;
            set
            {
                if (_isDownloaded != value)
                {
                    _isDownloaded = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _bypassMethodName;
        /// <summary>
        /// Название подобранного метода обхода для этой версии
        /// </summary>
        public string BypassMethodName
        {
            get => _bypassMethodName;
            set
            {
                if (_bypassMethodName != value)
                {
                    _bypassMethodName = value;
                    OnPropertyChanged();
                    _model.BypassMethodName = value;
                }
            }
        }

        public ZapretVersionViewModel(ZapretVersion model, bool isDownloaded)
        {
            _model = model;
            _isDownloaded = isDownloaded;
            _bypassMethodName = model.BypassMethodName; 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Главная ВьюМодель приложения
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public Logger Logger => _logger;
        public FlowDocument LogDocument => _logger?.LogDocument;

        private bool _isFullscreen = false;

        private string _appNameWithVersion;

        private Logger _logger;

        private CancellationTokenSource _bypassCheckerCtSource;

        private const string SavedDataFileName = "appdata.json";

        /// <summary>
        /// Имя приложения Auto Check Zapret с версией, которое отображается в заголовке приложения
        /// </summary>
        public string AppNameWithVersion
        {
            get { return _appNameWithVersion; }
            set
            {
                _appNameWithVersion = value;
                OnPropertyChanged("AppNameWithVersion");
            }
        }

        private RelayCommand _minimizeWindowCommand;
        /// <summary>
        /// Команда для кнопки минимизации окна
        /// </summary>
        public RelayCommand MinimizeWindowCommand
        {
            get
            {
                return _minimizeWindowCommand ?? (_minimizeWindowCommand = new RelayCommand(MinimizeWindow));
            }
        }

        private RelayCommand _maximizeWindowCommand;
        /// <summary>
        /// Команда для кнопки разворачивания окна на весь экран
        /// </summary>
        public RelayCommand MaximizeWindowCommand
        {
            get
            {
                return _maximizeWindowCommand ?? (_maximizeWindowCommand = new RelayCommand(MaximizeWindow));
            }
        }

        private RelayCommand _shutdownApplicationCommand;
        /// <summary>
        /// Команда для кнопки закрывания приложения
        /// </summary>
        public RelayCommand ShutdownApplicationCommand
        {
            get
            {
                return _shutdownApplicationCommand ?? (_shutdownApplicationCommand = new RelayCommand(ShutdownApplication));
            }
        }

        private ObservableCollection<ZapretVersionViewModel> _zapretVersions;
        /// <summary>
        /// Полученные из официального репозитория версии Zapret
        /// </summary>
        public ObservableCollection<ZapretVersionViewModel> ZapretVersions
        {
            get { return _zapretVersions; }
            set
            {
                _zapretVersions = value;
                OnPropertyChanged("ZapretVersions");

                if (value != null) { CanSelectZapretVersion = true; }
            }
        }

        private bool _canSelectZapretVersion;
        /// <summary>
        /// Были ли получены версии Zapret с официального репозитория
        /// </summary>
        public bool CanSelectZapretVersion
        {
            get { return _canSelectZapretVersion; }
            set
            {
                _canSelectZapretVersion = value;
                OnPropertyChanged("CanSelectZapretVersion");
            }
        }

        private ZapretVersionViewModel _selectedZapretVersion;
        /// <summary>
        /// Выбранная в выпадающем списке версия Zapret
        /// </summary>
        public ZapretVersionViewModel SelectedZapretVersion
        {
            get { return _selectedZapretVersion; }
            set
            {
                if (_selectedZapretVersion != value)
                {
                    // Отписываемся от старой обёртки
                    if (_selectedZapretVersion != null)
                        _selectedZapretVersion.PropertyChanged -= OnSelectedVersionPropertyChanged;

                    _selectedZapretVersion = value;

                    // Подписываемся на изменения новой обёртки
                    if (_selectedZapretVersion != null)
                        _selectedZapretVersion.PropertyChanged += OnSelectedVersionPropertyChanged;

                    OnPropertyChanged("SelectedZapretVersion");
                    UpdateActionButtonsState();
                }
            }
        }

        // Обработчик изменения свойств выбранной обёртки
        private void OnSelectedVersionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ZapretVersionViewModel.IsDownloaded))
                UpdateActionButtonsState();
            // Если нужно обновлять что-то ещё при изменении BypassMethodName, можно добавить условие
        }

        // Метод для обновления кнопок в одном месте
        private void UpdateActionButtonsState()
        {
            if (SelectedZapretVersion == null)
            {
                CanDownloadZapretVersion = false;
                CanDeleteZapretVersion = false;
                ChooseBypassMethodButtonContent = "Не выбрана версия Zapret";
                return;
            }

            if (!SelectedZapretVersion.IsDownloaded)
            {
                CanDownloadZapretVersion = true;
                CanDeleteZapretVersion = false;
                CanStartZapretVersion = false;
                ChooseBypassMethodButtonContent = $"Скачайте Zapret v{SelectedZapretVersion.Number}, чтобы начать работу";
            }
            else
            {
                CanDownloadZapretVersion = false;
                CanDeleteZapretVersion = true;
                CanStartZapretVersion = true;
                ChooseBypassMethodButtonContent = $"Подобрать обход для Zapret v{SelectedZapretVersion.Number}";
            }

            if (SelectedZapretVersion.IsDownloaded && string.IsNullOrWhiteSpace(SelectedZapretVersion.BypassMethodName))
            {
                CanDownloadZapretVersion = false;
                CanDeleteZapretVersion = true;
                ChooseBypassMethodButtonContent = $"Подобрать обход для Zapret v{SelectedZapretVersion.Number}";
            }
            else if (SelectedZapretVersion.IsDownloaded && !string.IsNullOrWhiteSpace(SelectedZapretVersion.BypassMethodName))
            {
                CanDownloadZapretVersion = false;
                CanDeleteZapretVersion = true;
                ChooseBypassMethodButtonContent = $"Запустить Zapret v{SelectedZapretVersion.Number}";
            }
        }

        private bool _canDownloadZapretVersion;
        /// <summary>
        /// Может ли пользователь нажать кнопку для скачивания версии Zapret
        /// </summary>
        public bool CanDownloadZapretVersion
        {
            get { return _canDownloadZapretVersion; }
            set
            {
                _canDownloadZapretVersion = value;
                OnPropertyChanged("CanDownloadZapretVersion");
            }
        }

        private bool _isZapretRunning;
        public bool IsZapretRunning
        {
            get { return _isZapretRunning; }
            set
            {
                _isZapretRunning = value;
                OnPropertyChanged("IsZapretRunning");

                if (value)
                {
                    CanSelectZapretVersion = false;
                    ChooseBypassMethodButtonContent = $"Остановить Zapret v{SelectedZapretVersion.Number}";
                }
                else
                {
                    CanSelectZapretVersion = true;
                    ChooseBypassMethodButtonContent = $"Запустить Zapret v{SelectedZapretVersion.Number}";
                }
            }
        }

        private bool _isChoosingBypassMethod;
        /// <summary>
        /// Происходит ли прямо сейчас процесс подбора подходящего обхода для версии Zapret
        /// </summary>
        public bool IsChoosingBypassMethod
        {
            get { return _isChoosingBypassMethod; }
            set
            {
                _isChoosingBypassMethod = value;
                OnPropertyChanged("IsChoosingBypassMethod");

                if (value)
                {
                    CanSelectZapretVersion = false;
                    ChooseBypassMethodButtonContent = $"Остановить подбор обхода";
                }
                else
                {
                    CanSelectZapretVersion = true;
                    // Восстанавливаем текст кнопки
                    UpdateActionButtonsState();
                }
            }
        }

        private bool _canDeleteZapretVersion;
        /// <summary>
        /// Может ли пользователь удалить версию Zapret или работать с ней
        /// </summary>
        public bool CanDeleteZapretVersion
        {
            get { return _canDeleteZapretVersion; }
            set
            {
                _canDeleteZapretVersion = value;
                OnPropertyChanged("CanDeleteZapretVersion");
                // Текст кнопки теперь обновляется через UpdateActionButtonsState
            }
        }

        private bool _canStartZapretVersion;
        /// <summary>
        /// Может ли пользователь запускать выбранную версию Zapret
        /// </summary>
        public bool CanStartZapretVersion
        {
            get { return _canStartZapretVersion; }
            set
            {
                _canStartZapretVersion = value;
                OnPropertyChanged("CanStartZapretVersion");
            }
        }

        private RelayCommand _downloadZapretVersionCommand;
        /// <summary>
        /// Команда для скачивания и распаковки выбранной версии Zapret
        /// </summary>
        public RelayCommand DownloadZapretVersionCommand
        {
            get
            {
                return _downloadZapretVersionCommand ?? (_downloadZapretVersionCommand = new RelayCommand(DownloadZapretVersion));
            }
        }

        private RelayCommand _deleteZapretVersionCommand;
        /// <summary>
        /// Команда удаления версии Zapret
        /// </summary>
        public RelayCommand DeleteZapretVersionCommand
        {
            get
            {
                return _deleteZapretVersionCommand ?? (_deleteZapretVersionCommand = new RelayCommand(DeleteZapretVersion));
            }
        }

        private string _chooseBypassMethodButtonContent;
        /// <summary>
        /// Текст на кнопке внизу приложения для подбора обхода для выбранной версии Zapret
        /// </summary>
        public string ChooseBypassMethodButtonContent
        {
            get { return _chooseBypassMethodButtonContent; }
            set
            {
                _chooseBypassMethodButtonContent = value;
                OnPropertyChanged("ChooseBypassMethodButtonContent");
            }
        }

        private RelayCommand _startStopVersionCommand;
        /// <summary>
        /// Команда подбора обхода для выбранной версии Zapret
        /// </summary>
        public RelayCommand StartStopVersionCommand
        {
            get
            {
                return _startStopVersionCommand ?? (_startStopVersionCommand = new RelayCommand(StartStopVersion));
            }
        }

        private ZapretVersionsService _zapretVersionsService;
        private Updater _updater;

        /// <summary>
        /// Конструктор главной ВьюМодели
        /// </summary>
        public MainViewModel()
        {
            _logger = new Logger();

            _bypassCheckerCtSource = new CancellationTokenSource();

            _updater = new Updater();
            _updater.CheckUpdate();

            _zapretVersionsService = new ZapretVersionsService();
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version!;
            // В конце используем Build, потому что в .csproj используем вид Major.Minor.Feature, а не Major.Minor.Feature.Build
            // А VS определяет последнюю цифру как Build
            AppNameWithVersion = $"Auto Check Zapret v{version.Major}.{version.Minor}.{version.Build}";

            _ = FetchAvailableZapretVersions();

            ChooseBypassMethodButtonContent = "Не выбрана версия Zapret";
        }

        /// <summary>
        /// Обработчик события изменения свойства ВьюМодели
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Метод для вызова обработчика события изменения свойства ВьюМодели
        /// </summary>
        /// <param name="prop">Имя изменившегося свойства</param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void MinimizeWindow(object param)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object param)
        {
            if (param is Window window)
            {
                window.WindowState = window.WindowState == WindowState.Normal
                    ? WindowState.Maximized
                    : WindowState.Normal;
            }
        }

        private void ShutdownApplication(object param)
        {
            // Сохраняем оригинальные модели скачанных версий
            List<ZapretVersion> downloadedZapretVersions = ZapretVersions
                .Where(vm => vm.IsDownloaded)
                .Select(vm => vm.Model)
                .ToList();

            SavedApplicationData dataToSave = new SavedApplicationData()
            {
                LastSelectedZapretVersion = SelectedZapretVersion,
                DownloadedZapretVersions = downloadedZapretVersions
            };

            string configJson = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);

            File.WriteAllText(SavedDataFileName, configJson);

            Application.Current.Shutdown();
        }

        private async Task FetchAvailableZapretVersions()
        {
            _logger.AddInfo("Получение версий Zapret...");
            try
            {
                var models = await _zapretVersionsService.FetchAvailableVersions();
                var viewModels = models.Select(v => new ZapretVersionViewModel(
                    v,
                    _zapretVersionsService.IsZapretVersionDownloaded(v)
                )).ToList();

                ZapretVersions = new ObservableCollection<ZapretVersionViewModel>(viewModels);
                _logger.AddSuccess($"Получено {ZapretVersions.Count} версий.", false);
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка получения версий: {ex.Message}");
            }

            // Читаем сохранённые данные пользователя, если они есть
            if (File.Exists(SavedDataFileName))
            {
                string savedJson = File.ReadAllText(SavedDataFileName);

                SavedApplicationData? savedData = null;
                try
                {
                    savedData = JsonConvert.DeserializeObject<SavedApplicationData>(savedJson);
                }
                catch (JsonSerializationException)
                {
                    MessageBox.Show("Файл с сохранёнными настройками приложения был повреждён. Были загружены настройки по умолчанию", "Файл с сохранёнными настройками повреждён", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (savedData != null)
                {
                    ZapretVersionViewModel? lastSelectedVersion = ZapretVersions.FirstOrDefault(vm => vm.Number == savedData.LastSelectedZapretVersion?.Number);
                    if (lastSelectedVersion != null)
                    {
                        SelectedZapretVersion = lastSelectedVersion;
                    }

                    foreach (ZapretVersionViewModel fetchedVersion in ZapretVersions)
                    {
                        foreach (ZapretVersion downloadedVersion in savedData.DownloadedZapretVersions)
                        {
                            if (downloadedVersion.Number == fetchedVersion.Number)
                            {
                                fetchedVersion.BypassMethodName = downloadedVersion.BypassMethodName;
                            }
                        }
                    }

                    return;
                }
            }

            if (ZapretVersions != null && ZapretVersions.Any())
                SelectedZapretVersion = ZapretVersions[0];
        }

        private async void DownloadZapretVersion(object param)
        {
            if (SelectedZapretVersion == null) return;
            CanSelectZapretVersion = false;
            CanDownloadZapretVersion = false;

            ZapretVersionsService downloaderService = new ZapretVersionsService();
            _logger.AddInfo($"Cкачивание версии Zapret {SelectedZapretVersion.Number}...");

            try
            {
                await downloaderService.DownloadZapretVersion(SelectedZapretVersion.Model);

                SelectedZapretVersion.IsDownloaded = true; // теперь вызывает событие, кнопки обновятся автоматически
                _logger.AddSuccess("Скачивание завершено!", false);

                CanSelectZapretVersion = true;
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка скачивания Zapret: {ex.Message}", false);
                CanDownloadZapretVersion = true; // возвращаем возможность повторной попытки

                CanSelectZapretVersion = true;
            }
        }

        private async void DeleteZapretVersion(object param)
        {
            if (SelectedZapretVersion == null) return;

            MessageBoxResult questionResult = MessageBox.Show($"Вы уверены, что хотите удалить Zapret версии {SelectedZapretVersion.Number}?", "Подтверждение удаления версии Zapret", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (questionResult != MessageBoxResult.Yes) { return; }

            _logger.AddInfo($"Удаление версии Zapret {SelectedZapretVersion.Number}...");

            // Если версия не может быть удалена, то только из-за того, что какой-то из файлов используется службой Zapret
            // Убиваем службу и снова удаляем выбранную версию Zapret
            bool isVersionDeleted = _zapretVersionsService.DeleteZapretVersion(SelectedZapretVersion.Model);
            if (!isVersionDeleted)
            {
                string versionPath = AppDomain.CurrentDomain.BaseDirectory + $"versions\\{SelectedZapretVersion.Number}";
                ZapretService zapretService = new ZapretService(versionPath);

                await zapretService.RemoveServiceAsync();
                _zapretVersionsService.DeleteZapretVersion(SelectedZapretVersion.Model);
            }

            SelectedZapretVersion.IsDownloaded = false;
            _logger.AddSuccess("Версия удалена.", false);
        }

        /// <summary>
        /// Запуск процесса подбора обхода или запуск уже подобранного обхода для выбранной версии Zapret
        /// </summary>
        /// <param name="param">Параметр для работы MVVM, не уверен, что это</param>
        private async void StartStopVersion(object param)
        {
            if (SelectedZapretVersion == null) return;

            string versionPath = AppDomain.CurrentDomain.BaseDirectory + $"versions\\{SelectedZapretVersion.Number}";
            ZapretService zapretService = new ZapretService(versionPath);

            // Если подход ещё не подобран для выбранной версии
            if (string.IsNullOrWhiteSpace(SelectedZapretVersion.BypassMethodName))
            {
                if (IsChoosingBypassMethod)
                {
                    _bypassCheckerCtSource.Cancel();
                    IsChoosingBypassMethod = false;
                    _logger.AddInfo("Отмена процесса подбора обхода...");
                    _logger.AddInfo("");
                    return;
                }

                IsChoosingBypassMethod = true;
                _logger.AddInfo("");
                _logger.AddInfo($"Запущен процесс подбора обхода для Zapret v{SelectedZapretVersion.Number}.");

                bool hasFoundBypassMethod = false;
                string bypassMethodName = string.Empty;
                try
                {
                    (hasFoundBypassMethod, bypassMethodName) = await BypassCheckerService.FindBypassMethodAsync(zapretService, _logger, _bypassCheckerCtSource.Token);
                }
                catch (OperationCanceledException)
                {
                    _bypassCheckerCtSource = new CancellationTokenSource();
                    _logger.AddInfo("Процесс подбора обхода отменён.");
                    IsChoosingBypassMethod = false;
                    return;
                }

                if (hasFoundBypassMethod)
                {
                    _logger.AddInfo("");
                    _logger.AddSuccess("Найден подходящий обход!");
                    SelectedZapretVersion.BypassMethodName = bypassMethodName;
                }
                else
                {
                    _logger.AddError($"Не удалось подобрать подходящий обход для версии Zapret {SelectedZapretVersion.Number}...");
                }

                IsChoosingBypassMethod = false;
            }
            else
            {
                if (!IsZapretRunning)
                {
                    await zapretService.InstallServiceAsync(SelectedZapretVersion.BypassMethodName);
                    IsZapretRunning = true;

                    _logger.AddInfo($"Zapret v{SelectedZapretVersion.Number} запущен. Приятного пользования!");
                }
                else
                {
                    await zapretService.RemoveServiceAsync();
                    IsZapretRunning = false;

                    _logger.AddInfo($"Zapret v{SelectedZapretVersion.Number} остановлен.");
                }
            }
        }
    }
}