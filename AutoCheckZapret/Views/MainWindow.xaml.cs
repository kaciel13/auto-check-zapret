using AutoCheckZapret.ViewModels;
using System.Windows;

namespace AutoCheckZapret
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly WindowStateService _windowStateService;
   
        /// <summary>
        /// Конструктор главного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            //// Инициализируем сервис с передачей ссылок на элементы
            //_windowStateService = new WindowStateService(
            //    this,
            //    MainGrid,
            //    DragHeader,
            //    btnToggleFullscreen
            //);
        }
    }
}