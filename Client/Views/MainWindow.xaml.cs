using Client.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(configuration);
        }
    }
}
