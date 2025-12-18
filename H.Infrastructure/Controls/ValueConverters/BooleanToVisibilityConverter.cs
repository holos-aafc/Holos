#region Imports

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = bool.Parse(value.ToString());
            if (param)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}