﻿using H.CLI.Interfaces;
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
using System.Globalization;
using H.CLI.ComponentKeys;

namespace H.CLI.Converters
{
    public class OtherLiveStockConverter : AnimalConverterBase, IConverter
    {
        #region Properties

        #endregion

        #region Public Methods

        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> fileList, Farm farm)
        {
            foreach (var inputFile in fileList)
            {
                var component = this.BuildComponent<OtherLiveStockTemporaryInput>(inputFile);

                this.Components.Add(component);
            }

            return Components;
        }

        protected override void PopulateRowData(AnimalComponentBase component, AnimalGroup animalGroup, ManagementPeriod managementPeriod, List<string> row)
        {
            row.Add(component.Name);
            row.Add(component.GetType().ToString());
            row.Add(animalGroup.Name);
            row.Add(animalGroup.GroupType.ToString());

            row.Add(managementPeriod.Name);
            row.Add(managementPeriod.Start.ToString("d"));
            row.Add(managementPeriod.Duration.Days.ToString());
            row.Add(managementPeriod.NumberOfAnimals.ToString());
            row.Add(managementPeriod.ProductionStage.ToString());

            row.Add(managementPeriod.ManureDetails.YearlyManureMethaneRate.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.NitrogenExretionRate.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.YearlyEntericMethaneRate.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.N2ODirectEmissionFactor.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.VolatilizationFraction.ToString(CultureInfo.InvariantCulture));

            row.Add(managementPeriod.ManureDetails.DailyManureMethaneEmissionRate.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.MethaneProducingCapacityOfManure.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.MethaneConversionFactor.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.VolatileSolids.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.EmissionFactorVolatilization.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.EmissionFactorLeaching.ToString(CultureInfo.InvariantCulture));
            row.Add(managementPeriod.ManureDetails.LeachingFraction.ToString(CultureInfo.InvariantCulture));
        }

        public override AnimalKeyBase GetHeaders()
        {
            return new OtherLivestockKeys();
        }

        #endregion
    }
}
