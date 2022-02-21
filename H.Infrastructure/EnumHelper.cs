#region Imports

using System;
using System.ComponentModel;

#endregion

namespace H.Infrastructure
{
    /// <summary>
    /// </summary>
    public static class EnumHelper
    {
        #region Public Methods

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type",
                                            nameof(enumerationValue));
            }

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }

            return enumerationValue.ToString();
        }

        #endregion

        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}