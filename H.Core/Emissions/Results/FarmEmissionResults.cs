using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using H.Core.Calculators.Infrastructure;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Providers.Climate;

namespace H.Core.Emissions.Results
{
    public class FarmEmissionResults : ResultsViewItemBase
    {
        public class DailyPrint
        {
            public string Component { get; set; }
            public string AnimalGroup { get; set; }
            public DateTime Date { get; set; }
            public double Temp { get; set; }
            public double ProteinIntake { get; set; }
            public double ProteinRetainedForPregnancy { get; set; }
            public double ProteinRetainedForLactation { get; set; }
            public double EmptyBodyWeight { get; set; }
            public double EmptyBodyGain { get; set; }
            public double RetainedEnergy { get; set; }
            public double ProteinRetainedForGain { get; set; }
            public double AverageTemparatureLast30DaysKelvin { get; set; }
            public double ClimateFactor { get; set; }
            public double FractionOfNitrogenExcretedInUrine { get; set; }
            public double TanExcretionRate { get; set; }
            public double TanExcretion { get; set; }
            public double NitrogenExcretionRate { get; set; }
            public double FecalNitrogenExcretionRate { get; set; }
            public double OrganicNitrogenCreatedOnDay { get; set; }
            public double AdjustedAmountOfTanInStoredManureOnDay { get; set; }
            public double FecalNitrogenExcretion { get; set; }
            public double AmountOfNitrogenAddedFromBedding { get; set; }
            public double ManureDirectN2ONEmission { get; set; }
            public double ManureN2ONLeachingEmission { get; set; }
            public double ManureNitrateLeachingEmission { get; set; }
            public double StartWeight { get; set; }
            public double EndWeight { get; set; }
            public double NEMaintenance { get; set; }
            public double NEactivity { get; set; }
            public double ADG { get; set; }
            public double NEgain { get; set; }
            public double REM { get; set; }
            public double REG { get; set; }
            public double GEI { get; set; }
            public double EntericRate { get; set; }
            public double EntericEmissions { get; set; }
            public double VolatileSolidsProduced { get; set; }
            public double VolatileSolidsLoaded { get; set; }
            public double VolatileSolidsConsumed { get; set; }
            public double ManureMethaneEmission { get; set; }
            public double ManureDirectN2ONEmissionRate { get; set; }
            public double AdjustedAmmoniaEmissionFactorForHousing { get; set; }
            public double AmmoniaEmissionRateFromHousing { get; set; }
            public double AmmoniaConcentrationInHousing { get; set; }
            public double AmmoniaEmissionsFromHousingSystem { get; set; }
            public double AdjustedNH3NFromHousing { get; set; }
            public double TanEnteringStorageSystem { get; set; }
            public double AdjustedAmountOfTanFlowingIntoStorageEachDay { get; set; }
            public double AmbientAirTemperatureAdjustmentForStorage { get; set; }
            public double AdjustedAmmoniaEmissionFactorForStorage { get; set; }
            public double AmmoniaEmissionsFromStorageSystem { get; set; }
            public double AccumulatedTanInStorageOnDay { get; set; }
            public double VolatilizationForStorage { get; set; }
            public double AmmoniaLossFromStorage { get; set; }
            public double AdjustedAmmoniaFromStorage { get; set; }
            public double ManureIndirectN2ONEmission { get; set; }
            public double AmountOfCarbonLostAsMethaneDuringManagement { get; set; }
            public double FecalCarbonExcretion { get; set; }
            public double CarbonAddedFromBeddingMaterial { get; set; }
            public double NonAccumulatedCarbonCreatedOnDay { get; set; }
            public double AccumulatedAmountOfCarbonInStoredManureOnDay { get; set; }
            public double AccumulatedTANAvailableForLandApplicationOnDay { get; set; }
            public double AccumulatedNitrogenAvailableForLandApplicationOnDay { get; set; }
            public double ManureCarbonNitrogenRatio { get; set; }
            public double TotalAmountOfNitrogenInStoredManureAvailableForDay { get; set; }
            public double TotalVolumeOfManureAvailableForLandApplication { get; set; }
            public double AccumulatedVolume { get; set; }
            public double VolatileSolidsAvailable { get; set; }
            public double NetEnergyForLactation { get; set; }
            public double NetEnergyForPregnancy { get; set; }
        }

        #region Fields

        private readonly EmissionTypeConverter _emissionTypeConverter = new EmissionTypeConverter();

        private Farm _farm;

