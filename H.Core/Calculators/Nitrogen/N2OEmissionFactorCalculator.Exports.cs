using H.Core.Models;
using System;
using System.Linq;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        /// <summary>
        /// Equation 2.6.5-6
        /// Equation 2.7.4-6
        /// 
        /// (kg N2O-N ha^-1)
        /// </summary>
        /// <returns>Returns the total direct N2O emissions from exported manure (kg N2O-N ha^-1)</returns>
        public double CalculateTotalDirectN2ONFromExportedManure(Farm farm, int year)
        {
            var result = 0d;

            foreach (var manureExportViewItem in farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year))
            {
                result += this.CalculateTotalDirectN2ONFromExportedManure(farm, manureExportViewItem);
            }

            return result;
        }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem)
        {
            var totalNitrogen = _manureService.GetTotalNitrogenFromExportedManure(manureExportViewItem);
            var viewItemsByYear = farm.GetCropDetailViewItemsByYear(manureExportViewItem.DateOfExport.Year, false);
            var weightedEmissionFactor = this.CalculateWeightedOrganicNitrogenEmissionFactor(viewItemsByYear, farm);
            var emissionsForFarm = this.CalculateTotalDirectN2ONFromExportedManure(totalNitrogen, weightedEmissionFactor);
            var result = emissionsForFarm / farm.GetTotalAreaOfFarm(false, manureExportViewItem.DateOfExport.Year);

            return result;
        }
        
        /// <summary>
        /// Equation 4.6.1-9
        ///
        /// Returns the total N2O-N from exported manure for entire farm
        /// 
        /// (kg N2O-N year^-1 farm^-1)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(Farm farm, int year)
        {
            var result = 0d;

            result = this.CalculateTotalDirectN2ONFromExportedManure(farm, year) * farm.GetTotalAreaOfFarm(false, year);

            return result;
        }

        #endregion
    }
}