using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public  class Table_24_Average_Milk_Production_Dairy_Cows_Data
    {
        /// <summary>
        /// Represents the province for which we need the average milk production value for.
        /// </summary>
        public Province Province { get; set; }

        /// <summary>
        /// Represents the year for which we need the milk production value for.
        /// </summary>
        public double Year { get; set; }

        /// <summary>
        /// The average milk production value based on the province and year specified by user.
        /// </summary>
        public double AverageMilkProduction { get; set; }
    }
}
