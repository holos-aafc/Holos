#region Imports

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// </summary>
    public class BooleanToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = bool.Parse(value.ToString());
            if (param)
            {
                return new GridLength(1, GridUnitType.Star);
            }
            return new GridLength(1, GridUnitType.Auto);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}