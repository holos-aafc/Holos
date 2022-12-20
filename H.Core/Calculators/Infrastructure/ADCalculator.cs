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
using H.Core.Providers.Climate;

namespace H.Core.Calculators.Infrastructure
{
    public class ADCalculator
    {
        #region Fields

        protected readonly Table_47_Solid_Liquid_Separation_Coefficients_Provider _solidLiquidSeparationCoefficientsProvider = new Table_47_Solid_Liquid_Separation_Coefficients_Provider();
        private readonly Table_45_Parameter_Adjustments_For_Manure_Provider _reductionFactors = new Table_45_Parameter_Adjustments_For_Manure_Provider();
        private readonly Table_46_Biogas_Methane_Production_Parameters_Provider _biogasMethaneProductionParametersProvider = new Table_46_Biogas_Methane_Production_Parameters_Provider();

        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        public List<SubstrateFlowInformation> GetStoredManureFlowRates(
            AnaerobicDigestionComponent component, 
            GroupEmissionsByDay dailyEmissions, 
            ManagementPeriod managementPeriod)
        {
            var result = new List<SubstrateFlowInformation>();

            var reductionFactor = _reductionFactors.GetParametersAdjustmentInstance(managementPeriod.ManureDetails.StateType);

            // TODO: Add flow rate column to farm residue grid view. Also, unit attributes/comments needed for FarmResidueSubstrateViewItem class.
            var manureViewItems = component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.Where(y => y.IsFreshManure == false).GroupBy(x => x.AnimalType).ToList();

            foreach (var residueGroup in manureViewItems)
            {
                var substrateFlowRate = new SubstrateFlowInformation();
                substrateFlowRate.SubstrateType = SubstrateType.StoredManure;
                substrateFlowRate.AnimalType = residueGroup.Key;

                foreach (var viewItem in residueGroup)
                {
                    // Equation 4.8.1-16
                    var flow = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication * component.ProportionTotalManureAddedToAD;

                    substrateFlowRate.TotalMassFlow += flow;
                    substrateFlowRate.TotalSolidsFlow += flow * viewItem.TotalSolids;

                    if (managementPeriod.ManureDetails.StateType.IsLiquidManure())
                    {
                        // Equation 4.8.1-18
                        // TODO: this needs to be the sum of the daily vs_loaded and daily vs_consumed
                        substrateFlowRate.VolatileSolidsFlow += (dailyEmissions.VolatileSolidsLoaded - dailyEmissions.VolatileSolidsConsumed) * component.ProportionTotalManureAddedToAD;
                    }
                    else
                    {
                        // Equation 4.8.1-19
                        substrateFlowRate.VolatileSolidsFlow += (1 - reductionFactor.VolatileSolidsReductionFactor) * managementPeriod.NumberOfAnimals * component.ProportionTotalManureAddedToAD;
                    }

                    // Equation 4.8.1-20
                    substrateFlowRate.NitrogenFlow += dailyEmissions.NitrogenAvailableForLandApplication + component.ProportionTotalManureAddedToAD;

                    if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType())
                    {
                        // Equation 4.8.1-22
                        substrateFlowRate.OrganicNitrogenFlow += dailyEmissions.OrganicNitrogenAvailableForLandApplication * component.ProportionTotalManureAddedToAD;

                        // Equation 4.8.1-23
                        substrateFlowRate.TotalAmmonicalNitrogenFlow += dailyEmissions.TanAvailableForLandApplication * component.ProportionTotalManureAddedToAD;
                    }
                    else
                    {
                        /*
                         * Poultry
                         */

                        // Equation 4.8.1-22
                        substrateFlowRate.OrganicNitrogenFlow += (substrateFlowRate.NitrogenFlow - substrateFlowRate.TotalAmmonicalNitrogenFlow);

                        // Equation 4.8.1-24
                        substrateFlowRate.TotalAmmonicalNitrogenFlow += (dailyEmissions.TanExcretion - (dailyEmissions.AmmoniaConcentrationInHousing + dailyEmissions.AmmoniaLostFromStorage)) * component.ProportionTotalManureAddedToAD;
                    }

                    // Equation 4.8.1-25
                    substrateFlowRate.CarbonFlow += dailyEmissions.AmountOfCarbonInStoredManure * component.ProportionTotalManureAddedToAD;
                }

                result.Add(substrateFlowRate);
            }

            return result;
        }

        public SubstrateFlowInformation GetStoredManureFlowRateFromAnimals(
            AnaerobicDigestionComponent component,
            GroupEmissionsByDay dailyEmissions, 
            ManagementPeriod managementPeriod)
        {
            var substrateFlowRate = new SubstrateFlowInformation
            {
                SubstrateType = SubstrateType.StoredManure,
                AnimalType = managementPeriod.AnimalType,
                DateCreated = dailyEmissions.DateTime,
            };

            // Equation 4.8.1-16
            substrateFlowRate.TotalMassFlow = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication * component.ProportionTotalManureAddedToAD;

            var biogasData = _biogasMethaneProductionParametersProvider.GetBiogasMethaneProductionInstance(managementPeriod.AnimalType, managementPeriod.HousingDetails.BeddingMaterialType);

            substrateFlowRate.TotalSolidsFlow = substrateFlowRate.TotalMassFlow + biogasData.TotalSolids;

            var reductionFactor = _reductionFactors.GetParametersAdjustmentInstance(managementPeriod.ManureDetails.StateType);
            if (managementPeriod.ManureDetails.StateType.IsLiquidManure())
            {
                // Equation 4.8.1-18
                // TODO: this needs to be the sum of the daily vs_loaded and daily vs_consumed
                substrateFlowRate.VolatileSolidsFlow = (dailyEmissions.VolatileSolidsLoaded - dailyEmissions.VolatileSolidsConsumed) * component.ProportionTotalManureAddedToAD;
            }
            else
            {
                // Equation 4.8.1-19
                substrateFlowRate.VolatileSolidsFlow = (1 - reductionFactor.VolatileSolidsReductionFactor) * managementPeriod.NumberOfAnimals * component.ProportionTotalManureAddedToAD;
            }

            // Equation 4.8.1-20
            substrateFlowRate.NitrogenFlow = dailyEmissions.NitrogenAvailableForLandApplication + component.ProportionTotalManureAddedToAD;

            if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType())
            {
                // Equation 4.8.1-22
                substrateFlowRate.OrganicNitrogenFlow = dailyEmissions.OrganicNitrogenAvailableForLandApplication * component.ProportionTotalManureAddedToAD;

                // Equation 4.8.1-23
                substrateFlowRate.TotalAmmonicalNitrogenFlow = dailyEmissions.TanAvailableForLandApplication * component.ProportionTotalManureAddedToAD;
            }
            else
            {
                /*
                 * Poultry
                 */

                // Equation 4.8.1-22
                substrateFlowRate.OrganicNitrogenFlow = (substrateFlowRate.NitrogenFlow - substrateFlowRate.TotalAmmonicalNitrogenFlow);

                // Equation 4.8.1-24
                substrateFlowRate.TotalAmmonicalNitrogenFlow = (dailyEmissions.TanExcretion - (dailyEmissions.AmmoniaConcentrationInHousing + dailyEmissions.AmmoniaLostFromStorage)) * component.ProportionTotalManureAddedToAD;
            }

            // Equation 4.8.1-25
            substrateFlowRate.CarbonFlow = dailyEmissions.AmountOfCarbonInStoredManure * component.ProportionTotalManureAddedToAD;

            this.CalculateBiogassProduction(substrateFlowRate, biogasData, component);

