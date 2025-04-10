using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Microsoft.Win32;

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
            LoadDrives();
        }

        private void LoadDrives()
        {
            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    var item = CreateTreeItem(drive.Name);
                    treeView.Items.Add(item);
                }
            }
        }

        private TreeViewItem CreateTreeItem(string path)
        {
            var item = new TreeViewItem
            {
                Header = System.IO.Path.GetFileName(path) == string.Empty ? path : System.IO.Path.GetFileName(path),
                Tag = path
            };

            item.Items.Add(null); // Placeholder for lazy loading
            item.Expanded += Folder_Expanded;
            return item;
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;

            if (item.Items.Count == 1 && item.Items[0] == null)
            {
                item.Items.Clear();

                try
                {
                    var directories = Directory.GetDirectories((string)item.Tag);
                    foreach (var dir in directories)
                    {
                        try
                        {
                            var dirInfo = new DirectoryInfo(dir);
                            if ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                                (dirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                                continue;

                            item.Items.Add(CreateTreeItem(dir));
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Optional: add a placeholder item that says "Access Denied"
                            var accessDeniedItem = new TreeViewItem
                            {
                                Header = "[Access Denied]",
                                IsEnabled = false
                            };
                            item.Items.Add(accessDeniedItem);
                        }
                        catch (Exception ex)
                        {
                            // Optional: log other exceptions
                            Console.WriteLine($"Error reading directory '{dir}': {ex.Message}");
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Optional: Display access denied on the root node itself
                    var accessDeniedItem = new TreeViewItem
                    {
                        Header = "[Access Denied]",
                        IsEnabled = false
                    };
                    item.Items.Add(accessDeniedItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error expanding folder '{item.Tag}': {ex.Message}");
                }
            }
        }

        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = treeView.SelectedItem as TreeViewItem;
            if (selected != null)
            {
                string selectedPath = selected.Tag.ToString();
                // Do something with selectedPath if needed
                Console.WriteLine($"Selected folder: {selectedPath}");
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