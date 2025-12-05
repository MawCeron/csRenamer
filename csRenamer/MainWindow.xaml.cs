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
    }
}
