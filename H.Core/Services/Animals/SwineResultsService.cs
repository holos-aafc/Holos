#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Swine;
using H.Core.Providers.Feed;
#endregion

namespace H.Core.Services.Animals
{
    /// <summary>
    /// </summary>
    public class SwineResultsService : AnimalResultsServiceBase, ISwineResultsService
    {
        #region Fields

        #endregion

        #region Constructors

        public SwineResultsService() : base()
        {
            _animalComponentCategory = ComponentCategory.Swine;
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods


        #endregion

        #region Private Methods

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

            // Swine piglets do not have a diet as they are still suckling. They still need to be assigned a SelectedDiet
            // otherwise any calls to SelectedDiet throws an exception as managementPeriod.SelectedDiet will be null.
            if (managementPeriod.AnimalType == AnimalType.SwinePiglets)
            {
               managementPeriod.SelectedDiet = farm.Diets.SingleOrDefault(x => x.IsCustomPlaceholderDiet);
            }
         

            /*
             * Enteric methane (CH4)
             */

            // Equation 3.4.1-1
            dailyEmissions.EntericMethaneEmission = this.CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
                entericMethaneEmissionRate: managementPeriod.ManureDetails.YearlyEntericMethaneRate, 
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C) and methane (CH4)
             */

            // Equation 4.1.1-2
            dailyEmissions.FecalCarbonExcretionRate = this.CalculateCarbonExcretionRate(
                dailyDryMatterIntakeOfFeed: managementPeriod.SelectedDiet.DailyDryMatterFeedIntakeOfFeed);