        private ObservableCollection<AnimalComponentEmissionsResults> _animalComponentEmissionsResults;
        private ObservableCollection<EconomicsResultsViewItem> _economicsResultsViewItems;
        private ObservableCollection<CropViewItem> _finalFieldResultViewItems;
        private ObservableCollection<DigestorDailyOutput> _anaerobicDigestorResults;
        private ObservableCollection<ManureExportResultViewItem> _manureExportResultsViewItems;
        private ObservableCollection<CropResidueExportResultViewItem> _cropResidueExportResultsViewItems;

        private double _economicsProfit;

        #endregion

        #region Constructors

        public FarmEmissionResults()
        {
            this.AnimalComponentEmissionsResults = new ObservableCollection<AnimalComponentEmissionsResults>();
            this.EconomicResultsViewItems = new ObservableCollection<EconomicsResultsViewItem>();
            this.FinalFieldResultViewItems = new ObservableCollection<CropViewItem>();
            this.AnaerobicDigestorResults = new ObservableCollection<DigestorDailyOutput>();
            this.ManureExportResultsViewItems = new ObservableCollection<ManureExportResultViewItem>();
            this.CropResidueExportResultsViewItems = new ObservableCollection<CropResidueExportResultViewItem>();
        }

        #endregion

        #region Properties

        public int Year { get; set; }

        public double EconomicsProfit
        {
            get => _economicsProfit;
            set => SetProperty(ref _economicsProfit, value);
        }

        public ObservableCollection<EconomicsResultsViewItem> EconomicResultsViewItems
        {
            get => _economicsResultsViewItems;
            set => SetProperty(ref _economicsResultsViewItems, value);
        }

        public ObservableCollection<CropViewItem> FinalFieldResultViewItems
        {
            get => _finalFieldResultViewItems;
            set => SetProperty(ref _finalFieldResultViewItems, value);
        }

        /// <summary>
        /// Emission results from all animal components on the farm
        /// </summary>
        public ObservableCollection<AnimalComponentEmissionsResults> AnimalComponentEmissionsResults
        {
            get => _animalComponentEmissionsResults;
            set => SetProperty(ref _animalComponentEmissionsResults, value);
        }

        public Farm Farm
        {
            get => _farm;
            set => SetProperty(ref _farm, value);
        }

        /// <summary>
        /// The total emissions for the entire farm !!!
        ///
        /// (kg CO2e)
        /// </summary>
        public double TotalCarbonDioxideEquivalentsFromFarm
        {
            get
            {
                return this.TotalCarbonDioxideEquivalentsFromLandManagementForFarm + this.TotalCarbonDioxideEquivalentsFromAnimalsForFarm;
            }
        }

        public double TotalN2OAsCarbonDioxideEquivalentsFromAnimals
        {
            get
            {
                return this.AnimalComponentEmissionsResults.TotalNitrousOxideAsCarbonDioxideEquivalents();
            }
        }

        public double TotalOrganicNitrogenAvailableForLandApplication
        {
            get
            {
                return this.AnimalComponentEmissionsResults.TotalOrganicNitrogenAvailableForLandApplication();
            }
        }

        /// Equation 4.5.2-5
        ///
        /// Total available manure N available for land application from all animals on the farm.
        /// 
        /// (kg N)
        /// </summary>
        public double TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication
        {
            get
            {
                return this.AnimalComponentEmissionsResults.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication();
            }
        }

        /// <summary>
        /// Returns the total enteric methane produced from all animals on the farm
        ///
        /// (kg CH4)
        /// </summary>
        public double TotalEntericMethaneFromFarm
        {
            get
            {
                var result = this.AnimalComponentEmissionsResults.TotalEntericMethane();

                return result;
            }
        }

        /// <summary>
        /// Returns the total manure methane produced from all animals on the farm
        ///
        /// (kg CH4)
        /// </summary>
        public double TotalManureMethaneFromFarm
        {
            get
            {
                var results = this.AnimalComponentEmissionsResults.TotalManureMethane();

                return results;
            }
        }

        public double TotalCombinedCO2FromFarm
        {
            get
            {
                return this.TotalCarbonDioxideFromAnimals +
                       this.TotalCO2FromFarm +
                       this.TotalEnergyCarbonDioxideFromManureSpreading;
            }
        }
        /// <summary>
        /// Returns the sum of all CO2e from land management of the farm.
        ///
        /// <remarks>Does not include upstream emissions from herbicide and fertilizer production.</remarks>
        ///
        /// (kg CO2e)
        /// </summary>
        public double TotalCarbonDioxideEquivalentsFromLandManagementForFarm
        {
            get
            {
                var result = this.TotalOnFarmCO2FromLandManagement * CoreConstants.CO2ToCO2eConversionFactor +
                             this.TotalN2OEmissionsFromLandManagement * CoreConstants.N2OToCO2eConversionFactor;

                return result;
            }
        }

