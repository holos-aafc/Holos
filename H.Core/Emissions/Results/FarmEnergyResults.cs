using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Infrastructure;

namespace H.Core.Emissions.Results
{
    public class FarmEnergyResults : ModelBase
    {
        #region Fields

        private double _energyCarbonDioxideFromManureApplication;
        private double _totalCroppingEnergyEmissionsForFarm;

        #endregion

        #region Properties

        /// <summary>
        /// (kg CO2e)
        /// </summary>
        public double TotalCroppingEnergyEmissionsForFarmAsCarbonDioxideEquivalents
        {
            get
            {
                return this.TotalCroppingEnergyEmissionsForFarm * CoreConstants.CO2ToCO2eConversionFactor;
            }
        }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromManureApplication
        {
            get => _energyCarbonDioxideFromManureApplication;
            set => SetProperty(ref _energyCarbonDioxideFromManureApplication, value);
        }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalCroppingEnergyEmissionsForFarm
        {
            get => _totalCroppingEnergyEmissionsForFarm;
            set => SetProperty(ref _totalCroppingEnergyEmissionsForFarm, value);
        }

        #endregion
    }
}
