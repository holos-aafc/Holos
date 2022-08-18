#region Imports

using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 39. Default methane conversion factors, direct N2O emission factors, volatilization fractions and
    /// emission factors and leaching fractions and emission factors, by livestock group and manure handling
    /// system. For beef cattle, dairy cattle and broilers, layers and turkeys, the Fracvolatilization values are
    /// estimated by Holos 
    /// </summary>
    public class Table_39_Livestock_Emission_Conversion_Factors_Provider : IEmissionDataProvider
    {
        #region Fields

        private readonly Table_40_MCF_By_Climate_Livestock_MansureSystem_Provider _mcfByClimateZoneLivestockManureSystemProvider = new Table_40_MCF_By_Climate_Livestock_MansureSystem_Provider();

        #endregion

        public Table_39_Livestock_Emission_Conversion_Factors_Provider()
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
            var climateDependentMethaneConversionFactor = _mcfByClimateZoneLivestockManureSystemProvider.GetByClimateAndHandlingSystem(
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
                                    var factors =  new Table_39_Livestock_Emission_Conversion_Factors_Data
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
                                            // SoilTexture = Coarse
                                            // Footnote 1
                                            factors.N20DirectEmissionFactor = 0.0047;
                                        }
                                    }

                                    return factors;
                                }

                            case ManureStateType.SolidStorage:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.CompostIntensive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.65,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.CompostPassive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.60,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.DeepBedding:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.25,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.035,
                                    EmissionFactorLeach = 0.0011
                                };

                            case ManureStateType.AnaerobicDigester:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data
                                {
                                    MethaneConversionFactor = 0.01, // Footnote 4
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.0,
                                    EmissionFactorLeach = 0.0011
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider)}.{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data();
                        }
                    }

                case ComponentCategory.Dairy:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.Pasture:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.0047,
                                    N20DirectEmissionFactor = climateDependentDirectEmissionFactor,
                                    VolatilizationFraction = 0.21,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.DailySpread:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.0,
                                    VolatilizationFraction = 0.07,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.SolidStorage:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.3,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostIntensive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.5,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostPassive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.DeepBedding:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.25,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.035,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidCrust:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.3,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidNoCrust:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    N20DirectEmissionFactor = 0.0,
                                    VolatilizationFraction = 0.48,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidWithSolidCover:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.AnaerobicDigester:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.01, // Footnote 4
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider)}.{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data();
                        }
                    }

                case ComponentCategory.Swine:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.SolidStorage:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidWithNaturalCrust:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.0,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.3,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidNoCrust:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.0,
                                    N20DirectEmissionFactor = 0.0,
                                    VolatilizationFraction = 0.48,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.LiquidWithSolidCover:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.0,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.1,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostIntensive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.5,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.CompostPassive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.45,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.AnaerobicDigester:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.01, // Footnote 4
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1, // Footnote 5
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            case ManureStateType.DeepPit:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.3514,
                                    N20DirectEmissionFactor = 0.002,
                                    VolatilizationFraction = 0.25,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    EmissionFactorLeach = 0.011,
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider)}.{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data();
                        }
                    }

                case ComponentCategory.Sheep:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.Pasture:
                            case ManureStateType.Paddock:
                            case ManureStateType.Range:
                                {
                                    var factors = new Table_39_Livestock_Emission_Conversion_Factors_Data
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
                                            // SoilTexture = Coarse
                                            // Footnote 1
                                            factors.N20DirectEmissionFactor = 0.0047;
                                        }
                                    }

                                    return factors;
                                }

                            case ManureStateType.SolidStorage:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.12,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.011
                                };

                            case ManureStateType.CompostIntensive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.20,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.06,
                                    EmissionFactorLeach = 0.011
                                };

                            case ManureStateType.CompostPassive:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.005,
                                    VolatilizationFraction = 0.18,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.04,
                                    EmissionFactorLeach = 0.011
                                };

                            case ManureStateType.DeepBedding:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.4,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.035,
                                    EmissionFactorLeach = 0.011
                                };
                            case ManureStateType.AnaerobicDigester:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.01, // Footnote 4
                                    N20DirectEmissionFactor = 0.0006,
                                    VolatilizationFraction = 0.1, // Footnote 5
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider)}.{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data();
                        }
                    }

                case ComponentCategory.Poultry:
                    {
                        if (manureStateType == ManureStateType.AnaerobicDigester)
                        {
                            return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                            {
                                MethaneConversionFactor = 0.01, // Footnote 7
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
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.015, // Footnote 7
                                    N20DirectEmissionFactor = 0.001, // Footnote 7
                                    VolatilizationFraction = 0.4,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };
                            }
                            else
                            {
                                // Bedding without litter
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data()
                                {
                                    MethaneConversionFactor = 0.015, // Footnote 7
                                    N20DirectEmissionFactor = 0.001, // Footnote 7
                                    VolatilizationFraction = 0.48,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0,
                                    EmissionFactorLeach = 0.011,
                                };
                            }
                        }

                        System.Diagnostics.Trace.TraceError(
                            $"{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider)}.{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider.GetFactors)}" +
                            $": Unable to get data for manure state type: {manureStateType}." +
                            $" Returning default value.");

                        return new Table_39_Livestock_Emission_Conversion_Factors_Data();
                    }

                case ComponentCategory.OtherLivestock:
                    {
                        switch (manureStateType)
                        {
                            case ManureStateType.Pasture:
                            case ManureStateType.Paddock:
                            case ManureStateType.Range:
                                {
                                    var factors = new Table_39_Livestock_Emission_Conversion_Factors_Data
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
                                            // SoilTexture = Coarse
                                            // Footnote 1
                                            factors.N20DirectEmissionFactor = 0.0047;
                                        }
                                    }

                                    return factors;
                                }

                            case ManureStateType.SolidStorage:
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data
                                {
                                    MethaneConversionFactor = climateDependentMethaneConversionFactor,
                                    N20DirectEmissionFactor = 0.01,
                                    VolatilizationFraction = 0.12,
                                    EmissionFactorVolatilization = climateDependentEmissionFactorForVolatilization,
                                    LeachingFraction = 0.02,
                                    EmissionFactorLeach = 0.0011
                                };

                            default:
                                System.Diagnostics.Trace.TraceError(
                                    $"{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider)}.{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider.GetFactors)}" +
                                    $": Unable to get data for manure state type: {manureStateType}." +
                                    $" Returning default value.");
                                return new Table_39_Livestock_Emission_Conversion_Factors_Data();
                        }
                    }

                // Unknown component category (or no values for category yet)
                default:
                    {
                        System.Diagnostics.Trace.TraceError($"{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider)}.{nameof(Table_39_Livestock_Emission_Conversion_Factors_Provider.GetFactors)}" +
                                                            $": Unable to get data for manure state type '{manureStateType.GetDescription()}' and component category '{componentCategory.GetDescription()}'." +
                                                            $" Returning default value.");
                        return new Table_39_Livestock_Emission_Conversion_Factors_Data();
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
                // Footnote 2
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

        #region Footnotes

        /*
         * Source: IPCC (2019): Table 10.17 (MCF); Table 10.21 (EFdirect, except for pasture/range/paddock);
           Table 11.1 (EFdirect for pasture/range/paddock; Table 10.22 (Fracvolatilization (except for pasture/range/paddock ); Fracleach);
           Table 11.3 (Fracvolatilization for pasture/range/paddock; EFleach))
         *
         * 1 * Direct N2O EFs for dung and urine deposited on pasture are derived from the National Inventory Report (ECCC, 2022),
               Table A3.4–33 for all livestock groups. Note: All emissions related to manure deposited by livestock directly on pasture are accounted for in Section 5.
         *
         * 2 * In IPCC (2019), Table 11.3: Disaggregation by climate for EFvolatilization (based on long-term averages): Wet climates 
               occur in temperate and boreal zones where the ratio of annual precipitation (P):potential evapotranspiration (PE) >1; Dry climates occur in temperate and 
               boreal zones where the ratio of annual P:PE <1
         *
         * 3 * Fracleach for pasture/range/paddock calculated in Eq. 2.5.3 1
         *
         * 4 * Low leakage, high quality gastight storage, best complete industrial technology
         *
         * 5 * In Table 10.22 (IPCC, 2019), a range is given: 0.05-0.50 for all livestock groups. Nitrogen losses from digestate storage 
               strongly depend on the digestate composition and on the storage cover. These ranges also apply to co-digestates. The lower 
               range of 0.05 losses is valid for digestate with a high dry matter content and a cover. For the Holos model, we have assumed 
               for all livestock groups that anaerobic digestion systems are closed but with a low dry matter content and thus have used a value of 0.1 kg NH3-N (kg N)-1
         *
         * 6 * Value is for solid storage – covered/compacted
         *
         * 7 * MCF and EFdirect values for poultry manure with and without litter

         */
        #endregion
    }
}