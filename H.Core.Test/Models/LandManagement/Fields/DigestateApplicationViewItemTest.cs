using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Calculators.Carbon;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models.LandManagement.Fields
{
    [TestClass]
    public class DigestateApplicationViewItemTest
    {
        #region Fields

        private DigestateApplicationViewItem _sut;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new DigestateApplicationViewItem();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void AttemptedToGoOverMaximumReturnsFalse()
        {
            _sut.MaximumAmountOfDigestateAvailablePerHectare = 100;
            _sut.AmountAppliedPerHectare = 50;

            Assert.IsFalse(_sut.AttemptedToGoOverMaximum);
        }

        [TestMethod]
        public void AttemptedToGoOverMaximumReturnsTrue()
        {
            //_sut.MaximumAmountOfDigestateAvailablePerHectare = 100;
            //_sut.AmountAppliedPerHectare = 500;

            //Assert.IsTrue(_sut.AttemptedToGoOverMaximum);
        }

        #endregion
    }
}
