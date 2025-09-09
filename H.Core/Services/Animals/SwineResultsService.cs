#region Imports

using System;
using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;

#endregion

namespace H.Core.Services.Animals
{
    /// <summary>
    /// </summary>
    public class SwineResultsService : AnimalResultsServiceBase, ISwineResultsService
    {
        #region Fields

        private readonly Table_40_Swine_Default_PrGain_Values_Provider _swineDefaultPrGainValuesProvider =
            new Table_40_Swine_Default_PrGain_Values_Provider();

        #endregion

        #region Constructors

        public SwineResultsService()
        {
            _animalComponentCategory = ComponentCategory.Swine;
        }

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

            var temperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);

            InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            if (managementPeriod.ProductionStage == ProductionStages.Weaning)
                // No emissions from piglets who are still nursing
                return dailyEmissions;

            /*
             * Enteric methane (CH4)
             */

            dailyEmissions.EntericMethaneEmission = CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
                managementPeriod.ManureDetails.YearlyEntericMethaneRate,
                managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C) and methane (CH4)
             */

            // Old farms had the DMI/Intake associated with the management period and not the diet
            var dryMatterIntake = managementPeriod.SelectedDiet.DailyDryMatterFeedIntakeOfFeed > 0
                ? managementPeriod.SelectedDiet.DailyDryMatterFeedIntakeOfFeed
                : managementPeriod.FeedIntakeAmount;
            dailyEmissions.DryMatterIntake = dryMatterIntake;

            dailyEmissions.FecalCarbonExcretionRate = CalculateCarbonExcretionRate(
                dryMatterIntake);

            dailyEmissions.FecalCarbonExcretion = CalculateAmountOfFecalCarbonExcreted(
                dailyEmissions.FecalCarbonExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = CalculateRateOfCarbonAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            if (animalGroup.GroupType == AnimalType.SwinePiglets &&
                managementPeriod.ProductionStage != ProductionStages.Weaned)
                dailyEmissions.CarbonAddedFromBeddingMaterial = 0;
            else
                dailyEmissions.CarbonAddedFromBeddingMaterial = CalculateAmountOfCarbonAddedFromBeddingMaterial(
                    dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                    managementPeriod.NumberOfAnimals);

            dailyEmissions.CarbonFromManureAndBedding = CalculateAmountOfCarbonFromManureAndBedding(
                dailyEmissions.FecalCarbonExcretion,
                dailyEmissions.CarbonAddedFromBeddingMaterial);

            dailyEmissions.VolatileSolidsAdjusted = CalculateVolatileSolidAdjusted(
                managementPeriod.ManureDetails.VolatileSolidExcretion,
                managementPeriod.SelectedDiet.VolatileSolidsAdjustmentFactorForDiet);

            dailyEmissions.VolatileSolids = CalculateVolatileSolids(
                dailyEmissions.VolatileSolidsAdjusted,
                dryMatterIntake);

            /*
             * Manure methane calculations differ depending if the manure is stored as a liquid or as a solid
             *
             * If user specifies custom a custom methane conversion factor, then skip liquid calculations (even if system is liquid, calculate manure methane using 2-4 and 2-5.)
             */

            if (managementPeriod.ManureDetails.StateType.IsSolidManure() ||
                managementPeriod.ManureDetails.UseCustomMethaneConversionFactor)
            {
                dailyEmissions.ManureMethaneEmissionRate = CalculateManureMethaneEmissionRate(
                    dailyEmissions.VolatileSolids,
                    managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    managementPeriod.ManureDetails.MethaneConversionFactor);

                dailyEmissions.ManureMethaneEmission = CalculateManureMethane(
                    dailyEmissions.ManureMethaneEmissionRate,
                    managementPeriod.NumberOfAnimals);
            }
            else
            {
                CalculateManureMethaneFromLiquidSystems(
                    dailyEmissions,
                    previousDaysEmissions,
                    managementPeriod,
                    temperature,
                    farm);
            }

            CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            dailyEmissions.ProteinIntake = CalculateProteinIntake(
                dryMatterIntake,
                managementPeriod.SelectedDiet.CrudeProteinContent);

            var isBreedingProductionStage = managementPeriod.ProductionStage == ProductionStages.Open ||
                                            managementPeriod.ProductionStage == ProductionStages.Gestating ||
                                            managementPeriod.ProductionStage == ProductionStages.Lactating;

            var isBreedingSow = animalGroup.GroupType == AnimalType.SwineSows ||
                                (animalGroup.GroupType == AnimalType.SwineGilts && isBreedingProductionStage);

            // Protein retained calculations for breeding sows only apply to the farrow to finish and farrow to wean components where the protein retained from weaned piglets must be considered
            if (isBreedingSow)
            {
                var fertilityRate = animalComponentBase.GetFertilityRate(
                    animalGroup.GroupType,
                    new List<ProductionStages>
                        { ProductionStages.Gestating, ProductionStages.Lactating, ProductionStages.Open });

                var weightChangeOfPregnantAnimals = animalGroup.CalculateWeightChangeOfPregnantAnimal();

                dailyEmissions.ProteinRetainedForGain = CalculateProteinRetainedForGainForBreedingSows(
                    fertilityRate,
                    weightChangeOfPregnantAnimals);

                var litterSize = 0d;
                if (animalGroup.LitterSizeOfBirthingAnimal > 0)
                    litterSize = animalGroup.LitterSizeOfBirthingAnimal;
                else
                    litterSize = animalComponentBase.CalculateAverageLitterSize(
                        new Tuple<AnimalType, ProductionStages>(AnimalType.SwinePiglets, ProductionStages.Weaning),
                        new List<Tuple<AnimalType, ProductionStages>>
                        {
                            new Tuple<AnimalType, ProductionStages>(AnimalType.SwineGilts, ProductionStages.Gestating),
                            new Tuple<AnimalType, ProductionStages>(AnimalType.SwineSows, ProductionStages.Gestating)
                        });

                dailyEmissions.ProteinRetainedByPiglets = CalculateProteinRetainedForNursingPiglets(
                    litterSize,
                    fertilityRate,
                    animalGroup.WeightOfPigletsAtBirth,
                    animalGroup.WeightOfWeanedAnimals);

                dailyEmissions.ProteinRetained = CalculateProteinRetainedForBreedingSows(
                    dailyEmissions.ProteinRetainedForGain,
                    dailyEmissions.ProteinRetainedByPiglets);
            }
            else
            {
                var medianWeight = (managementPeriod.StartWeight + managementPeriod.EndWeight) / 2.0;
                var nitrogenRequiredForGain =
                    _swineDefaultPrGainValuesProvider.GetNitrogenRequiredForGain(medianWeight);

                dailyEmissions.ProteinRetained = CalculateProteinRetainedForGrowingPigs(
                    managementPeriod.StartWeight,
                    managementPeriod.EndWeight,
                    nitrogenRequiredForGain,
                    managementPeriod.NumberOfDays);
            }

            dailyEmissions.NitrogenExcretionRate = CalculateNitrogenExcretionRateForSwine(
                dailyEmissions.ProteinIntake,
                dailyEmissions.ProteinRetained,
                managementPeriod.SelectedDiet.NitrogenExcretionAdjustFactorForDiet);

            dailyEmissions.AmountOfNitrogenExcreted = CalculateAmountOfNitrogenExcreted(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial = CalculateRateOfNitrogenAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            if (animalGroup.GroupType == AnimalType.SwinePiglets &&
                managementPeriod.ProductionStage != ProductionStages.Weaned)
                dailyEmissions.AmountOfNitrogenAddedFromBedding = 0;
            else
                dailyEmissions.AmountOfNitrogenAddedFromBedding = CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                    dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                    managementPeriod.NumberOfAnimals);

            dailyEmissions.ManureDirectN2ONEmissionRate = CalculateManureDirectNitrogenEmissionRate(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            dailyEmissions.ManureDirectN2ONEmission = CalculateManureDirectNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmissionRate,
                managementPeriod.NumberOfAnimals);

            /*
             * Indirect manure N2O
             */

            CalculateIndirectEmissionsFromHousingAndStorage(dailyEmissions, managementPeriod);

            dailyEmissions.ManureN2ONEmission = CalculateManureNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmission,
                dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage =
                CalculateNitrogenAvailableForLandApplicationFromSheepSwineAndOtherLivestock(
                    dailyEmissions.AmountOfNitrogenExcreted,
                    dailyEmissions.AmountOfNitrogenAddedFromBedding,
                    dailyEmissions.ManureDirectN2ONEmission,
                    dailyEmissions.TotalNitrogenLossesFromHousingAndStorage,
                    dailyEmissions.ManureN2ONLeachingEmission,
                    dailyEmissions.ManureNitrateLeachingEmission);

            // Equation 4.5.2-22
            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay =
                dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage +
                (previousDaysEmissions == null
                    ? 0
                    : previousDaysEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = CalculateManureCarbonToNitrogenRatio(
                dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay =
                dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                CalculateTotalVolumeOfManureAvailableForLandApplication(
                    dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage,
                    managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null
                                                   ? 0
                                                   : previousDaysEmissions.AccumulatedVolume);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            GetEmissionsFromGrazingSheepSwineAndOtherLiveStock(
                managementPeriod,
                dailyEmissions);

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm,
            AnimalComponentBase animalComponentBase)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsNewlyHatchedEggs() ||
                groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsEggs()) return;

            var energyConversionFactor = _energyConversionDefaultsProvider.GetElectricityConversionValue(
                groupEmissionsByMonth.MonthsAndDaysData.Year,
                farm.DefaultSoilData.Province);

            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = CalculateTotalCarbonDioxideEmissionsFromSwineHousing(
                groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                energyConversionFactor);
        }

        #endregion

        #region Equations

        /// <summary>
        ///     Equation 4.2.1-20
        /// </summary>
        /// <param name="proteinRetainedForGainForBreedingSows">Protein retained for gain for breeding sows (kg head^-1 day^-1)</param>
        /// <param name="proteinRetainedForNursingPiglets">Protein retained of weaned piglets (kg head^-1 day^-1)</param>
        /// <returns>Protein retained for breeding sows (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForBreedingSows(
            double proteinRetainedForGainForBreedingSows,
            double proteinRetainedForNursingPiglets)
        {
            return proteinRetainedForGainForBreedingSows + proteinRetainedForNursingPiglets;
        }

        /// <summary>
        ///     Equation 4.2.1-21
        /// </summary>
        /// <param name="fertilityRateOfSows">Fertility rate of sows (litters year^-1)</param>
        /// <param name="liveWeightChangeOfSowsDuringGestation">Weight change of animals during gestation (kg head^-1)</param>
        /// <returns>Protein retained for gain for breeding sows (kg head^-1)</returns>
        public double CalculateProteinRetainedForGainForBreedingSows(
            double fertilityRateOfSows,
            double liveWeightChangeOfSowsDuringGestation)
        {
            return 0.025 * fertilityRateOfSows * liveWeightChangeOfSowsDuringGestation / 350.0;
        }

        /// <summary>
        ///     Equation 4.2.1-22
        /// </summary>
        /// <param name="litterSize">Total number of weaned piglets</param>
        /// <param name="fertilityRateOfSows">Fertility rate of sows (litter year^-1)</param>
        /// <param name="liveWeightOfPigletAtBirth">Live weight of a piglet at birth (kg head^-1)</param>
        /// <param name="liveWeightOfPigletAtWeaningAge">Live weight of a piglet at weaning age (kg head^-1)</param>
        /// <returns>Protein retained of weaned piglets (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForNursingPiglets(
            double litterSize,
            double fertilityRateOfSows,
            double liveWeightOfPigletAtBirth,
            double liveWeightOfPigletAtWeaningAge)
        {
            return 0.025 * litterSize * fertilityRateOfSows *
                ((liveWeightOfPigletAtWeaningAge - liveWeightOfPigletAtBirth) / 0.98) / 350.0;
        }

        /// <summary>
        ///     Equation 4.2.1-23
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
            return (finalBodyWeight - initialBodyWeight) * (nitrogenRequiredForGain * 6.25) / numberOfDays;
        }

        /// <summary>
        ///     Equation 4.2.1-24
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
            return (proteinIntake / 6.25 - proteinRetained / 6.25) * nitrogenExcretedAdjustment;
        }

        /// <summary>
        ///     Equation 4.1.1-2
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
        ///     Equation 4.1.2-2
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
        ///     Equation 4.1.2-3
        /// </summary>
        /// <param name="volatileSolidAdjusted">Volatile solid adjusted</param>
        /// <param name="feedIntake">Feed intake (kg head^-1 day^-1)</param>
        /// <returns>Volatile solids (kg head^-1 day^-1)</returns>
        public double CalculateVolatileSolids(double volatileSolidAdjusted, double feedIntake)
        {
            return volatileSolidAdjusted * feedIntake;
        }

        /// <summary>
        ///     Equation 3.3.2-4
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
        ///     Equation 4.2.1-19
        /// </summary>
        /// <param name="dryMatterIntake">Feed intake (kg head^-1 day^-1)</param>
        /// <param name="crudeProteinIntake">Crude protein content (kg kg^-1)</param>
        /// <returns>Protein intake (kg head^-1 day^-1)</returns>
        public new double CalculateProteinIntake(double dryMatterIntake, double crudeProteinIntake)
        {
            return dryMatterIntake * crudeProteinIntake;
        }

        /// <summary>
        ///     Equation 3.3.3-3
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
        ///     Equation 3.3.3-6
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
        ///     Equation 3.3.3-12
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
        ///     Equation 3.3.5-1
        /// </summary>
        /// <param name="totalManureDirectNitrogenEmission">Total manure direct N emission from swine (kg N2O-N year^-1)</param>
        /// <returns>Total manure direct N2O emission from swine (kg N2O year^-1)</returns>
        public double CalculateTotalManureDirectN2OEmissionFromSwine(double totalManureDirectNitrogenEmission)
        {
            return totalManureDirectNitrogenEmission * 44 / 28;
        }

        /// <summary>
        ///     Equation 3.3.5-2
        /// </summary>
        /// <param name="totalManureIndirectNitrogenEmission">Total manure indirect N emission from swin (kg N2O-N year^-1)</param>
        /// <returns>Total manure indirect N2O emission from swin (kg N2O year^-1)</returns>
        public double CalculateTotalManureIndirectN2OEmissionFromSwine(double totalManureIndirectNitrogenEmission)
        {
            return totalManureIndirectNitrogenEmission * 44 / 28;
        }

        /// <summary>
        ///     Equation 3.3.5-3
        /// </summary>
        /// <param name="totalManureNitrogenEmission">Total manure N emission from swine (kg N2O-N year^-1)</param>
        /// <returns>Total manure N2O emission from swine (kg N2O year^-1)</returns>
        public double CalculateTotalManureN2OEmissionFromSwine(double totalManureNitrogenEmission)
        {
            return totalManureNitrogenEmission * 44 / 28;
        }

        /// <summary>
        ///     Equation 6.2.2-6
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

            var result = numberOfAnimals * (swineConversion / CoreConstants.DaysInYear) * electricityConversion *
                         numberOfDays;

            return result;
        }

        #endregion
    }
}