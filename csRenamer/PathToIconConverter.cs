using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace csRenamer
{
    public class PathToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;

            if (string.IsNullOrEmpty(path))
                return null;

            // Drives have no parent
            bool isDrive = Directory.GetParent(path) == null;

            // Return the correct resource
            return isDrive
                ? App.Current.Resources["DriveIcon"]
                : App.Current.Resources["FolderIcon"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
