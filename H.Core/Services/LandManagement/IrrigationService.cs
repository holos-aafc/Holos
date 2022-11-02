using System;
using H.Core.Models;
using H.Core.Providers.Climate;
using H.Core.Providers.Irrigation;

namespace H.Core.Services.LandManagement
{
    public class IrrigationService
    {
        #region Fields

        private Table_7_Monthly_Irrigation_Water_Application_Provider _irrigationProvider = new Table_7_Monthly_Irrigation_Water_Application_Provider();
        

        #endregion

        #region Constuctors


        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.2.1-45
        /// </summary>
        public double GetDefaultIrrigationForYear(Farm farm, int year)
        {
            var annualPrecipitation = farm.ClimateData.GetTotalPrecipitationForYear(year);
            var potentialEvapotranspiration = farm.ClimateData.GetTotalEvapotranspirationForYear(year);

            if (potentialEvapotranspiration > annualPrecipitation)
            {
                return potentialEvapotranspiration - annualPrecipitation;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}