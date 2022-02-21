using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Tools;

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// Table 15
    /// 
    /// Soil nitrous oxide emission factors (N2O EF) as influenced by source of nitrogen, soil texture, tillage practice and crop type in Canada (adapted from Liang et al., 2020)
    /// </summary>
    public class SoilNitrousOxideEmissionFactorProvider_Table_15
    {
        #region Inner Classes

        public enum NitrogenSourceTypes
        {
            SyntheticNitrogen,
            OrganicNitrogen,
            CropResidueNitrogen,
        }

        #endregion

        #region Fields      

        #endregion

        #region Constructors

        public SoilNitrousOxideEmissionFactorProvider_Table_15()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Public Methods

        public double GetFactorForNitrogenSource(NitrogenSourceTypes nitrogenSourceType, CropViewItem cropViewItem)
        {
            // For all perennials systems, nitrogen source has no effect (RF = 1)
            if (cropViewItem.CropType.IsPerennial() || cropViewItem.CropType.IsGrassland())
            {
                return 1;
            }

            switch (nitrogenSourceType)
            {
                case NitrogenSourceTypes.CropResidueNitrogen:
                case NitrogenSourceTypes.OrganicNitrogen:
                    return 0.84;
                case NitrogenSourceTypes.SyntheticNitrogen:
                    return 1;
                default:
                    {
                        const double defaultValue = 1;

                        Trace.TraceError($"{nameof(SoilNitrousOxideEmissionFactorProvider_Table_15)}.{nameof(GetFactorForSoilTexture)}: unknown value for {nameof(nitrogenSourceType)}: {nitrogenSourceType}. Returning {defaultValue}");
                        return 1;
                    }
            }
        }

        public double GetFactorForCroppingSystem(CropType cropType)
        {
            if (cropType.IsAnnual())
            {
                return 1;
            }
            else
            {
                // Perennials
                return 0.19;
            }
        }

        public double GetFactorForSoilTexture(SoilTexture soilTexture, Region region)
        {
            if (region == Region.WesternCanada)
            {
                // In the Prairies and the Peace River Section of BC, RF_TX = 1

                return 1;
            }

            if (soilTexture == SoilTexture.Fine)
            {
                return 2.55;
            }

            if (soilTexture == SoilTexture.Coarse || soilTexture == SoilTexture.Medium)
            {
                return 0.49;
            }

            const double defaultValue = 1;

            Trace.TraceError($"{nameof(SoilNitrousOxideEmissionFactorProvider_Table_15)}.{nameof(GetFactorForSoilTexture)}: unknown value for {nameof(soilTexture)}: {soilTexture}, and {nameof(region)}: {region}. Returning {defaultValue}");

            return defaultValue;
        }

        public double GetFactorForTillagePractice(Region region, CropViewItem cropViewItem)
        {
            // For all perennials systems, tillage has no effect (RF = 1)
            if (cropViewItem.CropType.IsPerennial() || cropViewItem.CropType.IsGrassland())
            {
                return 1;
            }

            var tillageType = cropViewItem.TillageType;

            // Roland says:
            // Conservation tillage will be used for both reduced tillage and no-tillage
            // Conventional tillage will be used for intensive tillage

            if (region == Region.EasternCanada)
            {
                if (tillageType == TillageType.Reduced || tillageType == TillageType.NoTill)
                {
                    // Conservation tillage

                    return 1.05;
                }
                else
                {
                    // Conventional tillage (intensive)

                    return 1;
                }
            }

            if (region == Region.WesternCanada)
            {
                if (tillageType == TillageType.Reduced || tillageType == TillageType.NoTill)
                {
                    // Conservation tillage

                    return 0.73;
                }
                else
                {
                    // Conventional tillage (intensive)

                    return 1;
                }
            }

            const double defaultValue = 1;

            Trace.TraceError($"{nameof(SoilNitrousOxideEmissionFactorProvider_Table_15)}.{nameof(GetFactorForTillagePractice)}: unknown {nameof(region)}: {region}. Returning {defaultValue}");

            return defaultValue;
        }

        #endregion
    }
}