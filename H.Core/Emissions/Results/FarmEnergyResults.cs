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
        private double _totalOnFarmCroppingEnergyEmissionsForFarm;

        #endregion

        #region Properties

        /// <summary>
        /// <remarks>This value does should not include emissions from upstream production of herbicide, fertilizer, etc. Only on-farm emissions are included.</remarks>
        /// 
        /// (kg CO2e)
        /// </summary>
        public double TotalCroppingEnergyEmissionsForFarmAsCarbonDioxideEquivalents
        {
            get
            {
                return this.TotalOnFarmCroppingEnergyEmissionsForFarm * CoreConstants.CO2ToCO2eConversionFactor;
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
        public double TotalOnFarmCroppingEnergyEmissionsForFarm
        {
            get => _totalOnFarmCroppingEnergyEmissionsForFarm;
            set => SetProperty(ref _totalOnFarmCroppingEnergyEmissionsForFarm, value);
        }

        #endregion
    }
}
