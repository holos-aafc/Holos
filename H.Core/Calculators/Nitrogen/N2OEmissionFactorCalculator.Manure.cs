using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Services.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        public List<LandApplicationEmissionResult> CalculateAmmoniaEmissionsFromLandAppliedManure(
            Farm farm, 
            AnimalComponentEmissionsResults animalComponentEmissionsResults)
        {
            var componentCategory = animalComponentEmissionsResults.Component.ComponentCategory;
            var animalType = componentCategory.GetAnimalTypeFromComponentCategory();
            var totalManureProducedByAnimals = animalComponentEmissionsResults.TotalVolumeOfManureAvailableForLandApplication * 1000;
            var totalTanForLandApplicationOnDate = animalComponentEmissionsResults.TotalTANAvailableForLandApplication;
            var applicationsAndCropByAnimalType = farm.GetManureApplicationsAndAssociatedCropByAnimalType(animalType);
            var results = new List<LandApplicationEmissionResult>();
            var annualPrecipitation = farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation();
            var annualTemperature = farm.ClimateData.TemperatureData.GetMeanAnnualTemperature();
            var evapotranspiration = farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration();

            var emissionFactorData = _livestockEmissionConversionFactorsProvider.GetFactors(ManureStateType.Pasture, componentCategory, annualPrecipitation, annualTemperature, evapotranspiration, 0.0, animalType, farm);
            foreach (var tuple in applicationsAndCropByAnimalType)
            {
                var applicationEmissionResult = new LandApplicationEmissionResult();

                var crop = tuple.Item1;
                applicationEmissionResult.CropViewItem = crop;

                var manureApplication = tuple.Item2;

                var date = manureApplication.DateOfApplication;
                var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(date.Year, (Months)date.Month);

                var fractionOfManureUsed = (manureApplication.AmountOfManureAppliedPerHectare * crop.Area) / totalManureProducedByAnimals;
                if (fractionOfManureUsed > 1.0)
                    fractionOfManureUsed = 1.0;

                applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication = manureApplication.AmountOfNitrogenAppliedPerHectare * crop.Area;

                applicationEmissionResult.TotalVolumeOfManureUsedDuringApplication = manureApplication.AmountOfManureAppliedPerHectare * crop.Area;

                var adjustedEmissionFactor = CalculateAmbientTemperatureAdjustmentForLandApplication(temperature);

                var emissionFactorForLandApplication = GetEmissionFactorForLandApplication(crop, manureApplication);
                var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(emissionFactorForLandApplication, adjustedEmissionFactor);

                var fractionVolatilized = 0d;
                if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
                {
                    // Equation 4.6.2-3
                    applicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * totalTanForLandApplicationOnDate * adjustedAmmoniaEmissionFactor;

                    // Equation 4.6.3-1
                    fractionVolatilized = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0 ? applicationEmissionResult.AmmoniacalLoss / applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication : 0;
                }
                else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
                {
                    // Equation 4.6.2-7
                    applicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * emissionFactorData.VolatilizationFraction;

                    fractionVolatilized = emissionFactorData.VolatilizationFraction;
                }
                else
                {
                    var emissionFraction = 0d;
                    if (temperature >= 15)
                    {
                        emissionFraction = 0.85;
                    }
                    else if (temperature >= 10 && temperature < 15)
                    {
                        emissionFraction = 0.73;
                    }
                    else if (temperature >= 5 && temperature < 10)
                    {
                        emissionFraction = 0.35;
                    }
                    else
                    {
                        emissionFraction = 0.25;
                    }

                    // Equation 4.6.2-5
                    applicationEmissionResult.AmmoniacalLoss = fractionOfManureUsed * totalManureProducedByAnimals * emissionFraction;

                    // Equation 4.6.3-1
                    fractionVolatilized = applicationEmissionResult.AmmoniacalLoss / applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication;
                }

                // Equation 4.6.2-4
                // Equation 4.6.2-6
                var ammoniaLoss = applicationEmissionResult.AmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                // Equation 4.6.3-2
                applicationEmissionResult.TotalN2ONFromManureVolatilized = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * fractionVolatilized * emissionFactorData.EmissionFactorVolatilization;

                // Equation 4.6.3-3
                var n2OVolatilized = applicationEmissionResult.TotalN2ONFromManureVolatilized * CoreConstants.ConvertN2ONToN2O;

                // Equation 4.6.3-4
                applicationEmissionResult.AdjustedAmmoniacalLoss = applicationEmissionResult.AmmoniacalLoss - applicationEmissionResult.TotalN2ONFromManureVolatilized;

                // Equation 4.6.3-5
                var adjustedAmmoniaEmissions = applicationEmissionResult.AdjustedAmmoniacalLoss * CoreConstants.ConvertNH3NToNH3;

                var leachingFraction = CalculateLeachingFraction(annualPrecipitation, evapotranspiration);

                // Equation 4.6.4-1
                applicationEmissionResult.TotalN2ONFromManureLeaching = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * emissionFactorData.EmissionFactorLeach;

                // Equation 4.6.4-4
                applicationEmissionResult.TotalNitrateLeached = applicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication * leachingFraction * (1.0 - emissionFactorData.EmissionFactorLeach);

                // Equation 4.6.5-1
                applicationEmissionResult.TotalIndirectN2ONEmissions = applicationEmissionResult.TotalN2ONFromManureVolatilized + applicationEmissionResult.TotalN2ONFromManureLeaching;

                // Equation 4.6.5-2
                applicationEmissionResult.TotalIndirectN2OEmissions = applicationEmissionResult.TotalIndirectN2ONEmissions * CoreConstants.ConvertN2ONToN2O;

                results.Add(applicationEmissionResult);
            }

            return results;
        }

        #endregion

        #region Private Methods

        private double GetEmissionFactorForLandApplication(
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            return !manureApplicationViewItem.ManureStateType.IsLiquidManure()
                ? _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(
                    cropViewItem.TillageType)
                : _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(
                    manureApplicationViewItem.ManureApplicationMethod);
        }

        #endregion
    }
}
