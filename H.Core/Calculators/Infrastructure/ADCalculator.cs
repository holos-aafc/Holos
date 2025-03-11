using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.Animals;
using H.Core.Providers.AnaerobicDigestion;
using System.ComponentModel;
using AutoMapper;
using H.Core.Providers.Climate;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using SubstrateFlowInformation = H.Core.Models.Infrastructure.SubstrateFlowInformation;

namespace H.Core.Calculators.Infrastructure
{
    public partial class ADCalculator : IADCalculator  
    {
        #region Fields

        
        private readonly IMapper _substrateMapper;

        protected readonly Table_47_Solid_Liquid_Separation_Coefficients_Provider
            _solidLiquidSeparationCoefficientsProvider = new Table_47_Solid_Liquid_Separation_Coefficients_Provider();

        private readonly Table_45_Parameter_Adjustments_For_Manure_Provider _reductionFactors =
            new Table_45_Parameter_Adjustments_For_Manure_Provider();

        #endregion

        #region Constructors

        public ADCalculator()
        {
            var substrateFlowMapperConfiguration = new MapperConfiguration(configure: configuration =>
            {
                configuration.CreateMap<SubstrateFlowInformation, SubstrateFlowInformation>()
                    .ForMember(property => property.Name, options => options.Ignore())
                    .ForMember(property => property.Guid, options => options.Ignore());
            });

            _substrateMapper = substrateFlowMapperConfiguration.CreateMapper();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public SubstrateFlowInformation GetFreshManureFlowRate(AnaerobicDigestionComponent component,
            GroupEmissionsByDay dailyEmissions,
            ADManagementPeriodViewItem adManagementPeriod, Farm farm)
        {
            var managementPeriod = adManagementPeriod.ManagementPeriod;

            var substrateFlowRate = new SubstrateFlowInformation()
            {
                DateCreated = dailyEmissions.DateTime,
                SubstrateType = SubstrateType.FreshManure,
                AnimalType = managementPeriod.AnimalType,
                ManagementPeriod = managementPeriod,
                Component = component,
                BiomethanePotential = adManagementPeriod.BiomethanePotential,
                MethaneFraction = adManagementPeriod.MethaneFraction,
            };

            var fractionAdded = adManagementPeriod.DailyFractionOfManureAdded;
            var manureComposition =
                farm.GetManureCompositionData(ManureStateType.Pasture, managementPeriod.AnimalType);

            // Equation 4.8.1-2
            substrateFlowRate.TotalMassFlowOfSubstrate =
                (((dailyEmissions.FecalNitrogenExcretion * 100) / manureComposition.NitrogenFraction) +
                 managementPeriod.HousingDetails.UserDefinedBeddingRate * managementPeriod.NumberOfAnimals) *
                fractionAdded;

            // Equation 4.8.1-3
            substrateFlowRate.TotalSolidsFlowOfSubstrate =
                substrateFlowRate.TotalMassFlowOfSubstrate * (adManagementPeriod.TotalSolids / 1000);

            // Equation 4.8.1-4
            substrateFlowRate.VolatileSolidsFlowOfSubstrate =
                dailyEmissions.VolatileSolids * managementPeriod.NumberOfAnimals * fractionAdded;

            // Equation 4.8.1-5
            substrateFlowRate.NitrogenFlowOfSubstrate =
                ((dailyEmissions.AmountOfNitrogenExcreted + dailyEmissions.AmountOfNitrogenAddedFromBedding) -
                 (dailyEmissions.ManureDirectN2ONEmission + dailyEmissions.AmmoniaConcentrationInHousing)) *
                fractionAdded;

            var animalType = managementPeriod.AnimalType;
            if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType() || animalType.IsSheepType())
            {
                // Equation 4.8.1-6
                substrateFlowRate.OrganicNitrogenFlowOfSubstrate =
                    (dailyEmissions.OrganicNitrogenInStoredManure - dailyEmissions.ManureDirectN2ONEmission) *
                    fractionAdded;
            }
            else
            {
                // Broilers, layers, and turkey

                // Equation 4.8.1-7
                substrateFlowRate.OrganicNitrogenFlowOfSubstrate = (dailyEmissions.AmountOfNitrogenExcreted - dailyEmissions.TanExcretionRate - dailyEmissions.ManureDirectN2ONEmission) * fractionAdded;
            }

            // Equation 4.8.1-8
            substrateFlowRate.ExcretedTanInSubstrate = dailyEmissions.TanEnteringStorageSystem * fractionAdded;

            // Equation 4.8.1-9
            substrateFlowRate.CarbonFlowOfSubstrate = dailyEmissions.AmountOfCarbonInStoredManure * fractionAdded;

            return substrateFlowRate;
        }

