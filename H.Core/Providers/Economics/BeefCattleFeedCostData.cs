using H.Core.CustomAttributes;
using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Providers.Economics
{
    public abstract class BeefCattleFeedCostData
    {
        /// <summary>
        /// The type of animal for which feed costs are needed.
        /// </summary>
        public AnimalType AnimalType { get; set; }

        /// <summary>
        /// The diet type of the animal for which feed costs are needed.
        /// </summary>
        public DietType DietType { get; set; }

        /// <summary>
        /// Non-feed and non-labour related variable costs.
        /// </summary>
        [Units(MetricUnitsOfMeasurement.DollarsPerDayPerAnimal)]
        public double VariableCostsNonFeed { get; set; }

        /// <summary>
        /// Fixed costs related to feed.
        /// </summary>
        [Units(MetricUnitsOfMeasurement.DollarsPerDayPerAnimal)]
        public double FixedCosts { get; set; }

        /// <summary>
        /// Labour related costs of the feed.
        /// </summary>
        [Units(MetricUnitsOfMeasurement.DollarsPerDayPerAnimal)]
        public double LabourCosts { get; set; }
    }
}
