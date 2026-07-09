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

        #region Mortality Scenarios

        /*
         * Worked examples that drive the production path (ShelterbeltComponent -> CalculateInitialResults ->
         * TotalResultsForEachYear) and assert the reported numbers. Each scenario is a shelterbelt at age 51, 100
         * planted trees over a 200 m row, identical tree size; the only difference is mortality (100 vs 50 live trees).
         */

        /// <summary>
        /// Builds and runs one shelterbelt through the production path and returns every year's reported result
        /// (ordered by year), plus the RealGrowthRatio at observation. Mortality is expressed as the live fraction:
        /// 1.0 == 0% mortality (100 of 100 live), 0.5 == 50% mortality (50 of 100 live).
        /// </summary>
        private List<TrannumResultViewItem> RunShelterbelt(TreeSpecies species, HardinessZone zone, double liveFraction, out double realGrowthRatio)
        {
            var component = new ShelterbeltComponent
            {
                HardinessZone = zone,
                EcoDistrictId = 0,          // not Saskatchewan => hardiness-zone (Table 12) lookup path
                YearOfObservation = 2050,
            };

            var row = component.NewRowData();
            row.Length = 200; // metres

            var treeGroup = row.NewTreeGroupData();
            treeGroup.TreeSpecies = species;
            treeGroup.PlantYear = 2000;
            treeGroup.CutYear = 2050;       // age 51 at observation
            treeGroup.PlantedTreeCount = 100;
            treeGroup.LiveTreeCount = 100 * liveFraction; // 100 => 0% mortality, 50 => 50% mortality
            treeGroup.CircumferenceData.UserCircumference = 282.0; // identical tree size in both runs

            calc.CalculateInitialResults(component);
            realGrowthRatio = component.TrannumData.OrderBy(x => x.Year).Last().RealGrowthRatio;

            return calc.TotalResultsForEachYear(new[] { component }).OrderBy(x => x.Year).ToList();
        }

        [DataTestMethod]
        [DataRow(TreeSpecies.ScotsPine)]
        [DataRow(TreeSpecies.WhiteSpruce)]
        [DataRow(TreeSpecies.GreenAsh)]
        [DataRow(TreeSpecies.HybridPoplar)]
        public void LivingBiomassHalvesAtFiftyPercentMortality(TreeSpecies species)
        {
            // The correct, intuitive response: kill half the trees, get half the living biomass carbon.
            var noMortality = this.RunShelterbelt(species, HardinessZone.H3b, liveFraction: 1.0, out _).Last();
            var halfMortality = this.RunShelterbelt(species, HardinessZone.H3b, liveFraction: 0.5, out _).Last();

            var ratio = halfMortality.TotalLivingBiomassCarbon / noMortality.TotalLivingBiomassCarbon;

            Assert.AreEqual(0.50, ratio, 0.03,
                $"Living biomass at 50% mortality should be half of the 0% case (species {species}).");
        }

        [TestMethod]
        public void DeadOrganicMatterContributionIsNearlyInsensitiveToMortality()
        {
            // DOM is NOT scaled by mortality/RealGrowthRatio (issue #429): it comes straight from the carbon tables,
            // so halving the trees does NOT halve the DOM contribution. It moves only a little.
            var noMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 1.0, out _).Last();
            var halfMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 0.5, out _).Last();

            var dom0 = noMortality.TotalDeadOrganicMatterCarbon;
            var dom50 = halfMortality.TotalDeadOrganicMatterCarbon;

            // Were DOM scaled by mortality it would be ~half (~0.5 * dom0). Instead it stays the same order of
            // magnitude — far from a 50% reduction — which is exactly the #429 behaviour.
            Assert.IsTrue(dom50 > 0.75 * dom0 && dom50 < 1.75 * dom0,
                $"DOM contribution should be roughly mortality-insensitive, but moved from {dom0:F2} to {dom50:F2} Mg C/km.");
        }

        [TestMethod]
        public void DeadOrganicMatterChangeWithMortalityHasNoConsistentSign()
        {
            // A common assumption is "more dead trees => more dead organic matter." The tables don't bear this out:
            // the DOM contribution rises with mortality for some species and falls for others. So no single-direction
            // claim about DOM-vs-mortality is correct.
            var pine0 = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 1.0, out _).Last().TotalDeadOrganicMatterCarbon;
            var pine50 = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 0.5, out _).Last().TotalDeadOrganicMatterCarbon;
            var ash0 = this.RunShelterbelt(TreeSpecies.GreenAsh, HardinessZone.H3b, 1.0, out _).Last().TotalDeadOrganicMatterCarbon;
            var ash50 = this.RunShelterbelt(TreeSpecies.GreenAsh, HardinessZone.H3b, 0.5, out _).Last().TotalDeadOrganicMatterCarbon;

            Assert.IsTrue(pine50 > pine0, $"Scots pine DOM contribution rises with mortality ({pine0:F2} -> {pine50:F2}).");
            Assert.IsTrue(ash50 < ash0, $"Green ash DOM contribution falls with mortality ({ash0:F2} -> {ash50:F2}).");
        }

        [TestMethod]
        public void TotalEcosystemCarbonDropsWithMortalityButByLessThanLivingBiomass()
        {
            // Living biomass halves but the DOM term barely moves, so the total falls by less than a full half: the
            // unscaled DOM dampens the total's response to mortality.
            var noMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 1.0, out _).Last();
            var halfMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 0.5, out _).Last();

            var biomassRatio = halfMortality.TotalLivingBiomassCarbon / noMortality.TotalLivingBiomassCarbon;
            var ecosystemRatio = halfMortality.TotalEcosystemCarbon / noMortality.TotalEcosystemCarbon;

            Assert.AreEqual(0.50, biomassRatio, 0.03, "Living biomass halves...");
            Assert.IsTrue(ecosystemRatio > biomassRatio + 0.05,
                $"...but total ecosystem carbon is diluted by the unscaled DOM (ratio {ecosystemRatio:F2} vs biomass {biomassRatio:F2}).");
            Assert.IsTrue(ecosystemRatio < 0.90,
                $"The total still responds to mortality (ratio {ecosystemRatio:F2}); it is not nearly identical to the 0% case.");
        }

        [TestMethod]
        public void TotalEcosystemCarbonMutingIsLargerForSmallerBelts()
        {
            // The dilution depends on how big the living biomass is relative to the (fixed) DOM term. White spruce
            // builds far more living biomass than Manitoba maple, so its total tracks mortality more closely.
            var spruce0 = this.RunShelterbelt(TreeSpecies.WhiteSpruce, HardinessZone.H3b, 1.0, out _).Last();
            var spruce50 = this.RunShelterbelt(TreeSpecies.WhiteSpruce, HardinessZone.H3b, 0.5, out _).Last();
            var maple0 = this.RunShelterbelt(TreeSpecies.ManitobaMaple, HardinessZone.H3b, 1.0, out _).Last();
            var maple50 = this.RunShelterbelt(TreeSpecies.ManitobaMaple, HardinessZone.H3b, 0.5, out _).Last();

            var spruceRatio = spruce50.TotalEcosystemCarbon / spruce0.TotalEcosystemCarbon; // ~0.53 (large biomass)
            var mapleRatio = maple50.TotalEcosystemCarbon / maple0.TotalEcosystemCarbon;    // ~0.70 (small biomass)

            Assert.IsTrue(spruceRatio < mapleRatio,
                $"A larger belt's total responds more strongly to mortality (spruce {spruceRatio:F2} < maple {mapleRatio:F2}).");
        }

        [TestMethod]
        public void BothMortalityLevelsStartFromZeroInThePlantingYear()
        {
            // DOM is reported as a contribution from planting, so both mortality levels start at zero in the planting
            // year; the 0% vs 50% comparison is not offset by a shared soil-carbon pedestal.
            var noMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 1.0, out _).First();
            var halfMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 0.5, out _).First();

            Assert.AreEqual(0.0, noMortality.TotalDeadOrganicMatterCarbon, 0.0001, "0% mortality starts at zero DOM contribution.");
            Assert.AreEqual(0.0, halfMortality.TotalDeadOrganicMatterCarbon, 0.0001, "50% mortality starts at zero DOM contribution.");
            Assert.AreEqual(0.0, noMortality.TotalEcosystemCarbon, 0.0001);
            Assert.AreEqual(0.0, halfMortality.TotalEcosystemCarbon, 0.0001);
        }

        [TestMethod]
        public void ScalingDeadOrganicMatterByGrowthRatioWouldMakeTheTotalTrackMortality()
        {
            // The production total leaves DOM unscaled (#429). This test recomputes the total as it would be if DOM
            // were instead scaled by RealGrowthRatio (biomass + DOM * RGR): under that alternative the total drops by
            // ~half with mortality. It documents the trade-off between the two DOM-scaling choices.
            var noMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 1.0, out var rgr0).Last();
            var halfMortality = this.RunShelterbelt(TreeSpecies.ScotsPine, HardinessZone.H3b, 0.5, out var rgr50).Last();

            var scaledTotal0 = noMortality.TotalLivingBiomassCarbon + noMortality.TotalDeadOrganicMatterCarbon * rgr0;
            var scaledTotal50 = halfMortality.TotalLivingBiomassCarbon + halfMortality.TotalDeadOrganicMatterCarbon * rgr50;
            var scaledRatio = scaledTotal50 / scaledTotal0;

            // Compare against the AS-CODED total, which is diluted (well above 0.5).
            var asCodedRatio = halfMortality.TotalEcosystemCarbon / noMortality.TotalEcosystemCarbon;

            Assert.AreEqual(0.50, scaledRatio, 0.08,
                $"With DOM scaled by RealGrowthRatio the total would drop by ~half (ratio {scaledRatio:F2}).");
            Assert.IsTrue(asCodedRatio > scaledRatio + 0.05,
                $"As coded (DOM unscaled, #429) the total is less responsive ({asCodedRatio:F2}) than the scaled alternative ({scaledRatio:F2}).");
        }

        #endregion
    }
}