        public SubstrateFlowInformation GetStoredManureFlowRate(AnaerobicDigestionComponent component,
            GroupEmissionsByDay dailyEmissions,
            ADManagementPeriodViewItem adManagementPeriodViewItem,
            GroupEmissionsByDay previousDaysEmissions, Farm farm)
        {
            var managementPeriod = adManagementPeriodViewItem.ManagementPeriod;

            var substrateFlowRate = new SubstrateFlowInformation
            {
                SubstrateType = SubstrateType.StoredManure,
                AnimalType = managementPeriod.AnimalType,
                DateCreated = dailyEmissions.DateTime,
                ManagementPeriod = managementPeriod,
                BiomethanePotential = adManagementPeriodViewItem.BiomethanePotential,
                MethaneFraction = adManagementPeriodViewItem.MethaneFraction,
                Component = component,
            };

            var fractionUsed = adManagementPeriodViewItem.DailyFractionOfManureAdded;

            var totalMassFlowOfSubstrate = 0d;
            var animalType = managementPeriod.AnimalType;
            var nitrogenContentOfManure = managementPeriod.ManureDetails.FractionOfNitrogenInManure;
            var manureComposition = farm.GetManureCompositionData(managementPeriod.ManureDetails.StateType, managementPeriod.AnimalType);
            var moistureContent = manureComposition.MoistureContent;
            if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType() || animalType.IsPoultryType())
            {
                // Equation 4.8.1-16
                totalMassFlowOfSubstrate = (((dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay + dailyEmissions.OrganicNitrogenCreatedOnDay) * 100) / nitrogenContentOfManure) * fractionUsed; 
            }
            else
            {
                // Equation 4.8.1-17

                // Sheep, swine, and other livestock
                totalMassFlowOfSubstrate = ((dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay * 100) / nitrogenContentOfManure) * fractionUsed;
            }

            substrateFlowRate.TotalMassFlowOfSubstrate = totalMassFlowOfSubstrate;

            // Equation 4.8.1-18
            substrateFlowRate.TotalSolidsFlowOfSubstrate = substrateFlowRate.TotalMassFlowOfSubstrate * (1 - (moistureContent / 100.0));

            if (managementPeriod.ManureDetails.StateType.IsLiquidManure())
            {
                var volatileSolidsAvailableOnCurrentDay = dailyEmissions.VolatileSolidsAvailable;
                var volatileSolidsOnPreviousDay = previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsAvailable;

                // Equation 4.8.1-19
                var a = volatileSolidsAvailableOnCurrentDay / (dailyEmissions.TotalVolumeOfManureAvailableForLandApplication * 1000);
                var b = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication - (previousDaysEmissions == null ? 0 : previousDaysEmissions.TotalVolumeOfManureAvailableForLandApplication);
                var c = fractionUsed * 1000;

                var result = a * (b - c);

                substrateFlowRate.VolatileSolidsFlowOfSubstrate = result;

                if (substrateFlowRate.VolatileSolidsFlowOfSubstrate < 0)
                {
                    substrateFlowRate.VolatileSolidsFlowOfSubstrate = 0;
                }
            }
            else
            {
                var reductionFactor = _reductionFactors.GetParametersAdjustmentInstance(managementPeriod.ManureDetails.StateType);

                // Equation 4.8.1-20
                substrateFlowRate.VolatileSolidsFlowOfSubstrate = dailyEmissions.VolatileSolids * (1 - reductionFactor.VolatileSolidsReductionFactor) * managementPeriod.NumberOfAnimals * fractionUsed;
            }

            if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
            {
                // Equation 4.8.1-21
                substrateFlowRate.NitrogenFlowOfSubstrate = (dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay + dailyEmissions.OrganicNitrogenCreatedOnDay) * fractionUsed;

                // Equation 4.8.1-22
                substrateFlowRate.OrganicNitrogenFlowOfSubstrate = dailyEmissions.OrganicNitrogenCreatedOnDay * fractionUsed;

                // Equation 4.8.1-24
                substrateFlowRate.ExcretedTanInSubstrate = dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay * fractionUsed;
            }
            else if (animalType.IsPoultryType())
            {
                /*
                 * Poultry
                 */

                // Equation 4.8.1-21
                substrateFlowRate.NitrogenFlowOfSubstrate = dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage * fractionUsed;

                // Equation 4.8.1-23
                substrateFlowRate.OrganicNitrogenFlowOfSubstrate = substrateFlowRate.NitrogenFlowOfSubstrate - dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay;
                if (substrateFlowRate.OrganicNitrogenFlowOfSubstrate < 0)
                {
                    substrateFlowRate.OrganicNitrogenFlowOfSubstrate = 0;
                }

                // Equation 4.8.1-25
                substrateFlowRate.ExcretedTanInSubstrate = dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay * fractionUsed;
            }
            else
            {
                /*
                 * Other livestock
                 */

                // Equation 4.8.1-21
                substrateFlowRate.NitrogenFlowOfSubstrate = dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage * fractionUsed;
            }

