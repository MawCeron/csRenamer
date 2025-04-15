using System.IO;
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
        private CancellationTokenSource? cancellationTokenSource;

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

        private async void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = treeView.SelectedItem as TreeViewItem;
            var mode = comboOptions.SelectedIndex;
            var pattern = textboxPattern.Text.Trim();
            var recursively = checkboxRecursively.IsChecked == true;
            stopButton.Visibility = Visibility.Visible;

            if (selected != null)
            {
                // Cancelar cualquier operación anterior si sigue activa
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;

                progressBar.IsIndeterminate = true;
                string selectedPath = selected.Tag?.ToString() ?? "";
                directoryText.Text = selectedPath;

                FileServices.Files.Clear();

                try
                {
                    await Task.Run(() =>
                    {
                        if (recursively)
                            FileServices.Files = FileServices.GetFilesRecursively(selectedPath, mode, pattern, token);
                        else
                            FileServices.Files = FileServices.GetFiles(selectedPath, mode, pattern, token);
                    }, token);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Carga de archivos cancelada.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar archivos: {ex.Message}");
                }

                RefreshGrid();
                filesText.Text = renameGrid.Items.Count.ToString();
                stopButton.Visibility = Visibility.Collapsed;
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
            var option = renameOptions.SelectedIndex;

            switch (option)
            {
                case 0:
                    // Patterns
                    foreach (var file in FileServices.Files)
                    {
                        string newName = PatternRenamer.RenameUsingPatterns(file.FileName, file.FullPath, originalPattern.Text, renamedPattern.Text, 0,
                            keepExtensionCheckbox.IsChecked == true ? Path.GetExtension(file.FullPath).TrimStart('.') : "");

                        file.NewName = newName;
                    }
                    break;
                case 1:
                    // Subtitutions
                    foreach (var file in FileServices.Files)
                    {
                        string name = keepExtensionCheckbox.IsChecked == true ? Path.GetFileNameWithoutExtension(file.FileName) : file.FileName;
                        string extension = keepExtensionCheckbox.IsChecked == true ? Path.GetExtension(file.FileName) : "";

                        string newName = name;

                        if (spacesCheck.IsChecked == true)
                            newName = SubstitutionsRenamer.ReplaceSpaces(newName, spacesCombo.SelectedIndex);

                        if (replaceCheck.IsChecked == true)
                            newName = SubstitutionsRenamer.ReplaceWith(newName, replaceText.Text, replaceWithText.Text);

                        if (capitalizationCheck.IsChecked == true)
                            newName = SubstitutionsRenamer.ReplaceCapitalization(newName, capitalizationCombo.SelectedIndex);

                        if (accentsCheck.IsChecked == true)
                            newName = SubstitutionsRenamer.RemoveAccents(newName);

                        if (duplicatesCheck.IsChecked == true)
                            newName = SubstitutionsRenamer.RemoveDuplicatedSymbols(newName);

                        file.NewName = newName + extension;
                    }
                    break;
                case 2:
                    // Insert or Delete
                    foreach (var file in FileServices.Files)
                    {
                        string name = keepExtensionCheckbox.IsChecked == true ? Path.GetFileNameWithoutExtension(file.FileName) : file.FileName;
                        string extension = keepExtensionCheckbox.IsChecked == true ? Path.GetExtension(file.FileName) : "";

                        string newName = name;

                        if (insertRadioButton.IsChecked == true)
                        {
                            if (atEndCheckbox.IsChecked == true)
                                newName = AnotherRenamers.InsertAt(name, insertText.Text, 0);
                            else
                                newName = AnotherRenamers.InsertAt(name, insertText.Text, insertAtNumeric.Value);
                        }
                            

                        if (deleteRadioButton.IsChecked == true)
                            newName = AnotherRenamers.DeleteFrom(name, deleteFromNumeric.Value, deleteToNumeric.Value);

                        file.NewName = newName + extension;
                    }
                    break;
                case 3:
                    // Manual rename
                    var selectedFile = renameGrid.SelectedItem as FileServices.FileItem;                    
                    string fileExtension = keepExtensionCheckbox.IsChecked == true ? Path.GetExtension(selectedFile!.FileName) : "";

                    selectedFile!.NewName = manualRenameText.Text + fileExtension;
                    break;
            }

            RefreshGrid();
            progressBar.IsIndeterminate = false;            
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var file in FileServices.Files)
            {
                file.NewName = string.Empty;
            }

            RefreshGrid();
        }

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
            FileServices.RenameFiles();
            RefreshGrid();
        }
    }
}