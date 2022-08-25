using System.Diagnostics;
using AutoMapper.Configuration.Conventions;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 47.
    ///
    /// Fraction of organic N mineralized as TAN and the fraction of TAN immobilized to organic N and nitrified and
    /// denitrified during solid and liquid manure storage for beef and dairy cattle (based on TAN content) (Chai et al., 2014,2016).
    /// </summary>
    public class Table_47_Fraction_OrganicN_Mineralized_As_Tan_Provider
    {
        public Table_47_Fraction_OrganicN_Mineralized_As_Tan_Provider()
        {
            HTraceListener.AddTraceListener(); 
        }

        /// <summary>
        /// Returns fractional N data
        /// </summary>
        /// <param name="stateType">The manure handling system</param>
        /// <param name="animalType">The type of animal (beef, dairy, etc.)</param>
        /// <param name="fractionOfTanInLiquidManureStorageSystem">The fraction of excreted N in animal urine. Footnote 4</param>
        public FractionOfOrganicNitrogenMineralizedData GetByStorageType(
            ManureStateType stateType, 
            AnimalType animalType, 
            double fractionOfTanInLiquidManureStorageSystem = 1)
        {
            if (animalType.IsBeefCattleType())
            {
                // FracMineralized = Footnote 1.
                switch (stateType)
                {
                    // Solid-compost - beef
                    // Footnote 2
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
                    // Footnote 3
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
                    // Footnote 2
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
                    // Footnote 3
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
                // Footnote 5, 7
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
                // Footnote 6, 7
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

            Trace.TraceError($"{nameof(Table_47_Fraction_OrganicN_Mineralized_As_Tan_Provider)}.{nameof(GetByStorageType)} unknown type {stateType.GetDescription()}. Returning default.");

            return new FractionOfOrganicNitrogenMineralizedData();


            #region Footnotes

            /*
             * Footnote 1: Mineralization of organic N (fecal N and bedding N)
             * Footnote 2: Solid manure composted for ≥ 10 months; data from Chai et al. (2014)
             * Footnote 3: Solid manure stockpiled for ≥ 4 months; data from Chai et al. (2014)
             * Footnote 4: FracurinaryN is the fraction of TAN in the liquid manure storage system
             * Footnote 5: Nitrification of TAN in liquid manure with natural crust (formed from manure, bedding, or waste forage) was
                considered since the natural crust can be assumed as similar to solid manure (stockpile) in terms of being aerobic. 
                The N2O-N emission factor for liquid manure with a natural crust is 0.005 of total N IPCC (2006), which can be expressed as 
                the TAN based EFs knowing the TAN fraction of stored manure.
             * Footnote 6: Nitrification of TAN in liquid manure with no natural crust is assumed to be zero because of anaerobic conditions
             * Footnote 7: All nitrified TAN (nitrate-N) was assumed to be denitrified (no leaching, runoff) in liquid systems.

             */

            #endregion
        }
    }
}