using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        #region Fields

        private Table_65_Global_Warming_Emissions_Potential_Provider _globalWarmingEmissionsPotentialProvider = new Table_65_Global_Warming_Emissions_Potential_Provider();

        private readonly EmissionTypeConverter _emissionTypeConverter = new EmissionTypeConverter();

        private Farm _farm;

        private SoilN2OEmissionsResults _mineralN2OEmissionsResults;
        private SoilN2OEmissionsResults _manureN2OEmissionResults;

        private ObservableCollection<FieldComponentEmissionResults> _fieldComponentEmissionResults;
        private ObservableCollection<AnimalComponentEmissionsResults> _animalComponentEmissionsResults;

        private FarmEnergyResults _farmEnergyResults;

        private ObservableCollection<EconomicsResultsViewItem> _economicsResultsViewItems;
        private ObservableCollection<CropViewItem> _finalFieldResultViewItems;

        private ObservableCollection<ManureTank> _manureTanks;

        private double _economicsProfit;

        #endregion

        #region Constructors

        public FarmEmissionResults()
        {
            this.ManureN2OEmissionResults = new SoilN2OEmissionsResults();
            this.MineralN2OEmissionsResults = new SoilN2OEmissionsResults();
            this.FarmEnergyResults = new FarmEnergyResults();

            this.FieldComponentEmissionResults = new ObservableCollection<FieldComponentEmissionResults>();
            this.AnimalComponentEmissionsResults = new ObservableCollection<AnimalComponentEmissionsResults>();
            this.EconomicResultsViewItems = new ObservableCollection<EconomicsResultsViewItem>();
            this.FinalFieldResultViewItems = new ObservableCollection<CropViewItem>();

            this.ManureTanks = new ObservableCollection<ManureTank>()
            {
                new ManureTank() {AnimalType = AnimalType.Beef , Year = DateTime.Now.Year},
                new ManureTank() {AnimalType = AnimalType.Dairy  , Year = DateTime.Now.Year},
                new ManureTank() {AnimalType = AnimalType.Swine , Year = DateTime.Now.Year},
                new ManureTank() {AnimalType = AnimalType.Sheep , Year = DateTime.Now.Year},
                new ManureTank() {AnimalType = AnimalType.Poultry , Year = DateTime.Now.Year},
                new ManureTank() {AnimalType = AnimalType.OtherLivestock , Year = DateTime.Now.Year},
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// A collection of manure tanks where manure from each type of animal is stored. Each tank is then used for land applications.
        /// </summary>
        public ObservableCollection<ManureTank> ManureTanks
        {
            get => _manureTanks;
            set => SetProperty(ref _manureTanks, value);
        }

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

        public ObservableCollection<FieldComponentEmissionResults> FieldComponentEmissionResults
        {
            get => _fieldComponentEmissionResults;
            set => SetProperty(ref _fieldComponentEmissionResults, value);
        }

        /// <summary>
        /// Emission results from all animal components on the farm
        /// </summary>
        public ObservableCollection<AnimalComponentEmissionsResults> AnimalComponentEmissionsResults
        {
            get => _animalComponentEmissionsResults;
            set => SetProperty(ref _animalComponentEmissionsResults, value);
        }

        public SoilN2OEmissionsResults MineralN2OEmissionsResults
        {
            get => _mineralN2OEmissionsResults;
            set => SetProperty(ref _mineralN2OEmissionsResults, value);
        }

        /// <summary>
        /// Results calculated as if all manure was applied to field
        /// </summary>
        public SoilN2OEmissionsResults ManureN2OEmissionResults
        {
            get => _manureN2OEmissionResults;
            set => SetProperty(ref _manureN2OEmissionResults, value);
        }

        public FarmEnergyResults FarmEnergyResults
        {
            get => _farmEnergyResults;
            set => SetProperty(ref _farmEnergyResults, value);
        }

        public Farm Farm
        {
            get => _farm;
            set => SetProperty(ref _farm, value);
        }

        /// <summary>
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

        public double TotalEnergyEmissionsAsCarbonDioxideEquivalents
        {
            get
            {
                return this.FarmEnergyResults.TotalCroppingEnergyEmissionsForFarmAsCarbonDioxideEquivalents;
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

        public double TotalCombinedCarbonDioxideFromFarm
        {
            get
            {
                return this.TotalCarbonDioxideFromAnimals +
                       this.TotalCarbonDioxideFromFarm +
                       this.FarmEnergyResults.EnergyCarbonDioxideFromManureApplication;
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
                var result = this.FarmEnergyResults.TotalOnFarmCroppingEnergyEmissionsForFarm * CoreConstants.CO2ToCO2eConversionFactor +
                             this.TotalNitrousOxideEmissionsFromLandManagement * CoreConstants.N2OToCO2eConversionFactor;

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
                return this.FieldComponentEmissionResults.Sum(results => results.FieldSystemComponent.FieldArea);
            }
        }

        public double TotalHarvest
        {
            get
            {
                var result = 0.0;

                foreach (var fieldComponentEmissionResult in this.FieldComponentEmissionResults)
                {
                    foreach (var estimatesOfProductionResultsViewItem in fieldComponentEmissionResult.HarvestViewItems)
                    {
                        result += estimatesOfProductionResultsViewItem.Harvest;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 2.5 Single-year calculation of nitrous oxide
        /// </summary>
        public List<SoilN2OEmissionsResults> LandManagementN2OEmissionResults
        {
            get
            {
                var result = new List<SoilN2OEmissionsResults>();

                foreach (var fieldComponentEmissionResult in this.FieldComponentEmissionResults)
                {
                    result.Add(fieldComponentEmissionResult.CropN2OEmissionsResults);
                }

                result.Add(this.ManureN2OEmissionResults);
                result.Add(this.MineralN2OEmissionsResults);

                return result;
            }
        }

        /// <summary>
        /// Returns the sum of direct and indirect N2O emissions for the farm.
        /// </summary>
        public double TotalNitrousOxideEmissionsFromLandManagement
        {
            get
            {
                return this.TotalDirectNitrousOxideEmissionsFromLandManagement + this.TotalIndirectNitrousOxideEmissionsFromLandManagement;
            }
        }

        public double TotalNitrousOxideEmissionFromLandManagementAsCarbonDioxideEquivalents
        {
            get
            {
                return this.TotalNitrousOxideEmissionsFromLandManagement * CoreConstants.N2OToCO2eConversionFactor;
            }
        }

        public double TotalDirectNitrousOxideEmissionsFromLandManagement
        {
            get
            {
                return this.LandManagementN2OEmissionResults.Sum(emissions => emissions.DirectN2OEmissions);
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalDirectNitrousOxideFromFarm
        {
            get
            {
                return this.TotalDirectNitrousOxideEmissionsFromLandManagement + this.TotalDirectNitrousOxideFromAnimals;
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectNitrousOxideFromFarm
        {
            get
            {
                return this.TotalIndirectNitrousOxideEmissionsFromLandManagement + this.TotalIndirectNitrousOxideFromAnimals;
            }
        }
        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalCarbonDioxideFromFarm
        {
            get
            {
                return this.FarmEnergyResults.TotalOnFarmCroppingEnergyEmissionsForFarm;
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
                return this.FarmEnergyResults.TotalOnFarmCroppingEnergyEmissionsForFarm +
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
                var result = this.AnimalComponentEmissionsResults.TotalCarbonDioxide();

                return result;
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalDirectNitrousOxideFromAnimals
        {
            get
            {
                var result = this.AnimalComponentEmissionsResults.TotalDirectNitrousOxide();

                return result;
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectNitrousOxideFromAnimals
        {
            get
            {
                var result = this.AnimalComponentEmissionsResults.TotalIndirectNitrousOxide();

                return result;
            }
        }

        public double TotalIndirectNitrousOxideEmissionsFromLandManagement
        {
            get
            {
                return this.LandManagementN2OEmissionResults.Sum(emissions => emissions.IndirectN2OEmissions);
            }
        }

        #endregion

        #region Public Methods

        public FarmResultsByYear GetResultsByYear(int year)
        {
            var co2ToCo2eConversionByYear = _globalWarmingEmissionsPotentialProvider.GetGlobalWarmingEmissionsInstance(year, EmissionTypes.CO2);
            var co2ToCo2eConversion = co2ToCo2eConversionByYear.GlobalWarmingPotentialValue;

            var result = new FarmResultsByYear()
            {
                Year = year,
            };

            var cropResultsByYear = this.GetAllCropResultsByYear(year);

            var totalOnFarmCO2FromEnergyEmissions = cropResultsByYear.Sum(x => x.CropEnergyResults.TotalOnFarmCroppingEnergyEmissions);
            var totalDirectN2OFromCrops = cropResultsByYear.Sum(x => x.TotalDirectNitrousOxidePerHectare * x.Area);
            var totalIndirectN2OFromCrops = cropResultsByYear.Sum(x => x.TotalIndirectNitrousOxidePerHectare * x.Area);

            result.TotalEnergyCarbonDioxideFromManureSpreading = cropResultsByYear.Sum(x => x.CropEnergyResults.EnergyCarbonDioxideFromManureSpreading);

            result.TotalEmissions = totalOnFarmCO2FromEnergyEmissions * co2ToCo2eConversion;

            return result;
        }

        public ManureTank GetManureTankByAnimalType(AnimalType animalType, int year)
        {
            var tank = this.ManureTanks.SingleOrDefault(x => x.AnimalType.GetCategory() == animalType.GetCategory() && x.Year == year);
            if (tank == null)
            {
                // If no tank exists for this year, create one now
                tank = new ManureTank() {AnimalType = animalType, Year = year};

                this.ManureTanks.Add(tank);
            }

            return tank;
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