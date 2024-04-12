using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Services.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Media.Animation;
using AutoMapper.Configuration.Annotations;
using H.Core.Events;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Infrastructure;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        /// <summary>
        /// Equation 4.6.1-1
        /// 
        /// Calculates direct emissions from the manure specifically applied to the field
        ///
        /// (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreadingForField(
            CropViewItem viewItem,
            Farm farm)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            var totalLocalAndImportedNitrogenApplied = this.GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear(viewItem, farm);

            var result = totalLocalAndImportedNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-2
        /// 
        /// (kg N)
        /// </summary>
        public double GetTotalManureNitrogenAppliedFromLivestockAndImportsInYear(CropViewItem viewItem, Farm farm)
        {
            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null || (field.HasLivestockManureApplicationsInYear(viewItem.Year) == false && 
                                  field.HasImportedManureApplicationsInYear(viewItem.Year) == false))
            {
                return 0;
            }

            var totalNitrogen = 0d;

            foreach (var manureApplication in viewItem.ManureApplicationViewItems.Where(manureViewItem => manureViewItem.DateOfApplication.Year == viewItem.Year))
            {
                totalNitrogen += manureApplication.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
            }

            return totalNitrogen;
        }

        public double GetTotalManureVolumeAppliedFromLivestockAndImportsInYear(CropViewItem viewItem, Farm farm)
        {
            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null || (field.HasLivestockManureApplicationsInYear(viewItem.Year) == false && field.HasImportedManureApplicationsInYear(viewItem.Year) == false))
            {
                return 0;
            }

            var totalVolume = 0d;

            foreach (var manureApplication in viewItem.ManureApplicationViewItems.Where(manureViewItem => manureViewItem.DateOfApplication.Year == viewItem.Year))
            {
                totalVolume += manureApplication.AmountOfManureAppliedPerHectare * viewItem.Area;
            }

            return totalVolume;
        }

        /// <summary>
        /// Equation 4.6.1-3
        ///
        /// There can be multiple fields on a farm and the emission factor calculations are field-dependent (accounts for crop type, fertilizer, etc.). So
        /// we take the weighted average of these fields when calculating the EF for organic nitrogen (ON). This is to be used when calculating direct emissions
        /// from land applied manure or digestate. Native rangeland is not included.
        /// </summary>
        public double CalculateWeightedOrganicNitrogenEmissionFactor(
            List<CropViewItem> itemsByYear,
            Farm farm)
        {
            var fieldAreasAndEmissionFactors = new List<WeightedAverageInput>();
            var filteredItems = itemsByYear.Where(x => x.CropType.IsNativeGrassland() == false);

            foreach (var cropViewItem in filteredItems)
            {
                var emissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                    viewItem: cropViewItem,
                    farm: farm);

                fieldAreasAndEmissionFactors.Add(new WeightedAverageInput()
                {
                    Value = emissionFactor,
                    Weight = cropViewItem.Area,
                });
            }

            var weightedEmissionFactor = this.CalculateWeightedEmissionFactor(fieldAreasAndEmissionFactors);

            return weightedEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.1-4
        ///
        /// (kg N)
        /// </summary>
        public double CalculateTotalManureNitrogenRemaining(
            double totalManureNitrogenAvailableForLandApplication,
            double totalManureNitrogenAlreadyAppliedToFields,
            double totalManureNitrogenExported,
            double totalImportedNitrogen)
        {
            var result = totalManureNitrogenAvailableForLandApplication - (totalManureNitrogenAlreadyAppliedToFields - totalImportedNitrogen) - totalManureNitrogenExported;
            if (result < 0)
            {
                // Can't have a negative value of manure remaining
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-5
        ///
        /// Calculates the total N2O-N from the remaining manure N not applied to any field
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectN2ONFromRemainingManureNitrogen(
            double weightedEmissionFactor,
            double totalManureNitrogenRemaining)
        {
            var result = totalManureNitrogenRemaining * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-6
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateDirectN2ONFromLeftOverManureForField(
            Farm farm,
            CropViewItem viewItem)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var itemsByYear = farm.GetCropDetailViewItemsByYear(viewItem.Year, false);
            var weightedEmissionFactor = CalculateWeightedOrganicNitrogenEmissionFactor(itemsByYear, farm);

            // The total N after all applications and exports have been subtracted
            var totalNitrogenRemaining = this.ManureService.GetTotalNitrogenRemainingForFarmAndYear(viewItem.Year, farm);
            var emissionsFromNitrogenRemaining = this.CalculateTotalDirectN2ONFromRemainingManureNitrogen(
                weightedEmissionFactor: weightedEmissionFactor,
                totalManureNitrogenRemaining: totalNitrogenRemaining);

            var totalAreaOfAllFields = farm.GetTotalAreaOfFarm(false, viewItem.Year);
            if (totalAreaOfAllFields == 0)
            {
                totalAreaOfAllFields = 1;
            }

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            var areaOfThisField = field.FieldArea;

            // The total N2O-N that is left over and must be associated with this field so that all manure is applied to the fields in the same year (nothing is remaining to be applied)
            var result = emissionsFromNitrogenRemaining * (areaOfThisField / totalAreaOfAllFields);

            return result;
        }

        public double CalculateVolumeFromLeftOverManureForField(
            Farm farm,
            CropViewItem viewItem)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }
            
            // The total volume after all applications and exports have been subtracted
            var totalVolumeRemaining = this.ManureService.GetTotalVolumeRemainingForFarmAndYear(viewItem.Year, farm);

            var totalAreaOfAllFields = farm.GetTotalAreaOfFarm(false, viewItem.Year);
            if (totalAreaOfAllFields == 0)
            {
                totalAreaOfAllFields = 1;
            }

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            var areaOfThisField = field.FieldArea;

            // The total volume that is left over and must be associated with this field so that all manure is applied to the fields in the same year (nothing is remaining to be applied)
            var result = totalVolumeRemaining * (areaOfThisField / totalAreaOfAllFields);

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-9
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(Farm farm, int year)
        {
            var viewItemsByYear = farm.GetCropDetailViewItemsByYear(year, false);

            var weightedEmissionFactor = this.CalculateWeightedOrganicNitrogenEmissionFactor(viewItemsByYear, farm);
            var totalExportedManureNitrogen = this.ManureService.GetTotalNitrogenFromExportedManure(year, farm);

            var emissions = this.CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(totalExportedManureNitrogen, weightedEmissionFactor);

            return emissions;
        }

        /// <summary>
        /// Equation 4.6.1-9
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(
            double totalExportedManureNitrogen,
            double weightedEmissionFactor)
        {
            var result = totalExportedManureNitrogen * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-10
        ///
        /// Includes direct emissions from applied manure.
        /// 
        /// (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONFromFieldAppliedManure(
            Farm farm,
            CropViewItem viewItem, 
            bool includeRemainingAmounts = true)
        {
            var result = 0d;

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            var applied = this.CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreadingForField(viewItem, farm);
            var leftOver = this.CalculateDirectN2ONFromLeftOverManureForField(farm, viewItem);

            result = applied;

            if (includeRemainingAmounts)
            {
                result += leftOver;
            }

            return result;
        }

        public double CalculateVolumeFromFieldAppliedManure(
            Farm farm,
            CropViewItem viewItem,
            bool includeRemainingAmounts = true)
        {
            var result = 0d;

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            var applied = this.GetTotalManureVolumeAppliedFromLivestockAndImportsInYear(viewItem, farm);
            var leftOver = this.CalculateVolumeFromLeftOverManureForField(farm, viewItem);

            result = applied;

            if (includeRemainingAmounts)
            {
                result += leftOver;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-11
        ///
        /// Includes direct emissions from applied manure and direct emissions from remaining manure for the entire farm.
        /// 
        /// (kg N2O-N)
        /// </summary>
        public double CalculateDirectN2ONFromFieldAppliedManureForFarmAndYear(
            Farm farm,
            int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var viewItem in itemsByYear)
            {
                var emissions = this.CalculateDirectN2ONFromFieldAppliedManure(farm, viewItem);

                result += emissions;
            }

            var exports = this.CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(farm, year);

            result += exports;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-2
        /// </summary>
        /// <param name="emissionFactorForLandApplication">Default NH3 emission factor for land application</param>
        /// <param name="ambientTemperatureAdjustment">Ambient temperature based adjustment</param>
        public double CalculateAdjustedAmmoniaEmissionFactor(
            double emissionFactorForLandApplication,
            double ambientTemperatureAdjustment)
        {
            return emissionFactorForLandApplication * ambientTemperatureAdjustment;
        }

        /// <summary>
        /// Equation 4.6.2-2
        /// </summary>
        public double CalculateAdjustedAmmoniaEmissionFactor(
            CropViewItem viewItem, 
            ManureApplicationViewItem manureApplication, 
            double averageDailyTemperature)
        {
            var emissionFactorForLandApplication = GetEmissionFactorForLandApplication(viewItem, manureApplication);
            var adjustedEmissionFactor = CalculateAmbientTemperatureAdjustmentForLandApplication(averageDailyTemperature);
            var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(emissionFactorForLandApplication, adjustedEmissionFactor);

            return adjustedAmmoniaEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.2-3
        /// Equation 4.6.2-12
        /// Equation 4.6.2-20
        /// </summary>
        public double CalculateNH3NLossFromLandAppliedManure(
            Farm farm, 
            CropViewItem viewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var result = 0d;

            if (manureApplicationViewItem.IsImportedManure()) 
            {
                var landApplicationFactors = this.GetLandApplicationFactors(farm, manureApplicationViewItem);
                var nitrogenUsed = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;

                result = nitrogenUsed * landApplicationFactors.VolatilizationFraction;

                return result;
            }

            var animalType = manureApplicationViewItem.AnimalType;
            if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
            {
                var adjustedAmmoniaEmissionFactor = this.GetAdjustedAmmoniaEmissionFactor(farm, viewItem, manureApplicationViewItem);
                var tanUsed = this.ManureService.GetAmountOfTanUsedDuringLandApplication(viewItem, manureApplicationViewItem);
                result = tanUsed * adjustedAmmoniaEmissionFactor;
            }
            else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
            {
                var landApplicationFactors = this.GetLandApplicationFactors(farm, manureApplicationViewItem);
                var nitrogenUsed = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;

                // 4.6.2-12
                result = nitrogenUsed * landApplicationFactors.VolatilizationFraction;
            }
            else
            {
                var temperature = this.ClimateProvider.GetMeanTemperatureForDay(farm, manureApplicationViewItem.DateOfApplication);
                var volumeOfManureUsed = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;
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

                result = volumeOfManureUsed * emissionFraction;
            }

            return result;
        }

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        public double CalculateAmmoniacalLossFromManureForField(Farm farm, CropViewItem viewItem)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                result += this.CalculateNH3NLossFromLandAppliedManure(farm, viewItem, manureApplicationViewItem);
            }

            return result;
        }

        /// <summary>
        /// (kg NH3)
        /// </summary>
        public double CalculateAmmoniaLossFromManureForField(Farm farm, CropViewItem viewItem)
        {
            var result = 0d;

            var ammoniacalLoss = this.CalculateAmmoniacalLossFromManureForField(farm, viewItem);
            result = CoreConstants.ConvertToNH3(ammoniacalLoss);

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-5
        ///
        /// (kg NH3-N (kg N)^-1)
        /// </summary>
        public double CalculateWeightedLandApplicationEmissionFactor(
            int year,
            Farm farm)
        {
            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            var fieldAreasAndEmissionFactors = new List<WeightedAverageInput>();

            foreach (var cropViewItem in itemsByYear)
            {
                // Each field can have multiple manure applications, to simplify emission factor calculation take first application if there is more than
                // one. Alternative is to take average of emission factors calculated for each manure application
                var manureApplication = cropViewItem.ManureApplicationViewItems.FirstOrDefault();
                if (manureApplication == null)
                {
                    manureApplication = new ManureApplicationViewItem() {DateOfApplication = new DateTime(year, 10, 1)};
                }

                var averageDailyTemperature = farm.ClimateData.GetMeanTemperatureForDay(manureApplication.DateOfApplication);
                var adjustedAmmoniaEmissionFactor = this.CalculateAdjustedAmmoniaEmissionFactor(cropViewItem, manureApplication, averageDailyTemperature);
                fieldAreasAndEmissionFactors.Add(new WeightedAverageInput()
                {
                    Value = adjustedAmmoniaEmissionFactor,
                    Weight = cropViewItem.Area,
                });
            }

            var weightedEmissionFactor = this.CalculateWeightedEmissionFactor(fieldAreasAndEmissionFactors);

            return weightedEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.2-6
        /// </summary>
        public List<Tuple<double, AnimalType>> CalculateTANRemainingForAllManureTypes(
            Farm farm,
            int year)
        {
            var results = new List<Tuple<double, AnimalType>>();

            var viewItemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            var tanUsedByAnimalTypes = this.ManureService.GetTotalTanAppliedToAllFields(year, viewItemsByYear);
            var manureCategoriesUsed = this.ManureService.GetManureCategoriesProducedOnFarm(farm);
            foreach (var animalType in manureCategoriesUsed)
            {
                var totalTanCreatedByAnimalType = this.ManureService.GetTotalTANCreated(
                    year: year,
                    animalType: animalType);

                var tanUsedByAnimalType = tanUsedByAnimalTypes.Where(x => x.Item2 == animalType);
                var tanUsed =  tanUsedByAnimalType.Sum(x => x.Item1);
                var tanExportedByType = this.ManureService.GetTotalTANExportedByAnimalType(animalType, farm, year);

                var result = totalTanCreatedByAnimalType - tanUsed - tanExportedByType;

                var entry = new Tuple<double, AnimalType>(result, animalType);
                results.Add(entry);
            }

            return results;
        }

        /// <summary>
        /// Equation 4.6.2-7
        /// </summary>
        public List<Tuple<double, AnimalType>> CalculateAmmoniaFromLeftOverBeefAndDairyManureForFarm(
            int year,
            Farm farm)
        {
            var results = new List<Tuple<double, AnimalType>>();

            var weightedEmissionFactor = this.CalculateWeightedLandApplicationEmissionFactor(
                year: year,
                farm: farm);

            var tanRemainingForAllAnimalTypes = this.CalculateTANRemainingForAllManureTypes(farm, year);
            foreach (var tanRemainingForAllAnimalType in tanRemainingForAllAnimalTypes.Where(x => x.Item2.IsBeefCattleType() || x.Item2.IsDairyCattleType()))
            {
                var tan = tanRemainingForAllAnimalType.Item1;
                var ammonia = tan * weightedEmissionFactor;

                var result = new Tuple<double, AnimalType>(ammonia, tanRemainingForAllAnimalType.Item2);

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Equation 4.6.2-8
        /// </summary>
        public double CalculateAmmoniacalLossFromLeftOverBeefAndDairyManureForField(int year, Farm farm, CropViewItem cropViewItem)
        {
            var totalArea = farm.GetTotalAreaOfFarm(false, year);
            var areaOfField = cropViewItem.Area;

            var ammoniaFromLeftOverManureForFarm = CalculateAmmoniaFromLeftOverBeefAndDairyManureForFarm(year, farm).Sum(x => x.Item1);
            var result = ammoniaFromLeftOverManureForFarm * (areaOfField / totalArea);

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-9
        /// Equation 4.6.2-10
        /// </summary>
        public double CalculateAmountOfTANFromExportedManure(
            Farm farm,
            int year)
        {
            return this.ManureService.GetTANExportedForFarm(farm, year).Sum(x => x.Item1);
        }

        /// <summary>
        /// Equation 4.6.2-11
        /// </summary>
        public double CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(
            int year,
            Farm farm)
        {
            var totalTANExported = this.CalculateAmountOfTANFromExportedManure(farm, year);
            var weightedEmissionFactor = CalculateWeightedLandApplicationEmissionFactor(year, farm);

            return totalTANExported * weightedEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.2-12
        /// </summary>
        public double CalculateNH3NLossFromFarmSourcedLandAppliedManureForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in cropViewItem.GetLocalSourcedApplications(year))
            {
                result += this.CalculateNH3NLossFromLandAppliedManure(farm, cropViewItem, manureApplicationViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-13
        /// </summary>
        public double CalculateTotalNitrogenFromFieldApplication(
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in cropViewItem.GetLocalSourcedApplications(year))
            {
                result += manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area;
            }

            return result;
        }

        public double CalculateTotalNitrogenFromImportedFieldApplication(
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in cropViewItem.GetManureImportsByYear(year))
            {
                result += manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-13
        /// </summary>
        public double CalculateTotalNitrogenFromAllFieldApplications(
            Farm farm,
            int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                var nitrogenApplied = CalculateTotalNitrogenFromFieldApplication(cropViewItem, year);
                result += nitrogenApplied;
            }

            return result;
        }

        public double CalculateTotalNitrogenFromAllImportFieldApplications(
            Farm farm,
            int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                var nitrogenApplied = CalculateTotalNitrogenFromImportedFieldApplication(cropViewItem, year);
                result += nitrogenApplied;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-15
        /// </summary>
        public Dictionary<AnimalType, double> CalculateAmmoniaEmissionsFromLeftOverSheepSwineAndOtherManureForFarm(
            Farm farm,
            int year)
        {
            var dictionary = new Dictionary<AnimalType, double>();

            // Get left over manure by type - need to get only poultry, etc. types as beef dairy calculated using TAN
            var typesOfManureUsed = this.ManureService.GetManureCategoriesProducedOnFarm(farm).Where(x => x.IsSheepType() || x.IsSwineType() || x.IsOtherAnimalType());
            foreach (var animalType in typesOfManureUsed)
            {
                var volatilizationFractionForLandApplication = this.LivestockEmissionConversionFactorsProvider.GetVolatilizationFractionForLandApplication(animalType, farm.DefaultSoilData.Province, year);

                var createdByType = this.ManureService.GetTotalNitrogenCreated(year, animalType);
                var usedByType = this.ManureService.GetTotalNitrogenAppliedToAllFields(year);
                var remainingByType = createdByType - usedByType;

                var ammonia = remainingByType * volatilizationFractionForLandApplication;

                dictionary[animalType] = ammonia;
            }

            return dictionary;
        }

        /// <summary>
        /// Equation 4.6.2-16
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateNH3NEmissionsFromLeftOverManureForField(
            CropViewItem cropViewItem,
            int year,
            Farm farm)
        {
            var result = 0d;
            var totalAmmoniaLeftOverForSheepSwineAndOtherAnimals = 0d;

            // Get ammonia from sheep, swine, and other animals
            var ammoniaEmissionsFromLeftOverManureByType = this.CalculateAmmoniaEmissionsFromLeftOverSheepSwineAndOtherManureForFarm(farm, year);
            if (ammoniaEmissionsFromLeftOverManureByType.Any())
            {
                totalAmmoniaLeftOverForSheepSwineAndOtherAnimals = ammoniaEmissionsFromLeftOverManureByType.Sum(x => x.Value);
            }

            var totalAmmoniaFromBeefAndDairyLeftOverManure = CalculateAmmoniacalLossFromLeftOverBeefAndDairyManureForField(year, farm, cropViewItem);

            var areaOfFarm = farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, cropViewItem.Year);
            var areaOfField = cropViewItem.Area;

            result = (totalAmmoniaFromBeefAndDairyLeftOverManure + (totalAmmoniaLeftOverForSheepSwineAndOtherAnimals *(areaOfField / areaOfFarm))) ;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-17
        /// Equation 4.6.2-18
        /// </summary>
        public Dictionary<AnimalType, double> CalculateTotalNitrogenFromExportedManure(
            int year,
            Farm farm)
        {
            var dictionary = new Dictionary<AnimalType, double>();
            var typesExported = this.ManureService.GetManureTypesExported(farm, year);
            foreach (var animalType in typesExported)
            {
                var nitrogenExportedByType = this.ManureService.GetTotalNitrogenFromExportedManure(year, farm, animalType);
                dictionary[animalType] = nitrogenExportedByType;
            }

            return dictionary;
        }

        /// <summary>
        /// Equation 4.6.2-19
        /// </summary>
        public Dictionary<AnimalType, double> CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(
            Farm farm,
            int year)
        {
            var dictionary = new Dictionary<AnimalType, double>();

            var totalNitrogenFromExports = this.CalculateTotalNitrogenFromExportedManure(year, farm);
            foreach (var totalNitrogenFromExport in totalNitrogenFromExports)
            {
                var manureType = totalNitrogenFromExport.Key;
                var nitrogenExported = totalNitrogenFromExport.Value;
                var volatilizationFraction = this.LivestockEmissionConversionFactorsProvider.GetVolatilizationFractionForLandApplication(
                    animalType: manureType,
                    province: farm.DefaultSoilData.Province,
                    year);

                dictionary[manureType] = nitrogenExported * volatilizationFraction;
            }

            return dictionary;
        }

        /// <summary>
        /// Equation 4.6.2-20
        /// </summary>
        public Dictionary<AnimalType, double> CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var dictionary = new Dictionary<AnimalType, double>();

            var manureApplicationViewItems = cropViewItem.GetManureImportsByYear(year);
            foreach (var manureImport in manureApplicationViewItems)
            {
                var animalType = manureImport.AnimalType;
                var totalNitrogen = manureImport.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area;
                var volatilizationFractionForLandApplication = LivestockEmissionConversionFactorsProvider.GetVolatilizationFractionForLandApplication(animalType, farm.DefaultSoilData.Province, year);
                var emissions = (totalNitrogen * volatilizationFractionForLandApplication);

                if (dictionary.ContainsKey(animalType))
                {
                    dictionary[animalType] += emissions;
                }
                else
                {
                    dictionary[animalType] = emissions;
                }

            }

            return dictionary;
        }

        /// <summary>
        /// Equation 4.6.2-20
        /// </summary>
        public List<Dictionary<AnimalType, double>> CalculateAmmoniaEmissionsFromImportedManureForFields(
            Farm farm,
            List<CropViewItem> itemsByYear,
            int year)
        {
            var results = new List<Dictionary<AnimalType, double>>();

            foreach (var cropViewItem in itemsByYear)
            {
                var result = CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, cropViewItem, year);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Equation 4.6.2-20
        /// </summary>
        public double CalculateAmmoniaEmissionsFromImportedManureForFarmAndYear(
            Farm farm,
            int year)
        {
            var results = new List<Dictionary<AnimalType, double>>();

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                var result = CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, cropViewItem, year);
                results.Add(result);
            }

            var total = 0d;
            foreach (var result in results)
            {
                total += result.Sum(x => x.Value);
            }

            return total;
        }

        /// <summary>
        /// Equation 4.6.2-21
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromLandAppliedManureForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var ammoniaFromApplications = this.CalculateNH3NLossFromFarmSourcedLandAppliedManureForField(farm, cropViewItem, year);

            var ammoniaEmissionsFromLeftOver = this.CalculateAmmoniaEmissionsFromLeftOverSheepSwineAndOtherManureForFarm(farm, year);
            var totalAmmoniaFromLeftOverManure = ammoniaEmissionsFromLeftOver.Sum(x => x.Value);

            var ammoniaEmissionsFromImportedManure = this.CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, cropViewItem, year);
            var totalEmissionsFromImports = ammoniaEmissionsFromImportedManure.Sum(x => x.Value);

            result = ammoniaFromApplications + totalAmmoniaFromLeftOverManure + totalEmissionsFromImports;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-21
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromLandAppliedManureForFarmAndYear(
            Farm farm,
            List<CropViewItem> itemsByYear,
            int year)
        {
            var result = 0d;

            foreach (var viewItem in itemsByYear)
            {
                result += CalculateTotalAmmoniaEmissionsFromLandAppliedManureForField(farm, viewItem, year);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-22
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromFarm(
            Farm farm,
            List<CropViewItem> itemsByYear,
            int year)
        {
            var result = 0d;

            var ammoniaEmissionsFromLandAppliedManureForFarmAndYear = CalculateTotalAmmoniaEmissionsFromLandAppliedManureForFarmAndYear(farm, itemsByYear, year); ;
            var ammoniaEmissionsFromExportsForFarmAndYear = this.CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(farm, year).Sum(x => x.Value);
            var ammoniaEmissionsFromLeftOverManure = this.CalculateAmmoniaEmissionsFromLeftOverSheepSwineAndOtherManureForFarm(farm, year).Sum(x => x.Value);
            var ammoniaEmissionsFromImportedManure = this.CalculateAmmoniaEmissionsFromImportedManureForFarmAndYear(farm, year);

            result = ammoniaEmissionsFromImportedManure + ammoniaEmissionsFromLeftOverManure + ammoniaEmissionsFromExportsForFarmAndYear + ammoniaEmissionsFromLandAppliedManureForFarmAndYear;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-22
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromFarmAndYear(
            Farm farm,
            int year)
        {
            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            var result = this.CalculateTotalAmmoniaEmissionsFromFarm(farm, itemsByYear, year);

            return result;
        }

        public double CalculateNH3NLossFromExportedManure(Farm farm, 
            CropViewItem viewItem, 
            ManureExportViewItem manureExportViewItem)
        {
            var result = 0d;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-1
        /// Equation 4.6.3-3
        /// </summary>
        public double CalculateTotalN2ONFromManureVolatilized(
            Farm farm,
            CropViewItem viewItem, 
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var ammoniacalLoss = this.CalculateNH3NLossFromLandAppliedManure(farm, viewItem, manureApplicationViewItem);
            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, viewItem.Year);

            var result = ammoniacalLoss * emissionFactorForVolatilization;

            return result;
        }
        

        /// <summary>
        /// Equation 4.6.3-1
        /// </summary>
        public double CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedManureForField(int year, Farm farm, CropViewItem cropViewItem)
        {
            var field = farm.GetFieldSystemComponent(cropViewItem.FieldSystemComponentGuid);
            if (field == null || field.HasLivestockManureApplicationsInYear(cropViewItem.Year) == false)
            {
                return 0;
            }

            var ammoniaEmissionsFromLandAppliedManure = this.CalculateNH3NLossFromFarmSourcedLandAppliedManureForField(farm, cropViewItem, year);
            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, year);

            var result = ammoniaEmissionsFromLandAppliedManure * emissionFactorForVolatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-2
        /// </summary>
        public double CalculateN2ONFromVolatilizationOfLeftOverManureForField(int year, Farm farm, CropViewItem cropViewItem)
        {
            var field = farm.GetFieldSystemComponent(cropViewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, year);
            var leftOverAmmonia = this.CalculateNH3NEmissionsFromLeftOverManureForField(cropViewItem, year, farm);

            var result = leftOverAmmonia * emissionFactorForVolatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-3
        /// </summary>
        public double CalculateVolatilizationEmissionsFromExportedManureForFarmAndYear(
            Farm farm,
            int year)
        {
            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, year);
            var exportedAmmonia = this.CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(farm, year).Sum(x => x.Value);

            var result = exportedAmmonia * emissionFactorForVolatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-4
        /// </summary>
        public double CalculateVolatilizationEmissionsFromImportedManureForField(
            Farm farm,
            CropViewItem viewItem,
            int year)
        {
            var result = 0d;

            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, year);
            var exportedAmmonia = this.CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, viewItem, year).Sum(x => x.Value);

            result = emissionFactorForVolatilization * exportedAmmonia;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-5
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalManureN2OVolatilizationForField(
            CropViewItem cropViewItem,
            Farm farm,
            int year)
        {
            var result = 0d;

            var amount = this.CalculateTotalManureN2ONVolatilizationForField(cropViewItem, farm, year);
            result = CoreConstants.ConvertToN2O(amount);

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-5
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalManureN2ONVolatilizationForField(
            CropViewItem cropViewItem,
            Farm farm,
            int year, 
            bool includeRemainingAmounts = true)
        {
            if (cropViewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var result = 0d;

            var volatilizationFromApplications = this.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedManureForField(year, farm, cropViewItem);
            var volatilizationFromLeftOverManure = this.CalculateN2ONFromVolatilizationOfLeftOverManureForField(year, farm, cropViewItem);
            var ammoniaFromImportedManure = this.CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, cropViewItem, year).Sum(x => x.Value);
            var volatilizationFromImportedManure = ammoniaFromImportedManure * this.GetEmissionFactorForVolatilization(farm, year);

            result = volatilizationFromApplications + volatilizationFromImportedManure;

            if (includeRemainingAmounts)
            {
                result += volatilizationFromLeftOverManure;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-6
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalManureN2ONVolatilizationForFarmAndYear(
            Farm farm,
            int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += this.CalculateTotalManureN2ONVolatilizationForField(cropViewItem, farm, year);
            }

            result += this.CalculateVolatilizationEmissionsFromExportedManureForFarmAndYear(farm, year);

            return result;
        }

        public double CalculateTotalAdjustedAmmoniaEmissionsFromManureForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var ammoniacalLoss = CalculateTotalAdjustedAmmoniaEmissionsFromLandAppliedManureForField(farm, cropViewItem, year);
            result = CoreConstants.ConvertToNH3(ammoniacalLoss);

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-7
        /// </summary>
        public double CalculateTotalAdjustedAmmoniaEmissionsFromLandAppliedManureForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var ammoniacalLoss = this.CalculateNH3NLossFromFarmSourcedLandAppliedManureForField(farm, cropViewItem, year);
            var volatilization = this.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedManureForField(year, farm, cropViewItem);

            result = ammoniacalLoss - volatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-8
        /// </summary>
        public double CalculateTotalAdjustedAmmoniaEmissionsFromLeftOverManureForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var ammoniaLoss = this.CalculateNH3NEmissionsFromLeftOverManureForField(cropViewItem, year, farm);
            var volatilization = this.CalculateN2ONFromVolatilizationOfLeftOverManureForField(year, farm, cropViewItem);

            result = ammoniaLoss - volatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-9
        /// </summary>
        public double CalculateTotalAdjustedAmmoniaEmissionsFromExportsForFarm(
            Farm farm,
            int year)
        {
            var result = 0d;

            var ammoniaLoss = this.CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(farm, year).Sum(x => x.Value);
            var volatilization = this.CalculateVolatilizationEmissionsFromExportedManureForFarmAndYear(farm, year);

            result = ammoniaLoss - volatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-10
        /// </summary>
        public double CalculateTotalAdjustedAmmoniacalEmissionsFromImportsForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var ammoniaLoss = this.CalculateAmmoniaEmissionsFromVolatilizationOfImportedManureForField(farm, cropViewItem, year).Sum(x => x.Value);
            var volatilization = this.CalculateVolatilizationEmissionsFromImportedManureForField(farm, cropViewItem, year);

            result = ammoniaLoss - volatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-10
        /// </summary>
        public double CalculateTotalAdjustedAmmoniaEmissionsFromImportsForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            result = this.CalculateTotalAdjustedAmmoniacalEmissionsFromImportsForField(farm, cropViewItem, year);

            result = CoreConstants.ConvertToNH3(result);

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-11
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateTotalManureAmmoniaEmissionsForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var field = farm.GetFieldSystemComponent(cropViewItem.FieldSystemComponentGuid);
            if (field == null )
            {
                return 0;
            }

            if (cropViewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var result = 0d;

            var adjustedAmmoniaFromLandApplications = CalculateTotalAdjustedAmmoniaEmissionsFromLandAppliedManureForField(farm, cropViewItem, year);
            var adjustedAmmoniaFromImports = CalculateTotalAdjustedAmmoniacalEmissionsFromImportsForField(farm, cropViewItem, year);
            var adjustedAmmoniaFromLeftOverManure = CalculateTotalAdjustedAmmoniaEmissionsFromLeftOverManureForField(farm, cropViewItem, year);

            result = adjustedAmmoniaFromLandApplications + adjustedAmmoniaFromImports + adjustedAmmoniaFromLeftOverManure;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-12
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateTotalAdjustedAmmoniaEmissionsForFarmAndYear(
            Farm farm,
            int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += CalculateTotalManureAmmoniaEmissionsForField(farm, cropViewItem, year);
            }

            result += CalculateTotalAdjustedAmmoniaEmissionsFromExportsForFarm(farm, year);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONFromLeachingFromManureApplication(Farm farm, CropViewItem viewItem, ManureItemBase manureItemBase)
        {
            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;
            var nitrogenUsed = manureItemBase.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);

            var result = nitrogenUsed * leachingFraction * leachingEmissionFactorForLandApplication;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONFromManureLeachingForField(Farm farm, CropViewItem viewItem)
        {
            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null || field.HasLivestockManureApplicationsInYear(viewItem.Year) == false && field.HasImportedManureApplicationsInYear(viewItem.Year) == false)
            {
                return 0;
            }

            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var result = 0d;

            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                result += this.CalculateTotalN2ONFromLeachingFromManureApplication(farm, viewItem, manureApplicationViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-2
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONLeachingFromLeftOverManureLeachingForField(Farm farm, CropViewItem viewItem)
        {
            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var manureNitrogenRemaining = GetManureNitrogenRemainingForField(viewItem, farm);

            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);
            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;

            var result = manureNitrogenRemaining * leachingFraction * leachingEmissionFactorForLandApplication;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-3
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalLeachingN2ONFromExportedManure(Farm farm, int year)
        {
            var leachingFraction = this.GetLeachingFraction(farm, year);
            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;
            var nitrogenFromExportedManure = this.ManureService.GetTotalNitrogenFromExportedManure(year, farm);

            var result = nitrogenFromExportedManure * leachingFraction * leachingEmissionFactorForLandApplication;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-4
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONLeachingFromField(Farm farm, int year, CropViewItem viewItem)
        {
            var totalLeachingForField = this.CalculateTotalN2ONFromManureLeachingForField(farm, viewItem);
            var totalLeachingEmissionsFromLeftOverManure = CalculateTotalN2ONLeachingFromLeftOverManureLeachingForField(farm, viewItem);

            var result = totalLeachingForField + totalLeachingEmissionsFromLeftOverManure;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-5
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONLeachingForFarmAndYear(Farm farm, int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                var leachingFromField = CalculateTotalN2ONLeachingFromField(farm, year, cropViewItem);
                result += leachingFromField;
            }

            result += CalculateTotalLeachingN2ONFromExportedManure(farm, year);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-6
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalNitrateLeached(Farm farm, CropViewItem viewItem, ManureItemBase manureItemBase)
        {
            var totalNitrogen = manureItemBase.AmountOfNitrogenAppliedPerHectare * viewItem.Area;

            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff; 
            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);

            var result = totalNitrogen * leachingFraction * (1 - leachingEmissionFactorForLandApplication);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-6
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalManureNitrateLeached(Farm farm, CropViewItem viewItem)
        {
            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var result = 0d;

            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                result += this.CalculateTotalNitrateLeached(farm, viewItem, manureApplicationViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-7
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalNitrateLeachedFromLeftOverManureForField(Farm farm, CropViewItem viewItem)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var totalNitrogenRemainingForField = this.GetManureNitrogenRemainingForField(viewItem, farm);

            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;
            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);

            var result = totalNitrogenRemainingForField * leachingFraction * (1 - leachingEmissionFactorForLandApplication);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-8
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalNitrateLeachedFromExportedManureForFarmAndYear(Farm farm, int year)
        {
            var exportedManureNitrogen = this.ManureService.GetTotalNitrogenFromExportedManure(year, farm);

            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;
            var leachingFraction = this.GetLeachingFraction(farm, year);

            var result = exportedManureNitrogen * leachingFraction * (1 - leachingEmissionFactorForLandApplication);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-9
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalManureNitrateLeachedFromForField(Farm farm, CropViewItem viewItem)
        {
            var result = 0d;

            var totalNitrateLeachedFromApplications = CalculateTotalManureNitrateLeached(farm, viewItem);
            var nitrateLeachedFromLeftOverManureForField = CalculateTotalNitrateLeachedFromLeftOverManureForField(farm, viewItem);

            result = totalNitrateLeachedFromApplications + nitrateLeachedFromLeftOverManureForField;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-10
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalNitrateLeachedFromForFarmAndYear(Farm farm, int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += this.CalculateTotalManureNitrateLeachedFromForField(farm, cropViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.5-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalIndirectEmissionsFromManureForField(
            Farm farm,
            CropViewItem viewItem,
            int year)
        {
            var nitrateLeached = CalculateTotalManureNitrateLeachedFromForField(farm, viewItem);
            var totalVolatilization = CalculateTotalManureN2ONVolatilizationForField(viewItem, farm, year);

            return nitrateLeached + totalVolatilization;
        }

        /// <summary>
        /// Equation 4.6.5-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalIndirectEmissionsFromManure(
            Farm farm,
            int year)
        {
            var nitrateLeached = CalculateTotalNitrateLeachedFromForFarmAndYear(farm, year);
            var totalVolatilization = CalculateTotalManureN2ONVolatilizationForFarmAndYear(farm, year);

            return nitrateLeached + totalVolatilization;
        }

        /// <summary>
        /// Equation 4.6.6-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectEmissionsFromManure(int year, Farm farm)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += this.CalculateDirectN2ONEmissionsFromFieldSpecificManureSpreadingForField(cropViewItem, farm);
                result += CalculateDirectN2ONFromLeftOverManureForField(farm, cropViewItem);
            }

            result += CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(farm, year);

            return result;
        }

        /// <summary>
        /// Equation 4.6.5-2
        /// </summary>
        public double CalculateTotalEmissionsFromManure(
            Farm farm,
            int year)
        {
            var direct = this.CalculateTotalDirectEmissionsFromManure(year, farm);
            var indirect = CalculateTotalIndirectEmissionsFromManure(farm, year);

            return indirect + direct;
        }

        /// <summary>
        /// Remaining nitrogen is spread evenly across all fields
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double GetManureNitrogenRemainingForField(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var totalAreaOfFarm = farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, viewItem.Year);
            var fractionOfAreaByThisField = viewItem.Area / totalAreaOfFarm;
            var manureNitrogenRemaining = this.ManureService.GetTotalNitrogenRemainingForFarmAndYear(viewItem.Year, farm);

            var amountOfNitrogenAssignedToThisField = fractionOfAreaByThisField * manureNitrogenRemaining;

            return amountOfNitrogenAssignedToThisField;
        }

        #endregion

        #region Private Methods

        private double GetEmissionFactorForVolatilization(Farm farm, int year)
        {
            var precipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, year);
            var evapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, year);
            var emissionFactorForVolatilization = this.LivestockEmissionConversionFactorsProvider.GetEmissionFactorForVolatilizationBasedOnClimate(precipitation, evapotranspiration);

            return emissionFactorForVolatilization;
        }

        private double GetEmissionFactorForLandApplication(
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var tillageType = cropViewItem.TillageType;
            var manureStateType = manureApplicationViewItem.ManureStateType;
            var manureApplicationMethod = manureApplicationViewItem.ManureApplicationMethod;

            return !manureStateType.IsLiquidManure() ? this.AnimalAmmoniaEmissionFactorProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(tillageType) : this.AnimalAmmoniaEmissionFactorProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(manureApplicationMethod);
        }

        private IEmissionData GetLandApplicationFactors(
            Farm farm,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var annualPrecipitation = this.ClimateProvider.GetAnnualPrecipitation(farm, manureApplicationViewItem.DateOfApplication);
            var evapotranspiration = this.ClimateProvider.GetAnnualEvapotranspiration(farm, manureApplicationViewItem.DateOfApplication);
            var animalType = manureApplicationViewItem.AnimalType;
            var year = manureApplicationViewItem.DateOfApplication.Year;

            var landApplicationFactors = this.LivestockEmissionConversionFactorsProvider.GetLandApplicationFactors(farm, annualPrecipitation, evapotranspiration, animalType, year);

            return landApplicationFactors;
        }

        private double GetAdjustedAmmoniaEmissionFactor(
            Farm farm, 
            CropViewItem cropViewItem,
            ManureApplicationViewItem manureApplicationViewItem)
        {
            var averageDailyTemperature = this.ClimateProvider.GetMeanTemperatureForDay(farm, manureApplicationViewItem.DateOfApplication);

            var adjustedAmmoniaEmissionFactor = CalculateAdjustedAmmoniaEmissionFactor(cropViewItem, manureApplicationViewItem, averageDailyTemperature);

            return adjustedAmmoniaEmissionFactor;
        }

        private double GetLeachingFraction(Farm farm, int year)
        {
            var growingSeasonPrecipitation = this.ClimateProvider.GetGrowingSeasonPrecipitation(farm, year);
            var growingSeasonEvapotranspiration = this.ClimateProvider.GetGrowingSeasonEvapotranspiration(farm, year);
            var leachingFraction = this.CalculateLeachingFraction(growingSeasonPrecipitation, growingSeasonEvapotranspiration);

            return leachingFraction;
        }

        #endregion
    }
}
