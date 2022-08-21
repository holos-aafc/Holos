using System;


namespace H.Infrastructure
{
    
    public static class StringHelper
    {
        /// <summary>
        /// <para>Parses a string until the location of the first occurrence of the specified character. By default, this method
        /// looks for the character ( .</para>
        /// </summary>
        /// <param name="parseUntil">The character to parse the string until. By default this character is (</param>
        /// <returns>Returns a new string after parsing until a specific character. If the specified character doesn't exist inside the original string, the method returns the original(default) string.</returns>
        public static string ParseUntilOrDefault(this string text, string parseUntil = "(")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int characterLocation = text.IndexOf(parseUntil, StringComparison.Ordinal);

                if (characterLocation > 0)
                {
                    return text.Substring(0, characterLocation);
                }
            }

            return text;
        }
    }
}