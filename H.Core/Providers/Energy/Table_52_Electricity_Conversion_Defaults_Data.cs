using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Energy
{
    public class Table_52_Electricity_Conversion_Defaults_Data
    {
        /// <summary>
        /// The year corresponding to the electricity values.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The province corresponding to the electricity values.
        /// </summary>
        public Province Province { get; set; }

        /// <summary>
        /// The electricity conversion value given a province and year. Unit of measurement = kg CO2 kWh-1
        /// </summary>
        public double ElectricityValue { get; set; }
    }
}
