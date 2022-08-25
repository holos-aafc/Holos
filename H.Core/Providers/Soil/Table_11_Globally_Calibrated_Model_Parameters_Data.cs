using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Soil
{
    public class Table_11_Globally_Calibrated_Model_Parameters_Data
    {
        /// <summary>
        /// Globally calibrated model parameters to be used to estimate soc changes fro mineral soils with tier 2 steady-state-method
        /// </summary>
        public ModelParameters Parameter { get; set; }

        /// <summary>
        /// The tillage type used for the given parameter
        /// </summary>
        public TillageType TillageType { get; set; }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The minimum value of the parameter. The value is 0.0 if a maximum value is not available.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// The maximum value of the parameter. The value is 0.0 if a maximum value is not available.
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// The standard deviation value of the parmeter. The SD is 0.0 if max and min are not available.
        /// </summary>
        public double StandardDeviation { get; set; }

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public string Description { get; set; }
    }
}