            // Equation 4.8.1-26
            substrateFlowRate.CarbonFlowOfSubstrate = dailyEmissions.AmountOfCarbonInStoredManure * fractionUsed;

            return substrateFlowRate;
        }

        /// <summary>
        /// Calculates biogas production from both fresh and stored manure
        /// </summary>
        public void CalculateDailyBiogasProductionFromSingleSubstrate(SubstrateFlowInformation substrateFlowRate)
        {
            var biodegradableFraction = this.GetBiodegradableFraction(substrateFlowRate);
            var hydrolosisRate = 0.0;
            var substrateType = substrateFlowRate.SubstrateType;

            if (substrateType == SubstrateType.FreshManure || substrateFlowRate.IsLiquidManure())
            {
                hydrolosisRate = 0.18;
            }
            else if (substrateType == SubstrateType.StoredManure || substrateType == SubstrateType.ImportedManure)
            {
                hydrolosisRate = 0.05;
            }
            else
            {
                hydrolosisRate = 0.13;
            }

            // Equation 4.8.2-1
            substrateFlowRate.BiodegradableSolidsFlow =
                substrateFlowRate.VolatileSolidsFlowOfSubstrate * biodegradableFraction;

            // Equation 4.8.2-3 (equation number is correct, need this value before calculating next value
            substrateFlowRate.DegradedVolatileSolids = substrateFlowRate.BiodegradableSolidsFlow - (substrateFlowRate.BiodegradableSolidsFlow / (1 + hydrolosisRate * substrateFlowRate.Component.HydraulicRetentionTimeInDays));

            // Equation 4.8.2-2 (inner term)
            substrateFlowRate.MethaneProduction = substrateFlowRate.DegradedVolatileSolids * (substrateFlowRate.BiomethanePotential / 1000.0);

            // Equation 4.8.2-5
            substrateFlowRate.BiogasProduction =
                substrateFlowRate.MethaneProduction / substrateFlowRate.MethaneFraction;

            // Equation 4.8.2-7
            substrateFlowRate.CarbonDioxideProduction =
                substrateFlowRate.BiogasProduction - substrateFlowRate.MethaneProduction;
        }

        public void CalculateFlowsInDigestateFromSingleSubstrate(SubstrateFlowInformation substrateFlowRate)
        {
            // Equation 4.8.3-6
            var totalNitrogenContentOfVolatileSolids = substrateFlowRate.VolatileSolidsFlowOfSubstrate > 0
                ? (substrateFlowRate.NitrogenFlowOfSubstrate - substrateFlowRate.ExcretedTanInSubstrate) /
                  substrateFlowRate.VolatileSolidsFlowOfSubstrate
                : 0;

            // Equation 4.8.3-5 (inner term)
            substrateFlowRate.TanFlowInDigestate = substrateFlowRate.ExcretedTanInSubstrate +
                                                   substrateFlowRate.DegradedVolatileSolids *
                                                   totalNitrogenContentOfVolatileSolids;

            // Equation 4.8.3-7 (inner term)
            var organicFlow = 0d;
            organicFlow = substrateFlowRate.OrganicNitrogenFlowOfSubstrate - 
                          substrateFlowRate.DegradedVolatileSolids * 
                          totalNitrogenContentOfVolatileSolids;

            if (organicFlow < 0)
            {
                // Imported manure will have no organic N information available so above calculation will be negative
                organicFlow = 0;
            }

            substrateFlowRate.OrganicNitrogenFlowInDigestate = organicFlow;

            // Equation 4.8.3-8 (inner term)
            substrateFlowRate.CarbonFlowInDigestate = substrateFlowRate.VolatileSolidsFlowOfSubstrate > 0
                ? substrateFlowRate.CarbonFlowOfSubstrate - substrateFlowRate.DegradedVolatileSolids *
                (substrateFlowRate.CarbonFlowOfSubstrate / substrateFlowRate.VolatileSolidsFlowOfSubstrate)
                : 0;
        }

        /// <summary>
        /// The total mass flow rate of digestate is equal to the sum of the mass flow rates of the substrates for codigestion
        /// </summary>
        public void CalculateDigestateStorageEmissions(
            Farm farm,
            DateTime dateTime,
            AnaerobicDigestionComponent component,
            DigestorDailyOutput digestorDailyOutput)
        {
            var temperature =
                farm.ClimateData.GetAverageTemperatureForMonthAndYear(dateTime.Year, (Months) dateTime.Month);
            var methaneEmissionFactorDuringStorage = 0.0175 * Math.Pow(temperature, 2) - 0.0245 * temperature + 0.1433;

            // Equation 4.8.5-1
            digestorDailyOutput.MethaneEmissionsDuringStorage = (methaneEmissionFactorDuringStorage / 1000000.0)*
                                                                digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;

            // Equation 4.8.5-2
            digestorDailyOutput.N2OEmissionsDuringStorage =
                (0.0652 / 1000000.0) * digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;

            // Equation 4.8.5-3
            digestorDailyOutput.AmmoniaEmissionsDuringStorage =
                (3.495 / 1000000.0) * digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;
        }

