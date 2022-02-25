using System.Collections.Generic;
using H.Core.Calculators.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using H.Core.Enumerations;

namespace H.Core.Test.Calculators
{
    [TestClass]    
    public class ShelterbeltCalculatorTest
    {
        #region Initialization

        private ShelterbeltCalculator calc;

        [TestInitialize]
        public void Initialize()
        {
            calc = new ShelterbeltCalculator();
        }

        #endregion

        #region Convenience / Algorithm Document Text (near where hard to decouple from interface)

        /// <summary>
        /// Gets the A value for a species that is present in Beyhan Amichev's paper: Carbon sequestration and growth of six common tree and shrub shelterbelts in Saskatchewan, Canada
        /// </summary>
        [TestMethod]
        public void GetAReturnsCorrectValue()
        {
            TreeSpecies species = TreeSpecies.WhiteSpruce;
            var result = calc.GetA(species);
            Assert.AreEqual(0.0066, result);
        }


        /// <summary>
        /// Gets the B value for a species that is present in Beyhan Amichev's paper: Carbon sequestration and growth of six common tree and shrub shelterbelts in Saskatchewan, Canada
        /// </summary>
        [TestMethod]
        public void GetBReturnsCorrectValue()
        {
            TreeSpecies species = TreeSpecies.GreenAsh;
            var result = calc.GetB(species);
            Assert.AreEqual(2.1217, result);
        }


        /// <summary>
        /// This is used for averaging: Ctree (carbon per tree), and the circumferences output from the tables ONLY. Do not average coefficients. Do not only average final carbon. See the Algorithm Document.
        /// Specifically, use as part of process for creating Average Deciduous and Average Conifer output.
        /// Deciduous : Green Ash and Manitoba Maple
        /// Conifer : Scots Pine and White Spruce
        /// See emails with Beyhan Amichev to discover why.
        /// </summary>
        [TestMethod]
        public void AverageTwoReturnsCorrectValue()
        {
            double one = 273.500;
            double two = 77.250;
            var result = calc.AverageTwo(one, two);
            Assert.AreEqual(175.375, result);
        }

        #endregion

        #region From Algorithm Document Equations

        /// <summary>
        /// Equation 2.3.1-3
        /// </summary>
        [TestMethod]
        public void CalculateCircumferenceFromDiameterReturnsCorrectValue()
        {
            var diameter = 169.500;
            var result = calc.CalculateCircumferenceFromDiameter(diameter);
            Assert.AreEqual(532.499505, result);
        }


        /// <summary>
        ///   NOT IN DOCUMENT. Part of 2.3.1-3
        /// </summary>
        [TestMethod]
        public void CalculateDiameterFromCircumferenceReturnsCorrectValue()
        {
            var circumference = 532.499505;
            var result = calc.CalculateDiameterFromCircumference(circumference);
            Assert.AreEqual(169.500, result);
        }


        /// <summary>
        /// Equation 2.3.1-4
        /// </summary>
        [TestMethod]
        public void CalculateTreeCircumferenceReturnsCorrectValue()
        {
            var circumferences = new List<double> {49.000, 175.625, 90.12};
            var result = calc.CalculateTreeCircumference(circumferences);
            Assert.AreEqual(203.38818801739691, result);
        }


        /// <summary>
        /// Equation 2.3.1-5
        /// </summary>
        [TestMethod]
        public void CalculateAverageCircumferenceReturnsCorrectValue()
        {
            var treeCircumferences = new List<double> {56.000, 65.00};
            var result = calc.CalculateAverageCircumference(treeCircumferences);
            Assert.AreEqual(60.5, result);
        }


        /// <summary>
        /// Equation 2.3.1-6
        /// </summary>
        [TestMethod]
        public void CalculateTreeCountReturnsCorrectValue()
        {
            var rowLength = 207.625;
            var treeSpacing = 124.375;
            var mortality = 0;
            var result = calc.CalculateTreeCount(rowLength, treeSpacing, mortality);
            Assert.AreEqual(1.6693467336683416, result);
        }


