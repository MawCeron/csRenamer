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

        private void RefreshGrid()
        {
            // Refresh the grid by reassigning the ItemsSource
            renameGrid.ItemsSource = null; // Clear the current binding
            renameGrid.ItemsSource = FileServices.Files; // Rebind to the updated collection
        }

        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = treeView.SelectedItem as TreeViewItem;
            var mode = Int32.Parse(((ComboBoxItem)comboOptions.SelectedItem).Tag.ToString());
            var pattern = textboxPattern.Text.Trim();
            var recursively = checkboxRecursively.IsChecked == true;

            if (selected != null)
            {
                progressBar.IsIndeterminate = true;
                string selectedPath = selected.Tag.ToString();
                directoryText.Text = selectedPath;

                FileServices.Files.Clear();
                if (recursively)                    
                    FileServices.Files = FileServices.GetFilesRecursively(selectedPath, mode, pattern);
                else
                    FileServices.Files = FileServices.GetFiles(selectedPath, mode, pattern);

                RefreshGrid();

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

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void previewButton_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            var option = renameOptions.SelectedItem as TabItem;            
            switch (option.Tag)
            {
                case "0":
                    // Patterns
                    foreach (var file in FileServices.Files)
                    {
                        string newName = PatternRenamer.RenameUsingPatterns(file.FileName, file.FullPath, originalPattern.Text, renamedPattern.Text, 0);
                        file.NewName = newName;
                    }
                    break;
                case "1":
                    // Subtitutions
                    break;
                case "2":
                    // Insert and Delete
                    break;
                case "3":
                    // Manual rename
                    break;
            }

            RefreshGrid();
            progressBar.IsIndeterminate = false;            
        }
    }
}