#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Animals;
using H.Core.Providers.Economics;
using H.Infrastructure;

#endregion

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem : ModelBase
    {
        #region Constructors

        public CropViewItem()
        {
            PropertyChanged += OnPropertyChanged;

            Area = 1;
            PastFallowArea = Area; // Assume past areas are the same as current areas when starting out
            PastPerennialArea = Area; // Assume past areas are the same as current areas when starting out

            Yield = 2500; // Start all crops with this yield, assign a CAR yield when user adds crop to field
            IsIrrigated = Response.No;
            IrrigationType = IrrigationType.RainFed;

            TillageType = TillageType.Reduced;
            PastTillageType = TillageType.Reduced;

            HarvestMethod = HarvestMethods.CashCrop;
            MoistureContentOfCropPercentage = 12;
            SoilReductionFactor = SoilReductionFactors.None;

            MonthlyIpccTier2WaterFactors = new MonthlyValueBase<double>();
            MonthlyIpccTier2TemperatureFactors = new MonthlyValueBase<double>();

            CoverCropTerminationType = CoverCropTerminationType.Natural;

            NitrogenDepositionAmount = 5;
            NitrogenFertilizerRateOnFallow = 14; // From v3 GUI
            NitrogenFertilizerRateOnStubble = 47; // From v3 GUI

            YearOfFallowChange = CoreConstants.IcbmEquilibriumYear;
            YearOfTillageChange = CoreConstants.IcbmEquilibriumYear;
            YearOfConversion = CoreConstants.IcbmEquilibriumYear;

            CarbonConcentration = CoreConstants.CarbonConcentration;
            CustomReductionFactor = 1;

            CropEconomicData = new CropEconomicData();
            IpccTier2NitrogenResults = new IPCCTier2Results();
            IpccTier2CarbonResults = new IPCCTier2Results();

            IsPesticideUsed = Response.No;
            PerennialStandLength = DefaultPerennialStandLength;

            DoNotRecalculatePlantCarbonInAgriculturalProduct = false;

            GrazingViewItems = new ObservableCollection<GrazingViewItem>();

            FertilizerApplicationViewItems = new ObservableCollection<FertilizerApplicationViewItem>();
            FertilizerApplicationViewItems.CollectionChanged += FertilizerApplicationViewItemsOnCollectionChanged;

            DigestateApplicationViewItems = new ObservableCollection<DigestateApplicationViewItem>();

            HarvestViewItems.CollectionChanged += HarvestViewItemsOnCollectionChanged;
            HayImportViewItems.CollectionChanged += HayImportViewItemsOnCollectionChanged;
            GrazingViewItems.CollectionChanged += GrazingViewItemsOnCollectionChanged;
            ManureApplicationViewItems.CollectionChanged += ManureApplicationViewItemsOnCollectionChanged;
            DigestateApplicationViewItems.CollectionChanged += DigestateApplicationViewItemsOnCollectionChanged;
        }

        #endregion

        #region Fields

        private int _phaseNumber;
        private int _yearInPerennialStand;
        private int _perennialStandLength;
        private int _year;
        private int _yearOfSeeding;
        private int _yearOfConversion;
        private int _yearOfFallowChange;
        private int _yearOfTillageChange;
        private int _numberOfPesticidePasses;

        private string _fieldName;

        private bool _manureApplied;
        private bool _underSownCropsUsed;
        private bool _cropIsGrazed;
        private bool _isNativeGrassland;
        private bool _isBrokenGrassland;
        private bool _itemCanBeMovedUp;
        private bool _itemCanBeMovedDown;
        private bool _enableCustomUserDefaultsForThisCrop;
        private bool _cropEconomicDataApplied;
        private bool _hasHarvestViewItems;
        private bool _hasHayImportViewItems;
        private bool _hasGrazingViewItems;
        private bool _hasManureApplicationViewItems;
        private bool _isSecondaryCrop;

        private double _area;
        private double _pastFallowArea;
        private double _pastPerennialArea;
        private double _yield;
        private double _dryYield;
        private double _defaultYield;
        private double _totalHarvestBiomass;
        private double _soilTestNitrogen;
        private double _moistureContentOfCrop;
        private double _moistureContentOfCropPercentage;
        private double _amountOfIrrigation;
        private double _amountOfManureApplied;

        private double _nitrogenFertilizerEfficiencyPercentage;
        private double _nitrogenFertilizerRateOnStubble;
        private double _nitrogenFertilizerRateOnFallow;
        private double _nitrogenDepositionAmount;

        private double _percentageOfStrawReturnedToSoil;
        private double _percentageOfRootsReturnedToSoil;
        private double _percentageOfProductYieldReturnedToSoil;
        private double _fuelEnergy;
        private double _herbicideEnergy;

        /*
         * Main crop residues
         */

        private double _biomassCoefficientProduct;
        private double _biomassCoefficientStraw;
        private double _biomassCoefficientRoots;
        private double _biomassCoefficientExtraroot;

        private double _nitrogenContentInProduct;
        private double _nitrogenContentInStraw;
        private double _nitrogenContentInRoots;
        private double _nitrogenContentInExtraroot;

        private double _carbonConcentration;
        private double _carbonInputFromProduct;
        private double _carbonInputFromStraw;
        private double _carbonInputFromRoots;
        private double _carbonInputFromExtraroots;
        private double _lumCMax;
        private double _kValue;
        private double _nitrogenFixation;
        private double _nitrogenFixationPercentage;
        private double _forageUtilizationRate;

        private Guid _perennialStandGroupId;
        private Guid _detailViewItemToComponentSelectionViewItemMap;

        private Response _isIrrigated;
        private Response _isPesticideUsed;

        private ManureLocationSourceType _manureLocationSourceType;
        private ManureStateType _manureStateType;
        private ManureApplicationTypes _manureApplicationType;
        private ManureAnimalSourceTypes _manureAnimalSourceType;
        private FallowTypes _selectedFallowType;
        private IrrigationType _irrigationType;
        private CropType _cropType;
        private CropType _grasslandType;
        private CropEconomicData _cropEconomicData;
        private NitrogenFertilizerType _nitrogenFertilizerType;
        private FertilizerApplicationMethodologies _fertilizerApplicationMethodology;
        private Seasons _seasonOfFertilizerApplication;
        private CoverCropTerminationType _coverCoverCropTerminationType;

        private HarvestMethods _harvestMethod;

        private TillageType _tillageType;
        private TillageType _currentTillageType;
        private TillageType _pastTillageType;

        private double _aboveGroundCarbonInput;
        private double _belowGroundCarbonInput;
        private double _plantCarbonInAgriculturalProduct;
        private double _climateParameter;
        private double _tillageFactor;
        private double _managementFactor;
        private double _manureCarbonInput;
        private double _manureCarbonPerHectare;
        private double _manureCarbonPerHectareRemaining;
        private double _ligninContent;

        private string _timePeriodCategoryString;
        private string _managementPeriodName;
        private string _cropTypeString;
        private string _harvestMethodString;
        private string _tillageTypeString;
        private string _cropTypeStringWithYear;

        private double _abovegroundNitrogenInputs;
        private double _belowgroundNitrogenInputs;

        #endregion

        #region Properties

        public bool CropEconomicDataApplied
        {
            get => _cropEconomicDataApplied;
            set => SetProperty(ref _cropEconomicDataApplied, value);
        }

        /// <summary>
        ///     A collection of hay import events that occurs when animals grazing on pasture need additional forage for
        ///     consumption.
        /// </summary>
        public ObservableCollection<HayImportViewItem> HayImportViewItems { get; set; } =
            new ObservableCollection<HayImportViewItem>();

        public ObservableCollection<HarvestViewItem> HarvestViewItems { get; set; } =
            new ObservableCollection<HarvestViewItem>();

        /// <summary>
        ///     A collection of all manure applications made to the crop in the year.
        /// </summary>
        public ObservableCollection<ManureApplicationViewItem> ManureApplicationViewItems { get; set; } =
            new ObservableCollection<ManureApplicationViewItem>();

        /// <summary>
        ///     Returns true if the user has entered a value for the number of pesticide passes
        /// </summary>
        public bool IsHerbicideUsed => NumberOfPesticidePasses > 0;

        public bool ItemCanBeMovedUp
        {
            get => _itemCanBeMovedUp;
            set => SetProperty(ref _itemCanBeMovedUp, value);
        }

        public bool ItemCanBeMovedDown
        {
            get => _itemCanBeMovedDown;
            set => SetProperty(ref _itemCanBeMovedDown, value);
        }

        public static int DefaultPerennialStandLength => 1;

        public bool IsSelectedCropTypeRootCrop => CropType.IsRootCrop();

        public bool IsSelectedCropTypeBrokenGrassland => IsBrokenGrassland;

        public int PhaseNumber
        {
            get => _phaseNumber;
            set => SetProperty(ref _phaseNumber, value);
        }

        public CropType CropType
        {
            get => _cropType;
            set => SetProperty(ref _cropType, value, OnCropTypeChanged);
        }

        /// <summary>
        ///     Calling GetDescription (because of reflection) here is a huge bottleneck, return ToString only for now until a
        ///     better solution is found.
        /// </summary>
        public string CropTypeString
        {
            get => _cropTypeString;
            set => SetProperty(ref _cropTypeString, value);
        }

        /// <summary>
        ///     The current tillage type (there is a separate property for past tillage type)
        /// </summary>
        public TillageType TillageType
        {
            get => _tillageType;
            set => SetProperty(ref _tillageType, value, OnTillageTypeChanged);
        }

        /// <summary>
        ///     Total area of crop, fallow area, grassland, etc.
        ///     (ha)
        /// </summary>
        public double Area
        {
            get => _area;
            set => SetProperty(ref _area, value);
        }

        public bool ManureApplied
        {
            get => _manureApplied;
            set => SetProperty(ref _manureApplied, value);
        }

        public ManureLocationSourceType ManureLocationSourceType
        {
            get => _manureLocationSourceType;
            set => SetProperty(ref _manureLocationSourceType, value);
        }

        public ManureAnimalSourceTypes ManureAnimalSourceType
        {
            get => _manureAnimalSourceType;
            set => SetProperty(ref _manureAnimalSourceType, value);
        }

        /// <summary>
        ///     The yield of the crop (wet weight). Holos uses this property (not the optional user entered dry weight) in soil
        ///     carbon change calculations.
        ///     (kg ha^-1)
        /// </summary>
        public double Yield
        {
            get => _yield;
            set => SetProperty(ref _yield, value);
        }

        /// <summary>
        ///     The yield of the crop (dry weight not fresh weight - i.e. moisture weight is subtracted)
        ///     (kg ha^-1)
        /// </summary>
        public double DryYield
        {
            get => _dryYield;
            set => SetProperty(ref _dryYield, value);
        }

        /// <summary>
        ///     The total biomass value from the harvest tab of a perennial crop. The value is calculated based on
        ///     (number of bales * weight per bale). This is set by the user when adding harvest information.
        ///     This property is set to default yield value when crop is initially loaded. The value is reset to default
        ///     yield again when the user removes all harvest information for their component.
        /// </summary>
        public double TotalBiomassHarvest
        {
            get => _totalHarvestBiomass;
            set
            {
                if (!HasHarvestViewItems) value = DefaultYield;

                SetProperty(ref _totalHarvestBiomass, value);
            }
        }

        /// <summary>
        ///     The default yield as read from the data files. This is set when the yield value is set for the first
        ///     time based on lookup table defaults.
        ///     (kg ha^-1)
        /// </summary>
        public double DefaultYield
        {
            get => _defaultYield;
            set => SetProperty(ref _defaultYield, value);
        }

        public Response IsIrrigated
        {
            get => _isIrrigated;
            set => SetProperty(ref _isIrrigated, value, OnIsIrrigatedChanged);
        }

        public Response IsPesticideUsed
        {
            get => _isPesticideUsed;
            set => SetProperty(ref _isPesticideUsed, value);
        }

        public int PerennialStandLength
        {
            get => _perennialStandLength;
            set => SetProperty(ref _perennialStandLength, value);
        }

        public double AmountOfIrrigation
        {
            get => _amountOfIrrigation;
            set => SetProperty(ref _amountOfIrrigation, value);
        }

        /// <summary>
        ///     Fraction
        ///     (unitless)
        /// </summary>
        public double MoistureContentOfCrop
        {
            get => _moistureContentOfCrop;
            set
            {
                if (value >= 1)
                    // Old farms had this improperly set
                    value /= 100;

                if (value >= 1000)
                {
                    // Old farms had this improperly set
                    value /= 100;
                    value /= 100;
                }

                SetProperty(ref _moistureContentOfCrop, value);
            }
        }

        /// <summary>
        ///     Moisture content of crop expressed as a percentage.
        ///     %
        /// </summary>
        public double MoistureContentOfCropPercentage
        {
            get => _moistureContentOfCropPercentage;
            set
            {
                SetProperty(ref _moistureContentOfCropPercentage, value,
                    () => { MoistureContentOfCrop = value / 100; });
            }
        }

        /// <summary>
        ///     Set to true when this view item is a perennial crop and the previous year is an annual crop and the user wants to
        ///     indicate
        ///     that this year's crop (the perennial) is undersown into the previous year's crop (the annual).
        /// </summary>
        public bool UnderSownCropsUsed
        {
            get => _underSownCropsUsed;
            set => SetProperty(ref _underSownCropsUsed, value);
        }

        public string FieldName
        {
            get => _fieldName;
            set => SetProperty(ref _fieldName, value);
        }

        public int Year
        {
            get => _year;
            set
            {
                SetProperty(ref _year, value, () => { _cropTypeStringWithYear = $"[{_year}] - {_cropTypeString}"; });
            }
        }

        public int YearInPerennialStand
        {
            get => _yearInPerennialStand;
            set => SetProperty(ref _yearInPerennialStand, value, OnYearInPerennialStandChanged);
        }

        private void OnYearInPerennialStandChanged()
        {
        }

        public Guid PerennialStandGroupId
        {
            get => _perennialStandGroupId;
            set => SetProperty(ref _perennialStandGroupId, value);
        }

        public bool CropIsGrazed
        {
            get => _cropIsGrazed;
            set => SetProperty(ref _cropIsGrazed, value);
        }

        public int NumberOfPesticidePasses
        {
            get => _numberOfPesticidePasses;
            set => SetProperty(ref _numberOfPesticidePasses, value);
        }

        public ManureApplicationTypes ManureApplicationType
        {
            get => _manureApplicationType;
            set => SetProperty(ref _manureApplicationType, value);
        }

        public HarvestMethods HarvestMethod
        {
            get => _harvestMethod;
            set => SetProperty(ref _harvestMethod, value, OnHarvestMethodChanged);
        }

        public string HarvestMethodString
        {
            get => _harvestMethodString;
            set => SetProperty(ref _harvestMethodString, value);
        }

        /// <summary>
        ///     S_p
        ///     (%)
        /// </summary>
        public double PercentageOfProductYieldReturnedToSoil
        {
            get => _percentageOfProductYieldReturnedToSoil;
            set => SetProperty(ref _percentageOfProductYieldReturnedToSoil, value);
        }

        /// <summary>
        ///     S_r
        ///     (%)
        /// </summary>
        public double PercentageOfRootsReturnedToSoil
        {
            get => _percentageOfRootsReturnedToSoil;
            set => SetProperty(ref _percentageOfRootsReturnedToSoil, value);
        }

        /// <summary>
        ///     S_s
        ///     (%)
        /// </summary>
        public double PercentageOfStrawReturnedToSoil
        {
            get => _percentageOfStrawReturnedToSoil;
            set => SetProperty(ref _percentageOfStrawReturnedToSoil, value);
        }

        /// <summary>
        ///     (Fraction)
        /// </summary>
        public double CarbonConcentration
        {
            get => _carbonConcentration;
            set => SetProperty(ref _carbonConcentration, value);
        }

        public IrrigationType IrrigationType
        {
            get => _irrigationType;
            set => SetProperty(ref _irrigationType, value);
        }

        /// <summary>
        ///     R_p
        /// </summary>
        public double BiomassCoefficientProduct
        {
            get => _biomassCoefficientProduct;
            set => SetProperty(ref _biomassCoefficientProduct, value);
        }

        /// <summary>
        ///     R_s
        /// </summary>
        public double BiomassCoefficientStraw
        {
            get => _biomassCoefficientStraw;
            set => SetProperty(ref _biomassCoefficientStraw, value);
        }

        /// <summary>
        ///     R_r
        /// </summary>
        public double BiomassCoefficientRoots
        {
            get => _biomassCoefficientRoots;
            set => SetProperty(ref _biomassCoefficientRoots, value);
        }

        /// <summary>
        ///     R_e
        /// </summary>
        public double BiomassCoefficientExtraroot
        {
            get => _biomassCoefficientExtraroot;
            set => SetProperty(ref _biomassCoefficientExtraroot, value);
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double NitrogenContentInProduct
        {
            get => _nitrogenContentInProduct;
            set => SetProperty(ref _nitrogenContentInProduct, value);
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double NitrogenContentInStraw
        {
            get => _nitrogenContentInStraw;
            set => SetProperty(ref _nitrogenContentInStraw, value);
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double NitrogenContentInRoots
        {
            get => _nitrogenContentInRoots;
            set => SetProperty(ref _nitrogenContentInRoots, value);
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double NitrogenContentInExtraroot
        {
            get => _nitrogenContentInExtraroot;
            set => SetProperty(ref _nitrogenContentInExtraroot, value);
        }

        /// <summary>
        ///     The amount of manure applied to the crop.
        ///     kg
        /// </summary>
        public double AmountOfManureApplied
        {
            get => _amountOfManureApplied;
            set => SetProperty(ref _amountOfManureApplied, value);
        }

        public ManureStateType ManureStateType
        {
            get => _manureStateType;
            set => SetProperty(ref _manureStateType, value);
        }

        public DefaultManureCompositionData ManureCompositionData { get; set; }

        public FallowTypes SelectedFallowType
        {
            get => _selectedFallowType;
            set => SetProperty(ref _selectedFallowType, value);
        }

        private TillageType CurrentTillageType
        {
            get => _currentTillageType;
            set => SetProperty(ref _currentTillageType, value);
        }

        public TillageType PastTillageType
        {
            get => _pastTillageType;
            set => SetProperty(ref _pastTillageType, value);
        }

        /// <summary>
        ///     Used for both perennials and grassland
        /// </summary>
        [Obsolete(
            "Not used anymore, use YearOfConversion to capture the year of initial seeding since that is what is used in single year C calculations")]
        public int YearOfSeeding
        {
            get => _yearOfSeeding;
            set => SetProperty(ref _yearOfSeeding, value);
        }

        public bool IsNativeGrassland
        {
            get => _isNativeGrassland;
            set => SetProperty(ref _isNativeGrassland, value);
        }

        /// <summary>
        ///     Indicates if the growing area was changed from a grassland to a cropping system (e.g. wheat)
        /// </summary>
        public bool IsBrokenGrassland
        {
            get => _isBrokenGrassland;
            set => SetProperty(ref _isBrokenGrassland, value);
        }

        /// <summary>
        ///     Atmospheric nitrogen deposition
        ///     (kg N ha^-1)
        /// </summary>
        public double NitrogenDepositionAmount
        {
            get => _nitrogenDepositionAmount;

            set => SetProperty(ref _nitrogenDepositionAmount, value);
        }

        /// <summary>
        ///     C_ptoSoil (kg ha^-1)
        /// </summary>
        public double CarbonInputFromProduct
        {
            get => _carbonInputFromProduct;
            set => SetProperty(ref _carbonInputFromProduct, value);
        }

        /// <summary>
        ///     C_p (kg ha^-1)
        /// </summary>
        public double PlantCarbonInAgriculturalProduct
        {
            get => _plantCarbonInAgriculturalProduct;
            set => SetProperty(ref _plantCarbonInAgriculturalProduct, value);
        }

        /// <summary>
        ///     Used to prevent a custom C_p value from being overwritten from the usual method of calculation for C_p (used with
        ///     perennials only)
        /// </summary>
        public bool DoNotRecalculatePlantCarbonInAgriculturalProduct { get; set; }

        /// <summary>
        ///     C_s (kg ha^-1)
        /// </summary>
        public double CarbonInputFromStraw
        {
            get => _carbonInputFromStraw;
            set => SetProperty(ref _carbonInputFromStraw, value);
        }

        /// <summary>
        ///     C_r (kg ha^-1)
        /// </summary>
        public double CarbonInputFromRoots
        {
            get => _carbonInputFromRoots;
            set => SetProperty(ref _carbonInputFromRoots, value);
        }

        /// <summary>
        ///     C_e (kg ha^-1)
        /// </summary>
        public double CarbonInputFromExtraroots
        {
            get => _carbonInputFromExtraroots;
            set => SetProperty(ref _carbonInputFromExtraroots, value);
        }

        /// <summary>
        ///     Used to determine how much N fertilizer is required for a given user specified yield
        ///     (Kg N ha⁻¹)
        /// </summary>
        public double SoilTestNitrogen
        {
            get => _soilTestNitrogen;
            set => SetProperty(ref _soilTestNitrogen, value);
        }

        public double NitrogenFertilizerRateOnStubble
        {
            get => _nitrogenFertilizerRateOnStubble;
            set => SetProperty(ref _nitrogenFertilizerRateOnStubble, value);
        }

        public double NitrogenFertilizerRateOnFallow
        {
            get => _nitrogenFertilizerRateOnFallow;
            set => SetProperty(ref _nitrogenFertilizerRateOnFallow, value);
        }

        /// <summary>
        ///     Expressed as a ratio (fraction)
        /// </summary>
        public double NitrogenFertilizerEfficiency => NitrogenFertilizerEfficiencyPercentage / 100;

        /// <summary>
        ///     Expressed as a percentage (%)
        /// </summary>
        public double NitrogenFertilizerEfficiencyPercentage
        {
            get => _nitrogenFertilizerEfficiencyPercentage;
            set => SetProperty(ref _nitrogenFertilizerEfficiencyPercentage, value);
        }

        /// <summary>
        ///     Maximum C produced by seeding grassland (g m^-2)
        /// </summary>
        public double LumCMax
        {
            get => _lumCMax;
            set => SetProperty(ref _lumCMax, value);
        }

        /// <summary>
        ///     Rate constant
        /// </summary>
        public double KValue
        {
            get => _kValue;
            set => SetProperty(ref _kValue, value);
        }

        /// <summary>
        ///     Used to determine how much N fertilizer is required for a given user specified yield
        ///     (fraction)
        /// </summary>
        public double NitrogenFixation
        {
            get => _nitrogenFixation;
            set => SetProperty(ref _nitrogenFixation, value);
        }

        /// <summary>
        ///     (GJ ha^-1)
        /// </summary>
        public double FuelEnergy
        {
            get => _fuelEnergy;
            set => SetProperty(ref _fuelEnergy, value);
        }

        /// <summary>
        ///     (GJ ha^-1)
        /// </summary>
        public double HerbicideEnergy
        {
            get => _herbicideEnergy;
            set => SetProperty(ref _herbicideEnergy, value);
        }

        /// <summary>
        ///     When enabled, user provided defaults will be used when adding a new crop to a field (i.e. system defaults will not
        ///     be used).
        /// </summary>
        public bool EnableCustomUserDefaultsForThisCrop
        {
            get => _enableCustomUserDefaultsForThisCrop;
            set => SetProperty(ref _enableCustomUserDefaultsForThisCrop, value);
        }

        public CropEconomicData CropEconomicData
        {
            get => _cropEconomicData;
            set => SetProperty(ref _cropEconomicData, value);
        }

        /// <summary>
        ///     Used to capture the year when either a perennial was changed to an annual (broken grassland) or an annual was
        ///     changed to a perennial (seeded)
        /// </summary>
        public int YearOfConversion
        {
            get => _yearOfConversion;
            set => SetProperty(ref _yearOfConversion, value);
        }

        public int YearsSinceYearOfConversion => Year - YearOfConversion;

        public double PastFallowArea
        {
            get => _pastFallowArea;
            set => SetProperty(ref _pastFallowArea, value);
        }

        public double PastPerennialArea
        {
            get => _pastPerennialArea;
            set => SetProperty(ref _pastPerennialArea, value);
        }

        public int YearOfFallowChange
        {
            get => _yearOfFallowChange;
            set => SetProperty(ref _yearOfFallowChange, value);
        }

        public int YearOfTillageChange
        {
            get => _yearOfTillageChange;
            set => SetProperty(ref _yearOfTillageChange, value);
        }

        /// <summary>
        ///     Total carbon inputs
        ///     (kg C ha^-1)
        /// </summary>
        public double TotalCarbonInputs { get; set; }

        /// <summary>
        ///     C_ag
        ///     (kg C ha^-1)
        /// </summary>
        public double AboveGroundCarbonInput
        {
            get => _aboveGroundCarbonInput;
            set => SetProperty(ref _aboveGroundCarbonInput, value);
        }

        /// <summary>
        ///     C_bg
        ///     (kg C ha^-1)
        /// </summary>
        public double BelowGroundCarbonInput
        {
            get => _belowGroundCarbonInput;
            set => SetProperty(ref _belowGroundCarbonInput, value);
        }

        public int SizeOfFirstRotationForField { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="FieldSystemComponent" /> this view item belongs to.
        /// </summary>
        public Guid FieldSystemComponentGuid { get; set; }

        public string TimePeriodCategoryString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_timePeriodCategoryString) == false) return _timePeriodCategoryString;

                return TimePeriodCategory.Current.GetDescription();
            }
            set => SetProperty(ref _timePeriodCategoryString, value);
        }

        public double ClimateParameter
        {
            get => _climateParameter;
            set => SetProperty(ref _climateParameter, value);
        }

        public double ManagementFactor
        {
            get => _managementFactor;
            set => SetProperty(ref _managementFactor, value);
        }

        /// <summary>
        ///     (kg C year^-1)
        ///     Total manure C for entire year and for entire field
        /// </summary>
        public double ManureCarbonInput
        {
            get => _manureCarbonInput;
            set => SetProperty(ref _manureCarbonInput, value);
        }

        /// <summary>
        ///     (kg C ha^-1)
        ///     Total manure C from all manure applications, remaining manure, and imports
        /// </summary>
        public double ManureCarbonInputsPerHectare
        {
            get => _manureCarbonPerHectare;
            set => SetProperty(ref _manureCarbonPerHectare, value);
        }

        /// <summary>
        ///     (kg C ha^-1)
        ///     Total manure C from manure applications only
        /// </summary>
        public double ManureCarbonInputsFromManureOnly
        {
            get => _manureCarbonPerHectareRemaining;
            set => SetProperty(ref _manureCarbonPerHectareRemaining, value);
        }

        public double TillageFactor
        {
            get => _tillageFactor;
            set => SetProperty(ref _tillageFactor, value);
        }

        public string ManagementPeriodName
        {
            get => _managementPeriodName;
            set => SetProperty(ref _managementPeriodName, value);
        }

        public NitrogenFertilizerType NitrogenFertilizerType
        {
            get => _nitrogenFertilizerType;
            set => SetProperty(ref _nitrogenFertilizerType, value);
        }

        public FertilizerApplicationMethodologies FertilizerApplicationMethodology
        {
            get => _fertilizerApplicationMethodology;
            set => SetProperty(ref _fertilizerApplicationMethodology, value);
        }

        public Seasons SeasonOfFertilizerApplication
        {
            get => _seasonOfFertilizerApplication;
            set => SetProperty(ref _seasonOfFertilizerApplication, value);
        }

        public string CropTypeStringWithYear
        {
            get => _cropTypeStringWithYear;
            set => SetProperty(ref _cropTypeStringWithYear, value);
        }

        public string TillageTypeString
        {
            get => _tillageTypeString;
            set => SetProperty(ref _tillageTypeString, value);
        }

        public bool HasHarvestViewItems
        {
            get => _hasHarvestViewItems;
            set => SetProperty(ref _hasHarvestViewItems, value);
        }

        public bool HasHayImportViewItems
        {
            get => _hasHayImportViewItems;
            set => SetProperty(ref _hasHayImportViewItems, value);
        }

        public bool HasGrazingViewItems
        {
            get => _hasGrazingViewItems;
            set => SetProperty(ref _hasGrazingViewItems, value);
        }

        public CoverCropTerminationType CoverCropTerminationType
        {
            get => _coverCoverCropTerminationType;
            set => SetProperty(ref _coverCoverCropTerminationType, value);
        }

        public bool HasManureApplicationViewItems
        {
            get => _hasManureApplicationViewItems;
            set => SetProperty(ref _hasManureApplicationViewItems, value);
        }

        /// <summary>
        ///     Used to determine which component selection view item was used to create a detail view item
        /// </summary>
        public Guid DetailViewItemToComponentSelectionViewItemMap
        {
            get => _detailViewItemToComponentSelectionViewItemMap;
            set => SetProperty(ref _detailViewItemToComponentSelectionViewItemMap, value);
        }

        /// <summary>
        ///     Indicates if the view item represents a cover/winter or undersown crop
        /// </summary>
        public bool IsSecondaryCrop
        {
            get => _isSecondaryCrop;
            set => SetProperty(ref _isSecondaryCrop, value);
        }

        /// <summary>
        ///     Lignin content of carbon input
        ///     (unitless)
        /// </summary>
        public double LigninContent
        {
            get => _ligninContent;
            set => SetProperty(ref _ligninContent, value);
        }

        /// <summary>
        ///     Grazing utilization rate depends on the type of forage rather than the type of grazing system or number of animals
        ///     (%)
        /// </summary>
        public double ForageUtilizationRate
        {
            get => _forageUtilizationRate;
            set => SetProperty(ref _forageUtilizationRate, value);
        }

        /// <summary>
        ///     The total aboveground C inputs from the main crop residue and any secondary crop residues from the same year
        ///     (kg C ha^-1)
        /// </summary>
        public double CombinedAboveGroundInput { get; set; }

        /// <summary>
        ///     The total belowground C inputs from the main crop residue and any secondary crop residues from the same year
        ///     (kg C ha^-1)
        /// </summary>
        public double CombinedBelowGroundInput { get; set; }

        /// <summary>
        ///     The total manure C inputs from the main crop manure applications and any secondary crop manure applications from
        ///     the same year
        ///     (kg C ha^-1)
        /// </summary>
        public double CombinedManureInput { get; set; }

        /// <summary>
        ///     The total digestate C inputs from the main crop digestate applications and any secondary crop digestate
        ///     applications from the same year
        ///     (kg C ha^-1)
        /// </summary>
        public double CombinedDigestateInput { get; set; }

        /// <summary>
        ///     ICBM uses the same humification coefficient for manure and digestate so inputs are combined and added to the
        ///     manure pool
        /// </summary>
        public double CombinedManureAndDigestateInput => CombinedManureInput + CombinedDigestateInput;

        public double CombinedGrainNitrogen { get; set; }
        public double CombinedStrawNitrogen { get; set; }
        public double CombinedRootNitrogen { get; set; }
        public double CombinedExtrarootNitrogen { get; set; }

        private double _combinedAboveGroundResidueNitrogen;

        public double CombinedAboveGroundResidueNitrogen
        {
            get => _combinedAboveGroundResidueNitrogen;
            set => SetProperty(ref _combinedAboveGroundResidueNitrogen, value);
        }

        public double CombinedBelowGroundResidueNitrogen { get; set; }
        public double GrowingSeasonIrrigation { get; set; }

        public double NitrogenFixationPercentage
        {
            get => _nitrogenFixationPercentage;

            set { SetProperty(ref _nitrogenFixationPercentage, value, () => NitrogenFixation = value / 100.0); }
        }

        public double AbovegroundNitrogenInputs
        {
            get => _abovegroundNitrogenInputs;
            set => SetProperty(ref _abovegroundNitrogenInputs, value);
        }

        public double BelowgroundNitrogenInputs
        {
            get => _belowgroundNitrogenInputs;
            set => SetProperty(ref _belowgroundNitrogenInputs, value);
        }

        #endregion

        #region Public Methods

        public void CalculateDryYield()
        {
            DryYield = Yield * (1 - MoistureContentOfCrop);
        }

        public void CalculateWetWeightYield()
        {
            Yield = DryYield / (1 - MoistureContentOfCrop);
        }

        /// <summary>
        ///     This is the total amount of both organic fertilizer nitrogen and manure nitrogen.
        /// </summary>
        public double GetTotalOrganicAndManureNitrogenInYear()
        {
            var result = GetTotalOrganicNitrogenInYear() + GetTotalManureNitrogenAppliedFromLivestockInYear();

            return result;
        }

        /// <summary>
        ///     This is not manure or digestate, but organic fertilizers.
        ///     (kg N)
        /// </summary>
        /// <returns></returns>
        public double GetTotalOrganicNitrogenInYear()
        {
            var totalNitrogen = 0d;

            foreach (var fertilizerApplicationViewItem in FertilizerApplicationViewItems.Where(x =>
                         x.FertilizerBlendData.FertilizerBlend == FertilizerBlends.CustomOrganic))
                totalNitrogen += fertilizerApplicationViewItem.AmountOfNitrogenApplied * Area;

            return totalNitrogen;
        }

        public double GetTotalManureNitrogenAppliedFromLivestockInYear()
        {
            var totalNitrogen = 0d;

            foreach (var manureApplication in ManureApplicationViewItems.Where(manureViewItem =>
                         manureViewItem.DateOfApplication.Year == Year &&
                         manureViewItem.ManureLocationSourceType == ManureLocationSourceType.Livestock))
                totalNitrogen += manureApplication.AmountOfNitrogenAppliedPerHectare * Area;

            return totalNitrogen;
        }

        /// <summary>
        ///     (kg C year^-1)
        /// </summary>
        public double GetTotalCarbonFromAppliedManure()
        {
            return GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Livestock) +
                   GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Imported);
        }

        /// <summary>
        ///     (kg C year^-1)
        /// </summary>
        public double GetTotalCarbonFromAppliedManure(ManureLocationSourceType manureLocationSourceType)
        {
            var manureApplications = GetManureApplicationsInYear(manureLocationSourceType);
            var result = CalculateTotalCarbonFromManureApplications(manureApplications);

            return result;
        }

        public IEnumerable<ManureApplicationViewItem> GetManureApplicationsInYear(
            ManureLocationSourceType manureLocationSourceType)
        {
            return ManureApplicationViewItems.Where(manureApplicationViewItem =>
                manureApplicationViewItem.ManureLocationSourceType == manureLocationSourceType &&
                manureApplicationViewItem.DateOfApplication.Year == Year);
        }

        /// <summary>
        ///     Equation 4.7.1-1
        ///     Equation 4.7.1-2
        ///     (kg C year^-1)
        /// </summary>
        public double CalculateTotalCarbonFromManureApplications(
            IEnumerable<ManureApplicationViewItem> manureApplicationViewItems)
        {
            var result = 0d;

            foreach (var manureApplication in manureApplicationViewItems)
            {
                var carbonFraction = manureApplication.DefaultManureCompositionData.CarbonContent;
                var volumeOfManure = manureApplication.AmountOfManureAppliedPerHectare;
                var area = Area;

                result += carbonFraction * volumeOfManure * area;
            }

            return result;
        }

        public double CombinedResidueNitrogen()
        {
            return CombinedAboveGroundResidueNitrogen + CombinedBelowGroundResidueNitrogen;
        }

        #endregion

        #region Event Handlers

        private void GrazingViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasGrazingViewItems = GrazingViewItems.Count > 0;
        }

        private void ManureApplicationViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasManureApplicationViewItems = ManureApplicationViewItems.Count > 0;
        }

        private void HayImportViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasHayImportViewItems = HayImportViewItems.Count > 0;
        }

        private void HarvestViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasHarvestViewItems = HarvestViewItems.Count > 0;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }

        private void OnIsIrrigatedChanged()
        {
            IrrigationType = IsIrrigated == Response.Yes ? IrrigationType.Irrigated : IrrigationType.RainFed;
        }

        private void OnHarvestMethodChanged()
        {
            HarvestMethodString = HarvestMethod.GetDescription();
        }

        private void OnTillageTypeChanged()
        {
            TillageTypeString = TillageType.GetDescription();
        }

        private void OnCropTypeChanged()
        {
            CropTypeString = CropType.GetDescription();
            CropTypeStringWithYear = $"[{Year}] - {CropType.GetDescription()}";

            // If user changes from a perennial a non-perennial must reset this property to false
            if (CropType.IsPerennial() == false) UnderSownCropsUsed = false;
        }

        public override string ToString()
        {
            return
                $"{nameof(Name)}: {Name}, {CropTypeString}, {nameof(FieldName)}: {FieldName}, {nameof(Year)}: {Year}";
        }

        #endregion
    }
}