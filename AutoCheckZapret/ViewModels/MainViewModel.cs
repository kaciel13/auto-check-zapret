using AutoCheckZapret.Helpers;
using AutoCheckZapret.Models;
using AutoCheckZapret.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private bool _isFullscreen = false;
        private string _appNameWithVersion;
        private Logger _logger;
        public FlowDocument LogDocument => _logger?.LogDocument;
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

                if (value != null) { HasFetchedZapretVersions = true; }
            }
        }

        private bool _hasFetchedZapretVersions;
        /// <summary>
        /// Были ли получены версии Zapret с официального репозитория
        /// </summary>
        public bool HasFetchedZapretVersions
        {
            get { return _hasFetchedZapretVersions; }
            set
            {
                _hasFetchedZapretVersions = value;
                OnPropertyChanged("HasFetchedZapretVersions");
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

        private ZapretVersionsService _zapretVersionsService;
        private Updater _updater;
        
        //public Logger Logger => _logger;
        /// <summary>
        /// Конструктор главной ВьюМодели
        /// </summary>
        public MainViewModel()
        {
            _logger = new Logger();
            _updater = new Updater();
            _updater.CheckUpdate();
            _zapretVersionsService = new ZapretVersionsService();
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version!;
            //// В конце используем Build, потому что в .csproj используем вид Major.Minor.Feature, а не Major.Minor.Feature.Build
            //// А VS определяет последнюю цифру как Build
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
            Application.Current.Shutdown();
        }

        private async Task FetchAvailableZapretVersions()
        {
            _logger.AddInfo("Получение версий Zapret...");
            try {
                ZapretVersions = await _zapretVersionsService.FetchAvailableVersions();
                // TODO: Вот тут, наверное, нужно сделать проверку какую-то на то, были ли получены версии Запрета
                foreach (var version in ZapretVersions) { 
                    version.IsDownloaded = _zapretVersionsService.IsZapretVersionDownloaded(version);
                }
                _logger.AddSuccess($"Получено {ZapretVersions.Count} версий.", false);
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка получения версий: {ex.Message}");
            }

            SelectedZapretVersion = ZapretVersions[0];
        }

        private async void DownloadZapretVersion(object param)
        {
            CanDownloadZapretVersion = false;

            ZapretVersionsService downloaderService = new ZapretVersionsService();
            try {
                _logger.AddInfo($"Cкачивание версии zapret {SelectedZapretVersion.Number}...");
                await downloaderService.DownloadZapretVersion(SelectedZapretVersion);
                SelectedZapretVersion.IsDownloaded = true;
                CanDeleteOrWorkWithZapretVersion = true;
                _logger.AddSuccess("Скачивание завершено", false);
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка скачивания: {ex.Message}");
            }
        }

        private void DeleteZapretVersion(object param)
        {
            MessageBoxResult questionResult = MessageBox.Show($"Вы уверены, что хотите удалить Zapret версии {SelectedZapretVersion.Number}?", "Подтверждение удаления версии Zapret", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (questionResult != MessageBoxResult.Yes) { return; }
            try
            {
                _logger.AddInfo($"Удаление версии zapret {SelectedZapretVersion.Number}...");
                _zapretVersionsService.DeleteZapretVersion(SelectedZapretVersion);
                SelectedZapretVersion.IsDownloaded = false;
                CanDownloadZapretVersion = true;
                CanDeleteOrWorkWithZapretVersion = false;
                _logger.AddSuccess("Версия удалена", false);
            }
            catch (Exception ex)
            {
                _logger.AddError($"Ошибка: {ex.Message}");
            }

        }
    }
}
