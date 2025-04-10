using System.Windows;
using System.Windows.Controls;
using csRenamer.Services;

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
            FolderExplorer.LoadDrives(treeView);
        }

        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = treeView.SelectedItem as TreeViewItem;
            if (selected != null)
            {
                progressBar.IsIndeterminate = true;
                string selectedPath = selected.Tag.ToString();
                directoryText.Text = selectedPath;
                renameGrid.ItemsSource = FileServices.GetFiles(selectedPath, 0, "*");
                filesText.Text = renameGrid.Items.Count.ToString();
                progressBar.IsIndeterminate = false;
            }
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