#region Imports

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using H.Core.Enumerations;

#endregion

namespace H.Core.Converters
{
    /// <summary>
    /// </summary>
    public class ResponseToVisibilityConverter : IValueConverter
    {
        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Response response)
            {
                return response == Response.Yes ? Visibility.Visible : Visibility.Collapsed;
            }

            throw new ArgumentException("Value must be a value from the Reponse enumeration.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion

        #region Implementation of IValueConverter

        #endregion
    }
}