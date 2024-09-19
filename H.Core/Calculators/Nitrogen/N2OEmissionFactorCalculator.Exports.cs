using H.Core.Models;
using System;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        /// <summary>
        /// Equation 2.6.5-6
        /// Equation 2.7.4-6
        /// </summary>
        /// <returns>Returns the total direct N2O emissions from exported manure (kg N2O-N ha^-1)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public double CalculateTotalDirectN2ONFromExportedManure(Farm farm, int year)
        {
            var emissionsForFarm = this.CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(farm, year);
            var result = emissionsForFarm / farm.GetTotalAreaOfFarm(false, year);

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-9
        ///
        /// (kg N2O-N year^-1)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(Farm farm, int year)
        {
            var viewItemsByYear = farm.GetCropDetailViewItemsByYear(year, false);

            var weightedEmissionFactor = this.CalculateWeightedOrganicNitrogenEmissionFactor(viewItemsByYear, farm);
            var totalExportedManureNitrogen = _manureService.GetTotalNitrogenFromExportedManure(year, farm);

            var emissions = this.CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(totalExportedManureNitrogen, weightedEmissionFactor);

            return emissions;
        }

        #endregion
    }
}