            // Equation 4.1.1-4
            dailyEmissions.FecalCarbonExcretion = base.CalculateAmountOfFecalCarbonExcreted(
                excretionRate: dailyEmissions.FecalCarbonExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-5
            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = base.CalculateRateOfCarbonAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                carbonConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.1.1-6
            if (animalGroup.GroupType == AnimalType.SwinePiglets)
            {
                dailyEmissions.CarbonAddedFromBeddingMaterial = 0;
            }
            else
            {
                dailyEmissions.CarbonAddedFromBeddingMaterial = base.CalculateAmountOfCarbonAddedFromBeddingMaterial(
                    rateOfCarbonAddedFromBedding: dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            // Equation 4.1.2-2
            dailyEmissions.VolatileSolidsAdjusted = this.CalculateVolatileSolidAdjusted(
                volatileSolidExcretion: managementPeriod.ManureDetails.VolatileSolidExcretion,
                volatileSolidAdjustmentFactor: managementPeriod.SelectedDiet.VolatileSolidsAdjustmentFactorForDiet);

            // Equation 4.1.2-3
            dailyEmissions.VolatileSolids = this.CalculateVolatileSolids(
                volatileSolidAdjusted: dailyEmissions.VolatileSolidsAdjusted,
                feedIntake: managementPeriod.FeedIntakeAmount);

            /*
             * Manure methane calculations differ depending if the manure is stored as a liquid or as a solid
             *
             *  If user specifies custom a custom methane conversion factor, then skip liquid calculations (even if system is liquid, calculate manure methane using 2-4 and 2-5.)
             */

            if (managementPeriod.ManureDetails.StateType.IsSolidManure() || 
                managementPeriod.ManureDetails.UseCustomMethaneConversionFactor)
            {
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate = base.CalculateManureMethaneEmissionRate(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);

                // Equation 4.1.2-5
                dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                    emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }
            else
            {
                // Equation 4.1.3-8
                dailyEmissions.KelvinAirTemperature = base.CalculateDegreeKelvin(
                    degreesCelsius: temperature);

                // Equation 4.1.3-9
                dailyEmissions.ClimateFactor = base.CalculateClimateFactor(
                    kelvinAirTemperature: dailyEmissions.KelvinAirTemperature);

                // Equation 4.1.3-3
                dailyEmissions.VolatileSolidsProduced = base.CalculateVolatileSolidsProduced(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                // Equation 4.1.3-4
                dailyEmissions.VolatileSolidsLoaded = base.CalculateVolatileSolidsLoaded(
                    volatileSolidsProduced: dailyEmissions.VolatileSolidsProduced);

                // Equation 4.1.3-5
                dailyEmissions.VolatileSolidsAvailable = base.CalculateVolatileSolidsAvailable(
                    volatileSolidsLoaded: dailyEmissions.VolatileSolidsLoaded,
                    volatileSolidsAvailableFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsAvailable,
                    volatileSolidsConsumedFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsConsumed);

                // Equation 4.1.3-7
                dailyEmissions.VolatileSolidsConsumed = base.CalculateVolatileSolidsConsumed(
                    climateFactor: dailyEmissions.ClimateFactor,
                    volatileSolidsAvailable: dailyEmissions.VolatileSolidsAvailable);

                // Equation 4.1.3-8
                dailyEmissions.ManureMethaneEmission = base.CalculateLiquidManureMethane(
                    volatileSolidsConsumed: dailyEmissions.VolatileSolidsConsumed,
                    methaneProducingCapacityOfManure: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure);

                if (managementPeriod.ManureDetails.StateType.IsCoveredSystem())
                {
                    if (managementPeriod.ManureDetails.StateType == ManureStateType.LiquidWithNaturalCrust)
                    {
                        // Equation 4.1.3-9
                        dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethaneForCoveredSystem(
                            manureMethane: dailyEmissions.ManureMethaneEmission,
                            emissionReductionFactor: 0.4);
                    }
                    else
                    {
                        // Equation 4.1.3-9
                        dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethaneForCoveredSystem(
                            manureMethane: dailyEmissions.ManureMethaneEmission,
                            emissionReductionFactor: 0.5); // Not finalized at this time so going with 0.5 (range is 25-50%)
                    }
                }
            }

            // Equation 4.1.3-13
            dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement = base.CalculateCarbonLostAsMethaneDuringManagement(
                monthlyManureMethaneEmission: dailyEmissions.ManureMethaneEmission);

            // Equation 4.1.3-14
            dailyEmissions.AmountOfCarbonInStoredManure = base.CalculateAmountOfCarbonInStoredManure(
                monthlyFecalCarbonExcretion: dailyEmissions.FecalCarbonExcretion,
                monthlyAmountOfCarbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial,
                monthlyAmountOfCarbonLostAsMethaneDuringManagement: dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement);

            /*
             * Direct manure N2O
             */

            // Equation 4.2.1-19
            dailyEmissions.ProteinIntake = this.CalculateProteinIntake(
                feedIntake: managementPeriod.FeedIntakeAmount,
                crudeProteinIntake: managementPeriod.SelectedDiet.CrudeProteinContent);

            // Protein retained calculations for breeding sows only apply to the farrow to finish and farrow to wean components where the protein retained from weaned piglets must be considered
            if ((animalComponentBase.ComponentType == ComponentType.FarrowToFinish || animalComponentBase.ComponentType == ComponentType.FarrowToWean ) && 
                animalGroup.GroupType == AnimalType.SwineSows)
            {
                var fertilityRate = animalComponentBase.GetFertilityRate(
                    targetAnimalType: animalGroup.GroupType,
                    productionStages: new List<ProductionStages>() { ProductionStages.Gestating, ProductionStages.Lactating, ProductionStages.Open });

                var weightChangeOfPregnantAnimals = animalGroup.CalculateWeightChangeOfPregnantAnimal();

                // Equation 4.2.1-21
                dailyEmissions.ProteinRetainedForGain = this.CalculateProteinRetainedForGainForBreedingSows(
                    fertilityRateOfSows: fertilityRate,
                    liveWeightChangeOfSowsDuringGestation: weightChangeOfPregnantAnimals);

                var litterSize = 0d;
                if ( animalGroup.LitterSizeOfBirthingAnimal > 0)
                {
                    litterSize = animalGroup.LitterSizeOfBirthingAnimal;
                }
                else
                {
                    litterSize = animalComponentBase.CalculateAverageLitterSize(
                        youngAnimalTypeAndStage: new Tuple<AnimalType, ProductionStages> (AnimalType.SwinePiglets, ProductionStages.Weaning),
                        otherAnimalsAndStages: new List<Tuple<AnimalType, ProductionStages>>()
                        {
                            new Tuple<AnimalType, ProductionStages>(AnimalType.SwineGilts, ProductionStages.Gestating),
                            new Tuple<AnimalType, ProductionStages>(AnimalType.SwineSows, ProductionStages.Gestating),
                        });
                }

                // Equation 4.2.1-22
                dailyEmissions.ProteinRetainedByPiglets = this.CalculateProteinRetainedForWeanedPiglets(
                    numberOfAnimals: litterSize,
                    fertilityRateOfSows: fertilityRate,
                    liveWeightOfPigletAtBirth: animalGroup.WeightOfPigletsAtBirth,
                    liveWeightOfPigletAtWeaningAge: animalGroup.WeightOfWeanedAnimals);

                // Equation 4.2.1-20
                dailyEmissions.ProteinRetained = this.CalculateProteinRetainedForBreedingSows(
                    proteinRetainedForGainForBreedingSows: dailyEmissions.ProteinRetainedForGain,
                    proteinRetainedForWeanedPiglets: dailyEmissions.ProteinRetainedByPiglets);
            }
            else
            {
                // Equation 4.2.1-23
                dailyEmissions.ProteinRetained = this.CalculateProteinRetainedForGrowingPigs(
                    initialBodyWeight: managementPeriod.StartWeight,
                    finalBodyWeight: managementPeriod.EndWeight,
                    nitrogenRequiredForGain: managementPeriod.NitrogenRequiredForGain, 
                    numberOfDays: managementPeriod.NumberOfDays);
            }

            // Equation 4.2.1-24
            dailyEmissions.NitrogenExcretionRate = this.CalculateNitrogenExcretionRateForSwine(
                proteinIntake: dailyEmissions.ProteinIntake,
                proteinRetained: dailyEmissions.ProteinRetained,
                nitrogenExcretedAdjustment: managementPeriod.SelectedDiet.NitrogenExcretionAdjustFactorForDiet);

            // Equation 4.2.1-29 (used in volatilization calculation)
            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-30
            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial = base.CalculateRateOfNitrogenAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                nitrogenConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.2.1-31
            if (animalGroup.GroupType == AnimalType.SwinePiglets)
            {
                dailyEmissions.AmountOfNitrogenAddedFromBedding = 0;
            }
            else
            {
                dailyEmissions.AmountOfNitrogenAddedFromBedding = base.CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                    rateOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            // Equation 4.2.2-1
            dailyEmissions.ManureDirectN2ONEmissionRate = base.CalculateManureDirectNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                emissionFactor: managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            // Equation 4.2.2-2
            dailyEmissions.ManureDirectN2ONEmission = base.CalculateManureDirectNitrogenEmission(
                manureDirectNitrogenEmissionRate: dailyEmissions.ManureDirectN2ONEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Ammonia emissions
             */

            // Equation 4.3.3-1
            dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;

            // Equation 4.3.4-6
            dailyEmissions.AmmoniaEmissionRateFromHousingAndStorage = base.CalculateAmmoniaEmissionRateFromHousingAndStorage(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                rateOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized);

            // Equation 4.3.4-7
            dailyEmissions.TotalNitrogenLossesFromHousingAndStorage = base.CalculateTotalNitrogenLossFromHousingAndStorage(
                ammoniaEmissionRate: dailyEmissions.AmmoniaEmissionRateFromHousingAndStorage,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.4-8
            dailyEmissions.AmmoniaEmissionsFromHousingAndStorage = base.CalculateAmmoniaLossFromHousingAndStorage(
                totalNitrogenLossFromHousingAndStorage: dailyEmissions.TotalNitrogenLossesFromHousingAndStorage);

            /*
             * Indirect manure N2O
             */

            /*
             * Volatilization
             */

            // Equation 4.3.3-3
            dailyEmissions.ManureVolatilizationRate = base.CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            // Equation 4.3.3-4
            dailyEmissions.ManureVolatilizationN2ONEmission = base.CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Leaching
             */

            // Equation 4.3.4-1
            dailyEmissions.ManureNitrogenLeachingRate = base.CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            // Equation 4.3.4-2
            dailyEmissions.ManureN2ONLeachingEmission = this.CalculateManureLeachingNitrogenEmission(
                leachingNitrogenEmissionRate: dailyEmissions.ManureNitrogenLeachingRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.5-1
            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.3.7-1
            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.5.2-6
            dailyEmissions.NitrogenAvailableForLandApplication = base.CalculateTotalAvailableManureNitrogenInStoredManureForSheepSwinePoultryAndOtherLivestock(
                nitrogenExcretion: dailyEmissions.AmountOfNitrogenExcreted,
                nitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                directN2ONEmission: dailyEmissions.ManureDirectN2ONEmission,
                indirectN2ONEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.5.3-1
            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.NitrogenAvailableForLandApplication);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                nitrogenFractionOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            // Equation 4.6.1-4
            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = base.CalculateTotalAmmoniaEmissionsFromLandAppliedManure(
                farm: farm,
                dateTime: dateTime,
                dailyEmissions: dailyEmissions,
                animalType: animalGroup.GroupType,
                temperature: temperature,
                managementPeriod: managementPeriod);

            // Equation 5.2.5-8
            dailyEmissions.AmmoniaEmissionsFromGrazingAnimals = 0; // No methodology for grazing swine (manure management types for swine do no include pasture)

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsNewlyHatchedEggs() || groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsEggs())
            {
                return;
            }

            var energyConversionFactor = _energyConversionDefaultsProvider.GetElectricityConversionValue(
                year: groupEmissionsByMonth.MonthsAndDaysData.Year, 
                province: farm.DefaultSoilData.Province);

            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = this.CalculateTotalCarbonDioxideEmissionsFromSwineHousing(
                numberOfAnimals: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                numberOfDays: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                electricityConversion: energyConversionFactor);
        }

        #endregion

        #region Equations


        /// <summary>
        /// Equation 4.2.1-20
        /// </summary>
        /// <param name="proteinRetainedForGainForBreedingSows">Protein retained for gain for breeding sows (kg head^-1 day^-1)</param>
        /// <param name="proteinRetainedForWeanedPiglets">Protein retained of weaned piglets (kg head^-1 day^-1)</param>
        /// <returns>Protein retained for breeding sows (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForBreedingSows(
            double proteinRetainedForGainForBreedingSows,
            double proteinRetainedForWeanedPiglets)
        {
            return (proteinRetainedForGainForBreedingSows + proteinRetainedForWeanedPiglets) / 365;
        }

        /// <summary>
        /// Equation 4.2.1-21
        /// </summary>
        /// <param name="fertilityRateOfSows">Fertility rate of sows (litters year^-1)</param>
        /// <param name="liveWeightChangeOfSowsDuringGestation">Weight change of animals during gestation (kg head^-1)</param>
        /// <returns>Protein retained for gain for breeding sows (kg head^-1)</returns>
        public double CalculateProteinRetainedForGainForBreedingSows(
            double fertilityRateOfSows,
            double liveWeightChangeOfSowsDuringGestation)
        {
            return (0.025 * fertilityRateOfSows * liveWeightChangeOfSowsDuringGestation) / 350.0;
        }

        /// <summary>
        /// Equation 4.2.1-22
        /// </summary>
        /// <param name="numberOfAnimals">Total number of weaned piglets</param>
        /// <param name="fertilityRateOfSows">Fertility rate of sows (litter year^-1)</param>
        /// <param name="liveWeightOfPigletAtBirth">Live weight of a piglet at birth (kg head^-1)</param>
        /// <param name="liveWeightOfPigletAtWeaningAge">Live weight of a piglet at weaning age (kg head^-1)</param>
        /// <returns>Protein retained of weaned piglets (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForWeanedPiglets(
            double numberOfAnimals,
            double fertilityRateOfSows,
            double liveWeightOfPigletAtBirth,
            double liveWeightOfPigletAtWeaningAge)
        {
            return (0.025 * numberOfAnimals * fertilityRateOfSows * ((liveWeightOfPigletAtWeaningAge - liveWeightOfPigletAtBirth) / 0.98)) / 350.0;
        }

        /// <summary>
        /// Equation 4.2.1-23
        /// </summary>
        /// <param name="initialBodyWeight">Live weight of animal at beginning of growth stage (kg)</param>
        /// <param name="finalBodyWeight">Live weight of animal at end of growth stage (kg)</param>
        /// <param name="nitrogenRequiredForGain">Fraction of protein retained at a given body weight.</param>
        /// <param name="numberOfDays"></param>
        /// <returns>Protein retained of growing piglets (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForGrowingPigs(
            double initialBodyWeight, 
            double finalBodyWeight, 
            double nitrogenRequiredForGain, 
            double numberOfDays)
        {
            // Nitrogen required for gain is multiplied by 6.25 to get protein required for gain (PR_gain = N_gain * 6.25)
            return ((finalBodyWeight - initialBodyWeight) * (nitrogenRequiredForGain * 6.25) / numberOfDays);
        }

        /// <summary>
        /// Equation 4.2.1-24
        /// </summary>
        /// <param name="proteinIntake">Protein intake of animal (kg head^-1 day^-1)</param>
        /// <param name="proteinRetained">Protein retained by animal (kg head^-1 day^-1)</param>
        /// <param name="nitrogenExcretedAdjustment">Nitrogen excretion adjustment factor for diet (kg kg^-1)</param>
        /// <returns>Nitrogen excretion rate of animals (kg head^-1 day^-1)</returns>
        public double CalculateNitrogenExcretionRateForSwine(
            double proteinIntake,
            double proteinRetained,
            double nitrogenExcretedAdjustment)
        {
            return ((proteinIntake / 6.25) - (proteinRetained / 6.25)) * nitrogenExcretedAdjustment;
        }

        /// <summary>
        /// Equation 4.1.1-2
        /// </summary>
        /// <param name="dailyDryMatterIntakeOfFeed">Daily dry matter intake of feed (kg head^-1 day^-1)</param>
        /// <returns>Carbon excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateCarbonExcretionRate(
            double dailyDryMatterIntakeOfFeed)
        {
            const double CarbonDigestibility = 0.88;

            return dailyDryMatterIntakeOfFeed * 0.45 * (1 - CarbonDigestibility);
        }

        /// <summary>
        /// Equation 4.1.2-2
        /// </summary>
        /// <param name="volatileSolidExcretion">Volatile solid excretion (kg kg^-1)</param>
        /// <param name="volatileSolidAdjustmentFactor">Volatile solid adjustment factor (kg kg^-1)</param>
        /// <returns>Volatile solid adjusted</returns>
        public double CalculateVolatileSolidAdjusted(double volatileSolidExcretion,
                                                     double volatileSolidAdjustmentFactor)
        {
            return volatileSolidExcretion * volatileSolidAdjustmentFactor;
        }

        /// <summary>
        /// Equation 4.1.2-3
        /// </summary>
        /// <param name="volatileSolidAdjusted">Volatile solid adjusted</param>
        /// <param name="feedIntake">Feed intake (kg head^-1 day^-1)</param>
        /// <returns>Volatile solids (kg head^-1 day^-1)</returns>
        public double CalculateVolatileSolids(double volatileSolidAdjusted, double feedIntake)
        {
            return volatileSolidAdjusted * feedIntake;
        }

        /// <summary>
        /// Equation 3.3.2-4
        /// </summary>
        /// <param name="manureMethaneEmissionRate">Manure CH4 emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfPigs">Number of pigs</param>
        /// <param name="numberOfDays">Number of days in a month</param>
        /// <returns>Manure CH4 emission (kg CH4 year^-1)</returns>
        public double CalculateManureMethaneEmission(double manureMethaneEmissionRate, double numberOfPigs,
                                                     double numberOfDays)
        {
            return manureMethaneEmissionRate * numberOfPigs * numberOfDays;
        }

        /// <summary>
        /// Equation 4.2.1-19
        /// </summary>
        /// <param name="feedIntake">Feed intake (kg head^-1 day^-1)</param>
        /// <param name="crudeProteinIntake">Crude protein content (kg kg^-1)</param>
        /// <returns>Protein intake (kg head^-1 day^-1)</returns>
        public new double CalculateProteinIntake(double feedIntake, double crudeProteinIntake)
        {
            return feedIntake * crudeProteinIntake;
        }



        /// <summary>
        /// Equation 3.3.3-3
        /// </summary>
        /// <param name="proteinIntake">Protein intake (kg head^-1 day^-1)</param>
        /// <param name="proteinRetained">Protein retained (kg (kg protein intake)^-1)</param>
        /// <param name="nitrogenExcretedAdjustmentFactor">N excreted adjustment factor (kg kg^-1)</param>
        /// <returns>N excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateNitrogenExcretionRate(double proteinIntake, double proteinRetained,
                                                     double nitrogenExcretedAdjustmentFactor)
        {
            return proteinIntake * (1 - proteinRetained) * nitrogenExcretedAdjustmentFactor / 6.25;
        }

        /// <summary>
        /// Equation 3.3.3-6
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="volatilizationFraction">Volatilization fraction</param>
        /// <param name="emissionFactorForVolatilization">Emission factor for volatilization [kg N2O-N (kg N)^-1]</param>
        /// <returns>Manure volatilization N emission rate (kg head^-1 day^-1)</returns>
        public double CalculateManureVolatilizationNitrogenEmissionRate(double nitrogenExcretionRate,
                                                                        double volatilizationFraction, double emissionFactorForVolatilization)
        {
            return nitrogenExcretionRate * volatilizationFraction * emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 3.3.3-12
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfPigs">Number of pigs</param>
        /// <param name="numberOfDays">Number of days in a month</param>
        /// <param name="volatilizationFraction">Volatilization fraction</param>
        /// <param name="leachingFraction">Leaching fraction</param>
        /// <returns>Manure available for land application (kg N)</returns>
        public double CalculateManureAvailableForLandApplication(double nitrogenExcretionRate, double numberOfPigs,
                                                                 double numberOfDays, double volatilizationFraction, double leachingFraction)
        {
            var a = nitrogenExcretionRate * numberOfPigs * numberOfDays;
            var b = 1.0 - (volatilizationFraction + leachingFraction);
            return a * b;
        }

        /// <summary>
        /// Equation 3.3.5-1
        /// </summary>
        /// <param name="totalManureDirectNitrogenEmission">Total manure direct N emission from swine (kg N2O-N year^-1)</param>
        /// <returns>Total manure direct N2O emission from swine (kg N2O year^-1)</returns>
        public double CalculateTotalManureDirectN2OEmissionFromSwine(double totalManureDirectNitrogenEmission)
        {
            return totalManureDirectNitrogenEmission * 44 / 28;
        }

        /// <summary>
        /// Equation 3.3.5-2
        /// </summary>
        /// <param name="totalManureIndirectNitrogenEmission">Total manure indirect N emission from swin (kg N2O-N year^-1)</param>
        /// <returns>Total manure indirect N2O emission from swin (kg N2O year^-1)</returns>
        public double CalculateTotalManureIndirectN2OEmissionFromSwine(double totalManureIndirectNitrogenEmission)
        {
            return totalManureIndirectNitrogenEmission * 44 / 28;
        }

        /// <summary>
        /// Equation 3.3.5-3
        /// </summary>
        /// <param name="totalManureNitrogenEmission">Total manure N emission from swine (kg N2O-N year^-1)</param>
        /// <returns>Total manure N2O emission from swine (kg N2O year^-1)</returns>
        public double CalculateTotalManureN2OEmissionFromSwine(double totalManureNitrogenEmission)
        {
            return totalManureNitrogenEmission * 44 / 28;
        }

        /// <summary>
        /// Equation 6.2.2-6
        /// </summary>
        /// <param name="numberOfAnimals">Number of pigs</param>
        /// <param name="numberOfDays">Number of days in month</param>
        /// <param name="electricityConversion">Province specific value (kg CO2 kWh^-1)</param>
        /// <returns>Total CO2 emissions from swine operations (kg CO2 year^-1) – for each pig group</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromSwineHousing(
            double numberOfAnimals, 
            double numberOfDays,
            double electricityConversion)
        {
            const double swineConversion = 1.06;

            var result =  numberOfAnimals * (swineConversion / CoreConstants.DaysInYear) * electricityConversion * numberOfDays;

            return result;
        }

        #endregion
    }
}