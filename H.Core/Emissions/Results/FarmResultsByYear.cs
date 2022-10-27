using System;
using System.Text.RegularExpressions;
using H.Core.Models;

namespace H.Core.Emissions.Results
{
    public class FarmResultsByYear
    {
        #region Constructors

        public FarmResultsByYear()
        {
        }

        #endregion

        #region Properties
        
        public int Year { get; set; }

        /// <summary>
        /// (kg CO2e)
        /// </summary>
        public double TotalEmissions { get; set; }

        public double TotalNitrousOxideEmissionsFromLandManagement { get; set; }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalEnergyCarbonDioxideFromManureSpreading { get; set; }

        #endregion
    }
}