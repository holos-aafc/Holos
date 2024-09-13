using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Calculators.Carbon;
using H.Core.Models;

namespace H.Core.Test.Calculators.Carbon
{
    [TestClass]
    public class IPCCTier2CarbonInputCalculatorTest
    {
        #region Fields

        private IPCCTier2CarbonInputCalculator _sut = new IPCCTier2CarbonInputCalculator();

        #endregion

        #region Tests

        [TestMethod]
        public void CalculateAboveGroundResidueDryMatterExportedTest()
        {
            const double freshWeightOfYield = 1000;
            const double harvestRatio = 1;
            const double moistureContentOfCropAsPercentage = 0;
            const double percentageofStrawReturned = 50;

            var result = _sut.CalculateAboveGroundResidueDryMatterExported(
                freshWeightOfYield: freshWeightOfYield,
                harvestRatio: harvestRatio,
                moistureContentOfCropAsPercentage: moistureContentOfCropAsPercentage,
                percentageOfStrawReturned: percentageofStrawReturned);

            Assert.AreEqual(500, result);
        }

        [TestMethod]
        public void CalculateAboveGroundResidueDryMatterExportedTestAllStraw()
        {
            const double freshWeightOfYield = 1000;
            const double harvestRatio = 1;
            const double moistureContentOfCropAsPercentage = 0;
            const double percentageofStrawReturned = 100;

            var result = _sut.CalculateAboveGroundResidueDryMatterExported(
                freshWeightOfYield: freshWeightOfYield,
                harvestRatio: harvestRatio,
                moistureContentOfCropAsPercentage: moistureContentOfCropAsPercentage,
                percentageOfStrawReturned: percentageofStrawReturned);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateAboveGroundResidueDryMatterExportedTestNoStraw()
        {
            const double freshWeightOfYield = 1000;
            const double harvestRatio = 1;
            const double moistureContentOfCropAsPercentage = 0;
            const double percentageofStrawReturned = 0;

            var result = _sut.CalculateAboveGroundResidueDryMatterExported(
                freshWeightOfYield: freshWeightOfYield,
                harvestRatio: harvestRatio,
                moistureContentOfCropAsPercentage: moistureContentOfCropAsPercentage,
                percentageOfStrawReturned: percentageofStrawReturned);

            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void CanCalculateInputsForCropReturnsTrue()
        {
            var result = _sut.CanCalculateInputsForCrop(new CropViewItem() {CropType = CropType.Barley});

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanCalculateInputsForCropReturnsFalse()
        {
            var result = _sut.CanCalculateInputsForCrop(new CropViewItem() {CropType = CropType.TameMixed});

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CalculateInputs()
        {
            var viewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                Yield = 7018,
                PercentageOfStrawReturnedToSoil = 100,
                MoistureContentOfCrop = 0.12,
                BiomassCoefficientStraw = 0.2,
                BiomassCoefficientProduct = 0.6,
            };

            _sut.CalculateInputsForCrop(
                viewItem: viewItem, farm: new Farm());

            Assert.AreEqual(2157.45888556357, viewItem.AboveGroundCarbonInput, 1);
            Assert.AreEqual(997.775453968351, viewItem.BelowGroundCarbonInput, 1);
            Assert.AreEqual(3155.23433953193, viewItem.TotalCarbonInputs, 1);
        }

        #endregion
    }
}
