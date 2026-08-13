using System.Globalization;

namespace H.Infrastructure
{
    /// <summary>
    /// Parses latitude/longitude values typed by the user (issue #340). GPS coordinates are conventionally written with
    /// a dot decimal separator regardless of locale, so the invariant culture is tried first and the user's current
    /// culture (for example a comma-decimal locale such as fr-CA) is accepted as a fallback. Values are validated
    /// against the geographic range for a latitude or longitude.
    /// </summary>
    public static class CoordinateParser
    {
        public const double MinimumLatitude = -90.0;
        public const double MaximumLatitude = 90.0;
        public const double MinimumLongitude = -180.0;
        public const double MaximumLongitude = 180.0;

        /// <summary>
        /// Attempts to parse a latitude. Returns <c>true</c> only when the text is a number within [-90, 90].
        /// </summary>
        public static bool TryParseLatitude(string text, out double latitude)
        {
            return TryParseInRange(text, MinimumLatitude, MaximumLatitude, out latitude);
        }

        /// <summary>
        /// Attempts to parse a longitude. Returns <c>true</c> only when the text is a number within [-180, 180].
        /// </summary>
        public static bool TryParseLongitude(string text, out double longitude)
        {
            return TryParseInRange(text, MinimumLongitude, MaximumLongitude, out longitude);
        }

        /// <summary>
        /// Attempts to parse a coordinate as a number, without range validation. Leading/trailing whitespace is
        /// ignored, thousands separators are not permitted, and both the invariant and current cultures are accepted.
        /// </summary>
        public static bool TryParseCoordinate(string text, out double value)
        {
            value = 0.0;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            text = text.Trim();

            const NumberStyles numberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

            return double.TryParse(text, numberStyles, CultureInfo.InvariantCulture, out value) ||
                   double.TryParse(text, numberStyles, CultureInfo.CurrentCulture, out value);
        }

        private static bool TryParseInRange(string text, double minimum, double maximum, out double value)
        {
            if (TryParseCoordinate(text, out value) && value >= minimum && value <= maximum)
            {
                return true;
            }

            value = 0.0;
            return false;
        }
    }
}
