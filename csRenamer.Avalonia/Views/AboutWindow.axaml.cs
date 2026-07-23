using System.Diagnostics;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using csRenamer.Avalonia.Resources;

namespace csRenamer.Avalonia.Views;

public partial class AboutWindow : Window
{
    public AboutWindow(Window owner)
    {
        InitializeComponent();
        Owner = owner;

        var version = Assembly.GetExecutingAssembly()
                          .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                          ?.InformationalVersion ?? "Unknown";

        var parts = version.Split('+');
        versionText.Text = $"ver. {parts[0]}";
        commitText.Text = parts.Length > 1 ? $"commit {parts[1]}" : "";
    }

    private void Issue_Click(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/MawCeron/csRenamer/issues") { UseShellExecute = true });
    }

    private void Credits_Click(object? sender, RoutedEventArgs e)
    {
        ShowInfo("About csRenamer - Credits", Strings.Credits);
    }

    private void License_Click(object? sender, RoutedEventArgs e)
    {
        ShowInfo("About csRenamer - License", Strings.License);
    }

    private async void ShowInfo(string title, string message)
    {
        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap,
            FontSize = 12,
            Margin = new global::Avalonia.Thickness(12)
        };

        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new global::Avalonia.Thickness(0, 0, 0, 12),
            Padding = new global::Avalonia.Thickness(24, 6),
        };

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("*,Auto"),
            Children = { new ScrollViewer { Content = textBlock }, okButton }
        };
        Grid.SetRow(okButton, 1);

        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 380,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = grid
        };

        okButton.Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(this);
    }
}
