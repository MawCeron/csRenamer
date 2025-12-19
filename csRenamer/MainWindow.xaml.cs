using System;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using csRenamer.Services;
using csRenamer.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace csRenamer
{
    public sealed partial class MainWindow : Window
    {

        private CancellationTokenSource? cancellationTokenSource;
        private readonly FileReload _fileReload = new();
        private DispatcherTimer _patternDebounceTimer;

        public MainWindow()
        {
            this.InitializeComponent();
            SetWindowProperties();

            cbShowOptions.SelectedIndex = 0;
            FolderExplorer.LoadDrives(folderTreeView);
            
            if (folderTreeView.RootNodes.Count > 0)            
                folderTreeView.SelectedNode = folderTreeView.RootNodes[0];
            
            ContentFrame.Navigate(typeof(csRenamer.Pages.Patterns));

            _patternDebounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };

            _patternDebounceTimer.Tick += async (s, e) =>
            {
                _patternDebounceTimer.Stop();
                await ReloadFilesAsync();
            };
        }

        private void SetWindowProperties()
        {
            this.Title = "csRenamer";
            titleBar.Subtitle = "v. 0.2.0";
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(titleBar);
            this.AppWindow.SetIcon("Assets/csRenamer.ico");
            this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            this.AppWindow.Resize(new SizeInt32(900, 600));
            
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            btnRename.Visibility = Visibility.Collapsed;
            progressBar.IsIndeterminate = true;
            btnStop.Visibility = Visibility.Visible;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _fileReload.Cancel();
            btnStop.Visibility = Visibility.Collapsed;
            progressBar.IsIndeterminate = false;
            btnRename.Visibility = Visibility.Visible;
        }

        private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectedItem = sender.SelectedItem;
            int currentSelectedIndex = sender.Items.IndexOf(selectedItem);

            switch (currentSelectedIndex)
            {
                case 0:
                    ContentFrame.Navigate(typeof(csRenamer.Pages.Patterns));
                    break;
                case 1:
                    ContentFrame.Navigate(typeof(csRenamer.Pages.Substitutions));
                    break;
                case 2:
                    ContentFrame.Navigate(typeof(csRenamer.Pages.InsertDelete));
                    break;
                case 3:
                    ContentFrame.Navigate(typeof(csRenamer.Pages.Manual));
                    break;
                default:
                    break;
            }
        }

        private void folderTreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            var node = args.InvokedItem as TreeViewNode;

            if (node!=null)
                node.IsExpanded = !node.IsExpanded;

            var folderItem = node?.Content as FolderTreeItem;

            if (folderItem != null)
                System.Diagnostics.Debug.WriteLine($"Selected: {folderItem.FullPath}");
        }

        private void folderTreeView_Expanding(TreeView sender, TreeViewExpandingEventArgs args)
        {
            if (args.Node.HasUnrealizedChildren)
            {
                args.Node.Children.Clear();

                var folderItem = args.Node.Content as FolderTreeItem;
                if (folderItem == null) return;

                try
                {
                    var directories = Directory.GetDirectories(folderItem.FullPath);

                    foreach (var dir in directories)
                    {
                        var dirInfo = new DirectoryInfo(dir);

                        // Omitir carpetas ocultas o del sistema si quieres
                        if ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                            (dirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                            continue;

                        var childItem = new FolderTreeItem
                        {
                            Name = dirInfo.Name,
                            FullPath = dirInfo.FullName,
                            IconGlyph = "\uE8B7" // Folder icon
                        };

                        var childNode = new TreeViewNode
                        {
                            Content = childItem,
                            HasUnrealizedChildren = FolderExplorer.HasSubfolders(dirInfo.FullName)
                        };

                        args.Node.Children.Add(childNode);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // No tienes permisos para acceder a esta carpeta
                }
                catch (Exception ex)
                {
                    // Maneja otros errores
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                }

                args.Node.HasUnrealizedChildren = false;
            }
        }

        private async void folderTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
        {
            var node = sender.SelectedNodes.FirstOrDefault();
            if (node?.Content is not FolderTreeItem folderItem)
                return;

            directoryText.Text = folderItem.FullPath;
            await ReloadFilesAsync();
        }

        private async Task ReloadFilesAsync()
        {
            if (string.IsNullOrWhiteSpace(directoryText.Text))
                return;

            btnStop.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;

            try
            {
                var items = await _fileReload.ReloadAsync(
                    directoryText.Text,
                    cbShowOptions.SelectedIndex,
                    tbSelectionPattern.Text,
                    chbRecursive.IsChecked == true);

                renameGrid.ItemsSource = items;
                filesText.Text = items.Count.ToString();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                }.ShowAsync();
            }

            btnStop.Visibility = Visibility.Collapsed;
            progressBar.IsIndeterminate = false;
        }


        private async void cbShowOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await ReloadFilesAsync();
        }

        private void tbSelectionPattern_TextChanged(object sender, TextChangedEventArgs e)
        {
            _patternDebounceTimer.Stop();
            _patternDebounceTimer.Start();
        }

        private async void chbRecursive_Checked(object sender, RoutedEventArgs e)
        {
            await ReloadFilesAsync();
        }
    }
}
