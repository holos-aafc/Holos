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
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using H.CLI.ComponentKeys;

namespace H.CLI.Converters
{
    public class BeefConverter : AnimalConverterBase, IConverter
    {
        #region Properties

        #endregion

        #region Public Methods

        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> fileList, Farm farm)
        {
            foreach (var inputFile in fileList)
            {
                var component = this.BuildComponent<BeefCattleTemporaryInput>(inputFile);

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
            row.Add(animalGroup.GroupPairingNumber.ToString());
            row.Add(managementPeriod.Start.ToString("d"));
            row.Add(managementPeriod.Duration.Days.ToString());
            row.Add(managementPeriod.NumberOfAnimals.ToString());
            row.Add(managementPeriod.ProductionStage.ToString());
            row.Add(managementPeriod.NumberOfYoungAnimals.ToString());
            row.Add(managementPeriod.AnimalsAreMilkFedOnly.ToString());

            row.Add(managementPeriod.StartWeight.ToString(DoubleFormat));
            row.Add(managementPeriod.EndWeight.ToString(DoubleFormat));
            row.Add(managementPeriod.PeriodDailyGain.ToString(DoubleFormat));
            row.Add(managementPeriod.MilkProduction.ToString(DoubleFormat));
            row.Add(managementPeriod.MilkFatContent.ToString(DoubleFormat));
            row.Add(managementPeriod.MilkProteinContentAsPercentage.ToString(DoubleFormat));

            row.Add(managementPeriod.SelectedDiet.Name.ToString());
            row.Add(managementPeriod.DietAdditive.ToString());
            row.Add(managementPeriod.SelectedDiet.MethaneConversionFactor.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.MethaneConversionFactorAdjustment.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.DailyDryMatterFeedIntakeOfFeed.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.CrudeProtein.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Forage.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.TotalDigestibleNutrient.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Ash.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Starch.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Fat.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.MetabolizableEnergy.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.Ndf.ToString(DoubleFormat));
            row.Add(managementPeriod.SelectedDiet.DietaryNetEnergyConcentration.ToString(DoubleFormat));

            row.Add(managementPeriod.HousingDetails.HousingType.ToString());
            row.Add(managementPeriod.GainCoefficient.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.UserDefinedBeddingRate.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation.ToString(DoubleFormat));
            row.Add(managementPeriod.HousingDetails.BaselineMaintenanceCoefficient.ToString(DoubleFormat));

            if (managementPeriod.HousingDetails.PastureLocation != null)
            {
                row.Add(managementPeriod.HousingDetails.PastureLocation.Guid.ToString());
            }
            else
            {
                row.Add(CLILanguageConstants.NotApplicableString);
            }

            row.Add(managementPeriod.ManureDetails.MethaneConversionFactor.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.N2ODirectEmissionFactor.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorVolatilization.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.VolatilizationFraction.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.EmissionFactorLeaching.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.LeachingFraction.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.AshContentOfManure.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.MethaneProducingCapacityOfManure.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized.ToString(DoubleFormat));
            row.Add(managementPeriod.ManureDetails.StateType.ToString());
            row.Add(managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage.ToString(DoubleFormat));
        }

        public override AnimalKeyBase GetHeaders()
        {
            return new BeefCattleKeys();
        }

        #endregion
    }
}
