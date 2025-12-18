using System;
using System.Globalization;
using System.Windows.Data;

namespace H.Infrastructure.Controls.ValueConverters
{
    public class ToLowerStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string s)
            {
                return s.ToLowerInvariant();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}