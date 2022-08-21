using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Providers.Economics
{
    public class Table_62_Feed_Costs_For_Beef_Data
    {
        /// <summary>
        /// Hay Quality for beef
        /// </summary>
        public DietType DietType { get; set; }

        /// <summary>
        /// $ / Kg
        /// </summary>
        [Units(MetricUnitsOfMeasurement.DollarsPerKilogram)]
        public double Cost { get; set; }
    }
}
