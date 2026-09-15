using H.CLI.TemporaryComponentStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.CLI.Test.TemporaryComponentStorage
{
    /// <summary>
    /// These pin the end-of-input behaviour. Console.ReadLine returns null once there is nothing left to read, which
    /// is the normal state whenever the CLI is not attached to a terminal - a script, a scheduled job, a build
    /// pipeline. These predicates used to call input.Equals on that null and throw out of Main, after the run had
    /// already finished and written its results.
    /// </summary>
    [TestClass]
    public class InputHelperTest
    {
        private readonly InputHelper _sut = new InputHelper();

        #region End of input

        [TestMethod]
        public void IsYesResponseReturnsFalseForEndOfInput()
        {
            Assert.IsFalse(_sut.IsYesResponse(null));
            Assert.IsFalse(_sut.IsYesResponse(string.Empty));
            Assert.IsFalse(_sut.IsYesResponse("   "));
        }

        [TestMethod]
        public void IsNoResponseReturnsFalseForEndOfInput()
        {
            Assert.IsFalse(_sut.IsNoResponse(null));
            Assert.IsFalse(_sut.IsNoResponse(string.Empty));
            Assert.IsFalse(_sut.IsNoResponse("   "));
        }

        [TestMethod]
        public void IsNotApplicableInputReturnsFalseForEndOfInput()
        {
            Assert.IsFalse(_sut.IsNotApplicableInput(null));
            Assert.IsFalse(_sut.IsNotApplicableInput(string.Empty));
            Assert.IsFalse(_sut.IsNotApplicableInput("   "));
        }

        /// <summary>
        /// A null is neither a yes nor a no. The prompt sites rely on that to tell "nobody answered" apart from
        /// "the user said no", so they can take a default rather than asking again.
        /// </summary>
        [TestMethod]
        public void EndOfInputIsNeitherYesNorNo()
        {
            Assert.IsFalse(_sut.IsYesResponse(null));
            Assert.IsFalse(_sut.IsNoResponse(null));
        }

        #endregion

        #region Real answers still work

        [TestMethod]
        public void IsYesResponseAcceptsTheAnswersItAlwaysHas()
        {
            Assert.IsTrue(_sut.IsYesResponse("y"));
            Assert.IsTrue(_sut.IsYesResponse("Y"));
            Assert.IsTrue(_sut.IsYesResponse("o"));  // French "Oui"
            Assert.IsTrue(_sut.IsYesResponse(H.CLI.Properties.Resources.LabelYes));
        }

        [TestMethod]
        public void IsNoResponseAcceptsTheAnswersItAlwaysHas()
        {
            Assert.IsTrue(_sut.IsNoResponse("n"));
            Assert.IsTrue(_sut.IsNoResponse("N"));
            Assert.IsTrue(_sut.IsNoResponse(H.CLI.Properties.Resources.LabelNo));
        }

        [TestMethod]
        public void IsNotApplicableInputAcceptsTheAnswersItAlwaysHas()
        {
            Assert.IsTrue(_sut.IsNotApplicableInput("N/A"));
            Assert.IsTrue(_sut.IsNotApplicableInput("n/a"));
        }

        [TestMethod]
        public void UnrecognizedAnswersAreNeitherYesNorNo()
        {
            Assert.IsFalse(_sut.IsYesResponse("maybe"));
            Assert.IsFalse(_sut.IsNoResponse("maybe"));
        }

        #endregion
    }
}
