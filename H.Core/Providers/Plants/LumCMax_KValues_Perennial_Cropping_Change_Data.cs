﻿using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class LumCMax_KValues_Perennial_Cropping_Change_Data
    {
        #region Methods

        public override string ToString()
        {
            return
                $"{nameof(Ecozone)}: {Ecozone}, {nameof(SoilTexture)}: {SoilTexture}, {nameof(PerennialCroppingChangeType)}: {PerennialCroppingChangeType}, {nameof(LumCMax)}: {LumCMax}, {nameof(kValue)}: {kValue}";
        }

        #endregion

        #region Properties

        public Ecozone Ecozone { get; set; }
        public SoilTexture SoilTexture { get; set; }
        public PerennialCroppingChangeType PerennialCroppingChangeType { get; set; }
        public double LumCMax { get; set; }
        public double kValue { get; set; }

        #endregion
    }
}