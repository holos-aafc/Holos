using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;
using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.UserInput;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Swine;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;
using Diet = H.Core.Providers.Feed.Diet;

namespace H.CLI.Converters
{
    public class SwineConverter : IConverter
    {
        #region Properties

        private readonly ComponentConverterHandler _componentConverterHandler = new ComponentConverterHandler();

        public List<ComponentBase> SwineComponents { get; set; } = new List<ComponentBase>();

        #endregion

        #region Public Methods

        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> swineInputFileList, Farm farm)
        {
            // Loop over all of the swine component input files
            foreach (var inputFile in swineInputFileList)
            {
                var animalTypeOfFirstGroup = inputFile.First().GroupType;
                var componentName = inputFile.First().Name;
                var component = _componentConverterHandler.GetAnimalComponentFromAnimalType(animalTypeOfFirstGroup);

                component.Guid = Guid.NewGuid();
                component.Name = componentName;
                component.IsInitialized = true;

                // Need to group input rows (by animal group name) so that all management periods belonging to an animal group get assigned to that same group
                var inputRowsGroupedByAnimalGroup = inputFile.Cast<SwineTemporaryInput>().GroupBy(inputRow => inputRow.GroupName);
                foreach (var inputRowGroup in inputRowsGroupedByAnimalGroup)
                {
                    var animalGroup = new AnimalGroup();

                    // Each row is a management period belonging the animal group
                    foreach (var inputRow in inputRowGroup)
                    {
                        animalGroup.Name = inputRow.Name;
                        animalGroup.GroupType = inputRow.GroupType;
                        animalGroup.Name = inputRow.GroupName;

                        var diet = new Diet()
                        {
                            CrudeProtein = inputRow.CrudeProtein,
                            Forage = inputRow.Forage,
                            TotalDigestibleNutrient = inputRow.TDN,
                            Starch = inputRow.Starch,
                            Fat = inputRow.Fat,
                            MetabolizableEnergy = inputRow.ME,
                            Ndf = inputRow.NDF,
                            NitrogenExcretionAdjustFactorForDiet = inputRow.NitrogenExcretionAdjusted,
                            VolatileSolidsAdjustmentFactorForDiet = inputRow.VolatileSolidAdjusted,
                        };

                        var managementPeriod = new ManagementPeriod()
                        {
                            Name = inputRow.ManagementPeriodName,
                            AnimalGroupGuid = animalGroup.Guid,
                            AnimalType = inputRow.GroupType,
                            Start = inputRow.ManagementPeriodStartDate,
                            Duration = TimeSpan.FromDays(inputRow.ManagementPeriodDays),
                            NumberOfDays = inputRow.ManagementPeriodDays,

                            NumberOfAnimals = inputRow.NumberOfAnimals,
                            DietAdditive = inputRow.DietAdditiveType,

                            SelectedDiet = diet,

                            FeedIntakeAmount = inputRow.FeedIntake,

                            HousingDetails = new HousingDetails()
                            {
                                HousingType = inputRow.HousingType,
                                ActivityCeofficientOfFeedingSituation = inputRow.CA,
                                BaselineMaintenanceCoefficient = inputRow.CFTemp,
                            },

                            ManureDetails = new ManureDetails()
                            {
                                N2ODirectEmissionFactor = inputRow.N2ODirectEmissionFactor,
                                VolatilizationFraction = inputRow.VolatilizationFraction,
                                AshContentOfManure = inputRow.AshContent,
                                MethaneConversionFactor = inputRow.MethaneConversionFactor,
                                VolatileSolidExcretion = inputRow.VolatileSolidsExcretion,
                                EmissionFactorLeaching = inputRow.EmissionFactorLeaching,
                                YearlyEntericMethaneRate = inputRow.YearlyEntericMethaneRate,
                                MethaneProducingCapacityOfManure = inputRow.MethaneProducingCapacityOfManure,
                                LeachingFraction = inputRow.FractionLeaching,
                                EmissionFactorVolatilization = inputRow.EmissionFactorVolatilization,
                            },
                        };

                        animalGroup.ManagementPeriods.Add(managementPeriod);
                    }

                    component.Groups.Add(animalGroup);
                }

                this.SwineComponents.Add(component);
            }
            
            return SwineComponents;
        }

        /// <summary>
        /// Creates a CLI input file based on the swine components found in an exported GUI farm file
        /// </summary>
        public string SetTemplateCSVFileBasedOnExportedFarm(string path,
                                                            Dictionary<string, ImperialUnitsOfMeasurement?> componentKeys,
                                                            ComponentBase component,
                                                            bool writeToPath = true)
        {
            var columnSeparator = CLILanguageConstants.Delimiter;
            var filePath = path + @"\" + component.Name + CLILanguageConstants.DefaultInputFileExtension;
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in componentKeys)
            {
                var convertedKey = keyValuePair.Key.Trim();
                stringBuilder.Append(convertedKey + columnSeparator);
            }

            stringBuilder.Append(Environment.NewLine);

            if (component is AnimalComponentBase animalComponent)
            {
                var animalGroups = animalComponent.Groups;                

                foreach (var animalGroup in animalGroups)
                {
                    foreach (var managementPeriod in animalGroup.ManagementPeriods)
                    {
                        stringBuilder.Append(component.Name + columnSeparator);
                        stringBuilder.Append(animalGroup.Name + columnSeparator);
                        stringBuilder.Append(animalGroup.GroupType + columnSeparator);

                        stringBuilder.Append(managementPeriod.Name + columnSeparator);
                        stringBuilder.Append(managementPeriod.Start.ToString("d") + columnSeparator);
                        stringBuilder.Append(managementPeriod.Duration.Days + columnSeparator);
                        stringBuilder.Append(managementPeriod.NumberOfAnimals + columnSeparator);
                        stringBuilder.Append(managementPeriod.DietAdditive + columnSeparator);
                        stringBuilder.Append(managementPeriod.FeedIntakeAmount + columnSeparator);

                        stringBuilder.Append(managementPeriod.SelectedDiet.CrudeProtein + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.Forage + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.TotalDigestibleNutrient + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.Starch + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.Fat + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.MetabolizableEnergy + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.Ndf + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.VolatileSolidsAdjustmentFactorForDiet + columnSeparator);
                        stringBuilder.Append(managementPeriod.SelectedDiet.NitrogenExcretionAdjustFactorForDiet + columnSeparator);

                        stringBuilder.Append(managementPeriod.HousingDetails.NameOfPastureLocation + columnSeparator);
                        stringBuilder.Append(managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation + columnSeparator);
                        stringBuilder.Append(managementPeriod.HousingDetails.BaselineMaintenanceCoefficient + columnSeparator);

                        stringBuilder.Append(managementPeriod.ManureDetails.MethaneConversionFactor + columnSeparator);
                        stringBuilder.Append("0" + columnSeparator); // TODO: Methane conversion factor adjusted (should remove this?)
                        stringBuilder.Append(managementPeriod.ManureDetails.MethaneProducingCapacityOfManure + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.N2ODirectEmissionFactor + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.EmissionFactorVolatilization + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.VolatilizationFraction + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.EmissionFactorLeaching + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.LeachingFraction + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.AshContentOfManure + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.VolatileSolidExcretion + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.YearlyEntericMethaneRate + columnSeparator);

                        stringBuilder.AppendLine();
                    }
                }
            }

            if (writeToPath)
            {
                File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
            }

            return stringBuilder.ToString();
        } 

        #endregion
    }    
}

