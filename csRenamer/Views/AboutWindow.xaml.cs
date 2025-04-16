using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace csRenamer.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var version = Assembly.GetExecutingAssembly()
                              .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                              ?.InformationalVersion ?? "Unknown";

            versionText.Text = $"ver. {version.Split('+')[0]}";
            commitText.Text = $"commit {version.Split('+')[1]}";
            Debug.WriteLine($"Version: {version}");
        }

        private void Issue_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/MawCeron/csRenamer/issues") { UseShellExecute = true });
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            string aboutText = @"csRenamer - v0.1.0

Copyright © 2025 Mauricio Cerón Medina. All rights reserved.

=========================

Visual Resources:

Button Icons:
Copyright © 2021 Kamila Graf, MIT License, Repository: https://github.com/kamilagraf/react-swm-icon-pack  

GitHub Icon:
Copyright © 2021 Javis V. Pérez, Apache License 2.0, Repository: https://github.com/javisperez/toe-icons/

==========================

Inspiration:

csRenamer is inspired by pyRenamer and its functionalities.

pyRenamer Authors:  
Copyright © 2016 Thomas Freeman <tfree87@users.noreply.github.com>  
Copyright © 2006-2008 Adolfo González Blázquez <code@infinicode.org>

Licensed under the GNU General Public License v2.0.  
Repository: https://github.com/tfree87/pyRenamer/";

            MessageBox.Show(aboutText, "About csRenamer - Credits", MessageBoxButton.OK);
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            string aboutText = @"MIT License

Copyright © 2025 Mauricio Cerón Medina

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";

            MessageBox.Show(aboutText, "About csRenamer - License", MessageBoxButton.OK);
        }
    }
}
