using H.Core.Calculators.Infrastructure;
using H.Core.Models.Animals.Beef;
using H.Core.Models;
using H.Core.Providers.Feed;
using H.Core.Providers;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using H.Core.Services;
using H.Helpers;
using H.Views.ComponentViews.Beef.Backgrounders;
using H.Views.ComponentViews.Beef.CowCalf;
using H.Views.ComponentViews.Beef.Finishers;
using H.Views.DetailViews.Animals.BeefCattle.CowCalf;
using H.Views.ResultViews.DetailedEmissionReport;
using H.Views.SupportingViews.Menu;
using H.Views.TimelineViews.Animals.BeefCattle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Animals.Table_69;

namespace H.Core.Test.Providers.Animals.Table_69
{
    [TestClass]
    public class Table_69_Provider_Test
    {
        #region Fields
        
        private IVolatilizationFractionsFromLandAppliedManureProvider _sut; 

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
            _sut = new Table_69_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void GetDataReturnsZeroWhenIncorrectAnimalTypeIsUsed()
        {
            var result = _sut.GetData(AnimalType.Swine, Province.Alberta, 2000);

            Assert.AreEqual(0, result.ImpliedEmissionFactor);
        }

        [TestMethod]
        public void GetDataReturnsNonZeroWhenForProvinceAndYear()
        {
            var result = _sut.GetData(AnimalType.Dairy, Province.Quebec, 2017);

            Assert.AreEqual(0.15, result.ImpliedEmissionFactor);
        }
    }
}
