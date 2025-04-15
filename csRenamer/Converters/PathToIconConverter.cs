using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace csRenamer.Converters
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
                ? System.Windows.Application.Current.Resources["DriveIcon"]
                : System.Windows.Application.Current.Resources["FolderIcon"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
