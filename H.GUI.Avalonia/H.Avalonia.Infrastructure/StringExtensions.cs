using System.Text.RegularExpressions;

namespace H.Avalonia.Infrastructure;

public static class StringExtensions
{
    /// <summary>
    /// Removes unnecessary characters from a string and converts it into a standard format for the application.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ConvertToImportFormat(this string str)
    {
        var result = Regex.Replace(str, @"[\d-]", string.Empty)
            .ToLower()
            .Replace(" ", "_");
        return result;
    }
}