        /// <summary>
        /// Returns the sum of all CO2e from animals on the farm.
        ///
        /// (kg CO2e)
        /// </summary>
        public double TotalCarbonDioxideEquivalentsFromAnimalsForFarm
        {
            get
            {
                return _emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, this.TotalEntericMethaneFromFarm) +
                       _emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, this.TotalManureMethaneFromFarm) +
                       _emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, this.TotalDirectNitrousOxideFromAnimals) +
                       _emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, this.TotalIndirectNitrousOxideFromAnimals) +
                       _emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, EmissionDisplayUnits.KilogramsC02e, this.TotalCarbonDioxideFromAnimals);
            }
        }

        public double TotalLandArea
        {
            get
            {
                return this.GetAllCropResultsByYear(this.Year).Sum(x => x.Area);
            }
        }

        public double TotalHarvest
        {
            get
            {
                var result = 0.0;

                foreach (var cropViewItem in this.GetAllCropResultsByYear(this.Year))
                {
                    result += cropViewItem.EstimatesOfProductionResultsViewItem.Harvest;
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the sum of direct and indirect N2O emissions for the farm.
        ///
        /// (kg N2O)
        /// </summary>
        public double TotalN2OEmissionsFromLandManagement
        {
            get
            {
                return this.TotalDirectN2OEmissionsFromLandManagement + this.TotalIndirectN2OEmissionsFromLandManagement;
            }
        }

        public double TotalNitrousOxideEmissionFromLandManagementAsCarbonDioxideEquivalents
        {
            get
            {
                return this.TotalN2OEmissionsFromLandManagement * CoreConstants.N2OToCO2eConversionFactor;
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalDirectNitrousOxideFromFarm
        {
            get
            {
                return this.TotalDirectN2OEmissionsFromLandManagement + this.TotalDirectNitrousOxideFromAnimals;
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectNitrousOxideFromFarm
        {
            get
            {
                return this.TotalIndirectN2OEmissionsFromLandManagement + this.TotalIndirectNitrousOxideFromAnimals;
            }
        }
        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalCO2FromFarm
        {
            get
            {
                return this.TotalOnFarmCO2FromLandManagement;
                // Animal CO2 total is output under the Energy CO2 column (in the GUI and CLI reports) so don't include it here)
                //this.TotalCarbonDioxideFromAnimals;
            }
        }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalEnergyCarbonDioxideFromFarm
        {
            get
            {
                return this.TotalOnFarmCO2FromLandManagement +
                       this.TotalCarbonDioxideFromAnimals;
            }
        }

        /// <summary>
        /// Note: for animals, the total CO2 is equal to the total energy CO2
        /// 
        /// (kg CO2)
        /// </summary>
        public double TotalCarbonDioxideFromAnimals
        {
            get
            {
                return this.AnimalComponentEmissionsResults.TotalCarbonDioxide();
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalDirectNitrousOxideFromAnimals
        {
            get
            {
                return this.AnimalComponentEmissionsResults.TotalDirectNitrousOxide();
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectNitrousOxideFromAnimals
        {
            get
            {
                return this.AnimalComponentEmissionsResults.TotalIndirectNitrousOxide();

            }
        }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalOnFarmCO2FromLandManagement
        {
            get
            {
                return this.GetAllCropResultsByYear(this.Year).Sum(x => x.CropEnergyResults.TotalOnFarmCroppingEnergyEmissions);
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalDirectN2OEmissionsFromLandManagement
        {
            get
            {
                return this.GetAllCropResultsByYear(this.Year).Sum(x => x.TotalDirectNitrousOxidePerHectare * x.Area);
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectN2OEmissionsFromLandManagement
        {
            get
            {
                return this.GetAllCropResultsByYear(this.Year).Sum(x => x.TotalIndirectNitrousOxidePerHectare * x.Area);
            }
        }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalEnergyCarbonDioxideFromManureSpreading
        {
            get
            {
                return this.GetAllCropResultsByYear(this.Year).Sum(x => x.CropEnergyResults.EnergyCarbonDioxideFromManureSpreading);
            }
        }

        public ObservableCollection<DigestorDailyOutput> AnaerobicDigestorResults
        {
            get => _anaerobicDigestorResults;
            set => SetProperty(ref _anaerobicDigestorResults, value);
        }

        public ObservableCollection<ManureExportResultViewItem> ManureExportResultsViewItems
        {
            get => _manureExportResultsViewItems;
            set => SetProperty(ref _manureExportResultsViewItems, value);
        }

        public ObservableCollection<CropResidueExportResultViewItem> CropResidueExportResultsViewItems
        {
            get => _cropResidueExportResultsViewItems;
            set => SetProperty(ref _cropResidueExportResultsViewItems, value);
        }

        #endregion

        #region Public Methods

        public List<DailyPrint> GetDailyPrint()
        {
            var result = new List<DailyPrint>();

            foreach (var animalResult in this.AnimalComponentEmissionsResults)
            {
                foreach (var resultsForAllGroups in animalResult.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in resultsForAllGroups.GroupEmissionsByMonths)
                    {
                        foreach (var groupEmissionsByDay in groupEmissionsByMonth.DailyEmissions)
                        {
                            var a = new DailyPrint
                            {
                                Component = animalResult.Component.Name,
                                AnimalGroup = resultsForAllGroups.AnimalGroup.Name,
                                Date = groupEmissionsByDay.DateTime,
                                Temp = groupEmissionsByDay.Temperature,
                                AverageTemparatureLast30DaysKelvin =
                                    groupEmissionsByDay.AverageTemperatureOverLast30Days,
                                ClimateFactor = groupEmissionsByDay.ClimateFactor,
                                StartWeight = groupEmissionsByDay.AnimalWeight,
                                NEMaintenance = groupEmissionsByDay.NetEnergyForMaintenance,
                                NEactivity = groupEmissionsByDay.NetEnergyForActivity,
                                NetEnergyForLactation = groupEmissionsByDay.NetEnergyForLactation,
                                NetEnergyForPregnancy = groupEmissionsByDay.NetEnergyForPregnancy,
                                ADG = groupEmissionsByDay.AverageDailyGain,
                                NEgain = groupEmissionsByDay.NetEnergyForGain,
                                REM = groupEmissionsByDay.RatioOfEnergyAvailableForMaintenance,
                                REG = groupEmissionsByDay.RatioOfEnergyAvailableForGain,
                                GEI = groupEmissionsByDay.GrossEnergyIntake,
                                EntericRate = groupEmissionsByDay.EntericMethaneEmissionRate,
                                EntericEmissions = groupEmissionsByDay.EntericMethaneEmission,
                                ProteinIntake = groupEmissionsByDay.ProteinIntake,
                                ProteinRetainedForPregnancy = groupEmissionsByDay.ProteinRetainedForPregnancy,
                                ProteinRetainedForLactation = groupEmissionsByDay.ProteinRetainedForLactation,
                                EmptyBodyWeight = groupEmissionsByDay.EmptyBodyWeight,
                                EmptyBodyGain = groupEmissionsByDay.EmptyBodyGain,
                                RetainedEnergy = groupEmissionsByDay.RetainedEnergy,
                                ProteinRetainedForGain = groupEmissionsByDay.ProteinRetainedForGain,
                                FractionOfNitrogenExcretedInUrine = groupEmissionsByDay.FractionOfNitrogenExcretedInUrine,
                                TanExcretionRate = groupEmissionsByDay.TanExcretionRate,
                                TanExcretion = groupEmissionsByDay.TanExcretion,
                                NitrogenExcretionRate = groupEmissionsByDay.NitrogenExcretionRate,
                                FecalNitrogenExcretionRate = groupEmissionsByDay.FecalNitrogenExcretionRate,
                                OrganicNitrogenCreatedOnDay = groupEmissionsByDay.OrganicNitrogenCreatedOnDay,
                                AdjustedAmountOfTanInStoredManureOnDay = groupEmissionsByDay.AdjustedAmountOfTanInStoredManureOnDay,
                                FecalNitrogenExcretion = groupEmissionsByDay.FecalNitrogenExcretion,
                                AmountOfNitrogenAddedFromBedding = groupEmissionsByDay.AmountOfNitrogenAddedFromBedding,
                                ManureDirectN2ONEmission = groupEmissionsByDay.ManureDirectN2ONEmission,
                                ManureN2ONLeachingEmission = groupEmissionsByDay.ManureN2ONLeachingEmission,
                                ManureNitrateLeachingEmission = groupEmissionsByDay.ManureNitrateLeachingEmission,
                                VolatileSolidsProduced = groupEmissionsByDay.VolatileSolidsProduced,
                                VolatileSolidsLoaded = groupEmissionsByDay.VolatileSolidsLoaded,
                                VolatileSolidsConsumed = groupEmissionsByDay.VolatileSolidsConsumed,
                                VolatileSolidsAvailable = groupEmissionsByDay.VolatileSolidsAvailable,
                                ManureMethaneEmission = groupEmissionsByDay.ManureMethaneEmission,
                                ManureDirectN2ONEmissionRate = groupEmissionsByDay.ManureDirectN2ONEmissionRate,
                                AdjustedAmmoniaEmissionFactorForHousing = groupEmissionsByDay.AdjustedAmmoniaEmissionFactorForHousing,
                                AmmoniaEmissionRateFromHousing = groupEmissionsByDay.AmmoniaEmissionRateFromHousing,
                                AmmoniaConcentrationInHousing = groupEmissionsByDay.AmmoniaConcentrationInHousing,
                                AmmoniaEmissionsFromHousingSystem = groupEmissionsByDay.AmmoniaEmissionsFromHousingSystem,
                                AdjustedNH3NFromHousing = groupEmissionsByDay.AdjustedNH3NFromHousing,
                                TanEnteringStorageSystem = groupEmissionsByDay.TanEnteringStorageSystem,
                                AdjustedAmountOfTanFlowingIntoStorageEachDay = groupEmissionsByDay.AdjustedAmountOfTanFlowingIntoStorageEachDay,
                                AmbientAirTemperatureAdjustmentForStorage = groupEmissionsByDay.AmbientAirTemperatureAdjustmentForStorage,
                                AdjustedAmmoniaEmissionFactorForStorage = groupEmissionsByDay.AdjustedAmmoniaEmissionFactorForStorage,
                                AmmoniaEmissionsFromStorageSystem = groupEmissionsByDay.AmmoniaEmissionsFromStorageSystem,
                                AccumulatedTanInStorageOnDay = groupEmissionsByDay.AccumulatedTanInStorageOnDay,
                                VolatilizationForStorage = groupEmissionsByDay.VolatilizationForStorage,
                                AmmoniaLossFromStorage = groupEmissionsByDay.AmmoniaLossFromStorage,
                                AdjustedAmmoniaFromStorage = groupEmissionsByDay.AdjustedAmmoniaFromStorage,
                                ManureIndirectN2ONEmission = groupEmissionsByDay.ManureIndirectN2ONEmission,
                                AmountOfCarbonLostAsMethaneDuringManagement = groupEmissionsByDay.AmountOfCarbonLostAsMethaneDuringManagement,
                                FecalCarbonExcretion = groupEmissionsByDay.FecalCarbonExcretion,
                                CarbonAddedFromBeddingMaterial = groupEmissionsByDay.CarbonAddedFromBeddingMaterial,
                                NonAccumulatedCarbonCreatedOnDay = groupEmissionsByDay.NonAccumulatedCarbonCreatedOnDay,
                                AccumulatedAmountOfCarbonInStoredManureOnDay = groupEmissionsByDay.AccumulatedAmountOfCarbonInStoredManureOnDay,
                                AccumulatedTANAvailableForLandApplicationOnDay = groupEmissionsByDay.AccumulatedTANAvailableForLandApplicationOnDay,
                                AccumulatedNitrogenAvailableForLandApplicationOnDay = groupEmissionsByDay.AccumulatedNitrogenAvailableForLandApplicationOnDay,
                                ManureCarbonNitrogenRatio = groupEmissionsByDay.ManureCarbonNitrogenRatio,
                                TotalAmountOfNitrogenInStoredManureAvailableForDay = groupEmissionsByDay.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                                TotalVolumeOfManureAvailableForLandApplication = groupEmissionsByDay.TotalVolumeOfManureAvailableForLandApplication,
                                AccumulatedVolume = groupEmissionsByDay.AccumulatedVolume,
                            }; 

                            result.Add(a);
                        }
                    }
                }
            }

            return result;
        }

        public List<GroupEmissionsByDay> GetAllDailyResultsByYear(int year)
        {
            var result = this.GetAllDailyResults();

            var byYear = result.Where(x => x.DateTime.Year == year).ToList();

            return byYear;
        }

        public List<GroupEmissionsByDay> GetAllDailyResults()
        {
            var result = new List<GroupEmissionsByDay>();

            foreach (var animalComponentEmissionsResult in this.AnimalComponentEmissionsResults)
            {
                var results = animalComponentEmissionsResult.GetDailyEmissions();
                results.AddRange(results);
            }

            return result;
        }

        public List<CropViewItem> GetCropResultsByField(FieldSystemComponent fieldSystemComponent)
        {
            return this.FinalFieldResultViewItems.Where(x => x.FieldSystemComponentGuid.Equals(fieldSystemComponent.Guid)).ToList();
        }

        #endregion

        #region Private Methods

        private List<CropViewItem> GetAllCropResultsByYear(int year)
        {
            return this.FinalFieldResultViewItems.Where(x => x.Year == year).ToList();
        }

        #endregion
    }
}