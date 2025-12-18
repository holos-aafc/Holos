using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Economics
{
    public class CropEconomicData : ModelBase
    {
        #region Fields

        private double _expectedMarketPrice;
        private double _expectedYieldPerAcre;
        private double _seedCleaningAndTreatment;
        private double _fertilizer;
        private double _chemical;
        private double _fuelOilLube;
        private double _machineryRepairs;
        private double _hailCropInsurance;
        private double _truckingMarketing;
        private double _buildingRepairs;
        private double _customWork;
        private double _labour;
        private double _utilities;
        private double _operatingInterest;
        private double _cropSalesPerAcre;
        private double _pumpingCosts;
        private double _totalCost;
        private double _contributionMargin;
        private double _totalCostPerUnit;
        private double _breakEvenYield;
        private Province _province;
        private CropType _cropType;
        private CropType _viewItemCropType;
        private SoilFunctionalCategory _soilSoilFunctionalCategory;
        private string _fertilizerBlend;
        private string _dataSourceUrl;
        private EconomicMeasurementUnits _unit;
        private double _herbicideCost;
        private double _nitrogenCostPerTonne;
        private double _phosphorusCostPerTonne;
        private double _totalVariableCostPerUnit;
        private double _totalFixedCostPerUnit;
        private bool _isConverted;
        private bool _isUserDefined;
        private bool _fixedCostHandled;

        //constants are in $/acre
        private const double AcresPerHectare = 2.4711;
        private const double AlbertaBlackForageFixedCost = 107.53;
        private const double AlbertaBrownForageFixedCost = 80.87;
        private const double AlbertaDarkBrownForageFixedCost = 73.06;

        private const double AlbertaBlackCropFixedCost = 154.98;
        private const double AlbertaBrownCropFixedCost = 91.99;
        private const double AlbertaDarkBrownCropFixedCost = 98.13;

        private const double ManitobaAvgCropFixedCost = 138.66;

        private const double SaskBrownAvgFixedCost = 95.98;
        private const double SaskDarkBrownAvgFixedCost = 110.27;
        private const double SaskBlackAvgFixedCost = 126.51;

        #endregion

        #region Constructors

        public CropEconomicData()
        {
            this.FixedCostHandled = false;


            //this.PropertiesUserCanUpdate = new List<string>()
            //{
            //    nameof(ExpectedMarketPrice),
            //}.Concat(this.VariableProperties).ToList();

            this.PropertyChanged -= OnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        public string DataSourceUrl
        {
            get => _dataSourceUrl;
            set => SetProperty(ref _dataSourceUrl, value);
        }

        public bool FixedCostHandled
        {
            get => _fixedCostHandled;
            set => SetProperty(ref _fixedCostHandled, value);
        }

        public bool IsConverted
        {
            get => _isConverted;
            set => SetProperty(ref _isConverted, value);
        }

        /// <summary>
        /// bu, lb, or t the unit associated with <see cref="ExpectedMarketPrice"/> <see cref="ExpectedYieldPerAcre"/> <see cref="BreakEvenYield"/>
        /// all other costs are $/acre
        /// </summary>
        public EconomicMeasurementUnits Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }
        public double CropSalesPerAcre
        {
            get => _cropSalesPerAcre;
            set => SetProperty(ref _cropSalesPerAcre, value);
        }
        public string FertilizerBlend
        {
            get => _fertilizerBlend;
            set => SetProperty(ref _fertilizerBlend, value);
        }
        public double PumpingCosts
        {
            get => _pumpingCosts;
            set => SetProperty(ref _pumpingCosts, value);
        }
        public double TotalCost
        {
            get => _totalCost;
            set => SetProperty(ref _totalCost, value);
        }
        /// <summary>
        /// amount a particular crop contributes to fixed costs and return to management and equity.
        /// </summary>
        public double ContributionMargin
        {
            get => _contributionMargin;
            set => SetProperty(ref _contributionMargin, value);
        }
        public double TotalCostPerUnit
        {
            get => _totalCostPerUnit;
            set => SetProperty(ref _totalCostPerUnit, value);
        }
        public double BreakEvenYield
        {
            get => _breakEvenYield;
            set => SetProperty(ref _breakEvenYield, value);
        }

        /// <summary>
        /// Dollars per unit (lb, bu, t)
        /// </summary>
        public double ExpectedMarketPrice
        {
            get => _expectedMarketPrice;
            set => SetProperty(ref _expectedMarketPrice, value);
        }

        /// <summary>
        /// The CropType that we have economic data for on file
        /// </summary>
        public CropType CropType
        {
            get => _cropType;
            set => SetProperty(ref _cropType, value);
        }

        /// <summary>
        /// The CropType of the encapsulating CropViewItem.
        /// </summary>
        public CropType ViewItemCropType
        {
            get => _viewItemCropType;
            set => SetProperty(ref _viewItemCropType, value);
        }

        public string CropTypeString => _cropType.GetDescription();

        public SoilFunctionalCategory SoilFunctionalCategory
        {
            get => _soilSoilFunctionalCategory;
            set => SetProperty(ref _soilSoilFunctionalCategory, value);
        }

        public double ExpectedYieldPerAcre
        {
            get => _expectedYieldPerAcre;
            set => SetProperty(ref _expectedYieldPerAcre, value);
        }

        public double SeedCleaningAndTreatment
        {
            get => _seedCleaningAndTreatment;
            set => SetProperty(ref _seedCleaningAndTreatment, value);
        }

        public double Fertilizer
        {
            get => _fertilizer;
            set => SetProperty(ref _fertilizer, value);
        }

        public double Chemical
        {
            get => _chemical;
            set => SetProperty(ref _chemical, value);
        }

        public double FuelOilLube
        {
            get => _fuelOilLube;
            set => SetProperty(ref _fuelOilLube, value);
        }

        public double MachineryRepairs
        {
            get => _machineryRepairs;
            set => SetProperty(ref _machineryRepairs, value);
        }

        public double HailCropInsurance
        {
            get => _hailCropInsurance;
            set => SetProperty(ref _hailCropInsurance, value);
        }

        public double TruckingMarketing
        {
            get => _truckingMarketing;
            set => SetProperty(ref _truckingMarketing, value);
        }

        public double BuildingRepairs
        {
            get => _buildingRepairs;
            set => SetProperty(ref _buildingRepairs, value);
        }

        public double CustomWork
        {
            get => _customWork;
            set => SetProperty(ref _customWork, value);
        }

        public double Labour
        {
            get => _labour;
            set => SetProperty(ref _labour, value);
        }

        public double Utilities
        {
            get => _utilities;
            set => SetProperty(ref _utilities, value);
        }

        public double OperatingInterest
        {
            get => _operatingInterest;
            set => SetProperty(ref _operatingInterest, value);
        }
        public Province Province
        {
            get => _province;
            set => SetProperty(ref _province, value);
        }

        public double NitrogenCostPerTonne
        {
            get => _nitrogenCostPerTonne;
            set => SetProperty(ref _nitrogenCostPerTonne, value);
        }

        public double PhosphorusCostPerTonne
        {
            get => _phosphorusCostPerTonne;
            set => SetProperty(ref _phosphorusCostPerTonne, value);
        }

        public double HerbicideCost
        {
            get => _herbicideCost;
            set => SetProperty(ref _herbicideCost, value);
        }

        public double TotalFixedCostPerUnit
        {
            get => _totalFixedCostPerUnit;
            set => SetProperty(ref _totalFixedCostPerUnit, value);
        }

        public double TotalVariableCostPerUnit
        {
            get => _totalVariableCostPerUnit;
            set => SetProperty(ref _totalVariableCostPerUnit, value);
        }

        public List<string> PropertiesUserCanUpdate { get; }

        

        public bool IsUserDefined
        {
            get => _isUserDefined;
            set => SetProperty(ref _isUserDefined, value);
        }

        #endregion

        #region Public Methods

        public void SetUserDefinedVariableCostPerUnit()
        {
            var variableProperties = new List<string>()
            {
                nameof(SeedCleaningAndTreatment),
                nameof(Fertilizer),
                nameof(Chemical),
                nameof(HailCropInsurance),
                nameof(TruckingMarketing),
                nameof(FuelOilLube),
                nameof(MachineryRepairs),
                nameof(BuildingRepairs),
                nameof(CustomWork),
                nameof(Labour),
                nameof(Utilities),
                nameof(OperatingInterest),
            };

            var props = GetType().GetProperties().Where(propInfo => variableProperties.Contains(propInfo.Name));
            var numbers = props.Select(prop => (double)prop.GetValue(this));
            var sum = numbers.Sum();
            this.TotalVariableCostPerUnit = sum;
        }

        public void SetUserDefinedFixedCostPerUnit(MeasurementSystemType measurementSystem)
        {
            switch (Province)
            {
                case Province.Alberta:
                    switch (this.SoilFunctionalCategory)
                    {
                        case SoilFunctionalCategory.Black when this.ViewItemCropType.IsAnnual() && !this.ViewItemCropType.IsSilageCrop():
                            this.SetFixedCostByMeasurementSystem(measurementSystem, AlbertaBlackCropFixedCost);
                            break;
                        case SoilFunctionalCategory.Black:
                            this.SetFixedCostByMeasurementSystem(measurementSystem, AlbertaBlackForageFixedCost);
                            break;
                        case SoilFunctionalCategory.Brown when this.ViewItemCropType.IsAnnual() && !this.ViewItemCropType.IsSilageCrop():
                            this.SetFixedCostByMeasurementSystem(measurementSystem, AlbertaBrownCropFixedCost);
                            break;
                        case SoilFunctionalCategory.Brown:
                            this.SetFixedCostByMeasurementSystem(measurementSystem, AlbertaBrownForageFixedCost);
                            break;
                        case SoilFunctionalCategory.DarkBrown when this.ViewItemCropType.IsAnnual() && !this.ViewItemCropType.IsSilageCrop():
                            this.SetFixedCostByMeasurementSystem(measurementSystem, AlbertaDarkBrownCropFixedCost);
                            break;
                        case SoilFunctionalCategory.DarkBrown:
                            this.SetFixedCostByMeasurementSystem(measurementSystem, AlbertaDarkBrownForageFixedCost);
                            break;

                        default:
                            Trace.TraceError($"{nameof(CropEconomicData)}.{nameof(SetUserDefinedFixedCostPerUnit)}: '{this.SoilFunctionalCategory}' not handled for {this.Province}. Setting TotalFixed cost to 0.");
                            break;
                    }

                    break;

                case Province.Manitoba:
                    this.SetFixedCostByMeasurementSystem(measurementSystem, ManitobaAvgCropFixedCost);
                    break;

                case Province.Saskatchewan:
                    switch (SoilFunctionalCategory)
                    {
                        case SoilFunctionalCategory.Black:
                            this.SetFixedCostByMeasurementSystem(measurementSystem, SaskBlackAvgFixedCost);
                            break;
                        case SoilFunctionalCategory.Brown:
                            this.SetFixedCostByMeasurementSystem(measurementSystem, SaskBrownAvgFixedCost);
                            break;
                        case SoilFunctionalCategory.DarkBrown:
                            this.SetFixedCostByMeasurementSystem(measurementSystem, SaskDarkBrownAvgFixedCost);
                            break;

                        default:
                            Trace.TraceError($"{nameof(CropEconomicData)}.{nameof(SetUserDefinedFixedCostPerUnit)}: '{this.SoilFunctionalCategory}' not handled for {this.Province}. Setting TotalFixed cost to 0.");
                            break;
                    }

                    break;

                //TODO: is there something else we can do to calculate FixedCosts for other provinces?
                default:
                    Trace.TraceError($"{nameof(CropEconomicData)}.{nameof(SetUserDefinedFixedCostPerUnit)}: cannot provide TotalFixedCostPerUnit for {this.Province}. Defaulting to 0");
                    this.TotalFixedCostPerUnit = 0;
                    break;
            }

        }

        #endregion

        #region Private Methods

        private void SetFixedCostByMeasurementSystem(MeasurementSystemType measurementSystem, double fixedCost)
        {
            if (measurementSystem == MeasurementSystemType.Metric)
            {
                this.TotalFixedCostPerUnit = fixedCost * AcresPerHectare;
            }
            else
            {
                this.TotalFixedCostPerUnit = fixedCost;
            }
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is CropEconomicData cropEconomicData)
            {
            }
        }

        #endregion
    }
}
