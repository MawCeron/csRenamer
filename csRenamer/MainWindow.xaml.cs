using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void optionsButton_Click(object sender, RoutedEventArgs e)
        {
            optionsButton.Visibility = Visibility.Collapsed;
            optionsTitle.Visibility = Visibility.Visible;
            optionsPanel.Visibility = Visibility.Visible;
        }

        private void closeOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            optionsButton.Visibility = Visibility.Visible;
            optionsTitle.Visibility = Visibility.Collapsed;
            optionsPanel.Visibility = Visibility.Collapsed;
        }
    }
}