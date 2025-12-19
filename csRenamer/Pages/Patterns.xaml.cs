using csRenamer.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;

namespace csRenamer.Pages;
public sealed partial class Patterns : Page
{
    public Patterns()
    {
        InitializeComponent();
        Loaded += Patterns_Loaded;
    }

    private void Patterns_Loaded(object sender, RoutedEventArgs e)
    {
        cbOriginalPattern.ItemsSource = PatternService.LoadPatterns(
    @"Assets\Patterns\original_patterns.txt",
    "original_patterns.txt");

        cbRenamedPattern.ItemsSource = PatternService.LoadPatterns(
            @"Assets\Patterns\renamed_patterns.txt",
            "renamed_patterns.txt");

        cbOriginalPattern.SelectedIndex = 0;
        cbRenamedPattern.SelectedIndex = 0;
    }

    private void btOriginalPatternSave_Click(object sender, RoutedEventArgs e)
    {
        if (PatternService.SavePattern(
        cbOriginalPattern.Text,
        "original_patterns.txt"))
        {
            cbOriginalPattern.ItemsSource =
                PatternService.LoadPatterns(
                    @"Assets\Patterns\original_patterns.txt",
                    "original_patterns.txt");

            cbOriginalPattern.SelectedItem = cbOriginalPattern.Text;
        }
    }

    private void btRenamedPatternSave_Click(object sender, RoutedEventArgs e)
    {
        if (PatternService.SavePattern(
            cbRenamedPattern.Text,
            "renamed_patterns.txt"))
        {
            cbRenamedPattern.ItemsSource =
                PatternService.LoadPatterns(
                    @"Assets\Patterns\renamed_patterns.txt",
                    "renamed_patterns.txt");

            cbRenamedPattern.SelectedItem = cbRenamedPattern.Text;
        }
    }

    private async Task EditUserPatternsAsync(
    string fileName,
    Action refreshCallback)
    {
        var textBox = new TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 220,
            FontFamily = new FontFamily("Cascadia Mono"),
            FontSize = 14,
            Text = PatternService.LoadUserPatternsRaw(fileName)
        };

        var dialog = new ContentDialog
        {
            Title = "Edit custom patterns",
            Content = textBox,
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            PatternService.SaveUserPatternsRaw(fileName, textBox.Text);
            refreshCallback?.Invoke();
        }
    }

    private async void btOriginalPatternEdit_Click(object sender, RoutedEventArgs e)
    {
        await EditUserPatternsAsync(
        "original_patterns.txt",
        () =>
        {
            cbOriginalPattern.ItemsSource =
                PatternService.LoadPatterns(
                    @"Assets\Patterns\original_patterns.txt",
                    "original_patterns.txt");

            cbOriginalPattern.SelectedIndex = 0;
        });
    }

    private async void btRenamedPatternEdit_Click(object sender, RoutedEventArgs e)
    {
        await EditUserPatternsAsync(
            "renamed_patterns.txt",
        () =>
        {
            cbRenamedPattern.ItemsSource =
                PatternService.LoadPatterns(
                    @"Assets\Patterns\renamed_patterns.txt",
                    "renamed_patterns.txt");
            cbRenamedPattern.SelectedIndex = 0;
        });
    }
}
