using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;

namespace H.Core.Services.Animals
{
    public class PoultryResultsService : AnimalResultsServiceBase, IPoultryResultsService
    {
        #region Fields

        //Referenced in  Eqn. 3.5.3-4
        private const double FractionLeaching = 0;
        private Table_44_Poultry_NExcretionRate_Parameter_Values_Provider _poultryNExcretionRateValuesProvider = new Table_44_Poultry_NExcretionRate_Parameter_Values_Provider();
        private readonly DefaultDailyTanExcretionRatesForPoultry _defaultDailyTanExcretionRatesForPoultry = new DefaultDailyTanExcretionRatesForPoultry();

        #endregion

        #region Constructors

        public PoultryResultsService() : base()
        {
            _animalComponentCategory = ComponentCategory.Poultry;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override GroupEmissionsByDay CalculateDailyEmissions(
            AnimalComponentBase animalComponentBase,
            ManagementPeriod managementPeriod,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            AnimalGroup animalGroup,
            Farm farm)
        {
            var dailyEmissions = new GroupEmissionsByDay();
            var temperature = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month);

            if (animalGroup.GroupType.IsNewlyHatchedEggs() || animalGroup.GroupType.IsEggs())
            {
                return dailyEmissions;
            }

            /*
             * Enteric methane (CH4)
             */

            // Equation 3.4.1-1
            dailyEmissions.EntericMethaneEmission =
                this.CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
                    entericMethaneEmissionRate: managementPeriod.ManureDetails.YearlyEntericMethaneRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C) and methane (CH4)
             */

            // Equation 4.1.1-3
            dailyEmissions.FecalCarbonExcretionRate =
                base.CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
                    manureExcretionRate: managementPeriod.ManureDetails.ManureExcretionRate,
                    carbonFractionOfManure: managementPeriod.ManureDetails.FractionOfCarbonInManure);

