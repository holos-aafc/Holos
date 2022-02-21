#region Imports

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// </summary>
    public class EmptyCollectionToVisibilityConverter : IValueConverter
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
            var collection = (IEnumerable<object>) value;
            if (collection != null)
            {
                if (collection.Any())
                {
                    return Visibility.Visible;
                }

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

        #region Implementation of IValueConverter

        #endregion
    }
}