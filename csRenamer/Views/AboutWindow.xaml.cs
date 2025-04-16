using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            MessageBox.Show(aboutText, "About csRenamer", MessageBoxButton.OK);
        }
    }
}