        /// <summary>
        ///    Equation 2.3.1-7
        /// </summary>
        [TestMethod]
        public void CalculateTreeSpacingReturnsCorrectValue()
        {
            var rowLength = 139.250;
            var treeCount = 14.000;
            var result = calc.CalculateTreeSpacing(rowLength, treeCount);
            Assert.AreEqual(9.9464285714285712, result);
        }


        /// <summary>
        ///    Equation 2.3.1-8
        /// </summary>
        [TestMethod]
        public void CalculateCarbonForTreetypeReturnsCorrectValue()
        {
            var aboveGroundCarbonStocksPerTree = 9.375;
            var treeCount = 113.250;
            var result = calc.CalculateCarbonForTreetype(aboveGroundCarbonStocksPerTree, treeCount);
            Assert.AreEqual(1061.71875, result);
        }

        /// <summary>
        /// Equation 2.3.1-9
        /// </summary>
        [TestMethod]
        public void CalculateCarbonForLinearPlantingReturnsCorrectValue()
        {
            var aboveGroundCarbonForTreetypesInPlanting =
                new List<double> {60.625, 198.250, 145.875, 68.375, 142.500, 203.875, 19.00};
            var result = calc.CalculateCarbonForLinearPlanting(aboveGroundCarbonForTreetypesInPlanting);
            Assert.AreEqual(838.5, result);
        }


        /// <summary>
        /// Equation 2.3.2-1
        /// </summary>
        [TestMethod]
        public void CalculatePercentMortalityOfALinearPlantingReturnsCorrectValue()
        {
            List<double> plantedTreeCountAllSpecies = new List<double> { 276.750, 71.375, 82.000, 144.500, 41.250, 12.375, 156.750, 301.375, 161.87 };
            List<double> liveTreeCountAllSpecies = new List<double> { 42.625, 3.125, 204.750, 4.125, 89.375, 88.625, 108.000, 79.375, 184.50 };
            var result = calc.CalculatePercentMortalityOfALinearPlanting(plantedTreeCountAllSpecies, liveTreeCountAllSpecies);
            Assert.AreEqual(100.0 * (plantedTreeCountAllSpecies.Sum() - liveTreeCountAllSpecies.Sum()) / plantedTreeCountAllSpecies.Sum(), result, 0.0001);
        }


        /// <summary>
        /// Equation 2.3.2-2
        /// </summary>
        [TestMethod]
        public void CalculateMortalityLowReturnsCorrectValue()
        {
            var result = calc.CalculateMortalityLow(70);
            Assert.AreEqual(30, result);
            result = calc.CalculateMortalityLow(29.999);
            Assert.AreEqual(15, result);
            result = calc.CalculateMortalityLow(14.999);
            Assert.AreEqual(0, result);
            result = calc.CalculateMortalityLow(0);
            Assert.AreEqual(0, result);
        }


        /// <summary>
        /// Equation 2.3.2-3
        /// </summary>
        [TestMethod]
        public void CalculateMortalityHighReturnsCorrectValue()
        {
            var result = calc.CalculateMortalityHigh(0);
            Assert.AreEqual(15, result);
            result = calc.CalculateMortalityHigh(15);
            Assert.AreEqual(30, result);
            result = calc.CalculateMortalityHigh(30);
            Assert.AreEqual(50, result);
            try
            {
                calc.CalculateMortalityHigh(14);
                Assert.Fail();
            } catch(Exception)
            {
                //pass
            }
        }

        /// <summary>
        ///    Equation 2.3.3-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonForShelterbeltReturnsCorrectValue()
        {
            var aboveGroundCarbonForLinearPlantingsInShelterbelt =
                new List<double> {120.000, 142.375, 72.750, 164.750, 284.375, 238.375};
            var result =
                calc.CalculateCarbonForShelterbelt(aboveGroundCarbonForLinearPlantingsInShelterbelt);
            Assert.AreEqual(1022.625, result);
        }


        /// <summary>
        ///    Equation 2.3.4-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideSequesteredInShelterbeltReturnsCorrectValue()
        {
            var totalAboveGroundCarbonForShelterbelt = 26.875;
            var result = calc.CalculateCarbonDioxideSequesteredInShelterbelt(totalAboveGroundCarbonForShelterbelt);
            Assert.AreEqual(-98.541666666666657, result);
        }

        #endregion
    }
}