        public void CalculateLiquidSolidSeparation(
            DigestorDailyOutput digestorDailyOutput,
            AnaerobicDigestionComponent component)
        {
            component.VolumeOfDigestateEnteringStorage = digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;

            var rawMaterialCoefficients =
                _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(
                    DigestateParameters.RawMaterial);
            var rawMaterialCoefficient = component.IsCentrifugeType
                ? rawMaterialCoefficients.Centrifuge
                : rawMaterialCoefficients.BeltPress;


            // Equation 4.8.4-1
            digestorDailyOutput.FlowRateLiquidFraction = (1 - rawMaterialCoefficient) *
                                                         digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;

            // Equation 4.8.4-2
            digestorDailyOutput.FlowRateSolidFraction =
                rawMaterialCoefficient * digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;

            var totalSolidsCoefficients =
                _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(
                    DigestateParameters.TotalSolids);
            var totalSolidsCoefficient = component.IsCentrifugeType
                ? totalSolidsCoefficients.Centrifuge
                : totalSolidsCoefficients.BeltPress;

            // Equation 4.8.4-3
            digestorDailyOutput.FlowOfTotalSolidsInLiquidFraction = (1 - totalSolidsCoefficient) *
                                                                    digestorDailyOutput
                                                                        .FlowRateOfAllTotalSolidsInDigestate;

            // Equation 4.8.4-4
            digestorDailyOutput.FlowOfTotalSolidsInSolidFraction =
                totalSolidsCoefficient * digestorDailyOutput.FlowRateOfAllTotalSolidsInDigestate;

            var volatileSolidsCoefficients =
                _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(
                    DigestateParameters.VolatileSolids);
            var volatileSolidsCoefficient = component.IsCentrifugeType
                ? volatileSolidsCoefficients.Centrifuge
                : volatileSolidsCoefficients.BeltPress;

            // Equation 4.8.4-5
            digestorDailyOutput.TotalVolatileSolidsLiquidFraction = (1 - volatileSolidsCoefficient) *
                                                                    digestorDailyOutput
                                                                        .FlowRateOfAllVolatileSolidsInDigestate;

            // Equation 4.8.4-6
            digestorDailyOutput.TotalVolatileSolidsSolidFraction = volatileSolidsCoefficient *
                                                                   digestorDailyOutput
                                                                       .FlowRateOfAllVolatileSolidsInDigestate;

            var tanCoefficients =
                _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(
                    DigestateParameters.TotalAmmoniaNitrogen);
            var tanCoefficient = component.IsCentrifugeType ? tanCoefficients.Centrifuge : tanCoefficients.BeltPress;

            // Equation 4.8.4-9
            digestorDailyOutput.TotalTanLiquidFraction =
                (1 - tanCoefficient) * digestorDailyOutput.FlowOfAllTanInDigestate;

            // Equation 4.8.4-10
            digestorDailyOutput.TotalTanSolidFraction = tanCoefficient * digestorDailyOutput.FlowOfAllTanInDigestate;

            var organicNCoefficients =
                _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(
                    DigestateParameters.OrganicNitrogen);
            var organicNCoefficient = component.IsCentrifugeType
                ? organicNCoefficients.Centrifuge
                : organicNCoefficients.BeltPress;

            // Equation 4.8.4-11
            digestorDailyOutput.OrganicNLiquidFraction = (1 - organicNCoefficient) *
                                                         digestorDailyOutput.FlowRateOfAllOrganicNitrogenInDigestate;

            // Equation 4.8.4-12
            digestorDailyOutput.OrganicNSolidFraction =
                organicNCoefficient * digestorDailyOutput.FlowRateOfAllOrganicNitrogenInDigestate;

            var totalNitrogenLiquidFraction = digestorDailyOutput.TotalTanLiquidFraction + digestorDailyOutput.OrganicNLiquidFraction;
            var totalNitrogenSolidFraction = digestorDailyOutput.TotalTanSolidFraction + digestorDailyOutput.OrganicNSolidFraction;

            // Equation 4.8.4-7
            digestorDailyOutput.TotalNitrogenLiquidFraction = totalNitrogenLiquidFraction;

            // Equation 4.8.4-8
            digestorDailyOutput.TotalNitrogenSolidFraction = totalNitrogenSolidFraction;

            var carbonCoefficients =
                _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(
                    DigestateParameters.TotalCarbon);
            var carbonCoefficient = component.IsCentrifugeType
                ? carbonCoefficients.Centrifuge
                : carbonCoefficients.BeltPress;

            // Equation 4.8.4-13
            digestorDailyOutput.CarbonLiquidFraction = (1 - carbonCoefficient) * digestorDailyOutput.FlowOfAllCarbon;

            // Equation 4.8.4-14
            digestorDailyOutput.CarbonSolidFraction = carbonCoefficient * digestorDailyOutput.FlowOfAllCarbon;
        }

