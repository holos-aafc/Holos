#region Imports

using H.Core.Enumerations;
using H.Core.Tools;

#endregion

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 19
    /// </summary>
    public class BeefAndDairyCattleCoefficientDataProvider_Table_19 : IAnimalCoefficientDataProvider
    {
        public BeefAndDairyCattleCoefficientDataProvider_Table_19()
        {
            HTraceListener.AddTraceListener();
        }
        public AnimalCoefficientData GetCoefficientsByAnimalType(AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.BeefCalf:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefCalf,
                        BaselineMaintenanceCoefficient = CoreConstants.NotApplicable,
                        GainCoefficient = CoreConstants.NotApplicable,
                        DefaultInitialWeight = 39,
                        DefaultFinalWeight = 260
                    };
                }

                case AnimalType.BeefCowLactating:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefCowLactating,
                        BaselineMaintenanceCoefficient = 0.386,
                        GainCoefficient = 0.8,
                        DefaultInitialWeight = 610,
                        DefaultFinalWeight = 610
                    };
                }

                case AnimalType.BeefCowDry:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefCowDry,
                        BaselineMaintenanceCoefficient = 0.322,
                        GainCoefficient = 0.8,
                        DefaultInitialWeight = 610,
                        DefaultFinalWeight = 610
                    };
                }


                case AnimalType.BeefBulls:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefBulls,
                        BaselineMaintenanceCoefficient = 0.370,
                        GainCoefficient = 1.2,
                        DefaultInitialWeight = 820,
                        DefaultFinalWeight = 820
                    };
                }

                case AnimalType.BeefBackgrounderSteer:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefBackgrounderSteer,
                        BaselineMaintenanceCoefficient = 0.322,
                        GainCoefficient = 1,
                        DefaultInitialWeight = 250,
                        DefaultFinalWeight = 380
                    };
                }

                // Aklilu says these two animal groups have the same values
                case AnimalType.BeefBackgrounderHeifer:
                case AnimalType.BeefReplacementHeifers:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefBackgrounderHeifer,
                        BaselineMaintenanceCoefficient = 0.322,
                        GainCoefficient = 0.8,
                        DefaultInitialWeight = 240,
                        DefaultFinalWeight = 360
                    };
                }

                case AnimalType.BeefFinishingSteer:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefFinishingSteer,
                        BaselineMaintenanceCoefficient = 0.322,
                        GainCoefficient = 1.0,
                        DefaultInitialWeight = 310,
                        DefaultFinalWeight = 610
                    };
                }

                case AnimalType.BeefFinishingHeifer:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.BeefFinishingHeifer,
                        BaselineMaintenanceCoefficient = 0.322,
                        GainCoefficient = 0.8,
                        DefaultInitialWeight = 300,
                        DefaultFinalWeight = 580
                    };
                }

                case AnimalType.DairyLactatingCow:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.DairyLactatingCow,
                        BaselineMaintenanceCoefficient = 0.386,
                        GainCoefficient = 0.8,
                        DefaultInitialWeight = 687,
                        DefaultFinalWeight = 687,
                    };
                }

                case AnimalType.DairyDryCow:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.DairyDryCow,
                        BaselineMaintenanceCoefficient = 0.322,
                        GainCoefficient = 0.8,
                        DefaultInitialWeight = 687,
                        DefaultFinalWeight = 687,
                    };
                }

                case AnimalType.DairyHeifers:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.DairyHeifers,
                        BaselineMaintenanceCoefficient = 0.322,
                        GainCoefficient = 0.8,
                        DefaultInitialWeight = 637,
                        DefaultFinalWeight = 687,
                    };
                }

                case AnimalType.DairyBulls:
                {
                    return new AnimalCoefficientData()
                    {
                        AnimalType = AnimalType.DairyBulls,
                        BaselineMaintenanceCoefficient = 0.37,
                        GainCoefficient = 1.2,
                        DefaultInitialWeight = 1200,
                        DefaultFinalWeight = 1200,
                    };
                }

                default:
                {
                    var defaultValue = new AnimalCoefficientData()
                    {
                        BaselineMaintenanceCoefficient = 0,
                        GainCoefficient = 0,
                        DefaultInitialWeight = 0,
                        DefaultFinalWeight = 0,
                    };
                    System.Diagnostics.Trace.TraceError($"{nameof(BeefAndDairyCattleCoefficientDataProvider_Table_19)}.{nameof(BeefAndDairyCattleCoefficientDataProvider_Table_19.GetCoefficientsByAnimalType)}" +
                        $" unable to get data for animal type: {animalType}." +
                        $" Returning default value of {defaultValue}.");
                    return defaultValue;
                }
            }
        }
    }
}