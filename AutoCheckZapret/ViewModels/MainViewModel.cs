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
    /// Главная ВьюМодель приложения
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public FlowDocument LogDocument => _logger?.LogDocument;
        
        private bool _isFullscreen = false;
        
        private string _appNameWithVersion;

        private Logger _logger;

        private ZapretService _zapretService;
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

        private ObservableCollection<ZapretVersion> _zapretVersions;
        /// <summary>
        /// Полученные из официального репозитория версии Zapret
        /// </summary>
        public ObservableCollection<ZapretVersion> ZapretVersions 
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

        private ZapretVersion _selecredZapretVersion;
        /// <summary>
        /// Выбранная в выпадающем списке версия Zapret
        /// </summary>
        public ZapretVersion SelectedZapretVersion
        {
            get { return _selecredZapretVersion; }
            set
            {
                _selecredZapretVersion = value;
                OnPropertyChanged("SelectedZapretVersion");

                if (value != null) 
                {
                    if (_zapretVersionsService.IsZapretVersionDownloaded(value))
                    {
                        CanDownloadZapretVersion = false;
                        CanDeleteOrWorkWithZapretVersion = true;
                    }
                    else
                    {
                        CanDownloadZapretVersion = true;
                        CanDeleteOrWorkWithZapretVersion = false;
                    }
                }
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
                    ChooseBypassMethodButtonContent = $"Подобрать обход для Zapret v{SelectedZapretVersion.Number}";
                }
            }
        }
        private bool _canDeleteOrWorkWithZapretVersion;
        /// <summary>
        /// Может ли пользователь удалить версию Zapret или работать с ней
        /// </summary>
        public bool CanDeleteOrWorkWithZapretVersion
        {
            get { return _canDeleteOrWorkWithZapretVersion; }
            set
            {
                _canDeleteOrWorkWithZapretVersion = value;
                OnPropertyChanged("CanDeleteOrWorkWithZapretVersion");

                if (value)
                {
                    ChooseBypassMethodButtonContent = $"Подобрать обход для Zapret v{SelectedZapretVersion.Number}";
                }
                else
                {
                    ChooseBypassMethodButtonContent = $"Скачайте Zapret v{SelectedZapretVersion.Number}, чтобы начать работу";
                }
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

        private RelayCommand _chooseBypassMethodForVersionCommand;
        /// <summary>
        /// Команда подбора обхода для выбранной версии Zapret
        /// </summary>
        public RelayCommand ChooseBypassMethodForVersionCommand
        {
            get
            {
                return _chooseBypassMethodForVersionCommand ?? (_chooseBypassMethodForVersionCommand = new RelayCommand(ChooseBypassMethodForVersion));
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
            List<ZapretVersion> downloadedZapretVersions = ZapretVersions.Where(version => _zapretVersionsService.IsZapretVersionDownloaded(version)).ToList();

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
            try {
                ZapretVersions = await _zapretVersionsService.FetchAvailableVersions();
                foreach (var version in ZapretVersions)
                {
                    version.IsDownloaded = _zapretVersionsService.IsZapretVersionDownloaded(version);
                }
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

                SavedApplicationData? savedData = null; ;
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
                    ZapretVersion? lastSelectedVersion = ZapretVersions.FirstOrDefault(version => version.Number == savedData.LastSelectedZapretVersion.Number);
                    if (lastSelectedVersion != null)
                    {
                        SelectedZapretVersion = lastSelectedVersion;
                        return;
                    }
                }
            }

            SelectedZapretVersion = ZapretVersions[0];
        }

        private async void DownloadZapretVersion(object param)
        {
            CanDownloadZapretVersion = false;

            ZapretVersionsService downloaderService = new ZapretVersionsService();
            _logger.AddInfo($"Cкачивание версии Zapret {SelectedZapretVersion.Number}...");

            try
            {
                await downloaderService.DownloadZapretVersion(SelectedZapretVersion);

                SelectedZapretVersion.IsDownloaded = true;
                CanDeleteOrWorkWithZapretVersion = true;
                _logger.AddSuccess("Скачивание завершено!", false);
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка скачивания Zapret: {ex.Message}", false);
            }
        }

        private void DeleteZapretVersion(object param)
        {
            MessageBoxResult questionResult = MessageBox.Show($"Вы уверены, что хотите удалить Zapret версии {SelectedZapretVersion.Number}?", "Подтверждение удаления версии Zapret", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (questionResult != MessageBoxResult.Yes) { return; }

            _logger.AddInfo($"Удаление версии Zapret {SelectedZapretVersion.Number}...");

            try
            {
                _zapretVersionsService.DeleteZapretVersion(SelectedZapretVersion);

                SelectedZapretVersion.IsDownloaded = false;
                CanDownloadZapretVersion = true;
                CanDeleteOrWorkWithZapretVersion = false;
                _logger.AddSuccess("Версия удалена.", false);
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка удаления Zapret: {ex.Message}", false);
            }
        }

        private async void ChooseBypassMethodForVersion(object param)
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
            
            string versionPath = AppDomain.CurrentDomain.BaseDirectory + $"versions\\{SelectedZapretVersion.Number}";
            _zapretService = new ZapretService(versionPath);

            bool hasFoundBypassMethod = false;
            string bypassMethodName = string.Empty;
            try
            {
                (hasFoundBypassMethod, bypassMethodName) = await BypassCheckerService.FindBypassMethodAsync(_zapretService, _logger, _bypassCheckerCtSource.Token);
            }
            catch (OperationCanceledException)
            {
                _bypassCheckerCtSource = new CancellationTokenSource();
                _logger.AddInfo("Процесс подбора обхода отменён.");
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
    }
}
