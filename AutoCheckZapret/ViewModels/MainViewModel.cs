using AutoCheckZapret.Helpers;
using AutoCheckZapret.Models;
using AutoCheckZapret.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AutoCheckZapret.ViewModels
{
    /// <summary>
    /// Главная ВьюМодель приложения
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _appNameWithVersion;
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

        // TODO: Команда для разворачивания окна на весь экран и обратно

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
                    CanWorkWithZapretVersion = true;
                    ChooseBypassMethodButtonContent = $"Подобрать обход для Zapret v{value.Number}";
                }
            }
        }

        private bool _canWorkWithZapretVersion;
        /// <summary>
        /// Может ли пользователь взаимодействовать с версией Zapret (скачивать её, удалять, запускать)
        /// </summary>
        public bool CanWorkWithZapretVersion
        {
            get { return _canWorkWithZapretVersion; }
            set
            {
                _canWorkWithZapretVersion = value;
                OnPropertyChanged("CanWorkWithZapretVersion");
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

        /// <summary>
        /// Конструктор главной ВьюМодели
        /// </summary>
        public MainViewModel()
        {
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

        private void MinimizeWindow(object parameter)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        // TODO: Сделать метод для разворачивания окна на весь экран и обратно

        private void ShutdownApplication(object parameter)
        {
            Application.Current.Shutdown();
        }

        private async Task FetchAvailableZapretVersions()
        {
            ZapretDownloaderService zapretDownloaderService = new ZapretDownloaderService();
            ZapretVersions = await zapretDownloaderService.FetchAvailableVersions();
            // TODO: Вот тут, наверное, нужно сделать проверку какую-то на то, были ли получены версии Запрета

            SelectedZapretVersion = ZapretVersions[0];
        }

        private async void DownloadZapretVersion(object parameter)
        {
            CanWorkWithZapretVersion = false;

            ZapretDownloaderService downloaderService = new ZapretDownloaderService();
            bool isDownloaded = await downloaderService.DownloadZapretVersion(SelectedZapretVersion);
            if (!isDownloaded)
            {
                // TODO: В консоль нужно что-то выводить, собственно, по поводу возникшей при скачивании ошибки
                MessageBox.Show($"Ошибка скачивания Zapret версии {SelectedZapretVersion.Number}. Смотрите детали ошибки в консоли программы.", "Ошибка скачивания версии Zapret", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            CanWorkWithZapretVersion = true;
        }
    }
}
