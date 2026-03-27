using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;
using System.Collections.Generic;
using H.Core.Models;
using H.Core.Models.Animals;
using H.CLI.ComponentKeys;

namespace H.CLI.Converters
{
    public class SwineConverter : AnimalConverterBase, IConverter
    {
        #region Properties

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
            row.Add(component.Name);
            row.Add(component.GetType().ToString());
            row.Add(animalGroup.Name);
            row.Add(animalGroup.GroupType.ToString());

            row.Add(managementPeriod.Name);
            row.Add(managementPeriod.Start.ToString("d"));
            row.Add(managementPeriod.Duration.Days.ToString());
            row.Add(managementPeriod.NumberOfAnimals.ToString());
            row.Add(managementPeriod.ProductionStage.ToString());
            row.Add(managementPeriod.StartWeight.ToString(DoubleFormat));
            row.Add(managementPeriod.EndWeight.ToString(DoubleFormat));

            row.Add(managementPeriod.DietAdditive.ToString());
            row.Add(managementPeriod.SelectedDiet.DailyDryMatterFeedIntakeOfFeed.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.CrudeProtein.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Ash.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Forage.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.TotalDigestibleNutrient.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Starch.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Fat.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.MetabolizableEnergy.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Ndf.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.VolatileSolidsAdjustmentFactorForDiet.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.NitrogenExcretionAdjustFactorForDiet.ToString(DoubleFormat));

            row.Add(managementPeriod.HousingDetails.HousingType.ToString());
            row.Add(managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.MaintenanceCoefficientModifiedByTemperature.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.UserDefinedBeddingRate.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial.ToString(DoubleFormat));

            row.Add(managementPeriod.ManureDetails.MethaneConversionFactor.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.MethaneProducingCapacityOfManure.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.N2ODirectEmissionFactor.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorVolatilization.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.VolatilizationFraction.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorLeaching.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.LeachingFraction.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.AshContentOfManure.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.VolatileSolidExcretion.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.YearlyEntericMethaneRate.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.StateType.ToString());
        }

        public override AnimalKeyBase GetHeaders()
        {
            return new SwineKeys();
        }

        #endregion
    }    
}

