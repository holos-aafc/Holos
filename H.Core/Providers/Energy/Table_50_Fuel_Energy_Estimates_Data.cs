﻿using H.Core.Enumerations;

namespace H.Core.Providers.Energy
{
    public class Table_50_Fuel_Energy_Estimates_Data : EnergyDataCharacteristics
    {
        /// <summary>
        ///     The crop for which fuel energy estimate data is required.
        /// </summary>
        public CropType CropType { get; set; }

        /// <summary>
        ///     Fuel estimate of the crop based on the province, tillage and soil type.
        ///     Unit of measurement = GJ ha-1
        /// </summary>
        public double FuelEstimate { get; set; }
    }
}