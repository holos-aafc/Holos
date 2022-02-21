#region Imports

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// Converts a boolean value to a visibility. Can be inverted if a converter parameter is used.
    /// </summary>
    public class InvertibleBooleanToVisibilityConverter : IValueConverter
    {
        #region Properties

        private enum Parameters
        {
            Normal,
            Inverted
        }

        #endregion

        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value != null && value is bool && (bool) value;
            if (parameter != null)
            {
                var direction = (Parameters) Enum.Parse(typeof(Parameters), (string) parameter);
                if (direction == Parameters.Inverted)
                {
                    if (result == false)
                    {
                        return Visibility.Visible;
                    }

                    return Visibility.Collapsed;
                }
            }

            if (result == false)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
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