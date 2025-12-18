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
    public class NullToVisibilityConverter : IValueConverter
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
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}