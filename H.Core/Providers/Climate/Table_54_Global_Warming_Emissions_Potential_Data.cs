﻿using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class Table_54_Global_Warming_Emissions_Potential_Data
    {
        /// <summary>
        ///     The year for the global radiative forcing value
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        ///     The emission type whose value is required
        /// </summary>
        public EmissionTypes EmissionType { get; set; }

        /// <summary>
        ///     The value of the emission type given a year. Unit of measurement = Global Warming Potential
        /// </summary>
        public double GlobalWarmingPotentialValue { get; set; }

        /// <summary>
        ///     The source where the data is collected from
        /// </summary>
        public string Source { get; set; }
    }
}