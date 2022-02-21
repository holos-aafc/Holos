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

namespace H.CLI.Converters
{
    public class OtherLiveStockConverter : IConverter
    {
        #region Properties

        public List<ComponentBase> OtherLivestockComponents { get; set; } = new List<ComponentBase>();
        private readonly ComponentConverterHandler _componentConverterHandler = new ComponentConverterHandler();

        #endregion

        #region Public Methods

        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> otherLivestockInputFileList, Farm farm)
        {
            // Loop over all of the other livestock component input files
            foreach (var inputFile in otherLivestockInputFileList)
            {
                var animalTypeOfFirstGroup = inputFile.First().GroupType;
                var componentName = inputFile.First().Name;
                var component = _componentConverterHandler.GetAnimalComponentFromAnimalType(animalTypeOfFirstGroup);

                component.Guid = Guid.NewGuid();
                component.Name = componentName;
                component.IsInitialized = true;

                // Need to group input rows (by animal group name) so that all management periods belonging to an animal group get assigned to that same group
                var inputRowsGroupedByAnimalGroup = inputFile.Cast<OtherLiveStockTemporaryInput>().GroupBy(inputRow => inputRow.GroupName);
                foreach (var inputRowGroup in inputRowsGroupedByAnimalGroup)
                {
                    var animalGroup = new AnimalGroup();

                    // Each row is a management period belonging the animal group
                    foreach (var inputRow in inputRowGroup)
                    {
                        animalGroup.Name = inputRow.Name;
                        animalGroup.GroupType = inputRow.GroupType;
                        animalGroup.Name = inputRow.GroupName;

                        var managementPeriod = new ManagementPeriod()
                        {
                            Name = inputRow.ManagementPeriodName,
                            AnimalGroupGuid = animalGroup.Guid,
                            AnimalType = animalGroup.GroupType,
                            Start = inputRow.ManagementPeriodStartDate,
                            NumberOfDays = inputRow.ManagementPeriodDays,
                            Duration = TimeSpan.FromDays(inputRow.ManagementPeriodDays),

                            NumberOfAnimals = inputRow.NumberOfAnimals,

                            ManureDetails = new ManureDetails()
                            {
                                N2ODirectEmissionFactor = inputRow.N2ODirectEmissionFactor,
                                VolatilizationFraction = inputRow.VolatilizationFraction,
                                YearlyEntericMethaneRate = inputRow.YearlyEntericMethaneRate,
                                YearlyManureMethaneRate = inputRow.YearlyManureMethaneRate,
                                NitrogenExretionRate = inputRow.YearlyNitrogenExcretionRate,
                            }
                        };

                        animalGroup.ManagementPeriods.Add(managementPeriod);
                    }

                    component.Groups.Add(animalGroup);
                }

                this.OtherLivestockComponents.Add(component);
            }            

            return OtherLivestockComponents;
        }

        /// <summary>
        /// Creates a CLI input file based on the 'other' animal components found in an exported GUI farm file
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

                        stringBuilder.Append(managementPeriod.ManureDetails.YearlyManureMethaneRate + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.NitrogenExretionRate + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.YearlyEntericMethaneRate + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.N2ODirectEmissionFactor + columnSeparator);
                        stringBuilder.Append(managementPeriod.ManureDetails.VolatilizationFraction + columnSeparator);

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
