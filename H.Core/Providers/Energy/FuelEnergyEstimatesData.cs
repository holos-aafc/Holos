using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Energy
{
    public class FuelEnergyEstimatesData : FuelEnergyDataCharacteristics
    {
        /// <summary>
        /// The crop for which fuel energy estimate data is required.
        /// </summary>
        public CropType CropType { get; set; }

        /// <summary>
        /// Fuel estimate of the crop based on the province, tillage and soil type.
        ///  Unit of measurement = GJ ha-1
        /// </summary>
        public double FuelEstimate { get; set; }
    }
}
