using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace csRenamer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InsertDelete : Page
    {
        public InsertDelete()
        {
            InitializeComponent();
        }

        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton? radio = sender as RadioButton;
            if (radio == null) return;

            bool isChecked = radio.IsChecked ?? false;

            switch (radio.Name)
            {
                case "insertRadio":                    
                    insertTextBox.IsEnabled = isChecked;
                    insertPositionBox.IsEnabled = isChecked;
                    atEndCheck.IsEnabled = isChecked;
                    break;
                case "deleteRadio":
                    deletePositionBox.IsEnabled = isChecked;
                    deleteToPositionBox.IsEnabled = isChecked;
                    break;
            }
        }
    }
}
