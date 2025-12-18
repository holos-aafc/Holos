#region Imports

using System;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace H.Infrastructure
{
    /// <summary>
    /// </summary>
    public static class EnumHelper
    {
        public static Dictionary<object, string> Dictionary { get; set; } = new Dictionary<object, string>();

        #region Public Methods

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            if (Dictionary.ContainsKey(enumerationValue)) return Dictionary[enumerationValue];

            var type = enumerationValue.GetType();
            if (!type.IsEnum)
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type",
                    nameof(enumerationValue));

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    var result = ((DescriptionAttribute)attrs[0]).Description;

                    if (Dictionary.ContainsKey(enumerationValue) == false) Dictionary.Add(enumerationValue, result);

                    return result;
                }
            }

            return enumerationValue.ToString();
        }

        #endregion
    }
}