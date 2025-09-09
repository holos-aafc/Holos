﻿using H.Core.Enumerations;

namespace H.Core.Providers.Irrigation
{
    public class Table_4_Monthly_Irrigation_Water_Application_Data
    {
        /// <summary>
        ///     The month for the average irrigation data.
        /// </summary>
        public Months Month { get; set; }

        /// <summary>
        ///     The province for the average irrigation data.
        /// </summary>
        public Province Province { get; set; }

        /// <summary>
        ///     The irrigation volume given a month and province.
        ///     (%)
        /// </summary>
        public double IrrigationVolume { get; set; }
    }
}