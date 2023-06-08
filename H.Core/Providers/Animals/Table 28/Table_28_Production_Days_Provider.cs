using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals.Table_69;
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

            this.ReadFile(CsvResourceNames.AnimalSystemProductionDays);
        }

        #endregion

        #region Public Methods

        public ProductionDaysData GetBackgroundingData()
        {
            // Currently both heifers and steers have same values
            return _data.FirstOrDefault(x => x.AnimalType == AnimalType.BeefBackgrounderSteer);
        }

        public ProductionDaysData GetSwineData(AnimalType animalType, ComponentType componentType, ProductionStages? productionStage = null)
        {
            if (productionStage != null)
            {
                return _data.SingleOrDefault(x => x.AnimalType == animalType && x.ProductionStage == productionStage && x.ComponentType == componentType);
            }
            else
            {
                return _data.SingleOrDefault(x => x.AnimalType == animalType && x.ComponentType == componentType);
            }
        }

        public ProductionDaysData GetPoultryData(AnimalType animalType)
        {
            var result = _data.SingleOrDefault(x => x.AnimalType == animalType);

            if (result == null)
            {
                Trace.TraceError($"{nameof(Table_28_Production_Days_Provider)}.{nameof(GetPoultryData)}:" + $" animal type '{animalType.GetDescription()}' not found");

                return new ProductionDaysData();
            }
            else
            {
                return result;
            }
        }

        #endregion

        #region Private Methods

        private void ReadLines(List<string[]> fileLines)
        {
            foreach (var fileLine in fileLines.Skip(1))
            {
                var animalTypeString = fileLine[3];
                if (string.IsNullOrWhiteSpace(animalTypeString))
                {
                    continue;
                }

                var productionDaysData = new ProductionDaysData();

                var animtype = _animalTypeStringConverter.Convert(animalTypeString.ParseUntilOrDefault());
                productionDaysData.AnimalType = animtype;

                var componentTypeString = fileLine[1];
                if (string.IsNullOrWhiteSpace(componentTypeString) == false)
                {
                    var componentType = _componentTypeStringConverter.Convert(componentTypeString);
                    productionDaysData.ComponentType = componentType;
                }

                var productionStageString = fileLine[2];
                if (string.IsNullOrWhiteSpace(productionStageString) == false)
                {
                    var productionStageType = _productionStageStringConverter.Convert(productionStageString);
                    productionDaysData.ProductionStage = productionStageType;
                }

                var numberOfProductionDays = int.Parse(fileLine[4].ParseUntilOrDefault(), InfrastructureConstants.EnglishCultureInfo);
                var numberOfNonProductionDays = int.Parse(fileLine[5].ParseUntilOrDefault(), InfrastructureConstants.EnglishCultureInfo);

                productionDaysData.NumberOfDaysInProductionCycle = numberOfProductionDays;
                productionDaysData.NumberOfNonProductionDaysBetweenCycles = numberOfNonProductionDays;

                _data.Add(productionDaysData);
            }
        }

        private void ReadFile(CsvResourceNames resourceName)
        {
            var fileLines = CsvResourceReader.GetFileLines(resourceName).ToList();

            this.ReadLines(fileLines);
        }

        #endregion
    }
}