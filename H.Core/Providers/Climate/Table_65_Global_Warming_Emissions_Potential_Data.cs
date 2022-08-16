using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class Table_65_Global_Warming_Emissions_Potential_Data
    {
        /// <summary>
        /// The year for the global radiative forcing value
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The emission type whose value is reuquired
        /// </summary>
        public EmissionTypes EmissionType { get; set; }

        /// <summary>
        /// The value of the emission type given a year. Unit of measurement = Global Warming Potential
        /// </summary>
        public double GlobalWarmingPotentialValue { get; set; }

        /// <summary>
        /// The source where the data is collected from
        /// </summary>
        public string Source { get; set; }
    }
}
