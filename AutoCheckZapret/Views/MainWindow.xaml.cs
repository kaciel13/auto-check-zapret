using AutoCheckZapret.ViewModels;
using System.Windows;

namespace AutoCheckZapret
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {  
        /// <summary>
        /// Конструктор главного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void cbxZapretVersions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void cbxZapretVersions_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}