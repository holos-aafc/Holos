using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;
using H.Core.Enumerations;
using H.Core.Providers.Feed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.UserInput;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.LandManagement.Fields;
using System.Globalization;
using H.CLI.ComponentKeys;

namespace H.CLI.Converters
{
    public class SheepConverter : AnimalConverterBase, IConverter
    {
        #region Properties

        #endregion

        #region Public Methods

        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> fileList, Farm farm)
        {
            foreach (var inputFile in fileList)
            {
                var component = this.BuildComponent<SheepTemporaryInput>(inputFile);

                this.Components.Add(component);
            }

            return Components;
        }

        protected override void PopulateRowData(AnimalComponentBase component, AnimalGroup animalGroup, ManagementPeriod managementPeriod, List<string> row)
        {
            const string doubleFormat = "N2";

            row.Add(component.Name);
            row.Add(component.GetType().ToString());
            row.Add(animalGroup.Name);
            row.Add(animalGroup.GroupType.ToString());

            row.Add(managementPeriod.Name);
            row.Add(managementPeriod.Start.ToString("d"));
            row.Add(managementPeriod.Duration.Days.ToString());
            row.Add(managementPeriod.NumberOfAnimals.ToString());

            row.Add(managementPeriod.NumberOfYoungAnimals.ToString());
            row.Add(managementPeriod.StartWeight.ToString(doubleFormat));
            row.Add(managementPeriod.EndWeight.ToString(doubleFormat));
            row.Add(managementPeriod.PeriodDailyGain.ToString(doubleFormat));
            row.Add(managementPeriod.EnergyRequiredForWool.ToString(doubleFormat));
            row.Add(managementPeriod.WoolProduction.ToString(doubleFormat));
            row.Add(managementPeriod.EnergyRequiredForMilk.ToString(doubleFormat));

            row.Add(managementPeriod.DietAdditive.ToString());
            row.Add(managementPeriod.SelectedDiet.MethaneConversionFactor.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.MethaneConversionFactorAdjustment.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.DailyDryMatterFeedIntakeOfFeed.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.CrudeProtein.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Forage.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.TotalDigestibleNutrient.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Starch.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Fat.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.MetabolizableEnergy.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Ndf.ToString(doubleFormat));

            row.Add(managementPeriod.GainCoefficientA.ToString(doubleFormat));
            row.Add(managementPeriod.GainCoefficientB.ToString(doubleFormat));

            row.Add(managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation.ToString(doubleFormat));
            row.Add(managementPeriod.HousingDetails.BaselineMaintenanceCoefficient.ToString(doubleFormat));

            row.Add(managementPeriod.ManureDetails.MethaneConversionFactor.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.N2ODirectEmissionFactor.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorVolatilization.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.VolatilizationFraction.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorLeaching.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.LeachingFraction.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.AshContentOfManure.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.MethaneProducingCapacityOfManure.ToString(doubleFormat));
        }

        public override AnimalKeyBase GetHeaders()
        {
            return new SheepKeys();
        }

        #endregion
    }
}
