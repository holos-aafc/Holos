using System.Diagnostics;
using AutoMapper.Configuration.Conventions;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 40.
    ///
    /// Fraction of organic N mineralized as TAN and the fraction of TAN immobilized to organic N and nitrified and
    /// denitrified during solid and liquid manure storage for beef and dairy cattle (based on TAN content) (Chai et al., 2014,2016).
    /// </summary>
    public class FractionOfOrganicNitrogenMineralizedAsTanProvider_Table_40
    {
        public FractionOfOrganicNitrogenMineralizedAsTanProvider_Table_40()
        {
            HTraceListener.AddTraceListener(); 
        }

        /// <summary>
        /// Returns fractional N data
        /// </summary>
        /// <param name="stateType">The manure handling system</param>
        /// <param name="animalType">The type of animal (beef, dairy, etc.)</param>
        /// <param name="fractionOfTanInLiquidManureStorageSystem">The fraction of excreted N in animal urine</param>
        public FractionOfOrganicNitrogenMineralizedData GetByStorageType(
            ManureStateType stateType, 
            AnimalType animalType, 
            double fractionOfTanInLiquidManureStorageSystem = 1)
        {
            if (animalType.IsBeefCattleType())
            {
                switch (stateType)
                {
                    // Solid-compost - beef
                    case ManureStateType.CompostIntensive:
                    case ManureStateType.CompostPassive:
                    case ManureStateType.Composted:
                        return new FractionOfOrganicNitrogenMineralizedData()
                        {
                            FractionImmobilized = 0,
                            FractionMineralized = 0.46,
                            FractionNitrified = 0.25,
                            FractionDenitrified = 0,

                            N2O_N = 0.033,
                            NO_N = 0.0033,
                            N2_N = 0.099,
                            N_Leached = 0.0575,
                        };

                    // Solid-stockpiled - beef
                    case ManureStateType.Solid:
                    case ManureStateType.DeepBedding:
                    case ManureStateType.SolidStorage:
                        return new FractionOfOrganicNitrogenMineralizedData()
                        {
                            FractionImmobilized = 0,
                            FractionMineralized = 0.28,
                            FractionNitrified = 0.125,
                            FractionDenitrified = 0,

                            N2O_N = 0.033,
                            NO_N = 0.0033,
                            N2_N = 0.099,
                            N_Leached = 0.0575,
                        };
                }
            }
            else if (animalType.IsDairyCattleType())
            {
                switch (stateType)
                {
                    // Solid-compost - dairy
                    case ManureStateType.CompostIntensive:
                    case ManureStateType.CompostPassive:
                    case ManureStateType.Composted:
                        return new FractionOfOrganicNitrogenMineralizedData()
                        {
                            FractionImmobilized = 0,
                            FractionMineralized = 0.46,
                            FractionNitrified = 0.282,
                            FractionDenitrified = 0.152,

                            N2O_N = 0.037,
                            NO_N = 0.0037,
                            N2_N = 0.111,
                            N_Leached = 0.13,
                        };

                    // Solid-stockpiled - dairy
                    case ManureStateType.Solid:
                    case ManureStateType.DeepBedding:
                    case ManureStateType.SolidStorage:
                        return new FractionOfOrganicNitrogenMineralizedData()
                        {
                            FractionImmobilized = 0,
                            FractionMineralized = 0.28,
                            FractionNitrified = 0.141,
                            FractionDenitrified = 0.076,

                            N2O_N = 0.0185,
                            NO_N = 0.0019,
                            N2_N = 0.0555,
                            N_Leached = 0.065,
                        };
                }
            }

            // Liquid systems for both beef and dairy
            switch (stateType)
            {
                // Liquid with natural crust
                case ManureStateType.LiquidCrust:
                case ManureStateType.SlurryWithNaturalCrust:
                case ManureStateType.LiquidWithNaturalCrust:
                case ManureStateType.PitLagoonNoCover:
                case ManureStateType.LiquidWithSolidCover:
                    return new FractionOfOrganicNitrogenMineralizedData()
                    {
                        FractionImmobilized = 0,
                        FractionMineralized = 0.1,
                        FractionNitrified = 0.021 / (fractionOfTanInLiquidManureStorageSystem > 0 ? fractionOfTanInLiquidManureStorageSystem : 1),
                        FractionDenitrified = 0.021 / (fractionOfTanInLiquidManureStorageSystem > 0 ? fractionOfTanInLiquidManureStorageSystem : 1),

                        N2O_N = 0.005 / (fractionOfTanInLiquidManureStorageSystem > 0 ? fractionOfTanInLiquidManureStorageSystem : 1),
                        NO_N = 0.0005 / (fractionOfTanInLiquidManureStorageSystem > 0 ? fractionOfTanInLiquidManureStorageSystem : 1),
                        N2_N = 0.015 / (fractionOfTanInLiquidManureStorageSystem > 0 ? fractionOfTanInLiquidManureStorageSystem : 1),
                        N_Leached = 0,
                    };

                // Liquid without natural crust
                case ManureStateType.LiquidNoCrust:
                case ManureStateType.LiquidSeparated:
                case ManureStateType.SlurryWithoutNaturalCrust:
                case ManureStateType.Slurry:
                    return new FractionOfOrganicNitrogenMineralizedData()
                    {
                        FractionImmobilized = 0,
                        FractionMineralized = 0.1,
                        FractionNitrified = 0.0,
                        FractionDenitrified = 0,

                        N2O_N = 0,
                        NO_N = 0,
                        N2_N = 0,
                        N_Leached = 0,
                    };
            }

            Trace.TraceError($"{nameof(FractionOfOrganicNitrogenMineralizedAsTanProvider_Table_40)}.{nameof(GetByStorageType)} unknown type {stateType.GetDescription()}. Returning default.");

            return new FractionOfOrganicNitrogenMineralizedData();
        }
    }
}