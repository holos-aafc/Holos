using System.Collections.Generic;
using System.Linq;
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
        
        #endregion
    }
}