using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Core.Models;
using H.Infrastructure;

namespace H.Core.Providers.Animals.Table_28
{
    public class Table_28_Production_Days_Provider : ProviderBase, ITable_28_Production_Days_Provider
    {
        #region Fields

        private readonly List<ProductionDaysData> _data;

        #endregion

        #region Constructors

        public Table_28_Production_Days_Provider()
        {
            _data = new List<ProductionDaysData>();

            ReadFile(CsvResourceNames.AnimalSystemProductionDays);
        }

        #endregion

        #region Public Methods

        public bool HasProductionCycle(
            AnimalType animalType,
            ProductionStages productionStage,
            ComponentType? componentType = null)
        {
            var result = GetData(animalType, productionStage, componentType);

            return result.EmissionsShouldBeScaled;
        }

        public ProductionDaysData GetData(
            AnimalType animalType,
            ProductionStages productionStage,
            ComponentType? componentType = null)
        {
            var lookupType = animalType;

            if (animalType == AnimalType.SwineBoar) lookupType = AnimalType.SwineGrower;

            ProductionDaysData result = null;
            if (lookupType == AnimalType.SwinePiglets)
                // We only need the production stage when considering piglet groups
                result = _data.SingleOrDefault(x => x.AnimalType == lookupType &&
                                                    x.ProductionStage == productionStage &&
                                                    x.ComponentType == componentType);
            else if (lookupType.IsChickenType())
                result = _data.SingleOrDefault(x => x.AnimalType == lookupType);
            else
                result = _data.SingleOrDefault(x => x.AnimalType == lookupType &&
                                                    x.ComponentType == componentType);

            if (result == null)
            {
                Trace.TraceError($"{nameof(Table_28_Production_Days_Provider)}.{nameof(GetData)}:" +
                                 $" no data found for animal type: '{animalType}, production stage: {productionStage}, component type: {componentType}'");

                return new ProductionDaysData();
            }

            return result;
        }

        #endregion

        #region Private Methods

        private void ReadLines(List<string[]> fileLines)
        {
            foreach (var fileLine in fileLines.Skip(1))
            {
                var animalTypeString = fileLine[3];
                if (string.IsNullOrWhiteSpace(animalTypeString)) continue;

                var productionDaysData = new ProductionDaysData();

                var animalType = _animalTypeStringConverter.Convert(animalTypeString.ParseUntilOrDefault());
                productionDaysData.AnimalType = animalType;

                var componentTypeString = fileLine[1];
                if (string.IsNullOrWhiteSpace(componentTypeString) == false)
                {
                    var componentType = _componentTypeStringConverter.Convert(componentTypeString);
                    productionDaysData.ComponentType = componentType;
                }

                productionDaysData.ProductionStage = _productionStageStringConverter.Convert(fileLine[2]);

                var numberOfProductionDays = int.Parse(fileLine[4].ParseUntilOrDefault(),
                    InfrastructureConstants.EnglishCultureInfo);
                var numberOfNonProductionDays = int.Parse(fileLine[5].ParseUntilOrDefault(),
                    InfrastructureConstants.EnglishCultureInfo);

                productionDaysData.NumberOfDaysInProductionCycle = numberOfProductionDays;
                productionDaysData.NumberOfNonProductionDaysBetweenCycles = numberOfNonProductionDays;

                // If there is an entry in the table then callers need to scale up emissions
                productionDaysData.EmissionsShouldBeScaled = true;

                _data.Add(productionDaysData);
            }
        }

        private void ReadFile(CsvResourceNames resourceName)
        {
            var fileLines = CsvResourceReader.GetFileLines(resourceName).ToList();

            ReadLines(fileLines);
        }

        #endregion
    }
}