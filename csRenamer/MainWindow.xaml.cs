using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using csRenamer.Services;
using csRenamer.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace csRenamer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            FolderExplorer.LoadDrives(folderTreeView);
            
            if (folderTreeView.RootNodes.Count > 0)            
                folderTreeView.SelectedNode = folderTreeView.RootNodes[0];
            
            // Show the Patterns page on startup
            ContentFrame.Navigate(typeof(csRenamer.Pages.Patterns));
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
    }
}
