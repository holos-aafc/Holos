using H.Core.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers
{
    [TestClass]
    public class ProviderBaseTest
    {
        #region Fields

        private TestProvider _provider;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new TestProvider();
        }

        #endregion

        #region Tests

        [TestMethod]
        public void IsBlankReturnsTrueWhenEveryCellIsEmptyOrWhitespace()
        {
            Assert.IsTrue(_provider.IsBlank(new[] {string.Empty}));
            Assert.IsTrue(_provider.IsBlank(new[] {"   "}));
            Assert.IsTrue(_provider.IsBlank(new[] {string.Empty, string.Empty, "  ", string.Empty}));
        }

        /// <summary>
        /// A spacer row in a csv splits into a run of empty cells, one per comma.
        /// </summary>
        [TestMethod]
        public void IsBlankReturnsTrueForASpacerRow()
        {
            Assert.IsTrue(_provider.IsBlank(",,,,,,".Split(',')));
        }

        [TestMethod]
        public void IsBlankReturnsTrueForANullOrEmptyRow()
        {
            Assert.IsTrue(_provider.IsBlank(null));
            Assert.IsTrue(_provider.IsBlank(new string[] { }));
        }

        [TestMethod]
        public void IsBlankReturnsFalseWhenAnyCellHasContent()
        {
            Assert.IsFalse(_provider.IsBlank(new[] {"Soybeans", "55"}));
            Assert.IsFalse(_provider.IsBlank(new[] {string.Empty, "55", string.Empty}));
            Assert.IsFalse(_provider.IsBlank(new[] {"0"}));
        }

        /// <summary>
        /// A comment is not blank; a reader skipping both has to ask each question separately.
        /// </summary>
        [TestMethod]
        public void IsBlankAndIsCommentAreIndependent()
        {
            var comment = new[] {"# Source: Karimi et al. (2020)"};

            Assert.IsFalse(_provider.IsBlank(comment));
            Assert.IsTrue(_provider.IsComment(comment));
        }

        [TestMethod]
        public void IsCommentReturnsTrueWhenTheFirstCellStartsWithTheMarker()
        {
            Assert.IsTrue(_provider.IsComment(new[] {"# Source: Karimi et al. (2020)"}));
            Assert.IsTrue(_provider.IsComment(new[] {"#"}));
        }

        /// <summary>
        /// Comment blocks are often indented to line up under a heading.
        /// </summary>
        [TestMethod]
        public void IsCommentIgnoresLeadingWhitespace()
        {
            Assert.IsTrue(_provider.IsComment(new[] {"   # indented continuation of a comment"}));
        }

        /// <summary>
        /// A comment holding commas is split into several cells, so only the first one can be inspected.
        /// </summary>
        [TestMethod]
        public void IsCommentReturnsTrueWhenTheCommentWasSplitOnItsCommas()
        {
            Assert.IsTrue(_provider.IsComment(new[] {"# Karimi", " R.", " Pogue", " S.J."}));
        }

        [TestMethod]
        public void IsCommentReturnsFalseForADataRow()
        {
            Assert.IsFalse(_provider.IsComment(new[] {"Soybeans", "55", "Karimi et al. (2020)"}));
        }

        /// <summary>
        /// A '#' anywhere but the start is data, not a marker.
        /// </summary>
        [TestMethod]
        public void IsCommentReturnsFalseWhenTheMarkerIsNotAtTheStart()
        {
            Assert.IsFalse(_provider.IsComment(new[] {"Crop #1", "55"}));
        }

        [TestMethod]
        public void IsCommentReturnsFalseForABlankOrEmptyRow()
        {
            Assert.IsFalse(_provider.IsComment(new[] {string.Empty}));
            Assert.IsFalse(_provider.IsComment(new[] {"   "}));
            Assert.IsFalse(_provider.IsComment(new string[] { }));
            Assert.IsFalse(_provider.IsComment(null));
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// ProviderBase is abstract, so the shared helpers need something concrete to hang off.
        /// </summary>
        private class TestProvider : ProviderBase
        {
        }

        #endregion
    }
}
