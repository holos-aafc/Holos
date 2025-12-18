using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace H.Infrastructure.Controls.ValueConverters
{
    public class EmptyStringToBooleanConverter : IValueConverter
    {
        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value))
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}