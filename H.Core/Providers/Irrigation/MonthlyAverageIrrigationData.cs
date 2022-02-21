using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Providers.Irrigation
{
    public class MonthlyAverageIrrigationData
    {
        /// <summary>
        /// The month for the average irrigation data.
        /// </summary>
        public Months Month { get; set; }

        /// <summary>
        /// The province for the average irrigation data.
        /// </summary>
        public Province Province { get; set; }

        /// <summary>
        /// The irrigation volume given a month and province.
        /// </summary>
        public double IrrigationVolume { get; set; }

    }
}
