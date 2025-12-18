#region Imports

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// </summary>
    public class ObjectToTypeStringConverter : IValueConverter
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
            if (value != null)
            {
                var name = value.GetType();

                return name;
            }

            return null;
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