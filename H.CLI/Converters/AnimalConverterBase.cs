using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.CLI.UserInput;
using System.Text;
using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.CLI.ComponentKeys;

namespace H.CLI.Converters
{
    public abstract class AnimalConverterBase
    {
        #region Fields
        
        protected readonly ComponentConverterHandler _componentConverterHandler = new ComponentConverterHandler();

        #endregion

        #region Properties

        public List<ComponentBase> Components { get; set; } = new List<ComponentBase>();

        #endregion

        #region Abstract Methods

        public abstract AnimalKeyBase GetHeaders();
        protected abstract void PopulateRowData(AnimalComponentBase component, AnimalGroup animalGroup, ManagementPeriod managementPeriod, List<string> row);

        #endregion

        #region Public Methods

        public string SetTemplateCSVFileBasedOnExportedFarm(
           string path,
           ComponentBase component,
           bool writeToPath = true)
        {
            var animalGroups = this.BuildStringManagementRow((AnimalComponentBase)component);

            var columnSeparator = CLILanguageConstants.Delimiter;
            var filePath = path + @"\" + component.Name + CLILanguageConstants.DefaultInputFileExtension;
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in this.GetHeaders().Keys)
            {
                var convertedKey = keyValuePair.Key.Trim();
                stringBuilder.Append(convertedKey + columnSeparator);
            }

            stringBuilder.Append(Environment.NewLine);

            foreach (var animalGroup in animalGroups)
            {
                foreach (var managementPeriodProperty in animalGroup)
                {
                    stringBuilder.Append(managementPeriodProperty + columnSeparator);
                }

                stringBuilder.AppendLine();
            }

            if (writeToPath)
            {
                File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
            }

            return stringBuilder.ToString();
        }

        protected ManagementPeriod BuildManagementPeriod<T>(AnimalGroup animalGroup, T inputRow) where T : AnimalTemporaryInputBase
        {
            var managementPeriod = new ManagementPeriod
            {
                Name = inputRow.Name,
                AnimalGroupGuid = animalGroup.Guid,
                AnimalType = animalGroup.GroupType,
                Start = inputRow.ManagementPeriodStartDate,
                NumberOfDays = inputRow.ManagementPeriodDays,
                Duration = TimeSpan.FromDays(inputRow.ManagementPeriodDays),
                NumberOfAnimals = inputRow.NumberOfAnimals,
            };

            return managementPeriod;
        }

        protected virtual ManureDetails BuildManureDetails<T>(T inputRow) where T : AnimalTemporaryInputBase
        {
            var manureDetails = new ManureDetails
            {
                N2ODirectEmissionFactor = inputRow.N2ODirectEmissionFactor,
                VolatilizationFraction = inputRow.VolatilizationFraction,
                YearlyEntericMethaneRate = inputRow.YearlyEntericMethaneRate,
                YearlyManureMethaneRate = inputRow.YearlyManureMethaneRate,
                NitrogenExretionRate = inputRow.YearlyNitrogenExcretionRate,
                DailyManureMethaneEmissionRate = inputRow.DailyManureMethaneEmissionRate,
                MethaneProducingCapacityOfManure = inputRow.MethaneProducingCapacityOfManure,
                MethaneConversionFactor = inputRow.MethaneConversionFactor,
                VolatileSolids = inputRow.VolatileSolids,
            };

            return manureDetails;
        }

        protected virtual AnimalGroup BuildAnimalGroup<T>(IGrouping<string, T> inputRows) where T : AnimalTemporaryInputBase
        {
            var animalGroup = new AnimalGroup();

            foreach (var inputRow in inputRows)
            {
                animalGroup.Name = inputRow.GroupName;
                animalGroup.GroupType = inputRow.GroupType;

                var managementPeriod = this.BuildManagementPeriod(animalGroup, inputRow);
                managementPeriod.ManureDetails = this.BuildManureDetails(inputRow);

                animalGroup.ManagementPeriods.Add(managementPeriod);
            }

            return animalGroup;
        }

        protected AnimalComponentBase BuildComponent<T>(List<IComponentTemporaryInput> inputFile) where T : AnimalTemporaryInputBase
        {
            var componentType = inputFile.First().ComponentType.ToString();
            var component = _componentConverterHandler.GetAnimalComponentFromComponentTypeString(componentType);
            component.Name = inputFile.First().Name;

            component.Guid = Guid.NewGuid();
            component.IsInitialized = true;

            // Need to group input rows (by animal group name) so that all management periods belonging to an animal group get assigned to that same group
            var inputRowsGroupedByAnimalGroup = inputFile.Cast<T>().GroupBy(inputRow => inputRow.GroupName);
            foreach (var animalGroupRow in inputRowsGroupedByAnimalGroup)
            {
                var animalGroup = this.BuildAnimalGroup<T>(animalGroupRow);
                component.Groups.Add(animalGroup);
            }

            return component;
        }

        public virtual List<List<string>> BuildStringManagementRow(AnimalComponentBase component)
        {
            var result = new List<List<string>>();

            foreach (var animalGroup in component.Groups)
            {
                foreach (var managementPeriod in animalGroup.ManagementPeriods)
                {
                    var dictionary = new List<string>();

                    this.PopulateRowData(component, animalGroup, managementPeriod, dictionary);

                    result.Add(dictionary);
                }
            }

            return result;
        }

        #endregion
    }
}