using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.Results;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        private double _totalCarbonLossFromBaleExports;
        private double _totalDryMatterLostFromBaleExports;

        private EstimatesOfProductionResultsViewItem _estimatesOfProductionResultsViewItem;

        #endregion

        #region Properties

        /// <summary>
        /// Equation 12.3.2-4
        ///
        /// (kg C)
        /// </summary>
        public double TotalCarbonLossFromBaleExports
        {
            get => _totalCarbonLossFromBaleExports;
            set => SetProperty(ref _totalCarbonLossFromBaleExports, value);
        }

        public EstimatesOfProductionResultsViewItem EstimatesOfProductionResultsViewItem
        {
            get => _estimatesOfProductionResultsViewItem;
            set => SetProperty(ref _estimatesOfProductionResultsViewItem, value);
        }

        /// <summary>
        /// (kg DM year^-1)
        /// </summary>
        public double TotalDryMatterLostFromBaleExports
        {
            get => _totalDryMatterLostFromBaleExports;
            set => SetProperty(ref _totalDryMatterLostFromBaleExports, value);
        }

        #endregion

        #region Public Methods

        public double GetTotalWeightWeightFromHarvestedHay(int year)
        {
            var result = 0d;

            foreach (var harvestViewItem in this.HarvestViewItems.Where(x => x.Start.Year.Equals(year)))
            {
                result += harvestViewItem.TotalNumberOfBalesHarvested * harvestViewItem.BaleWeight;
            }

            return result;
        }

        public double GetTotalWeightWeightFromHarvestedHay()
        {
            var result = 0d;

            foreach (var harvestViewItem in this.HarvestViewItems)
            {
                result += harvestViewItem.TotalNumberOfBalesHarvested * harvestViewItem.BaleWeight;
            }

            return result;
        }

        public double GetTotalDryWeightFromHarvestedHay()
        {
            var result = 0d;

            foreach (var harvestViewItem in this.HarvestViewItems)
            {
                var amount = harvestViewItem.AboveGroundBiomass * (1 - (harvestViewItem.MoistureContentAsPercentage / 100.0));

                result += amount;
            }

            return result;
        }

        public double GetTotalDryWeightFromImportedHay()
        {
            var result = 0d;

            foreach (var hayImportViewItem in this.HayImportViewItems)
            {
                var amount = hayImportViewItem.AboveGroundBiomass * (1 - (hayImportViewItem.MoistureContentAsPercentage / 100.0));

                result += amount;
            }

            return result;
        }

        /// <summary>
        /// Returns all <see cref="HarvestViewItems"/> by year
        /// </summary>
        public List<HarvestViewItem> GetHayHarvestsByYear(int year)
        {
            return this.HarvestViewItems.Where(x => x.Start.Year.Equals(year)).ToList();
        }

        /// <summary>
        /// Returns all <see cref="HarvestViewItems"/> for the current year
        /// </summary>
        public List<HarvestViewItem> GetHayHarvests()
        {
            return this.GetHayHarvestsByYear(this.Year);
        }

        /// <summary>
        /// (%)
        /// </summary>
        public double AverageMoistureContentOfHarvests()
        {
            return this.GetHayHarvests().Average(x => x.MoistureContentAsPercentage);
        }

        public bool IsHarvested()
        {
            return this.GetHayHarvests().Any();
        }

        /// <summary>
        /// The share of this year's hayed product left on the field, as a percentage - the harvest loss, which is also
        /// the percentage of product returned to soil for a hayed year (S_p in the algorithm document).
        ///
        /// Cuts are weighted by the dry matter they removed, so a large cut counts for more than a small one. A simple
        /// mean is used when no biomass weights are available. Returns 0 when there are no hayed harvests.
        /// </summary>
        public double GetHayedHarvestLossPercentage()
        {
            var hayedHarvests = this.GetHayHarvests()
                .Where(harvest => harvest.ForageActivity == ForageActivities.Hayed)
                .ToList();

            if (hayedHarvests.Any() == false)
            {
                return 0;
            }

            var totalBiomass = hayedHarvests.Sum(harvest => harvest.AboveGroundBiomassDryWeight);
            if (totalBiomass > 0)
            {
                return hayedHarvests.Sum(harvest => harvest.HarvestLossPercentage * harvest.AboveGroundBiomassDryWeight) / totalBiomass;
            }

            return hayedHarvests.Average(harvest => harvest.HarvestLossPercentage);
        }

        /// <summary>
        /// True when a hay cut is recorded for this year but carries no weight Holos can use, so it cannot set the
        /// yield. The two entries contradict each other - you cannot cut a crop and record nothing coming off it - and
        /// the year would otherwise fall through to the estimate with nothing said. Used to tell the user rather than
        /// to change the arithmetic.
        /// </summary>
        public bool HasHayedHarvestWithNoUsableWeight()
        {
            var hayedHarvests = this.GetHayHarvests()
                .Where(harvest => harvest.ForageActivity == ForageActivities.Hayed)
                .ToList();

            if (hayedHarvests.Any() == false || this.Area <= 0)
            {
                return false;
            }

            return hayedHarvests.Sum(GetHarvestDryMatter) <= 0;
        }

        /// <summary>
        /// Sets this item's yield from the hay actually baled off it, and reports whether it could.
        ///
        /// This is the single place the derivation lives. It used to be done in two: once on the component selection
        /// screen when a harvest was entered, and once per year in the results pipeline - with different formulas, so
        /// the two disagreed whenever the bale and crop moistures differed.
        ///
        /// The two figures are on different moisture bases and have to be reconciled: a bale has been dried (around 15%
        /// moisture) while Yield means the standing crop as it grew (around 80% for forage), and C_p multiplies Yield by
        /// 1 - the CROP's moisture. Using the bale's wet weight directly would strip ~80% of the mass off material
        /// holding only ~15% water. So the harvest's dry matter is re-expressed on the crop's basis, leaving
        /// Yield x (1 - crop moisture) equal to the dry matter baled off, as the pipeline expects.
        ///
        /// Callers own the policy about WHEN to derive (yield assignment method, grazing, frozen values); this method
        /// only refuses when the numbers cannot support a derivation.
        /// </summary>
        public bool CalculateYieldFromHayHarvests()
        {
            var hayedHarvests = this.GetHayHarvests()
                .Where(harvest => harvest.ForageActivity == ForageActivities.Hayed)
                .ToList();

            if (hayedHarvests.Any() == false || this.Area <= 0)
            {
                return false;
            }

            var totalDryMatter = hayedHarvests.Sum(GetHarvestDryMatter);
            if (totalDryMatter <= 0)
            {
                return false;
            }

            // A crop recorded as all water leaves nothing to scale by.
            var dryMatterFraction = 1.0 - this.GetMoistureContentOfCropAsFraction();
            if (dryMatterFraction <= 0)
            {
                return false;
            }

            // The total biomass stays the wet weight actually baled - it is what the harvest tab reports - while the
            // yield is the standing-crop equivalent of that biomass.
            this.TotalBiomassHarvest = hayedHarvests.Sum(harvest => harvest.AboveGroundBiomass);
            this.DryYield = totalDryMatter / this.Area;
            this.Yield = this.DryYield / dryMatterFraction;

            return true;
        }

        /// <summary>
        /// The dry matter baled off in a harvest. <see cref="FieldActivityBase.AboveGroundBiomassDryWeight"/> is
        /// maintained whenever the bale count, bale weight or moisture changes, but is recomputed here from the wet
        /// weight when it was never populated (an older saved farm, or an item built without going through those setters).
        /// </summary>
        private static double GetHarvestDryMatter(HarvestViewItem harvestViewItem)
        {
            if (harvestViewItem.AboveGroundBiomassDryWeight > 0)
            {
                return harvestViewItem.AboveGroundBiomassDryWeight;
            }

            return harvestViewItem.AboveGroundBiomass * (1.0 - (harvestViewItem.MoistureContentAsPercentage / 100.0));
        }

        /// <summary>
        /// The crop's moisture content as a fraction. Some saved farms hold it as a percentage instead, which the carbon
        /// calculator corrects when it runs; this can be called earlier, so it has to tolerate both forms.
        /// </summary>
        public double GetMoistureContentOfCropAsFraction()
        {
            var moistureContent = this.MoistureContentOfCrop;

            return moistureContent > 1 ? moistureContent / 100.0 : moistureContent;
        }

        #endregion
    }
}