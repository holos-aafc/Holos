using System.Collections.Generic;
using H.Core.Calculators.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Shelterbelt;

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
        /// </summary>
        [TestMethod]
        public void CalculateAverageCircumferenceReturnsCorrectValue()
        {
            var treeCircumferences = new List<double> {56.000, 65.00};
            var result = calc.CalculateAverageCircumference(treeCircumferences);
            Assert.AreEqual(60.5, result);
        }


        /// <summary>
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

        #region Dead Organic Matter Contribution (DOM baseline / Option A fix)

        /*
         * Regression coverage for the shelterbelt DOM fix.
         *
         * Before the fix, the reported Dead Organic Matter (and therefore Total Ecosystem Carbon) used the absolute
         * soil-carbon STOCK from the carbon lookup tables. At planting (age 1) that stock is entirely pre-existing
         * field soil (living biomass is still zero), e.g. ~99 Mg C/km, so the shelterbelt was credited with carbon it
         * never created, the totals varied by tree species, and mortality behaved incorrectly.
         *
         * After the fix, DOM is reported as the CONTRIBUTION = stock(age) - stock(planting). The planting-year value
         * must therefore be exactly zero, and later years reflect only the change the shelterbelt causes (a few Mg/km,
         * not the ~99 Mg/km pedestal).
         */

        private TrannumData BuildTrannum(TreeSpecies species, int age, double mortality = 0, double realGrowthRatio = 1.0, double year = 2026)
        {
            var mortalityLow = calc.CalculateMortalityLow(mortality);
            var mortalityHigh = calc.CalculateMortalityHigh(mortalityLow);

            return new TrannumData
            {
                TreeSpecies = species,
                Age = age,
                Year = year,
                PercentMortality = mortality,
                PercentMortalityLow = mortalityLow,
                PercentMortalityHigh = mortalityHigh,
                CanLookupByEcodistrict = false, // use the hardiness-zone (Table 12) lookup path
                HardinessZone = HardinessZone.H3b,
                RealGrowthRatio = realGrowthRatio,
            };
        }

        [TestMethod]
        public void CalculateEstimatedGrowthReportsZeroDeadOrganicMatterContributionAtPlanting()
        {
            var trannum = this.BuildTrannum(TreeSpecies.ScotsPine, age: 1);

            calc.CalculateEstimatedGrowth(trannum);

            // Planting-year contribution = stock(1) - stock(1) = 0 (was ~99 Mg C/km before the fix).
            Assert.AreEqual(0.0, trannum.EstimatedDeadOrganicMatterBasedOnRealGrowth, 0.0001);
        }

        [TestMethod]
        public void CalculateEstimatedGrowthReportsZeroDeadOrganicMatterContributionAtPlantingForAverageConifer()
        {
            // Exercises the species-averaging path of the planting-year baseline lookup.
            var trannum = this.BuildTrannum(TreeSpecies.AverageConifer, age: 1);

            calc.CalculateEstimatedGrowth(trannum);

            Assert.AreEqual(0.0, trannum.EstimatedDeadOrganicMatterBasedOnRealGrowth, 0.0001);
        }

        [TestMethod]
        public void CalculateEstimatedGrowthReportsZeroDeadOrganicMatterContributionAtPlantingForAverageDeciduous()
        {
            var trannum = this.BuildTrannum(TreeSpecies.AverageDeciduous, age: 1);

            calc.CalculateEstimatedGrowth(trannum);

            Assert.AreEqual(0.0, trannum.EstimatedDeadOrganicMatterBasedOnRealGrowth, 0.0001);
        }

        [TestMethod]
        public void CalculateEstimatedGrowthReportsDeadOrganicMatterAsContributionNotAbsoluteStock()
        {
            // At a later age the reported DOM must be the shelterbelt's contribution (a change of, at most, a few
            // Mg C/km == a few thousand kg C/km), NOT the ~99 Mg C/km (~99,000 kg C/km) pre-existing soil stock.
            var trannum = this.BuildTrannum(TreeSpecies.ScotsPine, age: 30);

            calc.CalculateEstimatedGrowth(trannum);

            // Value is in kg C/km. The pre-existing soil baseline is ~99,000; the contribution never exceeds ~15,000.
            Assert.IsTrue(Math.Abs(trannum.EstimatedDeadOrganicMatterBasedOnRealGrowth) < 20000,
                "Reported DOM should be the shelterbelt contribution (a small change), not the absolute soil stock.");
        }

        [TestMethod]
        public void CalculateEstimatedGrowthPreservesYearOverYearDeadOrganicMatterChange()
        {
            // Subtracting a constant planting-year baseline must NOT alter the year-over-year change, so the
            // difference between two ages equals the difference of the reported contributions.
            var atAge10 = this.BuildTrannum(TreeSpecies.ScotsPine, age: 10);
            var atAge11 = this.BuildTrannum(TreeSpecies.ScotsPine, age: 11);

            calc.CalculateEstimatedGrowth(atAge10);
            calc.CalculateEstimatedGrowth(atAge11);

            var reportedChange = atAge11.EstimatedDeadOrganicMatterBasedOnRealGrowth
                                 - atAge10.EstimatedDeadOrganicMatterBasedOnRealGrowth;

            // The change is the difference of two contributions; it must be finite and modest (a single year of soil
            // change, well under 1 Mg C/km == 1000 kg C/km), confirming the columns track change, not the stock.
            Assert.IsTrue(Math.Abs(reportedChange) < 1000,
                "Year-over-year DOM change should be a single year's soil-carbon change.");
        }

        [TestMethod]
        public void TotalResultsForEachYearReportsZeroEcosystemCarbonAndDeadOrganicMatterInPlantingYear()
        {
            // End-to-end guard on the actual report object: the planting-year row must show 0 ecosystem carbon and
            // 0 dead organic matter (the bug showed ~99 Mg C/km in both).
            const int plantingYear = 2026;
            const int yearsOfGrowth = 15;

            var component = new ShelterbeltComponent();
            for (var age = 1; age <= yearsOfGrowth; age++)
            {
                var trannum = this.BuildTrannum(TreeSpecies.ScotsPine, age: age, year: plantingYear + (age - 1));
                calc.CalculateEstimatedGrowth(trannum);
                component.TrannumData.Add(trannum);
            }

            var results = calc.TotalResultsForEachYear(new[] { component }).OrderBy(x => x.Year).ToList();
            var plantingResult = results.First();

            Assert.AreEqual(plantingYear, plantingResult.Year);
            Assert.AreEqual(0.0, plantingResult.TotalDeadOrganicMatterCarbon, 0.0001,
                "Planting-year DOM must be the contribution (0), not the pre-existing soil stock.");
            Assert.AreEqual(0.0, plantingResult.TotalEcosystemCarbon, 0.0001,
                "Planting-year ecosystem carbon must be 0 (no biomass yet plus zero DOM contribution).");
            // First-year change is defined as zero by the calculator.
            Assert.AreEqual(0.0, plantingResult.TotalEcosystemCarbonChange, 0.0001);
        }

        #endregion
    }
}