using System;
using System.Globalization;
using System.Threading;
using H.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Infrastructure.Test
{
    /// <summary>
    /// Edge-case coverage for <see cref="CoordinateParser"/> - the latitude/longitude parsing used by the "Enter
    /// Coordinates" dialog (issue #340).
    /// </summary>
    [TestClass]
    public class CoordinateParserTest
    {
        private const double Delta = 0.0000001;

        #region Valid input

        [TestMethod]
        public void TryParseLatitude_ValidDecimal_ReturnsTrueWithValue()
        {
            var result = CoordinateParser.TryParseLatitude("52.30845", out var latitude);

            Assert.IsTrue(result);
            Assert.AreEqual(52.30845, latitude, Delta);
        }

        [TestMethod]
        public void TryParseLongitude_ValidNegativeDecimal_ReturnsTrueWithValue()
        {
            var result = CoordinateParser.TryParseLongitude("-112.51918", out var longitude);

            Assert.IsTrue(result);
            Assert.AreEqual(-112.51918, longitude, Delta);
        }

        /// <summary>
        /// The exact example values shown in the dialog. This is the pair that previously failed validation, so it
        /// guards against a regression of that bug.
        /// </summary>
        [TestMethod]
        public void TryParse_DialogExampleValues_Succeed()
        {
            Assert.IsTrue(CoordinateParser.TryParseLatitude("52.30845", out var latitude));
            Assert.IsTrue(CoordinateParser.TryParseLongitude("-112.51918", out var longitude));
            Assert.AreEqual(52.30845, latitude, Delta);
            Assert.AreEqual(-112.51918, longitude, Delta);
        }

        [TestMethod]
        public void TryParseCoordinate_LeadingPlusSign_ReturnsTrue()
        {
            var result = CoordinateParser.TryParseCoordinate("+52.3", out var value);

            Assert.IsTrue(result);
            Assert.AreEqual(52.3, value, Delta);
        }

        [TestMethod]
        public void TryParseCoordinate_SurroundingWhitespace_IsTrimmed()
        {
            var result = CoordinateParser.TryParseCoordinate("   52.3   ", out var value);

            Assert.IsTrue(result);
            Assert.AreEqual(52.3, value, Delta);
        }

        [TestMethod]
        public void TryParseCoordinate_Integer_ReturnsTrue()
        {
            var result = CoordinateParser.TryParseCoordinate("52", out var value);

            Assert.IsTrue(result);
            Assert.AreEqual(52.0, value, Delta);
        }

        [TestMethod]
        public void TryParseCoordinate_Zero_ReturnsTrue()
        {
            var result = CoordinateParser.TryParseCoordinate("0", out var value);

            Assert.IsTrue(result);
            Assert.AreEqual(0.0, value, Delta);
        }

        #endregion

        #region Malformed input

        [TestMethod]
        public void TryParseCoordinate_Null_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseCoordinate(null, out var value));
            Assert.AreEqual(0.0, value, Delta);
        }

        [TestMethod]
        public void TryParseCoordinate_EmptyString_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseCoordinate(string.Empty, out _));
        }

        [TestMethod]
        public void TryParseCoordinate_WhitespaceOnly_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseCoordinate("     ", out _));
        }

        [TestMethod]
        public void TryParseCoordinate_NonNumericText_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseCoordinate("north", out _));
        }

        [TestMethod]
        public void TryParseCoordinate_MultipleDecimalPoints_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseCoordinate("52.30.845", out _));
        }

        [TestMethod]
        public void TryParseCoordinate_ExponentNotation_ReturnsFalse()
        {
            // Coordinates are plain decimals; scientific notation is intentionally rejected.
            Assert.IsFalse(CoordinateParser.TryParseCoordinate("5e1", out _));
        }

        [TestMethod]
        public void TryParseCoordinate_ThousandsSeparator_ReturnsFalse()
        {
            // A comma is a decimal separator in some locales, so pin the culture to one where it would be a thousands
            // separator and confirm it is rejected (thousands separators are not permitted).
            RunInCulture("en-US", () =>
                Assert.IsFalse(CoordinateParser.TryParseCoordinate("1,234", out _)));
        }

        #endregion

        #region Range validation

        [TestMethod]
        public void TryParseLatitude_AboveMaximum_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseLatitude("90.1", out _));
        }

        [TestMethod]
        public void TryParseLatitude_BelowMinimum_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseLatitude("-90.1", out _));
        }

        [TestMethod]
        public void TryParseLatitude_Boundaries_ReturnTrue()
        {
            Assert.IsTrue(CoordinateParser.TryParseLatitude("90", out var high));
            Assert.IsTrue(CoordinateParser.TryParseLatitude("-90", out var low));
            Assert.AreEqual(90.0, high, Delta);
            Assert.AreEqual(-90.0, low, Delta);
        }

        [TestMethod]
        public void TryParseLongitude_AboveMaximum_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseLongitude("180.1", out _));
        }

        [TestMethod]
        public void TryParseLongitude_BelowMinimum_ReturnsFalse()
        {
            Assert.IsFalse(CoordinateParser.TryParseLongitude("-180.1", out _));
        }

        [TestMethod]
        public void TryParseLongitude_Boundaries_ReturnTrue()
        {
            Assert.IsTrue(CoordinateParser.TryParseLongitude("180", out var high));
            Assert.IsTrue(CoordinateParser.TryParseLongitude("-180", out var low));
            Assert.AreEqual(180.0, high, Delta);
            Assert.AreEqual(-180.0, low, Delta);
        }

        [TestMethod]
        public void TryParseLatitude_ValueValidOnlyAsLongitude_ReturnsFalse()
        {
            // 150 is a valid longitude but out of range for a latitude.
            Assert.IsFalse(CoordinateParser.TryParseLatitude("150", out _));
            Assert.IsTrue(CoordinateParser.TryParseLongitude("150", out _));
        }

        [TestMethod]
        public void TryParseLatitude_OutOfRange_ResetsOutValueToZero()
        {
            CoordinateParser.TryParseLatitude("500", out var value);
            Assert.AreEqual(0.0, value, Delta);
        }

        #endregion

        #region Culture handling

        [TestMethod]
        public void TryParseLatitude_DotDecimalUnderCommaCulture_StillParses()
        {
            // GPS values use a dot separator regardless of locale; the invariant culture is tried first.
            RunInCulture("fr-CA", () =>
            {
                Assert.IsTrue(CoordinateParser.TryParseLatitude("52.30845", out var latitude));
                Assert.AreEqual(52.30845, latitude, Delta);
            });
        }

        [TestMethod]
        public void TryParseLatitude_CommaDecimalUnderCommaCulture_Parses()
        {
            RunInCulture("fr-CA", () =>
            {
                Assert.IsTrue(CoordinateParser.TryParseLatitude("52,30845", out var latitude));
                Assert.AreEqual(52.30845, latitude, Delta);
            });
        }

        [TestMethod]
        public void TryParseLongitude_CommaDecimalNegativeUnderCommaCulture_Parses()
        {
            RunInCulture("fr-CA", () =>
            {
                Assert.IsTrue(CoordinateParser.TryParseLongitude("-112,51918", out var longitude));
                Assert.AreEqual(-112.51918, longitude, Delta);
            });
        }

        #endregion

        private static void RunInCulture(string cultureName, Action action)
        {
            var originalCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }
        }
    }
}
