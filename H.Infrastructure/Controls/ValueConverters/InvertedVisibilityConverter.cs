using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace H.Infrastructure.Controls.ValueConverters
{    
    public class InvertedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var result = (Visibility) value;
                if (result == Visibility.Collapsed || result == Visibility.Hidden)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}