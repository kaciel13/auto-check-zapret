using System.Reflection;
using System.Windows;

namespace AutoCheckZapret
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Get the Assembly object for the currently executing assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the AssemblyName object
            AssemblyName assemblyName = assembly.GetName();

            // Get the Version object
            Version version = assemblyName.Version!;

            // В конце используем Build, потому что в .csproj используем вид Major.Minor.Feature, а не Major.Minor.Feature.Build
            // А VS определяет последнюю цифру как Build
            this.Title = $"Auto Check Zapret v{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}