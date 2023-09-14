using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace H.Avalonia.Infrastructure
{
    /// <summary>
    /// A converter that converts an enum to a more readable format.
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        #region Private Methods

        private string GetEnumDescription(Enum enumObj)
        {
            var fieldInfo = enumObj.GetType()
                .GetField(enumObj.ToString());

            var attributes = fieldInfo.GetCustomAttributes(false);

            if (attributes.Length == 0)
            {
                return enumObj.ToString();
            }

            var attribute = attributes[0] as DescriptionAttribute;
            if (attribute != null)
            {
                return attribute.Description;
            }

            return string.Empty;
        }
        #endregion


        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumeration)
            {
                var description = this.GetEnumDescription(enumeration);

                return description;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion


    }
}