        /// <summary>
        /// Calculates fresh and stored digestate available for land application
        /// </summary>
        public void CalculateAmountsForLandApplication(DigestorDailyOutput digestorDailyOutput)
        {
            /*
             * Digestate that has not been liquid/solid separated
             */

            // Equation 4.8.3-1
            digestorDailyOutput.TotalAmountRawDigestateAvailableForLandApplication =
                digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;

            // Equation 4.8.3-4
            digestorDailyOutput.TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication =
                digestorDailyOutput.FlowRateOfTotalNitrogenInDigestate;

            // Equation 4.8.3-5
            digestorDailyOutput.TotalAmountOfTanInRawDigestateAvailalbleForLandApplication =
                digestorDailyOutput.FlowOfAllTanInDigestate;

            // Equation 4.8.3-7
            digestorDailyOutput.TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplication =
                digestorDailyOutput.FlowRateOfAllOrganicNitrogenInDigestate;

            // Equation 4.8.3-8
            digestorDailyOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplication =
                digestorDailyOutput.FlowOfAllCarbon;

            /*
             * Liquid fractions of liquid/solid separated digestate
             */

            // Equation 4.8.4-1
            digestorDailyOutput.TotalAmountRawDigestateAvailableForLandApplicationFromLiquidFraction =
                digestorDailyOutput.FlowRateLiquidFraction;

            // Equation 4.8.4-7
            digestorDailyOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction =
                digestorDailyOutput.TotalNitrogenLiquidFraction;

            // Equation 4.8.4-9
            digestorDailyOutput.TotalAmountOfTanInRawDigestateAvailalbleForLandApplicationFromLiquidFraction =
                digestorDailyOutput.TotalTanLiquidFraction;

            // Equation 4.8.4-11
            digestorDailyOutput
                    .TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction =
                digestorDailyOutput.OrganicNLiquidFraction;

            // Equation 4.8.4-13
            digestorDailyOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction =
                digestorDailyOutput.CarbonLiquidFraction;

            /*
             * Solid fractions of liquid/solid separated digestate
             */

            // Equation 4.8.4-2
            digestorDailyOutput.TotalAmountRawDigestateAvailableForLandApplicationFromSolidFraction =
                digestorDailyOutput.FlowRateSolidFraction;

            // Equation 4.8.4-8
            digestorDailyOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction =
                digestorDailyOutput.TotalNitrogenSolidFraction;

            // Equation 4.8.4-10
            digestorDailyOutput.TotalAmountOfTanInRawDigestateAvailalbleForLandApplicationFromSolidFraction =
                digestorDailyOutput.TotalTanSolidFraction;

            // Equation 4.8.4-12
            digestorDailyOutput.TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction =
                digestorDailyOutput.OrganicNSolidFraction;

            // Equation 4.8.4-14
            digestorDailyOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction =
                digestorDailyOutput.CarbonSolidFraction;

            /*
             * Stored digestate (liquid and solid fractions)
             */

            // Equation 4.8.3-1
            digestorDailyOutput.TotalAmountOfStoredDigestateAvailableForLandApplication =
                digestorDailyOutput.FlowRateOfAllSubstratesInDigestate;

            // Equation 4.8.4-1
            digestorDailyOutput.TotalAmountOfStoredDigestateAvailableForLandApplicationLiquidFraction =
                digestorDailyOutput.FlowRateLiquidFraction;

            // Equation 4.8.4-2
            digestorDailyOutput.TotalAmountOfStoredDigestateAvailableForLandApplicationSolidFraction =
                digestorDailyOutput.FlowRateSolidFraction;
        }

