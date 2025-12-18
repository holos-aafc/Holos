using System;
using System.Globalization;

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// https://stackoverflow.com/questions/9977344/grid-column-with-sharedsizegroup-does-not-reclaim-the-size-when-it-is-collapse
    /// </summary>
    public class VisibilityToSharedSizeGroupConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (((System.Windows.Visibility)value) == System.Windows.Visibility.Visible) ? parameter : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}