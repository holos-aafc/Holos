using System.ComponentModel;

namespace H.Avalonia.Infrastructure
{
    /// <summary>
    /// A class that includes method extensions for enumerations in the application.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Get the description of the enum as indicated in the Enum class file. For description to be shown, a
        /// matching resource file entry is needed for the enumeration.
        /// </summary>
        /// <returns>A string representing the description of the enum based on its Resource entry.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the item isn't an enumeration.</exception>
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
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return enumerationValue.ToString();
        }
    }
}