        public void CalculateTotalBiogasProductionFromAllSubstratesOnSameDay(
            DigestorDailyOutput digestorDailyOutput,
            List<SubstrateFlowInformation> flowInformationForAllSubstrates)
        {
            // Equation 4.8.2-2 (summation)
            digestorDailyOutput.TotalMethaneProduction = flowInformationForAllSubstrates.Sum(x => x.MethaneProduction);

            // Equation 4.8.2-4
            digestorDailyOutput.TotalFlowOfDegradedVolatileSolids =
                flowInformationForAllSubstrates.Sum(x => x.DegradedVolatileSolids);

            // Equation 4.8.2-6
            digestorDailyOutput.TotalBiogasProduction = flowInformationForAllSubstrates.Sum(x => x.BiogasProduction);

            // Equation 4.8.2-8
            digestorDailyOutput.TotalCarbonDioxideProduction = digestorDailyOutput.TotalBiogasProduction -
                                                               digestorDailyOutput.TotalMethaneProduction;

            if (digestorDailyOutput.TotalCarbonDioxideProduction < 0)
            {
                digestorDailyOutput.TotalCarbonDioxideProduction = 0;
            }

            // Equation 4.8.2-11
            digestorDailyOutput.TotalRecoverableMethane = digestorDailyOutput.TotalMethaneProduction * (1 - 0.03);

            // Equation 4.8.2-12
            digestorDailyOutput.TotalPrimaryEnergyProduction =
                digestorDailyOutput.TotalMethaneProduction * (35.17 / 3.6);

            // Equation 4.8.2-13
            digestorDailyOutput.ElectricityProduction = digestorDailyOutput.TotalPrimaryEnergyProduction * 0.4;

            // Equation 4.8.2-14
            digestorDailyOutput.HeatProduced = digestorDailyOutput.TotalPrimaryEnergyProduction * 0.5;

            digestorDailyOutput.FugitiveMethaneLost = digestorDailyOutput.TotalRecoverableMethane * 0.0081;

            // Equation 4.8.2-15
            digestorDailyOutput.MethaneToGrid = digestorDailyOutput.TotalRecoverableMethane * (1 - 0.0081);
        }

        public void CalculateTotalProductionFromAllSubstratesOnSameDay(
            DigestorDailyOutput digestorDailyOutput,
            List<SubstrateFlowInformation> flowInformationForAllSubstrates)
        {
            /*
             * Calculate total flows from all substrates
             */

            // Equation 4.8.3-1
            digestorDailyOutput.FlowRateOfAllSubstratesInDigestate =
                flowInformationForAllSubstrates.Sum(x => x.TotalMassFlowOfSubstrate);

            var flowRateOfAllTotalSolids = 0d;
            var flowRateOfAllVolatileSolids = 0d;
            foreach (var flowInformationForAllSubstrate in flowInformationForAllSubstrates)
            {
                var totalSolidsFlowOfSubstrate = flowInformationForAllSubstrate.TotalSolidsFlowOfSubstrate - flowInformationForAllSubstrate.DegradedVolatileSolids;
                if (totalSolidsFlowOfSubstrate < 0)
                {
                    // Imported manure will have no TS (TS = 0) so we don't add negative numbers here since the degraded VS will be non-zero
                    totalSolidsFlowOfSubstrate = 0;
                }

                var volatileSolidsFlowOfSubstrate = flowInformationForAllSubstrate.VolatileSolidsFlowOfSubstrate -flowInformationForAllSubstrate.DegradedVolatileSolids;
                if (volatileSolidsFlowOfSubstrate < 0)
                {
                    volatileSolidsFlowOfSubstrate = 0;
                }

                flowRateOfAllTotalSolids += totalSolidsFlowOfSubstrate;
                flowRateOfAllVolatileSolids += volatileSolidsFlowOfSubstrate;
            }

            // Equation 4.8.3-2
            digestorDailyOutput.FlowRateOfAllTotalSolidsInDigestate = flowRateOfAllTotalSolids;

            // Equation 4.8.3-3
            digestorDailyOutput.FlowRateOfAllVolatileSolidsInDigestate = flowRateOfAllVolatileSolids;

            // Equation 4.8.3-4 (summation)
            digestorDailyOutput.FlowRateOfTotalNitrogenInDigestate =
                flowInformationForAllSubstrates.Sum(x => x.NitrogenFlowOfSubstrate);

            // Equation 4.8.3-5 (summation)
            digestorDailyOutput.FlowOfAllTanInDigestate =
                flowInformationForAllSubstrates.Sum(x => x.TanFlowInDigestate);

            // Equation 4.8.3-7 (summation)
            digestorDailyOutput.FlowRateOfAllOrganicNitrogenInDigestate =
                flowInformationForAllSubstrates.Sum(x => x.OrganicNitrogenFlowInDigestate);

            // Equation 4.8.3-8 (summation)
            digestorDailyOutput.FlowOfAllCarbon = flowInformationForAllSubstrates.Sum(x => x.CarbonFlowOfSubstrate);
        }

        public List<DigestorDailyOutput> CalculateResults(
            Farm farm,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults)
        {
            var results = new List<DigestorDailyOutput>();

            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                return results;
            }

            var dailyFlowRates = new List<SubstrateFlowInformation>();

            var onFarmManureFlowRates = this.GetDailyManureFlowRates(farm, animalComponentEmissionsResults, component);
            var cropResidueFlowRates = this.GetDailyCropResidueFlowRates(component);
            var farmResidueFlowsRates = this.GetDailyFarmResidueFlowRates(component);
            var importedManureFlowRates = this.GetDailyImportedManureResidueFlowRates(component);

