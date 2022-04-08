#region Imports

using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 39.
    /// 
    /// Default methane conversion factors, N2O emission factors, and leaching fractions by livestock group, manure handling system, and climate.
    /// </summary>
    public class AnimalEmissionFactorProvider_Table_39 : IEmissionDataProvider
    {
        #region Fields

        private readonly MethaneConversionFactorsByClimateZone_Table_40 _methaneConversionFactorsByClimateZone = new MethaneConversionFactorsByClimateZone_Table_40();

        #endregion

        public AnimalEmissionFactorProvider_Table_39()
        {
            HTraceListener.AddTraceListener();
        }

        #region Public Methods

        public IEmissionData GetFactors(ManureStateType manureStateType,
            ComponentCategory componentCategory,
            double meanAnnualPrecipitation,
            double meanAnnualTemperature,
            double meanAnnualEvapotranspiration,
            double beddingRate,
            AnimalType animalType, Farm farm)
        {
            var climateDependentMethaneConversionFactor = _methaneConversionFactorsByClimateZone.GetByClimateAndHandlingSystem(
                manureStateType: manureStateType,
                meanAnnualTemperature: meanAnnualTemperature,
                meanAnnualPrecipitation: meanAnnualPrecipitation,
                meanAnnualPotentialEvapotranspiration: meanAnnualEvapotranspiration);

            var climateDependentEmissionFactorForVolatilization = this.GetEmissionFactorForVolatilizationBasedOnClimate(
                precipitation: meanAnnualPrecipitation,
                evapotranspiration: meanAnnualEvapotranspiration);

            var climateDependentDirectEmissionFactor = this.GetDirectEmissionFactorBasedOnClimate(
                precipitation: meanAnnualPrecipitation,
                evapotranspiration: meanAnnualEvapotranspiration);

            var region = farm.Province.GetRegion();
            var soilTexture = farm.DefaultSoilData.SoilTexture;

            switch (componentCategory)
            {
                case ComponentCategory.BeefProduction:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.Pasture:
                            case ManureStateType.Paddock:
                            case ManureStateType.Range:
                                {
                                    var factors =  new EmissionData
                                    {
                                        MethaneConversionFactor = 0.0047,
                                        N20DirectEmissionFactor = climateDependentDirectEmissionFactor,
                                        VolatilizationFraction = 0.21,
                                        EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                        EmissionFactorLeach = 0.011
                                    };

                                    if (region == Region.WesternCanada)
                                    {
                                        factors.N20DirectEmissionFactor = 0.0006;
                                    }
                                    else
                                    {
                                        if (soilTexture == SoilTexture.Fine)
                                        {
                                            factors.N20DirectEmissionFactor = 0.0078;
                                        }
                                        else if (soilTexture == SoilTexture.Medium)
                                        {
                                            factors.N20DirectEmissionFactor = 0.0062;
                                        }
                                        else
                                        {
                                            factors.N20DirectEmissionFactor = 0.0047;
                                        }
                                    }

                                    return factors;
                                }


                            case ManureStateType.SolidStorage:
                                return new EmissionData
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.CompostIntensive:
                                return new EmissionData
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.65,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.CompostPassive:
                                return new EmissionData
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.60,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.DeepBedding:
                                return new EmissionData
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.25,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.035,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.AnaerobicDigester:
                                return new EmissionData
                                {
                                    MethaneConversionFactor = 0.01,
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.0,
                                    EmissionFactorLeach = 0.0011
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(AnimalEmissionFactorProvider_Table_39)}.{nameof(AnimalEmissionFactorProvider_Table_39.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new EmissionData();
                        }
                    }

                case ComponentCategory.Dairy:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.Pasture:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.0047,
                                    N20DirectEmissionFactor = climateDependentDirectEmissionFactor,
                                    VolatilizationFraction = 0.21,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.DailySpread:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.0,
                                    VolatilizationFraction = 0.07,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.SolidStorage:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.3,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostIntensive:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.5,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostPassive:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.DeepBedding:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.25,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.035,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidCrust:
                                return new EmissionData()
                                {
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.3,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidNoCrust:
                                return new EmissionData()
                                {
                                    N20DirectEmissionFactor = 0.0,
                                    VolatilizationFraction = 0.48,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidWithSolidCover:
                                return new EmissionData()
                                {
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.AnaerobicDigester:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.01,
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(AnimalEmissionFactorProvider_Table_39)}.{nameof(AnimalEmissionFactorProvider_Table_39.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new EmissionData();
                        }
                    }

                case ComponentCategory.Swine:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.SolidStorage:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidWithNaturalCrust:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.0,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.3,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidNoCrust:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.0,
                                    N20DirectEmissionFactor = 0.0,
                                    VolatilizationFraction = 0.48,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidWithSolidCover:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.0,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostIntensive:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.5,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostPassive:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.AnaerobicDigester:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.01,
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.DeepPit:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.3514,
                                    N20DirectEmissionFactor = 0.002,
                                    VolatilizationFraction = 0.25,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(AnimalEmissionFactorProvider_Table_39)}.{nameof(AnimalEmissionFactorProvider_Table_39.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new EmissionData();
                        }
                    }

                case ComponentCategory.Sheep:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.Pasture:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.0047,
                                    N20DirectEmissionFactor = 0.003,
                                    VolatilizationFraction = 0.21,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011
                                };

                            case ManureStateType.SolidStorage:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.12,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011
                                };

                            case ManureStateType.CompostIntensive:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.20,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.011
                                };

                            case ManureStateType.CompostPassive:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.18,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.011
                                };

                            case ManureStateType.DeepBedding:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.4,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.035,
                                    EmissionFactorLeach = 0.011
                                };
                            case ManureStateType.AnaerobicDigester:
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.01,
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(AnimalEmissionFactorProvider_Table_39)}.{nameof(AnimalEmissionFactorProvider_Table_39.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new EmissionData();
                        }
                    }

                case ComponentCategory.Poultry:
                    {
                        if (manureStateType == ManureStateType.AnaerobicDigester)
                        {
                            return new EmissionData()
                            {
                                MethaneConversionFactor = 0.01,
                                N20DirectEmissionFactor = 0.0006,
                                VolatilizationFraction = 0.1,
                                EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                LeachingFraction = 0,
                                EmissionFactorLeach = 0.011,
                            };
                        }

                        if (manureStateType == ManureStateType.SolidStorage)
                        {
                            if (beddingRate > 0)
                            {
                                // Bedding with litter
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.015,
                                    N20DirectEmissionFactor = 0.001,
                                    VolatilizationFraction = 0.4,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };
                            }
                            else
                            {
                                // Bedding without litter
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.015,
                                    N20DirectEmissionFactor = 0.001,
                                    VolatilizationFraction = 0.48,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };
                            }
                        }

                        System.Diagnostics.Trace.TraceError(
                            $"{nameof(AnimalEmissionFactorProvider_Table_39)}.{nameof(AnimalEmissionFactorProvider_Table_39.GetFactors)}" +
                            $": Unable to get data for manure state type: {manureStateType}." +
                            $" Returning default value.");

                        return new EmissionData();
                    }

                case ComponentCategory.OtherLivestock:
                    {
                        if (animalType == AnimalType.Bison && manureStateType == ManureStateType.Pasture)
                        {
                            // Only situation where the EF_direct differs for the other animal groups category
                            return new EmissionData()
                            {
                                MethaneConversionFactor = 0.0047,
                                N20DirectEmissionFactor = climateDependentDirectEmissionFactor,
                                VolatilizationFraction = 0.21,
                                EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                EmissionFactorLeach = 0.011,
                            };
                        }
                        else
                        {
                            if (manureStateType == ManureStateType.SolidStorage)
                            {
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.12,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011,
                                };
                            }
                            else
                            {
                                // Pasture/range/paddock
                                return new EmissionData()
                                {
                                    MethaneConversionFactor = 0.0047,
                                    N20DirectEmissionFactor = 0.003,
                                    VolatilizationFraction = 0.21,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011,
                                };
                            }
                        }
                    }

                // Unknown component category (or no values for category yet)
                default:
                    {
                        System.Diagnostics.Trace.TraceError($"{nameof(AnimalEmissionFactorProvider_Table_39)}.{nameof(AnimalEmissionFactorProvider_Table_39.GetFactors)}" +
                                                            $": Unable to get data for manure state type '{manureStateType.GetDescription()}' and component category '{componentCategory.GetDescription()}'." +
                                                            $" Returning default value.");
                        return new EmissionData();
                    }
            }
        }

        #endregion

        #region Private Methods

        private double GetEmissionFactorForVolatilizationBasedOnClimate(double precipitation, double evapotranspiration)
        {
            if (precipitation > evapotranspiration)
            {
                // Wet
                return 0.014;
            }
            else
            {
                // Dry
                return 0.005;
            }
        }

        private double GetDirectEmissionFactorBasedOnClimate(
            double precipitation,
            double evapotranspiration)
        {
            if (precipitation > evapotranspiration)
            {
                // Wet
                return 0.006;
            }
            else
            {
                // Dry
                return 0.002;
            }
        }




        #endregion
    }
}