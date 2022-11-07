using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Tools;

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// <para>Table 15. N2O Reduction factor based on fertilizer application method</para>
    /// <para>Table 16: Soil nitrous oxide emission factors (N2O EF) as influenced by source of nitrogen, soil texture, tillage practice and crop type in Canada (adapted from Liang et al., 2020)</para>
    /// <para>Provider contains values for RF_NS, RF_TX, RF_TILL, TF_CS and RF_AM</para>
    /// </summary>
    public class Table_16_Soil_N2O_Emission_Factors_Provider
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

        public Table_16_Soil_N2O_Emission_Factors_Provider()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Table 16: Lookup function for Nitrogen source = RF_NS values.
        /// <para>Note: a = A Soil N2O ratio factor for nitrogen source is only applied on annual crops</para>
        /// </summary>
        /// <param name="nitrogenSourceType"></param>
        /// <param name="cropViewItem"></param>
        /// <returns></returns>
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

                        Trace.TraceError($"{nameof(Table_16_Soil_N2O_Emission_Factors_Provider)}.{nameof(GetFactorForSoilTexture)}: unknown value for {nameof(nitrogenSourceType)}: {nitrogenSourceType}. Returning {defaultValue}");
                        return 1;
                    }
            }
        }

        /// <summary>
        /// Table 16: Lookup function for Cropping System = RF_CS values.
        /// </summary>
        /// <param name="cropType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Table 16: Lookup function for Soil Texture = RF_TX values.
        /// </summary>
        /// <param name="soilTexture"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public double GetFactorForSoilTexture(SoilTexture soilTexture, Region region)
        {
            if (region == Region.WesternCanada)
            {
                // Note b: In the Prairies and the Peace River Section of BC, RF_TX = 1

                return 1;
            }

            if (soilTexture == SoilTexture.Fine)
            {
                return 2.55;
            }

            if (soilTexture == SoilTexture.Medium)
            {
                return 1;
            }

            if (soilTexture == SoilTexture.Coarse)
            {
                return 0.49;
            }

            const double defaultValue = 1;

            Trace.TraceError($"{nameof(Table_16_Soil_N2O_Emission_Factors_Provider)}.{nameof(GetFactorForSoilTexture)}: unknown value for {nameof(soilTexture)}: {soilTexture}, and {nameof(region)}: {region}. Returning {defaultValue}");

            return defaultValue;
        }
        /// <summary>
        /// Table 16: Lookup function for Tillage Practice = RF_TILL values.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="cropViewItem"></param>
        /// <returns></returns>
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

            Trace.TraceError($"{nameof(Table_16_Soil_N2O_Emission_Factors_Provider)}.{nameof(GetFactorForTillagePractice)}: unknown {nameof(region)}: {region}. Returning {defaultValue}");

            return defaultValue;
        }

        /// <summary>
        /// Table 16: Lookup function for N2O Reduction factor values = RF_AM
        /// </summary>
        /// <param name="soilReductionFactors"></param>
        /// <returns></returns>
        public double GetReductionFactorBasedOnApplicationMethod(SoilReductionFactors soilReductionFactors)
        {
            var defaultValue = 0d;
            switch (soilReductionFactors)
            {
                case SoilReductionFactors.ControlledRelease:
                    return 0.87;

                case SoilReductionFactors.NitrificationInhibitor:
                    return 0.90;

                case SoilReductionFactors.UreaseInhibitor:
                    return 1.14;

                case SoilReductionFactors.NitrificationAndUreaseInhibitor:
                    return 0.94;

                case SoilReductionFactors.None:
                    return 0;

                default:
                    Trace.TraceError($"{nameof(Table_16_Soil_N2O_Emission_Factors_Provider)}.{nameof(GetReductionFactorBasedOnApplicationMethod)} " +
                                     $":unknown Soil Reduction Factor: {nameof(soilReductionFactors)}, returning {defaultValue}");
                    return defaultValue;
            }
        }

        #endregion
    }
}
