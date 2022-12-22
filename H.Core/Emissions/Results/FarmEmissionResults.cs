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
        #region Fields

        private readonly EmissionTypeConverter _emissionTypeConverter = new EmissionTypeConverter();

        private Farm _farm;

        private ObservableCollection<AnimalComponentEmissionsResults> _animalComponentEmissionsResults;
        private ObservableCollection<EconomicsResultsViewItem> _economicsResultsViewItems;
        private ObservableCollection<CropViewItem> _finalFieldResultViewItems;
        private ObservableCollection<DigestorDailyOutput> _anaerobicDigestorResults;

        private double _economicsProfit;

        #endregion

        #region Constructors

        public FarmEmissionResults()
        {
            this.AnimalComponentEmissionsResults = new ObservableCollection<AnimalComponentEmissionsResults>();
            this.EconomicResultsViewItems = new ObservableCollection<EconomicsResultsViewItem>();
            this.FinalFieldResultViewItems = new ObservableCollection<CropViewItem>();
            this.AnaerobicDigestorResults = new ObservableCollection<DigestorDailyOutput>();
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

        #endregion

        #region Public Methods

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