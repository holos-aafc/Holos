﻿using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;

namespace H.CLI.ComponentKeys
{
    public class SheepKeys : AnimalKeyBase, IComponentKeys
    {
        #region Constructors
        
        public SheepKeys() : base()
        {
            base.Keys.Add(Properties.Resources.Key_Name, null);
            base.Keys.Add(H.Core.Properties.Resources.ComponentType, null);
            base.Keys.Add(Properties.Resources.GroupName, null);
            base.Keys.Add(Properties.Resources.GroupType, null);

            base.Keys.Add(Properties.Resources.ManagementPeriodName, null);
            base.Keys.Add(Properties.Resources.GroupPairingNumber, null);
            base.Keys.Add(Properties.Resources.ManagementPeriodStartDate, null);
            base.Keys.Add(Properties.Resources.ManagementPeriodDays, null);
            base.Keys.Add(Properties.Resources.NumberOfAnimals, null);
            base.Keys.Add(Properties.Resources.ProductionStage, null);

            base.Keys.Add(Properties.Resources.NumberOfYoungAnimals, null);
            base.Keys.Add(Properties.Resources.StartWeight, ImperialUnitsOfMeasurement.Pounds);
            base.Keys.Add(Properties.Resources.EndWeight, ImperialUnitsOfMeasurement.Pounds);
            base.Keys.Add(Properties.Resources.AverageDailyGain, ImperialUnitsOfMeasurement.Pounds);
            base.Keys.Add(Properties.Resources.EnergyRequiredToProduceWool, ImperialUnitsOfMeasurement.BritishThermalUnitPerPound);
            base.Keys.Add(Properties.Resources.WoolProduction, ImperialUnitsOfMeasurement.PoundsPerYear);
            base.Keys.Add(Properties.Resources.EnergyRequiredToProduceMilk, ImperialUnitsOfMeasurement.BritishThermalUnitPerPound);

            base.Keys.Add(Properties.Resources.DietAdditiveType, null);
            base.Keys.Add(Properties.Resources.MethaneConversionFactorOfDiet, ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane);
            base.Keys.Add(Properties.Resources.MethaneConversionFactorAdjusted, ImperialUnitsOfMeasurement.Percentage);
            base.Keys.Add(Properties.Resources.FeedIntake, ImperialUnitsOfMeasurement.PoundPerHeadPerDay);
            base.Keys.Add(Properties.Resources.CrudeProtein, ImperialUnitsOfMeasurement.PoundsPerPound);
            base.Keys.Add(Properties.Resources.Forage, ImperialUnitsOfMeasurement.PercentageDryMatter);
            base.Keys.Add(Properties.Resources.TDN, ImperialUnitsOfMeasurement.PercentageDryMatter);
            base.Keys.Add(Properties.Resources.AshContentOfDiet, ImperialUnitsOfMeasurement.PercentageDryMatter);
            base.Keys.Add(Properties.Resources.Starch, ImperialUnitsOfMeasurement.PercentageDryMatter);
            base.Keys.Add(Properties.Resources.Fat, ImperialUnitsOfMeasurement.PercentageDryMatter);
            base.Keys.Add(Properties.Resources.ME, ImperialUnitsOfMeasurement.BritishThermalUnitPerPound);
            base.Keys.Add(Properties.Resources.NDF, ImperialUnitsOfMeasurement.PercentageDryMatter);

            base.Keys.Add(Properties.Resources.GainCoefficientA, null);
            base.Keys.Add(Properties.Resources.GainCoefficientB, null);

            base.Keys.Add(Properties.Resources.ActivityCoefficientOfFeedingSituation, ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound);
            base.Keys.Add(Properties.Resources.MaintenanceCoefficient, ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound);
            base.Keys.Add(Properties.Resources.UserDefinedBeddingRate, null);
            base.Keys.Add(Properties.Resources.TotalCarbonKilogramsDryMatterForBedding, null);
            base.Keys.Add(Properties.Resources.TotalNitrogenKilogramsDryMatterForBedding, null);
            base.Keys.Add(Properties.Resources.MoistureContentOfBeddingMaterial, null);

            base.Keys.Add(Properties.Resources.MethaneConversionFactorOfManure, ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane);
            base.Keys.Add(Properties.Resources.N2ODirectEmissionFactor, ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN);
            base.Keys.Add(Properties.Resources.EmissionFactorVolatilization, null);
            base.Keys.Add(Properties.Resources.VolatilizationFraction, null);
            base.Keys.Add(Properties.Resources.EmissionFactorLeaching, null);
            base.Keys.Add(Properties.Resources.FractionLeaching, null);
            base.Keys.Add(Properties.Resources.AshContent, ImperialUnitsOfMeasurement.Percentage);
            base.Keys.Add(Properties.Resources.MethaneProducingCapacityOfManure, null);
            base.Keys.Add(Properties.Resources.ManureExcretionRate, null);
            base.Keys.Add(Properties.Resources.FractionOfCarbonInManure, null);
        } 

        #endregion
    }
}