            dailyFlowRates.AddRange(onFarmManureFlowRates);
            dailyFlowRates.AddRange(farmResidueFlowsRates);
            dailyFlowRates.AddRange(cropResidueFlowRates);
            dailyFlowRates.AddRange(importedManureFlowRates);

            foreach (var substrateFlowInformation in dailyFlowRates)
            {
                this.CalculateDailyBiogasProductionFromSingleSubstrate(substrateFlowInformation);

                if (substrateFlowInformation.SubstrateType != SubstrateType.FarmResidues)
                {
                    // These calculate flows from manure only
                    this.CalculateFlowsInDigestateFromSingleSubstrate(substrateFlowInformation);
                }
            }

            // Sum flows that occur on same day.
            var combinedDailyResults = this.CombineSubstrateFlowsOfSameTypeOnSameDay(dailyFlowRates, component, farm);

            results.AddRange(combinedDailyResults);

            return results.OrderBy(x => x.Date).ToList();
        }

        public List<SubstrateFlowInformation> GetDailyFlowRatesForSubstrateTypes(
            IEnumerable<SubstrateViewItemBase> substrateViewItems, 
            SubstrateType type,
            AnaerobicDigestionComponent component)
        {
            var result = new List<SubstrateFlowInformation>();

            // TODO: Unit attributes/comments needed for FarmResidueSubstrateViewItem class.
            foreach (var substrateViewItemBase in substrateViewItems)
            {
                var startDate = substrateViewItemBase.StartDate;
                var endDate = substrateViewItemBase.EndDate;

                for (DateTime currentDate = startDate; currentDate < endDate; currentDate += TimeSpan.FromDays(1))
                {
                    var substrateFlow = new SubstrateFlowInformation()
                    {
                        SubstrateType = type,
                        Component = component,
                        MethaneFraction = substrateViewItemBase.MethaneFraction,
                        BiomethanePotential = substrateViewItemBase.BiomethanePotential,
                        DateCreated = currentDate,
                        VolatileSolidsContent = substrateViewItemBase.VolatileSolidsContent,
                    };

                    substrateFlow.FlowRate = substrateViewItemBase.FlowRate;

                    /* Equation 4.8.1-10
                     */
                    substrateFlow.TotalMassFlowOfSubstrate = substrateViewItemBase.FlowRate;

                    /* Equation 4.8.1-11
                     *
                     * Note: TS will be 0 for manure
                     */
                    substrateFlow.TotalSolidsFlowOfSubstrate = substrateFlow.TotalMassFlowOfSubstrate * (substrateViewItemBase.TotalSolids / 1000.0);

                    // Equation 4.8.1-12
                    substrateFlow.VolatileSolidsFlowOfSubstrate = substrateViewItemBase.VolatileSolids;
                    if (type == SubstrateType.ImportedManure)
                    {
                        // Equation 4.8.1-29
                        substrateFlow.VolatileSolidsFlowOfSubstrate = substrateViewItemBase.FlowRate * substrateViewItemBase.VolatileSolidsContent;
                    }

                    // Equation 4.8.1-13
                    substrateFlow.NitrogenFlowOfSubstrate = (substrateViewItemBase.TotalNitrogen / 1000.0);
                    if (type == SubstrateType.ImportedManure)
                    {
                        // Equation 4.8.1-30
                        substrateFlow.NitrogenFlowOfSubstrate = substrateViewItemBase.FlowRate * (substrateViewItemBase.NitrogenContent / 100.0);
                    }

                    // Equation 4.8.1-14
                    substrateFlow.CarbonFlowOfSubstrate = substrateViewItemBase.TotalCarbon;
                    if (type == SubstrateType.ImportedManure)
                    {
                        // Equation 4.8.1-31
                        substrateFlow.CarbonFlowOfSubstrate = substrateViewItemBase.FlowRate * (substrateViewItemBase.CarbonContent / 100.0);
                    }

                    result.Add(substrateFlow);
                }
            }

            return result;
        }

        public List<SubstrateFlowInformation> GetDailyFarmResidueFlowRates(AnaerobicDigestionComponent component)
        {
            return this.GetDailyFlowRatesForSubstrateTypes(
                component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems, SubstrateType.FarmResidues,
                component);
        }

        public List<SubstrateFlowInformation> GetDailyCropResidueFlowRates(AnaerobicDigestionComponent component)
        {
            return this.GetDailyFlowRatesForSubstrateTypes(
                component.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems, SubstrateType.CropResidues,
                component);
        }

        public List<SubstrateFlowInformation> GetDailyImportedManureResidueFlowRates(AnaerobicDigestionComponent component)
        {
            return this.GetDailyFlowRatesForSubstrateTypes(
                component.AnaerobicDigestionViewItem.ManureSubstrateViewItems, SubstrateType.ImportedManure,
                component);
        }

        public List<SubstrateFlowInformation> GetDailyManureFlowRates(
            Farm farm,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            AnaerobicDigestionComponent component)
        {
            var flows = new List<SubstrateFlowInformation>();
            var selectedManagementPeriods = component.ManagementPeriodViewItems.Where(x => x.IsSelected);

            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                foreach (var animalGroupResults in animalComponentEmissionsResult
                             .EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in animalGroupResults.GroupEmissionsByMonths)
                    {
                        if (selectedManagementPeriods.Select(x => x.ManagementPeriod)
                            .Contains(groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod))
                        {
                            var managementPeriod = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod;
                            var adManagementPeriod =
                                selectedManagementPeriods.Single(x => x.ManagementPeriod.Equals(managementPeriod));

                            for (int i = 0; i < groupEmissionsByMonth.DailyEmissions.Count; i++)
                            {
                                var currentDayEmissions = groupEmissionsByMonth.DailyEmissions.ElementAt(i);
                                var previousDayEmissions = groupEmissionsByMonth.DailyEmissions.ElementAtOrDefault(i - 1);

                                var flowRates = this.GetStoredManureFlowRate(
                                    component,
                                    currentDayEmissions,
                                    adManagementPeriod, previousDayEmissions, farm);

                                flows.Add(flowRates);
                            }
                        }
                    }
                }
            }

            return flows;
        }

        /// <summary>
        /// We take all flows and combine any flows that occur on the same day and that are of the same type of substrate. For example,
        /// if there are two flows of dairy manure, we combine them together into a single flow. If the substrate are not of the same type
        /// we do not combine them and return two separate items.
        /// </summary>
        public List<DigestorDailyOutput> CombineSubstrateFlowsOfSameTypeOnSameDay(
            List<SubstrateFlowInformation> substrateFlows, AnaerobicDigestionComponent component, Farm farm)
        {
            var result = new List<DigestorDailyOutput>();

            // Sum flows that occur on same day.
            var flowsGroupedByDay = substrateFlows.GroupBy(x => x.DateCreated.Date);
            foreach (var byDayGroup in flowsGroupedByDay)
            {
                // Here, all the flows are for the same day

                var dailyOutput = new DigestorDailyOutput()
                {
                    Date = byDayGroup.Key.Date,
                };
                var flowsForDate = byDayGroup.Where(x => x.DateCreated.Date == byDayGroup.Key).ToList();

                // Equation 4.8.2-1
                var flowOfBiodegradableSolids = flowsForDate.Sum(x => x.BiodegradableSolidsFlow);

                this.CalculateTotalProductionFromAllSubstratesOnSameDay(dailyOutput, flowsForDate);
                this.CalculateTotalBiogasProductionFromAllSubstratesOnSameDay(dailyOutput, flowsForDate);
                this.CalculateLiquidSolidSeparation(dailyOutput, component);
                this.CalculateAmountsForLandApplication(dailyOutput);  
                this.CalculateDigestateStorageEmissions(farm, dailyOutput.Date, component, dailyOutput);

                // Equation 4.8.6-1
                dailyOutput.TotalNitrogenInDigestateAvailableForLandApplication = dailyOutput.FlowRateOfTotalNitrogenInDigestate - (CoreConstants.ConvertToN(dailyOutput.N2OEmissionsDuringStorage ) + CoreConstants.ConvertToNH3N(dailyOutput.AmmoniaEmissionsDuringStorage));
                if (dailyOutput.TotalNitrogenInDigestateAvailableForLandApplication < 0)
                {
                    dailyOutput.TotalNitrogenInDigestateAvailableForLandApplication = 0;
                }

                // Equation 4.8.6-2
                dailyOutput.TotalCarbonInDigestateAvailableForLandApplication = dailyOutput.FlowOfAllCarbon -  CoreConstants.ConvertToC(dailyOutput.MethaneEmissionsDuringStorage);
                if (dailyOutput.TotalCarbonInDigestateAvailableForLandApplication < 0)
                {
                    dailyOutput.TotalCarbonInDigestateAvailableForLandApplication = 0;
                }

                result.Add(dailyOutput);
            }

            return result;
        }

        /// <summary>
        /// No table number, see section 4.8.2
        /// </summary>
        protected double GetBiodegradableFraction(SubstrateFlowInformation substrateFlowInformation)
        {
            if (substrateFlowInformation.SubstrateType == SubstrateType.FarmResidues)
            {
                return 0.024;
            }

            if (substrateFlowInformation.AnimalType.IsDairyCattleType())
            {
                return 0.025;
            }
            else if (substrateFlowInformation.AnimalType.IsSwineType())
            {
                return 0.024;
            }
            else
            {
                return 0.55;
            }
        }

        #endregion
    }
}