            return substrateFlowRate;
        }

        /// <summary>
        /// This is the direct input from daily calculations - not 'additional' user defined inputs from component interface.
        /// </summary>
        public SubstrateFlowInformation GetFreshManureFlowRateFromAnimals(
            AnaerobicDigestionComponent component, 
            GroupEmissionsByDay dailyEmissions, 
            ManagementPeriod managementPeriod)
        {
            var substrateFlowRate = new SubstrateFlowInformation()
            {
                DateCreated = dailyEmissions.DateTime,
                SubstrateType = SubstrateType.FreshManure,
                AnimalType = managementPeriod.AnimalType,
            };

            // Equation 4.8.1-2
            substrateFlowRate.TotalMassFlow = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication * component.ProportionTotalManureAddedToAD;

            var biogasData = _biogasMethaneProductionParametersProvider.GetBiogasMethaneProductionInstance(managementPeriod.AnimalType, managementPeriod.HousingDetails.BeddingMaterialType);

            // Equation 4.8.1-3
            substrateFlowRate.TotalSolidsFlow = substrateFlowRate.TotalMassFlow * biogasData.TotalSolids;

            // Equation 4.8.1-4
            substrateFlowRate.VolatileSolidsFlow = dailyEmissions.VolatileSolids * managementPeriod.NumberOfAnimals * component.ProportionTotalManureAddedToAD;

            // Equation 4.8.1-5
            substrateFlowRate.NitrogenFlow = (dailyEmissions.AmountOfNitrogenExcreted + dailyEmissions.AmountOfNitrogenAddedFromBedding) * component.ProportionTotalManureAddedToAD;

            if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType() || managementPeriod.AnimalType.IsSheepType())
            {
                // Equation 4.8.1-6
                substrateFlowRate.OrganicNitrogenFlow = dailyEmissions.OrganicNitrogenInStoredManure * component.ProportionTotalManureAddedToAD;
            }
            else
            {
                // Equation 4.8.1-7
                substrateFlowRate.OrganicNitrogenFlow = (dailyEmissions.AmountOfNitrogenExcreted - (managementPeriod.ManureDetails.DailyTanExcretion * managementPeriod.NumberOfAnimals)) * component.ProportionTotalManureAddedToAD;
            }

            // Equation 4.8.1-8
            substrateFlowRate.TotalAmmonicalNitrogenFlow = dailyEmissions.TanExcretion * component.ProportionTotalManureAddedToAD;

            // Equation 4.8.1-9
            substrateFlowRate.CarbonFlow = dailyEmissions.CarbonFromManureAndBedding * component.ProportionTotalManureAddedToAD;

            this.CalculateBiogassProduction(substrateFlowRate, biogasData, component);

            return substrateFlowRate;
        }

        public List<SubstrateFlowInformation> GetFreshManureFlowRates(
            AnaerobicDigestionComponent component, 
            GroupEmissionsByDay dailyEmissions, 
            ManagementPeriod managementPeriod)
        {
            var result = new List<SubstrateFlowInformation>();

            // TODO: Add flow rate column to farm residue grid view. Also, unit attributes/comments needed for FarmResidueSubstrateViewItem class.
            var manureViewItems = component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.Where(y => y.IsFreshManure).GroupBy(x => x.AnimalType).ToList();

            foreach (var residueGroup in manureViewItems)
            {
                var substrateFlowRate = new SubstrateFlowInformation();
                substrateFlowRate.DateCreated = dailyEmissions.DateTime;

                substrateFlowRate.SubstrateType = SubstrateType.FreshManure;
                substrateFlowRate.AnimalType = residueGroup.Key;

                var biodegradableFraction = this.GetBiodegradableFraction(substrateFlowRate);
                var hydrolosisRate = 0.13;

                foreach (var viewItem in residueGroup)
                {
                    // Equation 4.8.1-2
                    var flow = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication * component.ProportionTotalManureAddedToAD;

                    // Equation 4.8.1-3
                    var totalSolidsFlow = flow * viewItem.TotalSolids;

                    // Equation 4.8.1-4
                    var volatileSolidsFlow = dailyEmissions.VolatileSolids * managementPeriod.NumberOfAnimals * component.ProportionTotalManureAddedToAD;

                    // Equation 4.8.1-5
                    var nitrogenFlow = (dailyEmissions.AmountOfNitrogenExcreted + dailyEmissions.AmountOfNitrogenAddedFromBedding) * component.ProportionTotalManureAddedToAD;

                    var organicNitrogenFlow = 0d;
                    if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType() || managementPeriod.AnimalType.IsSheepType())
                    {
                        // Equation 4.8.1-6
                        organicNitrogenFlow = dailyEmissions.OrganicNitrogenInStoredManure * component.ProportionTotalManureAddedToAD;
                    }
                    else
                    {
                        // Equation 4.8.1-7
                        organicNitrogenFlow = (dailyEmissions.AmountOfNitrogenExcreted - (managementPeriod.ManureDetails.DailyTanExcretion * managementPeriod.NumberOfAnimals)) * component.ProportionTotalManureAddedToAD;
                    }

                    // Equation 4.8.1-8
                    var tanFlow = dailyEmissions.TanExcretion * component.ProportionTotalManureAddedToAD;

                    // Equation 4.8.1-9
                    var carbonFlow = dailyEmissions.CarbonFromManureAndBedding * component.ProportionTotalManureAddedToAD;

                    // Equation 4.8.2-1
                    var biodegradableSolids = volatileSolidsFlow * biodegradableFraction;

                    // Equation 4.8.2-2
                    var methaneProduction = biodegradableSolids * viewItem.BiomethanePotential;

                    // Equation 4.8.2-3
                    var degradedVolatileSolids = biodegradableSolids - (biodegradableSolids / (1 + hydrolosisRate * component.HydraulicRetentionTimeInDays));

                    // Equation 4.8.2-5
                    var biogasProduction = methaneProduction / viewItem.MethaneFraction;

                    // Equation 4.8.2-7
                    var carbonDioxideProduction = biogasProduction - methaneProduction;

                    // Equation 4.8.3-6
                    var totalNitrogenContentOfVolatileSolids = (nitrogenFlow - tanFlow) / volatileSolidsFlow;

                    // Equation 4.8.3-5
                    var tanFlowInDigestate = tanFlow + degradedVolatileSolids * totalNitrogenContentOfVolatileSolids;

                    // Equation 4.8.3-7
                    var organicNitrogenFlowInDigestate = organicNitrogenFlow - degradedVolatileSolids * totalNitrogenContentOfVolatileSolids;

                    // Equation 4.8.3-8
                    var carbonFlowInDigestate = carbonFlow - degradedVolatileSolids * (carbonFlow / volatileSolidsFlow);

                    substrateFlowRate.TotalMassFlow += flow;
                    substrateFlowRate.TotalSolidsFlow += totalSolidsFlow;
                    substrateFlowRate.VolatileSolidsFlow += volatileSolidsFlow;
                    substrateFlowRate.NitrogenFlow += nitrogenFlow;
                    substrateFlowRate.OrganicNitrogenFlow += organicNitrogenFlow;
                    substrateFlowRate.TotalAmmonicalNitrogenFlow += tanFlow;
                    substrateFlowRate.CarbonFlow += carbonFlow;
                    substrateFlowRate.BiodegradableSolidsFlow += biodegradableSolids;
                    substrateFlowRate.MethaneProduction += methaneProduction;
                    substrateFlowRate.DegradedVolatileSolids += degradedVolatileSolids;
                    substrateFlowRate.BiogasProduction += biogasProduction;
                    substrateFlowRate.CarbonDioxideProduction += carbonDioxideProduction;
                    substrateFlowRate.TanFlowInDigestate = tanFlowInDigestate;
                    substrateFlowRate.OrganicNitrogenFlowInDigestate = organicNitrogenFlowInDigestate;
                    substrateFlowRate.CarbonFlowInDigestate = carbonFlowInDigestate;
                }

                result.Add(substrateFlowRate);
            }

            return result;
        }

        public List<SubstrateFlowInformation> GetFarmResidueFlowRates(AnaerobicDigestionComponent component)
        {
            var result = new List<SubstrateFlowInformation>();
            
            // TODO: Add flow rate column to farm residue grid view. Also, unit attributes/comments needed for FarmResidueSubstrateViewItem class.
            var farmResiduesGroupedByType = component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.GroupBy(x => x.FarmResidueType).ToList();

            // Group all residues of the same type (Wheat straw, food waste, etc.)
            foreach (var residueGroup in farmResiduesGroupedByType)
            {
                var substrateFlowRate = new SubstrateFlowInformation();
                substrateFlowRate.SubstrateType = SubstrateType.FarmResidues;
                substrateFlowRate.FarmResidueType = residueGroup.Key;

                var biodegradableFraction = this.GetBiodegradableFraction(substrateFlowRate);
                var hydrolosisRate = 0.13;

                foreach (var viewItem in residueGroup)
                {
                    // Equation 4.8.1-10
                    var flow = viewItem.FlowRate;

                    // Equation 4.8.1-11
                    var totalSolidsFlow = flow * viewItem.TotalSolids;

                    // Equation 4.8.1-12
                    var volatileSolidsFlow = flow * viewItem.VolatileSolids;

                    // Equation 4.8.1-13
                    var nitrogenFlow = flow * viewItem.TotalNitrogen;

                    // Equation 4.8.1-14
                    var carbonFlow = flow * viewItem.TotalCarbon;

                    // Equation 4.8.2-1
                    var biodegradableSolids = volatileSolidsFlow * biodegradableFraction;

                    // Equation 4.8.2-2
                    var methaneProduction = biodegradableSolids * viewItem.BiomethanePotential;

                    // Equation 4.8.2-3
                    var degradedVolatileSolids = biodegradableSolids - (biodegradableSolids / (1 + hydrolosisRate * component.HydraulicRetentionTimeInDays));

                    // Equation 4.8.2-5
                    var biogasProduction = methaneProduction / viewItem.MethaneFraction;

                    // Equation 4.8.2-7
                    var carbonDioxideProduction = biogasProduction - methaneProduction;

                    substrateFlowRate.TotalMassFlow += flow;
                    substrateFlowRate.TotalSolidsFlow += totalSolidsFlow;
                    substrateFlowRate.VolatileSolidsFlow += volatileSolidsFlow;
                    substrateFlowRate.NitrogenFlow += nitrogenFlow;
                    substrateFlowRate.CarbonFlow += carbonFlow;
                    substrateFlowRate.BiodegradableSolidsFlow += biodegradableSolids;
                    substrateFlowRate.MethaneProduction += methaneProduction;
                    substrateFlowRate.DegradedVolatileSolids += degradedVolatileSolids;
                    substrateFlowRate.BiogasProduction += biogasProduction;
                    substrateFlowRate.CarbonDioxideProduction += carbonDioxideProduction;
                }

                result.Add(substrateFlowRate);
            }
            
            return result;
        }

        public void CalculateDigestateStorageEmissions(
            Farm farm, 
            DateTime dateTime, 
            AnaerobicDigestionComponent component, 
            DigestorDailyOutput digestorDailyOutput)
        {
            var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(dateTime.Year, (Months)dateTime.Month);
            var methaneEmissionFactorDuringStorage = 0.0175 * Math.Pow(temperature, 2) - 0.0245 * temperature + 0.1433;

            // Equation 4.8.5-1
            digestorDailyOutput.MethaneEmissionsDuringStorage = methaneEmissionFactorDuringStorage * component.VolumeOfDigestateEnteringStorage;

            // Equation 4.8.5-2
            digestorDailyOutput.N2OEmissionsDuringStorage = 0.0652 * component.VolumeOfDigestateEnteringStorage;

            // Equation 4.8.5-3
            digestorDailyOutput.AmmoniaEmissionsDuringStorage = 3.495 * component.VolumeOfDigestateEnteringStorage;
        }

        public void CalculateLiquidSolidSeparation(
            DigestorDailyOutput digestorDailyOutput, 
            AnaerobicDigestionComponent component)
        {
            var rawMaterialCoefficients = _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.RawMaterial);
            var rawMaterialCoefficient = component.IsCentrifugeType ? rawMaterialCoefficients.Centrifuge : rawMaterialCoefficients.BeltPress;

            // Equation 4.8.4-1
            digestorDailyOutput.FlowRateLiquidFraction = (1 - rawMaterialCoefficient) * digestorDailyOutput.FlowRateOfAllSubstrates;

            // Equation 4.8.4-2
            digestorDailyOutput.FlowRateSolidFraction = rawMaterialCoefficient * digestorDailyOutput.FlowRateOfAllSubstrates;

            var totalSolidsCoefficients = _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.TotalSolids);
            var totalSolidsCoefficient = component.IsCentrifugeType ? totalSolidsCoefficients.Centrifuge : totalSolidsCoefficients.BeltPress;

            // Equation 4.8.4-3
            digestorDailyOutput.FlowOfTotalSolidsInLiquidFraction = (1 - totalSolidsCoefficient) * digestorDailyOutput.FlowOfTotalSolids;

            // Equation 4.8.4-4
            digestorDailyOutput.FlowOfTotalSolidsInSolidFraction = totalSolidsCoefficient * digestorDailyOutput.FlowOfTotalSolids;

            var volatileSolidsCoefficients = _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.VolatileSolids);
            var volatileSolidsCoefficient = component.IsCentrifugeType ? volatileSolidsCoefficients.Centrifuge : volatileSolidsCoefficients.BeltPress;

            // Equation 4.8.4-5
            digestorDailyOutput.TotalVolatileSolidsLiquidFraction = (1 - volatileSolidsCoefficient) * digestorDailyOutput.FlowOfVolatileSolids;

            // Equation 4.8.4-6
            digestorDailyOutput.TotalVolatileSolidsSolidFraction = volatileSolidsCoefficient * digestorDailyOutput.FlowOfVolatileSolids;

            var tanCoefficients = _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.TotalAmmoniaNitrogen);
            var tanCoefficient = component.IsCentrifugeType ? tanCoefficients.Centrifuge : tanCoefficients.BeltPress;

            // Equation 4.8.4-9
            digestorDailyOutput.TotalTanLiquidFraction = (1 - tanCoefficient) * digestorDailyOutput.TanFlowInDigestate;

            // Equation 4.8.4-10
            digestorDailyOutput.TotalTanSolidFraction = tanCoefficient * digestorDailyOutput.TanFlowInDigestate;

            var organicNCoefficients = _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.OrganicNitrogen);
            var organicNCoefficient = component.IsCentrifugeType ? organicNCoefficients.Centrifuge : organicNCoefficients.BeltPress;

            // Equation 4.8.4-11
            digestorDailyOutput.OrganicNLiquidFraction = (1 - organicNCoefficient) * digestorDailyOutput.OrganicNitrogenFlowInDigestate;

            // Equation 4.8.4-12
            digestorDailyOutput.OrganicNSolidFraction = organicNCoefficient * digestorDailyOutput.OrganicNitrogenFlowInDigestate;

            // Equation 4.8.4-7
            digestorDailyOutput.TotalNitrogenLiquidFraction = digestorDailyOutput.TotalTanLiquidFraction + digestorDailyOutput.OrganicNLiquidFraction;

            // Equation 4.8.4-8
            digestorDailyOutput.TotalNitrogenSolidFraction = digestorDailyOutput.TotalTanSolidFraction + digestorDailyOutput.OrganicNSolidFraction;

            var carbonCoefficients = _solidLiquidSeparationCoefficientsProvider.GetSolidLiquidSeparationCoefficientInstance(DigestateParameters.TotalCarbon);
            var carbonCoefficient = component.IsCentrifugeType ? carbonCoefficients.Centrifuge : carbonCoefficients.BeltPress;

            // Equation 4.8.4-13
            digestorDailyOutput.CarbonLiquidFraction = (1 - carbonCoefficient) * digestorDailyOutput.CarbonFlowInDigestate;

            // Equation 4.8.4-14
            digestorDailyOutput.CarbonSolidFraction = carbonCoefficient * digestorDailyOutput.CarbonFlowInDigestate;
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
            digestorDailyOutput.TotalAmountRawDigestateAvailableForLandApplication = digestorDailyOutput.FlowRateOfAllSubstrates;

            // Equation 4.8.3-4
            digestorDailyOutput.TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication = digestorDailyOutput.FlowRateOfTotalNitrogenInDigestate;

            // Equation 4.8.3-5
            digestorDailyOutput.TotalAmountOfTanInRawDigestateAvailalbleForLandApplication = digestorDailyOutput.TanFlowInDigestate;

            // Equation 4.8.3-7
            digestorDailyOutput.TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplication = digestorDailyOutput.OrganicNitrogenFlowInDigestate;

            // Equation 4.8.3-8
            digestorDailyOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = digestorDailyOutput.CarbonFlowInDigestate;

            /*
             * Solid fractions of liquid/solid separated digestate
             */

            // Equation 4.8.4-1
            digestorDailyOutput.TotalAmountRawDigestateAvailableForLandApplicationFromLiquidFraction = digestorDailyOutput.FlowRateLiquidFraction;

            // Equation 4.8.4-7
            digestorDailyOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = digestorDailyOutput.TotalNitrogenLiquidFraction;

            // Equation 4.8.4-9
            digestorDailyOutput.TotalAmountOfTanInRawDigestateAvailalbleForLandApplicationFromLiquidFraction = digestorDailyOutput.TotalTanLiquidFraction;

            // Equation 4.8.4-11
            digestorDailyOutput.TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = digestorDailyOutput.OrganicNLiquidFraction;

            // Equation 4.8.4-13
            digestorDailyOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction = digestorDailyOutput.CarbonLiquidFraction;

            /*
             * Liquid fractions of liquid/solid separated digestate
             */

            // Equation 4.8.4-2
            digestorDailyOutput.TotalAmountRawDigestateAvailableForLandApplicationFromSolidFraction = digestorDailyOutput.FlowRateSolidFraction;

            // Equation 4.8.4-8
            digestorDailyOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = digestorDailyOutput.TotalNitrogenSolidFraction;

            // Equation 4.8.4-10
            digestorDailyOutput.TotalAmountOfTanInRawDigestateAvailalbleForLandApplicationFromSolidFraction = digestorDailyOutput.TotalTanSolidFraction;

            // Equation 4.8.4-12
            digestorDailyOutput.TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = digestorDailyOutput.OrganicNSolidFraction;

            // Equation 4.8.4-14
            digestorDailyOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = digestorDailyOutput.CarbonSolidFraction;

            /*
             * Stored digestate (liquid and solid fractions)
             */

            // Equation 4.8.3-1
            digestorDailyOutput.TotalAmountOfStoredDigestateAvailableForLandApplication = digestorDailyOutput.FlowRateOfAllSubstrates;

            // Equation 4.8.4-1
            digestorDailyOutput.TotalAmountOfStoredDigestateAvailableForLandApplicationLiquidFraction = digestorDailyOutput.FlowRateLiquidFraction;

            // Equation 4.8.4-2
            digestorDailyOutput.TotalAmountOfStoredDigestateAvailableForLandApplicationSolidFraction = digestorDailyOutput.FlowRateSolidFraction;
        }

        public void CalculateFlows(
            DigestorDailyOutput digestorDailyOutput, 
            List<SubstrateFlowInformation> flowInformationForAllSubstrates)
        {
            // Equation 4.8.3-1
            digestorDailyOutput.FlowRateOfAllSubstrates = flowInformationForAllSubstrates.Sum(x => x.TotalMassFlow);

            // Equation 4.8.3-2
            digestorDailyOutput.FlowOfTotalSolids = flowInformationForAllSubstrates.Sum(x => x.TotalSolidsFlow) - flowInformationForAllSubstrates.Sum(x => x.DegradedVolatileSolids);

            // Equation 4.8.3-3
            digestorDailyOutput.FlowOfVolatileSolids = flowInformationForAllSubstrates.Sum(x => x.VolatileSolidsFlow) - flowInformationForAllSubstrates.Sum(x => x.DegradedVolatileSolids);

            // Equation 4.8.3-4
            digestorDailyOutput.FlowRateOfTotalNitrogenInDigestate = flowInformationForAllSubstrates.Sum(x => x.NitrogenFlow);

            // Equation 4.8.3-5
            digestorDailyOutput.TanFlowInDigestate = flowInformationForAllSubstrates.Sum(x => x.TanFlowInDigestate);

            // Equation 4.8.3-7
            digestorDailyOutput.OrganicNitrogenFlowInDigestate = flowInformationForAllSubstrates.Sum(x => x.OrganicNitrogenFlowInDigestate);

            // Equation 4.8.3-8
            digestorDailyOutput.CarbonFlowInDigestate = flowInformationForAllSubstrates.Sum(x => x.CarbonFlowInDigestate);
        }

        public void CalculateBiogassProduction(
            SubstrateFlowInformation substrateFlowRate,
            BiogasAndMethaneProductionParametersData biogasData, 
            AnaerobicDigestionComponent component)
        {
            var biodegradableFraction = this.GetBiodegradableFraction(substrateFlowRate);
            var hydrolosisRate = 0.13;

            // Equation 4.8.2-1
            substrateFlowRate.BiodegradableSolidsFlow = substrateFlowRate.VolatileSolidsFlow * biodegradableFraction;

            // Equation 4.8.2-2
            substrateFlowRate.MethaneProduction = substrateFlowRate.BiodegradableSolidsFlow * biogasData.BioMethanePotential;

            // Equation 4.8.2-3
            substrateFlowRate.DegradedVolatileSolids = substrateFlowRate.BiodegradableSolidsFlow - (substrateFlowRate.BiodegradableSolidsFlow / (1 + hydrolosisRate * component.HydraulicRetentionTimeInDays));

            // Equation 4.8.2-5
            substrateFlowRate.BiogasProduction = substrateFlowRate.MethaneProduction / biogasData.MethaneFraction;

            // Equation 4.8.2-7
            substrateFlowRate.CarbonDioxideProduction = substrateFlowRate.BiogasProduction - substrateFlowRate.MethaneProduction;

            // Equation 4.8.3-6
            var totalNitrogenContentOfVolatileSolids = substrateFlowRate.VolatileSolidsFlow > 0 ? (substrateFlowRate.NitrogenFlow - substrateFlowRate.TotalAmmonicalNitrogenFlow) / substrateFlowRate.VolatileSolidsFlow : 0;

            // Equation 4.8.3-5
            substrateFlowRate.TanFlowInDigestate = substrateFlowRate.TotalAmmonicalNitrogenFlow + substrateFlowRate.DegradedVolatileSolids * totalNitrogenContentOfVolatileSolids;

            // Equation 4.8.3-7
            substrateFlowRate.OrganicNitrogenFlowInDigestate = substrateFlowRate.OrganicNitrogenFlow - substrateFlowRate.DegradedVolatileSolids * totalNitrogenContentOfVolatileSolids;

            // Equation 4.8.3-8
            substrateFlowRate.CarbonFlowInDigestate = substrateFlowRate.VolatileSolidsFlow > 0 ? substrateFlowRate.CarbonFlow - substrateFlowRate.DegradedVolatileSolids * (substrateFlowRate.CarbonFlow / substrateFlowRate.VolatileSolidsFlow) : 0;
        }

        public void CalculateTotalBiogassProduction(
            DigestorDailyOutput digestorDailyOutput, 
            List<SubstrateFlowInformation> flowInformationForAllSubstrates)
        {
            // Equation 4.8.2-2
            digestorDailyOutput.TotalMethaneProduction = flowInformationForAllSubstrates.Sum(x => x.MethaneProduction);

            // Equation 4.8.2-4
            digestorDailyOutput.TotalFlowOfDegradedVolatileSolids = flowInformationForAllSubstrates.Sum(x => x.DegradedVolatileSolids);

            // Equation 4.8.2-6
            digestorDailyOutput.TotalBiogasProduction = flowInformationForAllSubstrates.Sum(x => x.BiogasProduction);

            // Equation 4.8.2-8
            digestorDailyOutput.TotalCarbonDioxideProduction = digestorDailyOutput.TotalBiogasProduction - digestorDailyOutput.TotalMethaneProduction;

            // Equation 4.8.2-11
            digestorDailyOutput.TotalRecoverableMethane = digestorDailyOutput.TotalMethaneProduction * (1 - 0.03);

            // Equation 4.8.2-12
            digestorDailyOutput.TotalPrimaryEnergyProduction = digestorDailyOutput.TotalMethaneProduction * (35.17 / 3.6);

            // Equation 4.8.2-13
            digestorDailyOutput.ElectricityProduction = digestorDailyOutput.TotalPrimaryEnergyProduction * 0.4;

            // Equation 4.8.2-14
            digestorDailyOutput.HeatProduced = digestorDailyOutput.TotalPrimaryEnergyProduction * 0.5;

            // Equation 4.8.2-15
            digestorDailyOutput.MethaneToGrid = digestorDailyOutput.TotalRecoverableMethane * 0.0081;

        }

        public DigestorDailyOutput CalculateResultsInternal(SubstrateFlowInformation cropResidueFlows,
            SubstrateFlowInformation freshManureFlows,
            SubstrateFlowInformation storedManureFlows,
            AnaerobicDigestionComponent component,
            Farm farm, 
            DateTime dateTime)
        {
            // All flows
            var flowInformationForAllSubstrates = new List<SubstrateFlowInformation>();

            // Add flows for all farm residues
            flowInformationForAllSubstrates.Add(cropResidueFlows);

            // Add flows for all fresh manure
            flowInformationForAllSubstrates.Add(freshManureFlows);

            // Add flows for all stored manure
            flowInformationForAllSubstrates.Add(storedManureFlows);

            var adOutput = new DigestorDailyOutput()
            {
                Date = dateTime,
            };

            /*
             * Totals of all flows in digester
             */

            // Equation 4.8.2-1
            var flowOfBiodegradableSolids = flowInformationForAllSubstrates.Sum(x => x.BiodegradableSolidsFlow);

            this.CalculateTotalBiogassProduction(adOutput, flowInformationForAllSubstrates);

            /*
             * Production of digestate and its composition
             */

            this.CalculateFlows(adOutput, flowInformationForAllSubstrates);

            this.CalculateLiquidSolidSeparation(adOutput, component);

            this.CalculateAmountsForLandApplication(adOutput);

            this.CalculateDigestateStorageEmissions(farm, DateTime.Now, component, adOutput);

            /*
             * Total nitrogen available for land application
             */

            // Equation 4.8.6-1
            adOutput.TotalNitrogenInDigestateAvailableForLandApplication = adOutput.FlowRateOfTotalNitrogenInDigestate - (adOutput.N2OEmissionsDuringStorage + adOutput.AmmoniaEmissionsDuringStorage);

            // Equation 4.8.6-2
            adOutput.TotalCarbonInDigestateAvailableForLandApplication = adOutput.CarbonFlowInDigestate - adOutput.MethaneEmissionsDuringStorage;

            return adOutput;
        }

        public DigestorDailyOutput CalculateResults(Farm farm, GroupEmissionsByDay dailyEmissions, ManagementPeriod managementPeriod)
        {
            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                return new DigestorDailyOutput();
            }

            //var cropResidueFlows = this.GetFarmResidueFlowRates(component);
            var freshManureFlows = this.GetFreshManureFlowRateFromAnimals(component, dailyEmissions, managementPeriod);
            var storedManureFlows = this.GetStoredManureFlowRateFromAnimals(component, dailyEmissions, managementPeriod);

            var dailyResults =  this.CalculateResultsInternal(new SubstrateFlowInformation(), freshManureFlows, storedManureFlows, component, farm, dailyEmissions.DateTime);

            return dailyResults;
        }

        public List<DigestorDailyOutput> CalculateResults(Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults)
        {
            var results = new List<DigestorDailyOutput>();

            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                foreach (var animalGroupResults in animalComponentEmissionsResult.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in animalGroupResults.GroupEmissionsByMonths)
                    {
                        foreach (var groupEmissionsByDay in groupEmissionsByMonth.DailyEmissions)
                        {
                            var result = this.CalculateResults(farm, groupEmissionsByDay, groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod);

                            results.Add(result);
                        }
                    }
                }
            }

            return results;
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

        #region Private Methods
        #endregion

        #region Equations

        #region 4.8.1.3 For Livestock Manure Stored For A Period Of Time Prior To Entering The Digester

        /// <summary>
        /// Equation 4.9.1-19
        /// </summary>
        /// <param name="volatileSolids">Volatile solids excreted (kg head-1 day^-1)</param>
        /// <param name="reductionFactor">Fixed reduction in VS in stored solid manure entering the digester following a pre-digester storage period</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="proportionOfManureAvailableForAdditionToDigester">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of volatile solids in stored solid manure entering the digester from previously stored solid manure (kg day^-1)</returns>
        public double CalculateVolatileSolidsFlowFromStoredManure(
            double volatileSolids,
            double reductionFactor,
            double numberOfAnimals,
            double proportionOfManureAvailableForAdditionToDigester)
        {
            return  volatileSolids * (1 - reductionFactor) * numberOfAnimals * proportionOfManureAvailableForAdditionToDigester;
        }

        /// <summary>
        /// Equation 4.9.1-17
        /// </summary>
        /// <param name="flowRate"></param>
        /// <param name="totalSolidsConcentration"></param>
        /// <returns></returns>
        public double CalculateFlowOfVSEnteringDigesterFromStoredManure(
            double flowRate,
            double totalSolidsConcentration)
        {
            return flowRate * totalSolidsConcentration;
        }

        /// <summary>
        /// Equation 4.9.1-20
        /// </summary>
        /// <param name="nitrogenAvailalbleForLandApplication">Flow rate of total N in stored manure (liquid or solid) entering the digester (kg day^-1)</param>
        /// <param name="proportionOfManureAvailableForAdditionToDigester">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of total N in substrate entering the digester (kg day^-1)</returns>
        public double CalculateFlowRateOfTotalNitrogenEnteringDigesterFromStoredManure(double nitrogenAvailalbleForLandApplication,
            double proportionOfManureAvailableForAdditionToDigester)
        {
            return nitrogenAvailalbleForLandApplication * proportionOfManureAvailableForAdditionToDigester;
        }

        /// <summary>
        /// Equation 4.9.1-21
        /// </summary>
        /// <param name="organicNitrogenAvailableForLandApplication"></param>
        /// <param name="proportionOfManureAvailableForAdditionToDigester"></param>
        /// <returns></returns>
        public double CalculateFlowOfOrganicNitrogenFromStoredManureBeefDairy(
            double organicNitrogenAvailableForLandApplication,
            double proportionOfManureAvailableForAdditionToDigester)
        {
            return organicNitrogenAvailableForLandApplication * proportionOfManureAvailableForAdditionToDigester;
        }

        /// <summary>
        /// Equation 4.9.1-22
        /// </summary>
        /// <param name="nitrogenFlowFromSubstrate">Flow rate of organic N in stored solid poultry manure entering the digester (kg day^-1)</param>
        /// <param name="tanFlowFromSubstrate">Flow rate of organic N in stored solid poultry manure entering the digester (kg day^-1)</param>
        /// <returns></returns>
        public double CalculateFlowOfOrganicNitrogenFromStoredPoultryManure(
            double nitrogenFlowFromSubstrate,
            double tanFlowFromSubstrate)
        {
            return nitrogenFlowFromSubstrate - tanFlowFromSubstrate;
        }

        /// <summary>
        /// Equation 4.9.1-23
        /// </summary>
        /// <param name="tanAvailableForLandApplication"></param>
        /// <param name="proportionOfManureAvailableForAdditionToDigester"></param>
        /// <returns></returns>
        public double CalculateFlowOfTanFromStoredManureBeefDairy(
            double tanAvailableForLandApplication,
            double proportionOfManureAvailableForAdditionToDigester)
        {
            return tanAvailableForLandApplication * proportionOfManureAvailableForAdditionToDigester;
        }

        /// <summary>
        /// Equation 4.9.1-24
        /// </summary>
        /// <param name="totalAmmonicalNitrogenExcretionRate">Total ammonical N (TAN) excretion rate (kg TAN head-1 day^-1).</param>
        /// <param name="indirectNLossesFromManureInHousingNH3">Indirect N losses from manure in housing via NH3 volatilization during the pre-AD stage (kg NH3-N day^-1)</param>
        /// <param name="indirectNLossesFromManureInStorageNH3">Indirect N losses from manure in storage via NH3 volatilization during the pre-AD stage. </param>
        /// <param name="numberOfAnimals">The total number of animals.</param>
        /// <param name="proportionOfManureAvailableForAdditionToDigester">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of TAN in substrate entering the digester (kg day^-1) for beef cattle, dairy cattle, broilers, layers and turkeys.</returns>
        public double CalculateFlowOfTanEnteringDigesterFromStoredPoultryManure(double totalAmmonicalNitrogenExcretionRate,
            double indirectNLossesFromManureInHousingNH3,
            double indirectNLossesFromManureInStorageNH3,
            double numberOfAnimals,
            double proportionOfManureAvailableForAdditionToDigester)
        {
            return (totalAmmonicalNitrogenExcretionRate * numberOfAnimals -
                    (indirectNLossesFromManureInHousingNH3 + indirectNLossesFromManureInStorageNH3)) * proportionOfManureAvailableForAdditionToDigester;
        }

        /// <summary>
        /// Equation 4.9.1-25
        /// </summary>
        /// <param name="totalCarbonInManure">Total amount of C added in manure (including bedding) (kg N day^-1)</param>
        /// <param name="proportionOfManureAvailableForAdditionToDigester">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of total C in substrate entering the digester (kg day^-1)</returns>
        public double CalculateFlowOfTotalCarbonEnteringDigesterFromStoredManure(
            double totalCarbonInManure,
            double proportionOfManureAvailableForAdditionToDigester)
        {
            return totalCarbonInManure * proportionOfManureAvailableForAdditionToDigester;
        }

        #endregion

        #region 4.9.2.1 Flow Of Biodegradable Volatile Solids And Methane Potential

        /// <summary>
        /// Equation 4.9.2-1
        /// </summary>
        /// <param name="flowRateOfSubstrateEnteringDigester">Amount of substrate i entering the digester (kg day^-1).</param>
        /// <param name="biodegradeableFractionOfVSForSubstrate">Biodegradable fraction of VS for substrate</param>
        /// <returns>Flow rate of biodegradable volatile solids (kg day^-1)</returns>
        public double CalculateFlowRateBiodegradeableVolatileSolids(
            double flowRateOfSubstrateEnteringDigester, 
            double biodegradeableFractionOfVSForSubstrate)
        {
            return flowRateOfSubstrateEnteringDigester * biodegradeableFractionOfVSForSubstrate;
        }

        /// <summary>
        /// Equation 4.9.2-2
        /// </summary>
        /// <param name="flowRateOfDegradedVSDuringDigestion">Flow rate of biodegradable volatile solids.</param>
        /// <param name="biomethanePotentialOfSubstrate">Theoretical biomethane potential of substrate i (Nm^3 kg VS^-1)</param>
        /// <returns>Total CH4 production (Nm^3 day^-1), where Nm^3 are normal meters cubed</returns>
        public double CalculateTotalCH4Production(
            double flowRateOfDegradedVSDuringDigestion, 
            double biomethanePotentialOfSubstrate)
        {
            return flowRateOfDegradedVSDuringDigestion * biomethanePotentialOfSubstrate;
        }

        /// <summary>
        /// Equation 4.9.2-3
        /// </summary>
        /// <param name="flowRateBiodegradeableVSInSubstrate">Flow rate of biodegradable VS in substrate i (Nm^3 VS day^-1)</param>
        /// <param name="hydrolysisRateSubstrateDuringDigestion">Hydrolysis rate of substrate i during digestion (day^-1)</param>
        /// <param name="hydraulicRetentionTime">Hydraulic retention time (days)</param>
        /// <returns>Flow rate of degraded VS during digestion (kg VS day^-1)</returns>
        public double CalculateFlowRateOfDegradedVolatileSolidsDuringDigestion(double flowRateBiodegradeableVSInSubstrate,
                                                                               double hydrolysisRateSubstrateDuringDigestion,
                                                                               double hydraulicRetentionTime)
        {
            return flowRateBiodegradeableVSInSubstrate -
                  (flowRateBiodegradeableVSInSubstrate / (1 + hydrolysisRateSubstrateDuringDigestion * hydraulicRetentionTime));
        }
        #endregion

        #region 4.9.2.2 Biogas Production

        /// <summary>
        /// Equation 4.9.2-5
        /// </summary>
        /// <param name="methaneProductionOfSubstrate">Methane production of substrate i (Nm^3 day^-1)</param>
        /// <param name="methaneFractionInBiogas">Fraction of methane in the biogas for substrate i (Table 46)</param>
        /// <returns>Biogas production of substrate i (Nm^3 day^-1)</returns>
        public double CalculateBiogasProductionOfSubstrate(double methaneProductionOfSubstrate,
                                                           double methaneFractionInBiogas)
        {
            return methaneProductionOfSubstrate / methaneFractionInBiogas;
        }

        /// <summary>
        /// Equation 4.9.2-6
        /// </summary>
        /// <param name="biogasProductionsOfSubstrates">A collection of Biogas production of substrate i values. Calculated using <see cref="CalculateBiogasProductionOfSubstrate(double, double)"/></param>
        /// <returns>Total biogas production upon co-digestion of multiple substrates (Nm^3 day^-1)</returns>
        public double CalculateTotalBiogasProduction(IEnumerable<double> biogasProductionsOfSubstrates)
        {
            return biogasProductionsOfSubstrates.Sum();
        }

        #endregion

        #region 4.9.2.3 Carbon Dioxide Production

        /// <summary>
        /// Equation 4.9.2-7
        /// </summary>
        /// <param name="biogasProductionOfSubstrate"></param>
        /// <param name="methaneProductionOfSubstrate"></param>
        /// <returns>Carbon dioxide production from substrate i (Nm^3 day^-1)</returns>
        public double CalculateCarbonDioxideProductionFromSubstrate(double biogasProductionOfSubstrate,
                                                                    double methaneProductionOfSubstrate)
        {
            return (biogasProductionOfSubstrate - methaneProductionOfSubstrate);
        }

        /// <summary>
        /// Equation 4.9.2-8
        /// </summary>
        /// <param name="totalBiogasProduction">Total biogas production upon co-digestion of multiple substrates (Nm^3 day^-1). Calculated using <see cref="CalculateTotalBiogasProduction"/></param>
        /// <param name="totalCH4Production"></param>
        /// <returns>Total CO2 production upon co-digestion of multiple substrates (Nm^3 day^-1)</returns>
        public double CalculateTotalCarbonDioxideProduction(double totalBiogasProduction,
                                                            double totalCH4Production)
        {
            return totalBiogasProduction - totalCH4Production;
        }

        #endregion

        #region 4.8.2.4 Reactor Dimensioning

        /// <summary>
        /// Equation 4.8.2-9
        /// </summary>
        /// <param name="totalFlowRateSubstrate">Total flow rate of substrate entering the digester (m3 day^-1)</param>
        /// <param name="hydraulicRetentionDays">Hydraulic retention time (days)</param>
        /// <returns>Reactor volume (m3)</returns>
        public double CalculateReactorVolume(double totalFlowRateSubstrate,
                                             double hydraulicRetentionDays)
        {
            return hydraulicRetentionDays / totalFlowRateSubstrate;
        }

        /// <summary>
        /// Equation 4.8.2-9
        /// </summary>
        /// <param name="totalFlowRateVolatileSolids">Total flow rate of VS entering the digester (m3 day^-1)</param>
        /// <param name="reactorVolume">Reactor volume (m3)</param>
        /// <returns>Organic loading rate (kg VS m-3 d-1) (upper limit: 3.5, average: 1.6 </returns>
        public double CalculateOrganicLoadingRate(double totalFlowRateVolatileSolids,
                                                  double reactorVolume)
        {
            return totalFlowRateVolatileSolids / reactorVolume;
        }

        #endregion

        #region 4.9.2.5 Valorization of Methane

        /// <summary>
        /// Equation 4.9.2-11
        /// </summary>
        /// <param name="totalMethaneProduction">Total CH4 production (Nm^3 day^-1)</param>
        /// <param name="fractionFugitiveMethaneLosses">Fraction of fugitive methane losses through digester equipment. A default value of 0.03 is used</param>
        /// <returns>Recoverable CH4 (Nm^3 day^-1)</returns>
        public double CalculateRecoverableMethane(
            double totalMethaneProduction, 
            double fractionFugitiveMethaneLosses = 0.03)
        {
            return totalMethaneProduction * (1 - fractionFugitiveMethaneLosses);
        }

        /// <summary>
        /// Equation 4.9.2-12
        /// </summary>
        /// <param name="totalMethaneProduction"></param>
        /// <param name="calorificValueCH4">Calorific value of CH4 (MJ Nm-3). A default value of 35.17 is used</param>
        /// <param name="conversionCoefficient">Conversion coefficient kWh to MJ (MJ kWh-1). A default value of 3.6 is used</param>
        /// <returns>Total primary energy production (kWh day^-1)</returns>
        public double CalculateTotalPrimaryEnergyProduction(
            double totalMethaneProduction, 
            double calorificValueCH4 = 35.17, 
            double conversionCoefficient = 3.6)
        {
            return totalMethaneProduction * (calorificValueCH4 / conversionCoefficient);
        }

        /// <summary>
        /// Equation 4.9.2-13
        /// </summary>
        /// <param name="totalPrimaryEnergyProduction">Total primary energy production (kWh day^-1). Calculated using <see cref="CalculateTotalPrimaryEnergyProduction(double, double, double)"/></param>
        /// <param name="fractionPrimaryEnergyToElectricity">Fraction of primary energy converted to electricity through CHP. A default value of 0.4 is used</param>
        /// <returns>Total electricity production through CHP (kWh day^-1)</returns>
        public double CalculateTotalElectricityProduction(
            double totalPrimaryEnergyProduction, 
            double fractionPrimaryEnergyToElectricity = 0.4)
        {
            return totalPrimaryEnergyProduction * fractionPrimaryEnergyToElectricity;
        }

        /// <summary>
        /// Equation 4.9.2-14
        /// </summary>
        /// <param name="totalPrimaryEnergyProduction">Total primary energy production (kWh day^-1). Calculated using <see cref="CalculateTotalPrimaryEnergyProduction(double, double, double)"></see></param>
        /// <param name="fractionPrimaryEnergyToHeat">Fraction of primary energy converted to heat through CHP. A default value of 0.5 is used</param>
        /// <returns>Total heat production through CHP (kWh day^-1)</returns>
        public double CalculateTotalHeatProduction(
            double totalPrimaryEnergyProduction, 
            double fractionPrimaryEnergyToHeat = 0.5)
        {
            return totalPrimaryEnergyProduction * fractionPrimaryEnergyToHeat;
        }

        /// <summary>
        /// Equation 4.9.2-15
        /// </summary>
        /// <param name="recoverableMethane">Recoverable CH4 (Nm^3 day^-1)</param>
        /// <param name="fractionMethaneLostInUpgradingPlants">	Fraction of methane lost in upgrading plants. A default value of 0.0081 is used</param>
        /// <returns>Potential CH4 injection to the gas grid (kWh day^-1)</returns>
        public double CalculatePotentialMethaneInjectionToGasGrid(
            double recoverableMethane, 
            double fractionMethaneLostInUpgradingPlants = 0.0081)
        {
            return recoverableMethane * fractionMethaneLostInUpgradingPlants;
        }
        #endregion

        #region 4.9.3 Production of Digestate And Its Composition

        /// <summary>
        /// Equation 4.9.3-1
        /// </summary>
        /// <param name="flowRatesSubstrate">A collection of values denoting the flow rate of substrate i entering the digester (t day^-1)</param>
        /// <returns>Returns the sum value of the parameter. Denotes the flow rate of total mass of digestate (t day^-1)</returns>
        public double CalculateFlowRateTotalMassOfDigestate(IEnumerable<double> flowRatesSubstrate)
        {
            return flowRatesSubstrate.Sum();
        }

        /// <summary>
        /// Equation 4.9.3-2
        /// </summary>
        /// <param name="flowRateTotalSolidsSubstrate">Flow rate of TS in substrate entering the digester (kg day^-1)</param>
        /// <param name="flowRateVolatileSolidsDegraded">Flow rate of VS degraded during digestion (kg day^-1)</param>
        /// <returns>Flow rate of VS in digestate (kg day^-1)</returns>
        public double CalculateFlowRateTotalSolidsInDigestate(
            double flowRateTotalSolidsSubstrate, 
            double flowRateVolatileSolidsDegraded)
        {
            return flowRateTotalSolidsSubstrate - flowRateVolatileSolidsDegraded;
        }

        /// <summary>
        /// Equation 4.9.3-3
        /// </summary>
        /// <param name="flowRateVolatileSolidsSubstrate">Flow rate of VS in substrate entering the digester (kg day^-1)</param>
        /// <param name="flowRateVolatileSolidsDegraded">Flow rate of VS degraded during digestion (kg day^-1)</param>
        /// <returns>Flow rate of VS in digestate (kg day^-1)</returns>
        public double CalculateFlowRateVolatileSolidsInDigestate(
            double flowRateVolatileSolidsSubstrate,
            double flowRateVolatileSolidsDegraded)
        {
            return flowRateVolatileSolidsSubstrate - flowRateVolatileSolidsDegraded;
        }

        /// <summary>
        /// Equation 4.9.3-4
        /// </summary>
        /// <param name="flowRatesTotalNitrogenInSubstrate">A collection of values denoting the flow rate of total N in substrate i entering the digester (kg day^-1)</param>
        /// <returns>Flow rate of total N in digestate (kg day^-1)</returns>
        public double CalculateFlowRateTotalNitrogenInDigestate(IEnumerable<double> flowRatesTotalNitrogenInSubstrate)
        {
            return flowRatesTotalNitrogenInSubstrate.Sum();
        }

        /// <summary>
        /// Equation 4.9.3-5
        /// </summary>
        /// <param name="tanFlowOfSubstrate">Flow rate of TAN in substrate i entering the digester (kg day-1)</param>
        /// <param name="volatileSolidsDegraded">Flow rate of VS in substrate i degraded during digestion (kg day-1)</param>
        /// <param name="nitrogenContentOfVolatileSolids">Total N content of VS (kg N kg-1 VS)</param>
        /// <returns>Flow rate of TAN in digestate (kg day-1)</returns>
        public double CalculateFlowRateOfTanInDigestate(
            double tanFlowOfSubstrate,
            double volatileSolidsDegraded,
            double nitrogenContentOfVolatileSolids)
        {
            return tanFlowOfSubstrate + volatileSolidsDegraded + nitrogenContentOfVolatileSolids;
        }

        /// <summary>
        /// Equation 4.9.3-6
        /// </summary>
        /// <param name="nitrogenFlowInSubstrate">Total N content of VS in substrate i (kg N kg-1 VS)</param>
        /// <param name="tanFlowInSubstate">Flow rate of TAN in substrate i entering the digester (kg day-1)</param>
        /// <param name="volatileSolidsFlowInSubstrate">Flow rate of VS in substrate i entering the digester (kg day-1)</param>
        /// <returns>Total N content of VS in substrate i (kg N kg-1 VS)</returns>
        public double CalculateTotalNitrogenContentInDigestate(
            double nitrogenFlowInSubstrate,
            double tanFlowInSubstate,
            double volatileSolidsFlowInSubstrate)
        {
            return (nitrogenFlowInSubstrate - tanFlowInSubstate) / volatileSolidsFlowInSubstrate;
        }

        /// <summary>
        /// Equation 4.9.3-7
        /// </summary>
        /// <param name="organicNitrogenFlowOfSubstrate">Flow rate of organic N in substrate i entering the digester (kg day-1)</param>
        /// <param name="volatileSolidsDegraded">Flow rate of VS in substrate i degraded during digestion (kg day-1)</param>
        /// <param name="totalNitrogenContentOfVolatileSolids"></param>
        /// <returns>Flow rate of organic N in digestate (kg day-1)</returns>
        public double CalculateTotalOrganicNitrogenContentInDigestate(
            double organicNitrogenFlowOfSubstrate,
            double volatileSolidsDegraded,
            double totalNitrogenContentOfVolatileSolids)
        {
            var result = (organicNitrogenFlowOfSubstrate - volatileSolidsDegraded) *
                         totalNitrogenContentOfVolatileSolids;

            return result;
        }

        /// <summary>
        /// Equation 4.9.3-8
        /// </summary>
        /// <param name="carbonFlowOfSubstrate"></param>
        /// <param name="volatileSolidsDegraded"></param>
        /// <param name="volatileSolidsFlowOfSubstrate"></param>
        /// <returns></returns>
        public double CalculateTotalCarbonContentInDigestate(
            double carbonFlowOfSubstrate,
            double volatileSolidsDegraded,
            double volatileSolidsFlowOfSubstrate)
        {
            return  carbonFlowOfSubstrate - volatileSolidsDegraded * (carbonFlowOfSubstrate / volatileSolidsFlowOfSubstrate);
        }

        #endregion

        #region 4.9.4 Solid-Liquid Separation Of Digestate

        /// <summary>
        /// Equation 4.9.4-1
        /// </summary>
        /// <param name="fractionRawMaterialCoefficient">Separation coefficient: fraction of raw material in solid fraction following solid-liquid separation</param>
        /// <param name="flowRateDigestate">Flow rate of digestate (t day^-1)</param>
        /// <returns>Flow rate of liquid fraction of digestate (t day^-1)</returns>
        public double CalculateFlowRateLiquidFractionDigestate(
            double fractionRawMaterialCoefficient, 
            double flowRateDigestate)
        {
            return (1 - fractionRawMaterialCoefficient) * flowRateDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-2
        /// </summary>
        /// <param name="fractionRawMaterialCoefficient">Separation coefficient: fraction of raw material in solid fraction following solid-liquid separation</param>
        /// <param name="flowRateDigestate">Flow rate of digestate (t day^-1)</param>
        /// <returns>Flow rate of solid fraction of digestate (t day^-1)</returns>
        public double CalculateFlowRateSolidFractionDigestate(
            double fractionRawMaterialCoefficient, 
            double flowRateDigestate)
        {
            return fractionRawMaterialCoefficient * flowRateDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-3
        /// </summary>
        /// <param name="flowRateOfTotalSolids">Flow rate of TS in the liquid fraction of digestate </param>
        /// <param name="fractionTotalSolidsCoefficient">Separation coefficient: fraction of TS in the solid fraction following solid-liquid separation</param>
        /// <returns>Flow rate of TS in the liquid fraction of digestate (kg day-1)</returns>
        public double CalculateFlowRateOfTotalSolidsInLiquidFractionDigestate(
            double flowRateOfTotalSolids,
            double fractionTotalSolidsCoefficient)
        {
            return (1 - fractionTotalSolidsCoefficient) * flowRateOfTotalSolids;
        }

        /// <summary>
        /// Equation 4.9.4-4
        /// </summary>
        /// <param name="flowRateOfTotalSolids">Flow rate of TS in the solid fraction of digestate </param>
        /// <param name="fractionTotalSolidsCoefficient">Separation coefficient: fraction of TS in the solid fraction following solid-liquid separation</param>
        /// <returns></returns>
        public double CalculateFlowRateOfTotalSolidsInSolidFractionDigestate(
            double flowRateOfTotalSolids,
            double fractionTotalSolidsCoefficient)
        {
            return fractionTotalSolidsCoefficient * flowRateOfTotalSolids;
        }

        /// <summary>
        /// Equation 4.9.4-5
        /// </summary>
        /// <param name="fractionVolatileSolidsCoefficient">Separation coefficient: fraction of VS in the solid fraction following solid-liquid </param>
        /// <param name="flowRateInDigestate">Flow rate of digestate (t day^-1)</param>
        /// <returns>Flow rate of VS in the liquid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowRateVolatileSolidsInDigestateLiquidFraction(
            double fractionVolatileSolidsCoefficient, 
            double flowRateInDigestate)
        {
            return (1 - fractionVolatileSolidsCoefficient) * flowRateInDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-6
        /// </summary>
        /// <param name="fractionVolatileSolidsCoefficient">Separation coefficient: fraction of VS in the solid fraction following solid-liquid </param>
        /// <param name="flowRateOfVolatileSolidsInDigestate">Flow rate of digestate (t day^-1)</param>
        /// <returns>Flow rate of VS in the solid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowRateVolatileSolidsInDigestateSolidFraction(
            double fractionVolatileSolidsCoefficient, 
            double flowRateOfVolatileSolidsInDigestate)
        {
            return fractionVolatileSolidsCoefficient * flowRateOfVolatileSolidsInDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-7
        /// </summary>
        /// <param name="tanFlowInLiquidFractionOfDigestate">Flow rate of TAN in the liquid fraction of digestate (kg day^-1)</param>
        /// <param name="organicNitrogenFlowInLiquidFractionOfDigestate">Flow rate of organic N in the liquid fraction of digestate (kg day^-1)</param>
        /// <returns>Flow rate of total N in the liquid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowOfTotalNitrogenInLiquidFractionOfDigestate(
            double tanFlowInLiquidFractionOfDigestate,
            double organicNitrogenFlowInLiquidFractionOfDigestate)
        {
            return tanFlowInLiquidFractionOfDigestate + organicNitrogenFlowInLiquidFractionOfDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-8
        /// </summary>
        /// <param name="tanFlowInSolidFractionOfDigestate">Flow rate of TAN in the solid fraction of digestate (kg day^-1)</param>
        /// <param name="organicNitrogenFlowInSolidFractionOfDigestate">Flow rate of organic N in the solid fraction of digestate (kg day^-1)</param>
        /// <returns>Flow rate of total N in the solid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowOfTotalNitrogenInSolidFractionOfDigestate(
            double tanFlowInSolidFractionOfDigestate,
            double organicNitrogenFlowInSolidFractionOfDigestate)
        {
            return tanFlowInSolidFractionOfDigestate + organicNitrogenFlowInSolidFractionOfDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-9
        /// </summary>
        /// <param name="fractionTANCoefficient">Separation coefficient: fraction of TAN in the solid fraction following solid-liquid separation (Table 47</param>
        /// <param name="flowRateTANInDigestate">Flow rate of TAN in digestate (kg day^-1)</param>
        /// <returns>Flow rate of TAN in the liquid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowRateTANInDigestateLiquidFraction(
            double fractionTANCoefficient, 
            double flowRateTANInDigestate)
        {
            return (1 - fractionTANCoefficient) * flowRateTANInDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-10
        /// </summary>
        /// <param name="fractionTANCoefficient">Separation coefficient: fraction of TAN in the solid fraction following solid-liquid separation (Table 47</param>
        /// <param name="flowRateTANInDigestate">Flow rate of TAN in digestate (kg day^-1)</param>
        /// <returns>Flow rate of TAN in the solid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowRateTANInDigestateSolidFraction(
            double fractionTANCoefficient, 
            double flowRateTANInDigestate)
        {
            return fractionTANCoefficient * flowRateTANInDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-11
        /// </summary>
        /// <param name="fractionOrganicNitrogenCoefficient">Separation coefficient: fraction of organic N in the solid fraction following solid-liquid separation (Table 47)</param>
        /// <param name="flowRateOrganicNitrogenInDigestate">Flow rate of organic N in digestate (kg day^-1)</param>
        /// <returns>Flow rate of organic N in the liquid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowRateOrganicNitrogenInDigestateLiquidFraction(
            double fractionOrganicNitrogenCoefficient, 
            double flowRateOrganicNitrogenInDigestate)
        {
            return (1 - fractionOrganicNitrogenCoefficient) * flowRateOrganicNitrogenInDigestate;
        }

        /// <summary>
        /// Equation 4.9.4-12
        /// </summary>
        /// <param name="fractionOrganicNitrogenCoefficient">Separation coefficient: fraction of organic N in the solid fraction following solid-liquid separation (Table 47)</param>
        /// <param name="flowRateOrganicNitrogenInDigestate">Flow rate of organic N in digestate (kg day^-1)</param>
        /// <returns>Flow rate of organic N in the solid fraction of digestate (kg day^-1)</returns>
        public double CalculateFlowRateOrganicNitrogenInDigestateSolidFraction(
            double fractionOrganicNitrogenCoefficient, 
            double flowRateOrganicNitrogenInDigestate)
        {
            return fractionOrganicNitrogenCoefficient * flowRateOrganicNitrogenInDigestate;
        }
        #endregion

        #region 4.9.5.1 Storage Of Digestate

        /// <summary>
        /// No equation number
        /// </summary>
        /// <param name="meanDailyTemperature">Mean daily temperature (°C)</param>
        /// <returns>Methane emissions during digestate storage (kg day^-1)</returns>
        public double CalculateMethaneEmissionFactorDuringStorage(
            double meanDailyTemperature)
        {
            return 0.0175 * Math.Pow(meanDailyTemperature, 2) - 0.0245 * meanDailyTemperature + 0.1433;
        }

        /// <summary>
        /// Equation 4.9.5-1
        /// </summary>
        /// <param name="methaneEmissionFactor">Methane emission factor for digestate storage (g m-3 day^-1) </param>
        /// <param name="storageVolume">Storage volume of digestate (m3)</param>
        /// <returns>Methane emissions during digestate storage (kg day^-1)</returns>
        public double CalculateMethaneEmissionsDuringDigestateStorage(double methaneEmissionFactor,
                                                                      double storageVolume)
        {
            return methaneEmissionFactor * storageVolume;
        }

        /// <summary>
        /// Equation 4.9.5-2
        /// </summary>
        /// <param name="nitrousOxideEmissionFactor">Nitrous oxide emission factor for digestate storage (g m-3 day^-1). </param>
        /// <param name="storageVolume">Storage volume of digestate (m3)</param>
        /// <returns>Nitrous oxide emissions during digestate storage (kg day^-1)</returns>
        public double CalculateNitrousOxideEmissionsDuringDigestateStorage(double nitrousOxideEmissionFactor,
                                                                           double storageVolume)
        {
            return nitrousOxideEmissionFactor * storageVolume;
        }

        /// <summary>
        /// Equation 4.9.5-3
        /// </summary>
        /// <param name="ammoniaEmissionFactor">Ammonia emission factor for digestate storage (g m-3 day^-1)</param>
        /// <param name="storageVolume">Storage volume of digestate (m3)</param>
        /// <returns>Ammonia emissions during digestate storage (kg day^-1)</returns>
        public double CalculateAmmoniaEmissionsDuringDigestateStorage(double ammoniaEmissionFactor,
                                                                      double storageVolume)
        {
            return ammoniaEmissionFactor * storageVolume;
        }
        #endregion

        #endregion
    }
}

