using AutoCheckZapret.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AutoCheckZapret
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScrollViewer _consoleScrollViewer;
        private Logger _logger;

        /// <summary>
        /// Конструктор главного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Подписываемся на событие загрузки окна, чтобы найти ScrollViewer
            this.Loaded += OnLoaded;

            // Получаем логгер из DataContext
            if (DataContext is MainViewModel viewModel)
            {
                _logger = viewModel.Logger;
                if (_logger != null)
                {
                    _logger.PropertyChanged += OnLoggerPropertyChanged;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Находим ScrollViewer внутри FlowDocumentScrollViewer
            _consoleScrollViewer = FindChild<ScrollViewer>(fdsViewerConsole);
        }

        private void OnLoggerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // При изменении свойства LogDocument (добавлено новое сообщение) прокручиваем вниз
            if (e.PropertyName == nameof(Logger.LogDocument))
            {
                // Используем Dispatcher, чтобы дождаться завершения визуального обновления
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _consoleScrollViewer?.ScrollToEnd();
                }), DispatcherPriority.Background);
            }
        }

        // Вспомогательный метод для поиска дочернего элемента определённого типа
        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                var subResult = FindChild<T>(child);
                if (subResult != null)
                    return subResult;
            }
            return null;
        }

        // Отписываемся при закрытии окна (опционально, но рекомендуется)
        protected override void OnClosed(EventArgs e)
        {
            if (_logger != null)
                _logger.PropertyChanged -= OnLoggerPropertyChanged;
            base.OnClosed(e);
        }
    }
}