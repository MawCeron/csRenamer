using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using csRenamer.Avalonia.Services;
using csRenamer.Avalonia.Views;

namespace csRenamer.Avalonia;

public partial class MainWindow : Window
{
    private CancellationTokenSource? cancellationTokenSource;

    public MainWindow()
    {
        InitializeComponent();
        foreach (var drive in FolderExplorer.GetDrives())
            treeView.Items.Add(drive);
    }

    private void RefreshGrid()
    {
        renameGrid.ItemsSource = null;
        renameGrid.ItemsSource = FileServices.Files;
    }

    private async void FolderTreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selected = treeView.SelectedItem as TreeNode;
        if (selected == null) return;

        var mode = comboOptions.SelectedIndex;
        var pattern = textboxPattern.Text?.Trim() ?? "";
        var recursively = checkboxRecursively.IsChecked == true;
        stopButton.IsVisible = true;

        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        progressBar.IsIndeterminate = true;
        directoryText.Text = selected.FullPath;

        FileServices.Files.Clear();

        try
        {
            var files = await Task.Run(() =>
                FileServices.GetFiles(selected.FullPath, mode, pattern, recursively, token), token);

            foreach (var file in files)
                FileServices.Files.Add(file);
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("File loading cancelled.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading files: {ex.Message}");
        }

        RefreshGrid();
        filesText.Text = FileServices.Files.Count.ToString();
        stopButton.IsVisible = false;
        progressBar.IsIndeterminate = false;
    }

    private void optionsButton_Click(object? sender, RoutedEventArgs e)
    {
        optionsButton.IsVisible = false;
        optionsTitle.IsVisible = true;
        optionsPanel.IsVisible = true;
    }

    private void closeOptionsButton_Click(object? sender, RoutedEventArgs e)
    {
        optionsButton.IsVisible = true;
        optionsTitle.IsVisible = false;
        optionsPanel.IsVisible = false;
    }

    private void quitButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void previewButton_Click(object? sender, RoutedEventArgs e)
    {
        progressBar.IsIndeterminate = true;
        var option = renameOptions.SelectedIndex;

        switch (option)
        {
            case 0: // Patterns
                int counter = 0;
                foreach (var file in FileServices.Files)
                {
                    string ext = keepExtensionCheckbox.IsChecked == true
                        ? Path.GetExtension(file.FullPath).TrimStart('.')
                        : "";
                    file.NewName = PatternRenamer.RenameUsingPatterns(
                        file.FileName, file.FullPath,
                        originalPattern.Text ?? "", renamedPattern.Text ?? "",
                        counter, ext);
                    counter++;
                }
                break;

            case 1: // Substitutions
                foreach (var file in FileServices.Files)
                {
                    string name = keepExtensionCheckbox.IsChecked == true
                        ? Path.GetFileNameWithoutExtension(file.FileName)
                        : file.FileName;
                    string extension = keepExtensionCheckbox.IsChecked == true
                        ? Path.GetExtension(file.FileName)
                        : "";

                    string newName = name;

                    if (spacesCheck.IsChecked == true)
                        newName = SubstitutionsRenamer.ReplaceSpaces(newName, spacesCombo.SelectedIndex);
                    if (replaceCheck.IsChecked == true)
                        newName = SubstitutionsRenamer.ReplaceWith(newName, replaceText.Text ?? "", replaceWithText.Text ?? "");
                    if (capitalizationCheck.IsChecked == true)
                        newName = SubstitutionsRenamer.ReplaceCapitalization(newName, capitalizationCombo.SelectedIndex);
                    if (accentsCheck.IsChecked == true)
                        newName = SubstitutionsRenamer.RemoveAccents(newName);
                    if (duplicatesCheck.IsChecked == true)
                        newName = SubstitutionsRenamer.RemoveDuplicatedSymbols(newName);

                    file.NewName = newName + extension;
                }
                break;

            case 2: // Insert or Delete
                foreach (var file in FileServices.Files)
                {
                    string name = keepExtensionCheckbox.IsChecked == true
                        ? Path.GetFileNameWithoutExtension(file.FileName)
                        : file.FileName;
                    string extension = keepExtensionCheckbox.IsChecked == true
                        ? Path.GetExtension(file.FileName)
                        : "";

                    string newName = name;

                    if (insertRadioButton.IsChecked == true)
                    {
                        int pos = atEndCheckbox.IsChecked == true
                            ? name.Length
                            : (int)(insertAtNumeric.Value ?? 1);
                        newName = OtherRenamers.InsertAt(name, insertText.Text ?? "", pos);
                    }

                    if (deleteRadioButton.IsChecked == true)
                    {
                        int from = (int)(deleteFromNumeric.Value ?? 1);
                        int to = (int)(deleteToNumeric.Value ?? 5);
                        newName = OtherRenamers.DeleteFrom(name, from, to);
                    }

                    file.NewName = newName + extension;
                }
                break;

            case 3: // Manual rename
                var selectedFile = renameGrid.SelectedItem as FileServices.FileItem;
                if (selectedFile != null)
                {
                    string ext = keepExtensionCheckbox.IsChecked == true
                        ? Path.GetExtension(selectedFile.FileName)
                        : "";
                    selectedFile.NewName = (manualRenameText.Text ?? "") + ext;
                }
                break;
        }

        RefreshGrid();
        progressBar.IsIndeterminate = false;
    }

    private void stopButton_Click(object? sender, RoutedEventArgs e)
    {
        cancellationTokenSource?.Cancel();
    }

    private void clearButton_Click(object? sender, RoutedEventArgs e)
    {
        foreach (var file in FileServices.Files)
            file.NewName = string.Empty;

        RefreshGrid();
    }

    private void renameButton_Click(object? sender, RoutedEventArgs e)
    {
        FileServices.RenameFiles();
        RefreshGrid();
    }

    private void aboutButton_Click(object? sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow(this);
        aboutWindow.ShowDialog(this);
    }
}
