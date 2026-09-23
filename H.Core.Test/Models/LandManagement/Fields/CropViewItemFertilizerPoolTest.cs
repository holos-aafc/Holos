using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Fertilizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models.LandManagement.Fields
{
    /// <summary>
    /// Nitrogen from a fertilizer application belongs to exactly one of the two pools the carbon calculators load:
    /// the synthetic pool, or the organic pool. Nitrogen present in both is emitted twice, once at each set of
    /// emission factors, in the direct term and in both indirect terms.
    /// </summary>
    [TestClass]
    public class CropViewItemFertilizerPoolTest
    {
        #region Tests

        [TestMethod]
        public void OrganicFertilizerIsCountedAsOrganic()
        {
            var viewItem = BuildFieldWith(FertilizerBlends.CustomOrganic, nitrogenApplied: 46);

            Assert.AreEqual(46, viewItem.GetTotalOrganicNitrogenFertilizerRateInYear(), 0.0001);
        }

        [TestMethod]
        public void SyntheticFertilizerIsNotCountedAsOrganic()
        {
            var viewItem = BuildFieldWith(FertilizerBlends.Urea, nitrogenApplied: 46);

            Assert.AreEqual(0, viewItem.GetTotalOrganicNitrogenFertilizerRateInYear(), 0.0001);
        }

        /// <summary>
        /// The per-hectare rate taken off the synthetic pool must equal the per-hectare amount the organic pool is
        /// loaded with, or nitrogen goes missing from both pools instead of being double counted in them.
        /// </summary>
        [TestMethod]
        public void TheRateRemovedMatchesTheAmountTheOrganicPoolReceives()
        {
            var viewItem = BuildFieldWith(FertilizerBlends.CustomOrganic, nitrogenApplied: 46);
            viewItem.Area = 37.5;
            AddApplication(viewItem, FertilizerBlends.Urea, nitrogenApplied: 12);

            Assert.AreEqual(
                viewItem.GetTotalOrganicNitrogenInYear() / viewItem.Area,
                viewItem.GetTotalOrganicNitrogenFertilizerRateInYear(),
                0.0001);
        }

        /// <summary>
        /// NitrogenFertilizerRate is the field's total fertilizer rate and is reported and used as such, so it keeps
        /// counting every application whatever the blend.
        /// </summary>
        [TestMethod]
        public void TheReportedFertilizerRateStillCountsEveryApplication()
        {
            var viewItem = BuildFieldWith(FertilizerBlends.Urea, nitrogenApplied: 46);
            AddApplication(viewItem, FertilizerBlends.CustomOrganic, nitrogenApplied: 46);

            Assert.AreEqual(92, viewItem.NitrogenFertilizerRate, 0.0001);
        }

        /// <summary>
        /// The pool the emission terms multiply. Organic fertilizer reaching it is emitted at the synthetic factors
        /// as well as the organic ones.
        /// </summary>
        [TestMethod]
        public void TheSyntheticPoolExcludesOrganicFertilizer()
        {
            var viewItem = BuildFieldWith(FertilizerBlends.CustomOrganic, nitrogenApplied: 46);
            viewItem.NitrogenDepositionAmount = 5;

            var calculator = new PoolLoader();
            calculator.LoadSyntheticPool(viewItem);

            Assert.AreEqual(5, calculator.SyntheticNitrogenPool, 0.0001, "only the deposition belongs in the synthetic pool");
        }

        [TestMethod]
        public void TheSyntheticPoolHoldsSyntheticFertilizerAndDeposition()
        {
            var viewItem = BuildFieldWith(FertilizerBlends.Urea, nitrogenApplied: 46);
            viewItem.NitrogenDepositionAmount = 5;

            var calculator = new PoolLoader();
            calculator.LoadSyntheticPool(viewItem);

            Assert.AreEqual(51, calculator.SyntheticNitrogenPool, 0.0001);
        }

        /// <summary>
        /// The command line interface assigns NitrogenFertilizerRate straight from the input file and creates no
        /// fertilizer applications at all, so the synthetic pool has to be reachable without them.
        /// </summary>
        [TestMethod]
        public void TheSyntheticPoolHoldsARateSetWithoutAnyApplications()
        {
            var viewItem = new CropViewItem() {Area = 1, NitrogenFertilizerRate = 100, NitrogenDepositionAmount = 0};

            var calculator = new PoolLoader();
            calculator.LoadSyntheticPool(viewItem);

            Assert.AreEqual(100, calculator.SyntheticNitrogenPool, 0.0001);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// CarbonCalculatorBase is abstract and loads the synthetic pool through a protected method, so the pool
        /// loading is reached through a minimal derivation rather than a whole model.
        /// </summary>
        private class PoolLoader : CarbonCalculatorBase
        {
            public void LoadSyntheticPool(CropViewItem viewItem)
            {
                base.SetSyntheticNStartState(viewItem);
            }

            protected override void SetCropResiduesStartState(Farm farm) { }
            protected override void SetManurePoolStartState(Farm farm) { }
            protected override void SetOrganicNitrogenPoolStartState() { }
            protected override void AdjustOrganicPool() { }
        }

        private static CropViewItem BuildFieldWith(FertilizerBlends blend, double nitrogenApplied)
        {
            var viewItem = new CropViewItem() {Area = 1};

            AddApplication(viewItem, blend, nitrogenApplied);

            return viewItem;
        }

        /// <summary>
        /// AmountOfNitrogenApplied is recalculated from the blend's nitrogen percentage and the amount of product, so
        /// the application is built to apply the wanted nitrogen rather than having it assigned.
        /// </summary>
        private static void AddApplication(CropViewItem viewItem, FertilizerBlends blend, double nitrogenApplied)
        {
            var application = new FertilizerApplicationViewItem();

            application.FertilizerBlendData = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
            {
                FertilizerBlend = blend,
                PercentageNitrogen = 10,
            };

            application.AmountOfBlendedProductApplied = nitrogenApplied / 0.1;

            viewItem.FertilizerApplicationViewItems.Add(application);
            viewItem.UpdateApplicationRateTotals();
        }

        #endregion
    }
}
