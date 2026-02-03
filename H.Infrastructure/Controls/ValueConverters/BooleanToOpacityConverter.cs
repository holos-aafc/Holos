#region Imports

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// Converts a boolean value to an opacity value.
    /// True = 1.0 (fully opaque), False = 0.5 (semi-transparent/greyed out)
    /// </summary>
    public class BooleanToOpacityConverter : IValueConverter
    {
        /// <summary>
        /// Opacity value when the boolean is true (default: 1.0 - fully opaque)
        /// </summary>
        public double TrueOpacity { get; set; } = 1.0;

        /// <summary>
        /// Opacity value when the boolean is false (default: 0.5 - greyed out)
        /// </summary>
        public double FalseOpacity { get; set; } = 0.5;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueOpacity : FalseOpacity;
            }

            return TrueOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
