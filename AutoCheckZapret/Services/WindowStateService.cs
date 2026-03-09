using System.Windows;
using System.Windows.Input;

namespace AutoCheckZapret.Services
{
    /// <summary>
    /// Класс для управления состоянием окна приложения.
    /// </summary>
    public class WindowStateService
    {
        private WindowState _windowState = Application.Current.MainWindow.WindowState;
        private MainWindow _mainWindow;

        /// <summary>
        /// Конструктор
        /// </summary>
        public WindowStateService(MainWindow window)
        {
            // Начальные настройки можно сделать здесь
            Application.Current.MainWindow.WindowState = _windowState;
            _mainWindow = window;
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
            _windowState = WindowState.Minimized;
            ChangeState(_windowState);
        }

        /// <summary>
        /// Переключает состояние окна между полноэкранным и обычным режимами.
        /// </summary>
        public void ToggleFullscreen()
        {
            if (_windowState == WindowState.Normal)
            {
                _windowState = WindowState.Maximized;
                ChangeState(_windowState);
            }
            else
            {
                _windowState = WindowState.Normal;
                ChangeState(_windowState);
            }
        }

        /// <summary>
        /// Изменяет состояние главного окна приложения.
        /// </summary>
        /// <param name="windowState">Новое состояние окна.</param>
        private void ChangeState(WindowState windowState)
        {
            Application.Current.MainWindow.WindowState = windowState;
        }
    }
}