            // Equation 4.1.1-4
            dailyEmissions.FecalCarbonExcretion = base.CalculateAmountOfFecalCarbonExcreted(
                excretionRate: dailyEmissions.FecalCarbonExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-5
            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = base.CalculateRateOfCarbonAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                carbonConcentrationOfBeddingMaterial: managementPeriod.HousingDetails
                    .TotalCarbonKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = base.CalculateAmountOfCarbonAddedFromBeddingMaterial(
                rateOfCarbonAddedFromBedding: dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            if (animalGroup.GroupType.IsChickenType())
            {
                // For chicken, we have VS values used to calculate the manure CH4 emission rate
                dailyEmissions.VolatileSolids = managementPeriod.ManureDetails.VolatileSolids;

                dailyEmissions.ManureMethaneEmissionRate = this.CalculateManureMethaneEmissionRate(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);
            }
            else
            {
                // For turkeys, we use a constant manure CH4 emission rate
                dailyEmissions.ManureMethaneEmissionRate =
                    managementPeriod.ManureDetails.DailyManureMethaneEmissionRate;
            }

            // Equation 4.1.2-5
            dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.3-13
            dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement =
                base.CalculateCarbonLostAsMethaneDuringManagement(
                    monthlyManureMethaneEmission: dailyEmissions.ManureMethaneEmission);

            // Equation 4.1.3-14
            dailyEmissions.AmountOfCarbonInStoredManure = base.CalculateAmountOfCarbonInStoredManure(
                monthlyFecalCarbonExcretion: dailyEmissions.FecalCarbonExcretion,
                monthlyAmountOfCarbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial,
                monthlyAmountOfCarbonLostAsMethaneDuringManagement: dailyEmissions
                    .AmountOfCarbonLostAsMethaneDuringManagement);

            /*
             * Direct manure N2O
             */

            if (managementPeriod.AnimalType.IsTurkeyType())
            {
                /*
                 * Turkeys groups have a constant rate defined by table 44
                 */

                // Equation 4.2.1-24
                dailyEmissions.NitrogenExcretionRate = managementPeriod.ManureDetails.NitrogenExretionRate;
            }
            else
            {
                /*
                 * Chickens have a calculated rate
                 */

                var poultryDietData = _poultryNExcretionRateValuesProvider.GetParameterValues(managementPeriod.AnimalType);

                dailyEmissions.DryMatterIntake = poultryDietData.DailyMeanIntake;

                dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                dailyEmissions.TotalCarbonUptakeForGroup = base.CaclulateDailyCarbonUptakeForGroup(
                    totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

                dailyEmissions.ProteinIntake = this.CalculateProteinIntakePoultry(
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    crudeProtein: poultryDietData.CrudeProtein);

                if (managementPeriod.AnimalType == AnimalType.ChickenHens)
                {
                    dailyEmissions.ProteinRetained = this.CalculateProteinRetainedLayers(
                        proteinInLiveWeight: poultryDietData.ProteinLiveWeight,
                        weightGain: poultryDietData.WeightGain,
                        proteinContentOfEggs: poultryDietData.ProteinContentEgg,
                        eggProduction: poultryDietData.EggProduction);
                }
                else
                {
                    dailyEmissions.ProteinRetained = this.CalculateProteinRetainedBroilers(
                        initialWeight: poultryDietData.InitialWeight,
                        finalWeight: poultryDietData.FinalWeight,
                        productionPeriod: poultryDietData.ProductionPeriod);
                }

                dailyEmissions.NitrogenExcretionRate = this.CalculateNitrogenExcretionRateChickens(
                    proteinIntake: dailyEmissions.ProteinIntake,
                    proteinRetained: dailyEmissions.ProteinRetained);
            }

            // Equation 4.2.1-29 (used in volatilization calculation)
            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-30
            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial =
                base.CalculateRateOfNitrogenAddedFromBeddingMaterial(
                    beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                    nitrogenConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                    moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.2.1-31
            dailyEmissions.AmountOfNitrogenAddedFromBedding = base.CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                rateOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.2-1
            dailyEmissions.ManureDirectN2ONEmissionRate = base.CalculateManureDirectNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                emissionFactor: managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            // Equation 4.2.2-2
            dailyEmissions.ManureDirectN2ONEmission = base.CalculateManureDirectNitrogenEmission(
                manureDirectNitrogenEmissionRate: dailyEmissions.ManureDirectN2ONEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.3-1
            dailyEmissions.TanExcretion = base.CalculateTANExcretion(
                tanExcretionRate: managementPeriod.ManureDetails.DailyTanExcretion,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.3-2
            dailyEmissions.FecalNitrogenExcretionRate = base.CalculateFecalNitrogenExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                tanExcretionRate: dailyEmissions.TanExcretionRate);

            // Equation 4.3.3-3
            dailyEmissions.FecalNitrogenExcretion = base.CalculateFecalNitrogenExcretion(
                fecalNitrogenExcretionRate: dailyEmissions.FecalNitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.3-4
            dailyEmissions.OrganicNitrogenInStoredManure = base.CalculateOrganicNitrogenInStoredManure(
                totalNitrogenExcretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            var emissionFactorForHousing = _defaultDailyTanExcretionRatesForPoultry.GetEmissionFactorForHousing(managementPeriod.AnimalType);

            // Equation 4.3.3-5
            dailyEmissions.AmmoniaEmissionRateFromHousing = CalculateAmmoniaEmissionRateFromHousing(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                adjustedEmissionFactor: emissionFactorForHousing);

            // Equation 4.3.3-6
            dailyEmissions.AmmoniaConcentrationInHousing = CalculateAmmoniaConcentrationInHousing(
                emissionRate: dailyEmissions.AmmoniaEmissionRateFromHousing,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.3-7
            dailyEmissions.AmmoniaEmissionsFromHousingSystem = CalculateTotalAmmoniaEmissionsFromHousing(
                ammoniaConcentrationInHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            // Equation 4.3.3-8
            dailyEmissions.TanEnteringStorageSystem = CalculateTanStorage(
                tanExcretion: dailyEmissions.TanExcretion,
                tanExcretionFromPreviousPeriod: previousDaysEmissions == null ? 0 : previousDaysEmissions.TanExcretion,
                ammoniaLostByHousingDuringPreviousPeriod: previousDaysEmissions == null ? 0 : previousDaysEmissions.AmmoniaConcentrationInHousing);

            // Equation 4.3.3-9
            dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateAmbientTemperatureAdjustmentForStorage(
                temperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));

            var emissionFactorForStorage = _defaultDailyTanExcretionRatesForPoultry.GetEmissionFactorForStorage(managementPeriod.AnimalType);

            // Equation 4.3.3-10
            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = this.CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: emissionFactorForStorage);

            // Equation 4.3.3-11
            dailyEmissions.AmmoniaLostFromStorage = CalculateAmmoniaLossFromStoredManure(
                tanInStoredManure: dailyEmissions.AdjustedAmountOfTanInStoredManure,
                ammoniaEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            // Equation 4.3.3-12
            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CalculateAmmoniaEmissionsFromStoredManure(
                ammoniaNitrogenLossFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

            // Equation 4.3.4-1
            if (managementPeriod.ManureDetails.UseCustomVolatilizationFraction)
            {
                dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;
            }
            else
            {
                dailyEmissions.FractionOfManureVolatilized = this.CalculateFractionOfManureVolatilized(
                    ammoniaEmissionsFromHousing: dailyEmissions.AmmoniaConcentrationInHousing,
                    ammoniaEmissionsFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                    amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                    amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);
            }

            // Equation 4.3.4-2
            dailyEmissions.ManureVolatilizationRate = this.CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogen: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            // Equation 4.3.4-3
            dailyEmissions.ManureVolatilizationN2ONEmission = this.CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.5-1
            dailyEmissions.ManureNitrogenLeachingRate = base.CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                amountOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial);

            // Equation 4.3.5-2
            dailyEmissions.ManureN2ONLeachingEmission = this.CalculateManureLeachingNitrogenEmission(
                leachingNitrogenEmissionRate: dailyEmissions.ManureNitrogenLeachingRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.6-1
            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.3.7-1
            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.3.4-7
            dailyEmissions.AdjustedAmmoniaFromHousing = this.CalculateAdjustedAmmoniaFromHousing(dailyEmissions, managementPeriod);

            // Equation 4.3.4-11
            dailyEmissions.AdjustedAmmoniaFromStorage = this.CalculateAdjustedAmmoniaFromStorage(dailyEmissions, managementPeriod);

            // Equation 4.5.2-7
            dailyEmissions.TanAvailableForLandApplication = dailyEmissions.TanEnteringStorageSystem - dailyEmissions.AmmoniaLostFromStorage;

            // Equation 4.5.2-11
            dailyEmissions.NitrogenAvailableForLandApplication =
                (dailyEmissions.AmountOfNitrogenExcreted + dailyEmissions.AmountOfNitrogenAddedFromBedding)
                - (dailyEmissions.ManureDirectN2ONEmission + dailyEmissions.AmmoniaConcentrationInHousing +
                   dailyEmissions.AmmoniaLostFromStorage + dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.5.2-9
            dailyEmissions.OrganicNitrogenAvailableForLandApplication =
                dailyEmissions.NitrogenAvailableForLandApplication - dailyEmissions.TanAvailableForLandApplication;

            // Equation 4.5.3-1
            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.NitrogenAvailableForLandApplication);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                    totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                    nitrogenFractionOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = this.CalculateAmmoniaEmissionsFromLandAppliedManure(
                farm: farm,
                dailyEmissions: dailyEmissions,
                managementPeriod: managementPeriod,
                dateTime: dateTime);

            dailyEmissions.NitrogenEmissionsFromVolatilizedLandManureApplication = this.CalculateVolatilizationFromLandAppliedManure(
                dailyEmissions: dailyEmissions,
                managementPeriod: managementPeriod);

            // Equation 4.6.3-4
            var adjustedAmmoniacalNitrogenFromLandAppliedManure = dailyEmissions.AmmoniacalNitrogenFromLandAppliedManure - dailyEmissions.NitrogenEmissionsFromVolatilizedLandManureApplication;

            // Equation 4.6.3-5
            var adjustedAmmoniaEmissionsFromLandApplication = adjustedAmmoniacalNitrogenFromLandAppliedManure * CoreConstants.ConvertNH3NToNH3;

            return dailyEmissions;
        }

        public double CalculateVolatilizationFromLandAppliedManure(GroupEmissionsByDay dailyEmissions, ManagementPeriod managementPeriod)
        {
            // Equation 4.6.3-1
            var volatilizationFractionFromLandApplication = dailyEmissions.AmmoniacalNitrogenFromLandAppliedManure / dailyEmissions.NitrogenAvailableForLandApplication;

            // Equation 4.6.3-2
            var volatilizationRateFromLandApplication = dailyEmissions.NitrogenAvailableForLandApplication * volatilizationFractionFromLandApplication * managementPeriod.ManureDetails.EmissionFactorVolatilization;

            // Equation 4.6.3-3
            var totalManureVolatilizationFromLandApplication = volatilizationRateFromLandApplication * managementPeriod.NumberOfAnimals;

            return totalManureVolatilizationFromLandApplication;
        }

        /// <summary>
        /// Calculate total ammonia emissions from land applied manure on a specified date
        /// </summary>
        public double CalculateAmmoniaEmissionsFromLandAppliedManure(
            Farm farm, 
            GroupEmissionsByDay dailyEmissions, 
            ManagementPeriod managementPeriod,
            DateTime dateTime)
        {
            var totalAmmoniacalNitrogenLost = 0d;

            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                var singleYearViewItem = fieldSystemComponent.GetSingleYearViewItem();
                if (singleYearViewItem == null)
                {
                    continue;
                }

                var manureApplications = singleYearViewItem.GetManureApplicationsFromLivestock(
                    animalType: managementPeriod.AnimalType,
                    dateOfManureApplication: dateTime);

                var totalManureAppliedOnDate = manureApplications.Sum(manureApplication => manureApplication.AmountOfManureAppliedPerHectare);
                var fractionOfManureUsed = totalManureAppliedOnDate / dailyEmissions.TotalVolumeOfManureAvailableForLandApplication;

                var emissionFactor = 0d;
                var t = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month);
                if (t >= 15)
                {
                    emissionFactor = 0.85;
                }
                else if (t >= 10 && t < 15)
                {
                    emissionFactor = 0.73;
                }
                else if (t >= 5 && t < 10)
                {
                    emissionFactor = 0.35;
                }
                else
                {
                    emissionFactor = 0.25;
                }

                var lossFromLandApplication = fractionOfManureUsed * dailyEmissions.TanAvailableForLandApplication * emissionFactor;

                totalAmmoniacalNitrogenLost += lossFromLandApplication;
            }

            // Equation 4.6.2-5
            dailyEmissions.AmmoniacalNitrogenFromLandAppliedManure = totalAmmoniacalNitrogenLost;

            // Equation 4.6.2-6
            var ammoniaEmissionsFromLandAppliedManure = dailyEmissions.AmmoniacalNitrogenFromLandAppliedManure * CoreConstants.ConvertNH3NToNH3;

            return ammoniaEmissionsFromLandAppliedManure;
        }

        public double CalculateAdjustedAmmoniaFromStorage(GroupEmissionsByDay dailyEmissions, ManagementPeriod managementPeriod)
        {
            var a = base.CalculateVolatalizationFractionForStorage(
                dailyAmmoniaEmissionsFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            var b = base.CalculateAmmoniaVolatilizationFromStorage(
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFractionFromStorage: a,
                emissionFactorForVolatilization: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            var c = base.CalculateAmmoniaStorageAdjustment(
                ammoniaFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                ammoniaVolatilizedDuringStorage: b);


            var d = base.CalculateAmmoniaEmissionsFromStorage(
                ammoniaFromStorageAdjustment: c);

            return d;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsNewlyHatchedEggs() || 
                groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsEggs())
            {
                return;
            }

            var energyConversionFactor  = _energyConversionDefaultsProvider.GetElectricityConversionValue(groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = this.CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(
                numberOfAnimals: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                numberOfDays: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                energyConversion: energyConversionFactor);
        }

        #endregion

        #region Equations

        /// <summary>
        /// Equation 3.5.1-1
        /// </summary>
        /// <param name="entericMethaneEmissionRate">Enteric CH4 emission rate (kg head^-1 year^-1)</param>
        /// <param name="numberOfPoultry">Number of poultry</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Enteric CH4 emission (kg CH4)</returns>
        public double CalculateEntericMethaneEmission(double entericMethaneEmissionRate,
            double numberOfPoultry,
            double numberOfDaysInMonth)
        {
            return entericMethaneEmissionRate * numberOfPoultry * numberOfDaysInMonth / CoreConstants.DaysInYear;
        }

        /// <summary>
        ///   Equation 3.5.2-1
        /// </summary>
        /// <param name="manureMethaneEmissionRate">Manure CH4 emission rate (kg head^-1 year^-1) </param>
        /// <param name="numberOfPoultry">Number of poultry</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Manure CH4 emission (kg CH4)</returns>
        public double CalculateManureMethaneEmission(double manureMethaneEmissionRate,
            double numberOfPoultry,
            double numberOfDaysInMonth)
        {
            return manureMethaneEmissionRate * numberOfPoultry * numberOfDaysInMonth / CoreConstants.DaysInYear;
        }

        /// <summary>
        /// Equation 4.2.1-25
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg head^-1 day^-1)</param>
        /// <param name="crudeProtein">Crude protein content in dietary dry matter (% of DM)</param>
        /// <returns>Protein intake (kg head^-1 day^-1)</returns>
        private double CalculateProteinIntakePoultry(
            double dryMatterIntake,
            double crudeProtein)
        {
            return dryMatterIntake * crudeProtein;
        }

        /// <summary>
        /// Equation 4.2.1-26
        /// </summary>
        /// <param name="proteinInLiveWeight">Average protein content in live weight (kg protein (kg head)^-1)</param>
        /// <param name="weightGain">Average daily weight gain for cohort (kg head^-1 day^-1)</param>
        /// <param name="proteinContentOfEggs">Average protein content of eggs (kg protein (kg egg)^-1)</param>
        /// <param name="eggProduction">Egg mass production (g egg head-1 day-1)</param>
        /// <returns>Protein retained in animal in cohort (kg head^-1 day^-1)</returns>
        private double CalculateProteinRetainedLayers(
            double proteinInLiveWeight,
            double weightGain,
            double proteinContentOfEggs,
            double eggProduction)
        {
            var result = (proteinInLiveWeight * weightGain) + ((proteinContentOfEggs * eggProduction) / 1000);

            return result;
        }

        /// <summary>
        /// Equation 4.2.1-27
        /// </summary>
        /// <param name="initialWeight">Live weight of the animal at the beginning of the stage (kg)</param>
        /// <param name="finalWeight">Live weight of the animal at the end of the stage (kg)</param>
        /// <param name="productionPeriod">Length of time from chick to slaughter</param>
        /// <returns>Protein retained in animal (kg head^-1 day^-1)</returns>
        private double CalculateProteinRetainedBroilers(
            double initialWeight,
            double finalWeight,
            double productionPeriod)
        {
            const double proteinRetainedForGain = 0.175;

            var result = ((finalWeight - initialWeight) * proteinRetainedForGain) / productionPeriod;

            return result;
        }

        /// <summary>
        /// Equation 4.2.1-28
        /// </summary>
        /// <param name="proteinIntake">Protein intake (kg head^-1 day^-1)</param>
        /// <param name="proteinRetained">Protein retained in animal in cohort (kg head^-1 day^-1)</param>
        /// <returns>N excretion rate (kg head-1 day-1)</returns>
        private double CalculateNitrogenExcretionRateChickens(
            double proteinIntake,
            double proteinRetained)
        {
            // Conversion from dietary protein to dietary N
            const double conversion = 6.25;

            var result = (proteinIntake / conversion) - (proteinRetained / conversion);

            return result;
        }

        /// <summary>
        /// Equation 4.3.3-9
        /// </summary>
        public double CalculateAmbientTemperatureAdjustmentForStorage(
            double temperature)
        {
            return 1 - 0.058 - (17 - temperature);
        }

        /// <summary>
        /// Equation 6.2.3-1
        /// </summary>
        /// <param name="numberOfAnimals">Barn capacity for poultry</param>
        /// <param name="numberOfDays">Number of days in month</param>
        /// <param name="energyConversion">kWh per poultry placement per year for electricity electricity (kWh poultry placement^-1 year^-1) </param>
        /// <returns>Total CO2 emissions from poultry operations (kg CO2)</returns>
        public double CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(
            double numberOfAnimals, 
            double numberOfDays, 
            double energyConversion)
        {
            const double poultryConversion = 2.88;

            return numberOfAnimals * (poultryConversion / CoreConstants.DaysInYear) * energyConversion * numberOfDays;
        }

        #endregion
    }
}
