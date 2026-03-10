using System.Windows;
using System.Windows.Controls;

namespace AutoCheckZapret.Services
{
    /// <summary>
    /// Класс для управления состоянием окна приложения.
    /// </summary>
    public class WindowStateService
    {
        private Window _window;
        private FrameworkElement _mainGrid;
        private FrameworkElement _dragHeader;
        private Button _toggleFullscreenButton;

        public WindowState WindowState { get; private set; }

        /// <summary>
        /// Конструктор с передачей необходимых элементов
        /// </summary>
        public WindowStateService(Window window, FrameworkElement mainGrid,
                                  FrameworkElement dragHeader, Button toggleFullscreenButton)
        {
            _window = window;
            _mainGrid = mainGrid;
            _dragHeader = dragHeader;
            _toggleFullscreenButton = toggleFullscreenButton;
            WindowState = window.WindowState;

            // Подписываемся на изменение состояния окна
            _window.StateChanged += Window_StateChanged;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowState = _window.WindowState;
            UpdateMaximizedState(WindowState == WindowState.Maximized);
        }

        /// <summary>
        /// Завершает работу приложения.
        /// </summary>
        public void ShutDownApplication()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Сворачивает текущее окно.
        /// </summary>
        public void MinimizeWindow()
        {
            _window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Переключает состояние окна между полноэкранным и обычным режимами.
        /// </summary>
        public void ToggleFullscreen()
        {
            if (_window.WindowState == WindowState.Normal)
            {
                _window.WindowState = WindowState.Maximized;
            }
            else
            {
                _window.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Обновляет отступы при максимизации
        /// </summary>
        private void UpdateMaximizedState(bool isMaximized)
        {
            if (_mainGrid == null || _dragHeader == null) return;

            if (isMaximized)
            {
                // Получаем размер рабочей области для корректных отступов
                var workingArea = SystemParameters.WorkArea;

                _mainGrid.Margin = new Thickness(7);
                _dragHeader.Margin = new Thickness(0);

                if (_toggleFullscreenButton != null)
                {
                    _toggleFullscreenButton.Content = "❐"; // Иконка восстановления
                }
            }
            else
            {
                _mainGrid.Margin = new Thickness(0);
                _dragHeader.Margin = new Thickness(0);

                if (_toggleFullscreenButton != null)
                {
                    _toggleFullscreenButton.Content = "☐"; // Иконка разворачивания
                }
            }
        }
    }
}