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
using H.CLI.ComponentKeys;

namespace H.CLI.Converters
{
    public class SwineConverter : AnimalConverterBase, IConverter
    {
        #region Properties

        public List<ComponentBase> SwineComponents { get; set; } = new List<ComponentBase>();

        #endregion

        #region Public Methods

        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> fileList, Farm farm)
        {
            foreach (var inputFile in fileList)
            {
                var component = this.BuildComponent<SwineTemporaryInput>(inputFile);

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

            row.Add(managementPeriod.DietAdditive.ToString());
            row.Add(managementPeriod.SelectedDiet.DailyDryMatterFeedIntakeOfFeed.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.CrudeProtein.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Forage.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.TotalDigestibleNutrient.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Starch.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Fat.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.MetabolizableEnergy.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.Ndf.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.VolatileSolidsAdjustmentFactorForDiet.ToString(doubleFormat));
            row.Add(managementPeriod.SelectedDiet.NitrogenExcretionAdjustFactorForDiet.ToString(doubleFormat));

            row.Add(managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation.ToString(doubleFormat));
            row.Add(managementPeriod.HousingDetails.MaintenanceCoefficientModifiedByTemperature.ToString(doubleFormat));

            row.Add(managementPeriod.ManureDetails.MethaneConversionFactor.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.MethaneProducingCapacityOfManure.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.N2ODirectEmissionFactor.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorVolatilization.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.VolatilizationFraction.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorLeaching.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.LeachingFraction.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.AshContentOfManure.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.VolatileSolidExcretion.ToString(doubleFormat));
            row.Add(managementPeriod.ManureDetails.YearlyEntericMethaneRate.ToString(doubleFormat));
        }

        public override AnimalKeyBase GetHeaders()
        {
            return new SwineKeys();
        }

        #endregion
    }    
}

