#region Imports

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertableBooleanConverter : IValueConverter
    {
        #region Fields

        private enum Parameters
        {
            Normal,
            Inverted
        }

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value != null && (bool) value;
            if (parameter != null)
            {
                var direction = (Parameters) Enum.Parse(typeof(Parameters), (string) parameter);
                if (direction == Parameters.Inverted)
                {
                    return !result;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}