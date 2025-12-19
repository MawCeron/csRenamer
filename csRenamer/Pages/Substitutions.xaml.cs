using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace csRenamer.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Substitutions : Page
{
    public Substitutions()
    {
        InitializeComponent();
    }

    private void Check_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox? checkbox = sender as CheckBox;
        if (checkbox == null) return; 

        bool status = checkbox.IsChecked ?? false;

        switch (checkbox.Name)
        {
            case "spacesCheck":
                spacesCombo.IsEnabled = status;
                break;
            case "replaceCheck":
                replaceText.IsEnabled = status;
                replaceWithText.IsEnabled = status;
                break;
            case "capitalizationCheck":
                capitalizationCombo.IsEnabled = status;
                break;
            default:
                break;
        }
    }
}
