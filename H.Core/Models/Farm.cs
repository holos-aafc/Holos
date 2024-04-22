#region Imports

using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Shelterbelt;
using H.Core.Providers;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Feed;
using H.Core.Providers.Soil;
using H.Core.Tools;
using H.Core.Models.Infrastructure;
using H.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using AutoMapper.Configuration.Conventions;
using H.Core.Models.Animals;
using H.Core.Converters;
using H.Core.Emissions.Results;

#endregion

namespace H.Core.Models
{
    public partial class Farm : ModelBase
    {
        #region Fields

        public enum ChosenClimateAcquisition
        {
            /// <summary>
            /// Uses the 'old' default (non-daily) temperatures where normals were used to extract daily values. This is deprecated in favor of NASA climate data
            /// </summary>
            SLC,

            /// <summary>
            /// A user created file of climate data whereby the user must specify daily values for temperature, precipitation, evapotranspiration, etc.
            /// </summary>
            Custom,

            /// <summary>
            /// Daily climate data is downloaded from NASA website API
            /// </summary>
            NASA,
        }

        private string _comments;
        private string _pathToYieldInputFile;

        private double _latitude;
        private double _longitude;
        private double _startingSoilOrganicCarbon;

        private int _polygonId;
        private int _carbonModellingEquilibriumYear;
        private int _totalBalesProducedByFarm;
        private int _totalBalesRemainingOnFarm;
        private int _totalNumberOfBalesAppliedToFarm;

        private DateTime _carbonModellingEquilibriumStartDate;
        private DateTime _carbonModellingEndDate;

        private bool _farmHasBales;
        private bool _showAdditionalColumnsOnDietForumtorView;
        private bool _showDefaultDietsInDietFormulator;
        private bool _showDetailsOnComponentSelectionView;
        private bool _enableCarbonModelling;
        private bool _showAvailableComponentsList;
        private bool _showFieldSystemResultsAsGrid;
        private bool _enableDebugDisplay;
        private bool _showSimplifiedResults;
        private bool _useCustomStartingSoilOrganicCarbon;
        private bool _isBasicMode;
        private bool _showAdditionalInformationInADView;
        private bool _isCommandLineMode;
        private bool _useCustomRunInTillage;

        private Defaults _defaults;
        private Province _province;
        private GeographicData _geographicData;
        private ClimateData _climateData;

        private bool _measurementSystemSelected;

        private MeasurementSystemType _measurementSystemType;

        private ChosenClimateAcquisition _climateAcquisition;
        private Table_15_Default_Soil_N2O_Emission_BreakDown_Provider _annualSoilN2OBreakdown;
        private YieldAssignmentMethod _yieldAssignmentMethod;

        private List<TimeFrame> _availableTimeFrame;
        private readonly ShelterbeltEnabledFromHardinessZoneConverter _shelterbeltFromHardinessZoneConverter = new ShelterbeltEnabledFromHardinessZoneConverter();

        #endregion

        #region Constructors

        public Farm()
        {
            HTraceListener.AddTraceListener();

            this.Defaults = new Defaults();
            this.Diets = new ObservableCollection<Diet>();
            this.DefaultManureCompositionData = new ObservableCollection<DefaultManureCompositionData>();
            this.DefaultsCompositionOfBeddingMaterials = new ObservableCollection<Table_30_Default_Bedding_Material_Composition_Data>();
            this.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;
            this.CarbonModellingEquilibriumYear = CoreConstants.IcbmEquilibriumYear;
            this.CarbonModellingEquilibriumStartDate = new DateTime(this.CarbonModellingEquilibriumYear, 1, 1);
            this.CarbonModellingEndDate = new DateTime(DateTime.Now.Year + CoreConstants.IcbmProjectionPeriod, 12, 31);

            this.ShowAvailableComponentsList = true;
            this.ShowSimplifiedResults = false;
            this.IsBasicMode = false;
            this.EnableCarbonModelling = true;

            this.StageStates = new List<StageStateBase>();
            this.Components.CollectionChanged += ComponentsOnCollectionChanged;

            this.PropertyChanged += FarmPropertyChanged;

            this.ClimateData = new ClimateData();
            this.GeographicData = new GeographicData();
            this.AnnualSoilN2OBreakdown = new Table_15_Default_Soil_N2O_Emission_BreakDown_Provider();
        }

        /// <summary>
        /// we need this to set the available timeframes when we set climate acquisition
        /// </summary>
        /// <param name="chosenClimateAcquisition">our type of climate acquisition</param>
        public void SetAvailableTimeFrame(ChosenClimateAcquisition chosenClimateAcquisition)
        {
            if (chosenClimateAcquisition == ChosenClimateAcquisition.NASA)
            {
                this.AvailableTimeFrames = new List<TimeFrame>()
                    {
                        TimeFrame.TwoThousandToCurrent,
                        TimeFrame.NineteenNinetyToTwoThousand,
                        TimeFrame.NineteenEightyToNineteenNinety,
                        TimeFrame.ProjectionPeriod
                    };
            }
            else if (chosenClimateAcquisition == ChosenClimateAcquisition.SLC)
            {
                this.AvailableTimeFrames = new List<TimeFrame>()
                    {
                        TimeFrame.NineteenFiftyToNineteenEighty,
                        TimeFrame.NineteenSixtyToNineteenNinety,
                        TimeFrame.NineteenSeventyToTwoThousand,
                        TimeFrame.NineteenEightyToTwoThousandTen,
                        TimeFrame.NineteenNinetyToTwoThousandSeventeen
                    };

            }
        }

        #endregion

        #region Properties

        public bool ResultsCalculated
        {
            get
            {
                if (this.Components.Any() == false)
                {
                    return false;
                }

                return this.Components.All(x => x.ResultsCalculated);
            }
        }

        public bool IsBasicMode
        {
            get { return _isBasicMode; }
            set { SetProperty(ref _isBasicMode, value, () => { this.RaisePropertyChanged(nameof(IsAdvancedMode)); }); }
        }

        public bool IsAdvancedMode
        {
            get { return this.IsBasicMode == false; }
        }

        public List<TimeFrame> AvailableTimeFrames
        {
            get { return _availableTimeFrame; }
            set { SetProperty(ref _availableTimeFrame, value); }
        }

        public YieldAssignmentMethod YieldAssignmentMethod
        {
            get { return _yieldAssignmentMethod; }
            set { SetProperty(ref _yieldAssignmentMethod, value); }
        }

        public bool IsSelectedForComparison { get; set; }

        public ChosenClimateAcquisition ClimateAcquisition
        {
            get { return _climateAcquisition; }
            set { SetProperty(ref _climateAcquisition, value); }
        }

        public MeasurementSystemType MeasurementSystemType
        {
            get { return _measurementSystemType; }
            set { this.SetProperty(ref _measurementSystemType, value); }
        }

        public bool MeasurementSystemSelected
        {
            get { return _measurementSystemSelected; }
            set { this.SetProperty(ref _measurementSystemSelected, value); }
        }

        public ObservableCollection<Diet> Diets { get; set; }

        public ObservableCollection<DefaultManureCompositionData> DefaultManureCompositionData
        {
            get;
            set;
        }

        public ObservableCollection<Table_30_Default_Bedding_Material_Composition_Data> DefaultsCompositionOfBeddingMaterials { get; set; }

        public int PolygonId
        {
            get { return _polygonId; }
            set { this.SetProperty(ref _polygonId, value); }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { this.SetProperty(ref _longitude, value); }
        }

        public double Latitude
        {
            get { return _latitude; }
            set { this.SetProperty(ref _latitude, value); }
        }

        public List<StageStateBase> StageStates { get; set; }

        /// <summary>
        /// A collection of all the components the user has selected for this farm.
        /// </summary>
        public ObservableCollection<ComponentBase> Components { get; set; } = new ObservableCollection<ComponentBase>();

        /// <summary>
        /// The default allocation of total N2O emissions within the year
        /// </summary>
        public Table_15_Default_Soil_N2O_Emission_BreakDown_Provider AnnualSoilN2OBreakdown
        {
            get
            {
                return _annualSoilN2OBreakdown;
            }
            set
            {
                SetProperty(ref _annualSoilN2OBreakdown, value);
            }
        }

        /// <summary>
        /// This property has to be held in the farm (or storage) and not on base view model. If this property is held in base view model, then
        /// each instance of a view model (timeline, results, etc.) would each have a separate instance of the property and that won't work.
        /// </summary>
        public bool EnableDebugDisplay
        {
            get { return _enableDebugDisplay; }
            set { SetProperty(ref _enableDebugDisplay, value); }
        }

        /// <summary>
        /// Additional user comments for the <see cref="Farm"/>.
        /// </summary>
        public string Comments
        {
            get { return _comments; }
            set { this.SetProperty(ref _comments, value); }
        }

        /// <summary>
        /// Indicates which province the user has selected. Do not use this value for calculations/lookups. Use the province associated with the
        /// <see cref="DefaultSoilData"/> instead since the two values for the province could differ if the selected polygon spans two polygons.
        /// </summary>
        public Province Province
        {
            get { return _province; }
            set { this.SetProperty(ref _province, value, () => { this.RaisePropertyChanged(nameof(this.ProvinceDescription)); }); }
        }

        public string ProvinceDescription
        {
            get { return this.Province.GetDescription(); }
        }

        public GeographicData GeographicData
        {
            get { return _geographicData; }
            set { this.SetProperty(ref _geographicData, value); }
        }

        /// <summary>
        /// The default soil data selected by the user if there was more than one soil component found within the selected polygon. The user
        /// has the option to define a field-level soil type as well <see cref="FieldSystemComponent.SoilData"/>.
        /// </summary>
        public SoilData DefaultSoilData
        {
            get
            {
                return GeographicData?.DefaultSoilData;
            }
        }
        public bool ShowAdditionalColumnsOnDietForumtorView
        {
            get { return _showAdditionalColumnsOnDietForumtorView; }
            set { this.SetProperty(ref _showAdditionalColumnsOnDietForumtorView, value); }
        }

        public bool ShowDefaultDietsInDietFormulator
        {
            get { return _showDefaultDietsInDietFormulator; }
            set { this.SetProperty(ref _showDefaultDietsInDietFormulator, value); }
        }

        public bool ShowDetailsOnComponentSelectionView
        {
            get { return _showDetailsOnComponentSelectionView; }
            set { SetProperty(ref _showDetailsOnComponentSelectionView, value); }
        }

        public bool EnableCarbonModelling
        {
            get { return _enableCarbonModelling; }
            set { SetProperty(ref _enableCarbonModelling, value); }
        }

        public bool ShowAdditionalInformationInADView
        {
            get => _showAdditionalInformationInADView;
            set => SetProperty(ref _showAdditionalInformationInADView, value);
        }

        /// <summary>
        /// Checks if hardiness zone data exists for the selected farm. The shelterbelt component is
        /// only available if we have hardiness zone data available for the selected location.
        /// Returns True if data is available.
        /// Return False otherwise.
        /// </summary>
        public bool IsShelterbeltComponentAvailable
        {
            get => _shelterbeltFromHardinessZoneConverter.Convert(GeographicData.HardinessZone);
        }
        public IEnumerable<AnimalComponentBase> AnimalComponents
        {
            get
            {
                return this.DairyComponents.Concat(this.BeefCattleComponents)
                                           .Concat(this.SwineComponents)
                                           .Concat(this.SheepComponents)
                                           .Concat(this.PoultryComponents)
                                           .Concat(this.OtherLivestockComponents).Cast<AnimalComponentBase>();
            }
        }

        /// <summary>
        /// Indicates if the farm has any components in which multi-year inputs are possible
        /// </summary>
        public bool HasMultiYearComponents
        {
            get
            {
                return this.FieldSystemComponents.Any() || this.Components.OfType<ShelterbeltComponent>().Any();
            }
        }

        public IEnumerable<ComponentBase> DairyComponents
        {
            get { return this.Components.Where(x => x.ComponentCategory == ComponentCategory.Dairy); }
        }

        public IEnumerable<FieldSystemComponent> FieldSystemComponents
        {
            get { return this.Components.Where(x => x.GetType() == typeof(FieldSystemComponent)).Cast<FieldSystemComponent>(); }
        }

        public IEnumerable<AnaerobicDigestionComponent> AnaerobicDigestionComponents
        {
            get => this.Components.Where(x => x.GetType() == typeof(AnaerobicDigestionComponent)).Cast<AnaerobicDigestionComponent>();
        }

        public IEnumerable<ComponentBase> BeefCattleComponents
        {
            get
            {
                return this.Components.Where(x => x.ComponentCategory == ComponentCategory.BeefProduction);
            }
        }

        public IEnumerable<ComponentBase> SwineComponents
        {
            get
            {
                return this.Components.Where(x => x.ComponentCategory == ComponentCategory.Swine);
            }
        }

        public IEnumerable<ComponentBase> SheepComponents
        {
            get
            {
                return this.Components.Where(x => x.ComponentCategory == ComponentCategory.Sheep);
            }
        }

        public IEnumerable<ComponentBase> PoultryComponents
        {
            get
            {
                return this.Components.Where(x => x.ComponentCategory == ComponentCategory.Poultry);
            }
        }

        public IEnumerable<ComponentBase> OtherLivestockComponents
        {
            get
            {
                return this.Components.Where(x => x.ComponentCategory == ComponentCategory.OtherLivestock);
            }
        }

        /// <summary>
        /// A collection of Animal Types on the farm based on the currently added animal components to the farm by the user. If a component is part of a specific group
        /// then a single entry of an animal represents that entire group. The "OtherLiveStock" component category is representing by each individual animal type
        /// inside the category.
        /// </summary>
        public ObservableCollection<AnimalType> FarmAnimalComponentsAnimalTypes { get; set; } = new ObservableCollection<AnimalType>();


        public double TotalAgriculturalArea
        {
            get
            {
                return this.FieldSystemComponents.Where(x => x.GetSingleYearViewItem() != null).Sum(x => x.GetSingleYearViewItem().Area);
            }
        }

        public double TotalIrrigatedArea
        {
            get
            {
                var fieldSystemComponents = this.FieldSystemComponents.Where(x => x.IsIrrigated);
                var sum = fieldSystemComponents.Sum(x => x.GetSingleYearViewItem().Area);

                return sum;
            }
        }

        /// <summary>
        /// Used to indicate the default starting date for the history of a field.
        /// </summary>
        public DateTime CarbonModellingEquilibriumStartDate
        {
            get { return _carbonModellingEquilibriumStartDate; }
            set { SetProperty(ref _carbonModellingEquilibriumStartDate, value); }
        }

        /// <summary>
        /// Used to indicate the end date for the history of a field.
        /// </summary>
        public DateTime CarbonModellingEndDate
        {
            get { return _carbonModellingEndDate; }
            set { SetProperty(ref _carbonModellingEndDate, value); }
        }

        /// <summary>
        /// Used to indicate the default starting year for the history of a field.
        /// </summary>
        public int CarbonModellingEquilibriumYear
        {
            get { return _carbonModellingEquilibriumYear; }
            set { SetProperty(ref _carbonModellingEquilibriumYear, value, () => { this.CarbonModellingEquilibriumStartDate = new DateTime(value, 1, 1); }); }
        }

        public bool ShowFieldSystemResultsAsGrid
        {
            get { return _showFieldSystemResultsAsGrid; }
            set { SetProperty(ref _showFieldSystemResultsAsGrid, value); }
        }

        /// <summary>
        /// Enables the showing/hiding of the list of available components on the component selection view. This allows users with smaller screen sizes to hide this
        /// additional information.
        /// </summary>
        public bool ShowAvailableComponentsList
        {
            get
            {
                return _showAvailableComponentsList;
            }

            set
            {
                SetProperty(ref _showAvailableComponentsList, value);
            }
        }

        public Defaults Defaults
        {
            get { return _defaults; }
            set { SetProperty(ref _defaults, value); }
        }

        /// <summary>
        /// For CLI, this is the path to the directory that contains all the input files for the farm.
        /// </summary>
        public string CliInputPath { get; set; }

        /// <summary>
        /// For CLI, the name of the settings file for the farm.
        /// </summary>
        public string SettingsFileName { get; set; }

        /// <summary>
        /// The path to a custom daily climate data file.
        /// </summary>
        public string ClimateDataFileName { get; set; }

        /// <summary>
        /// Climate data for the farm
        /// </summary>
        public ClimateData ClimateData
        {
            get { return _climateData; }
            set { SetProperty(ref _climateData, value); }
        }

        public string PathToYieldInputFile
        {
            get { return _pathToYieldInputFile; }
            set { SetProperty(ref _pathToYieldInputFile, value); }
        }

        /// <summary>
        /// Determines if the details view should be displayed based on the components in the <see cref="Farm"/>.
        /// </summary>        
        public bool HasComponentsThatRequireDetailsView()
        {
            return this.Components.OfType<FieldSystemComponent>().Any();
        }

        public double StartingSoilOrganicCarbon
        {
            get { return _startingSoilOrganicCarbon; }
            set { SetProperty(ref _startingSoilOrganicCarbon, value); }
        }

        public bool UseCustomStartingSoilOrganicCarbon
        {
            get { return _useCustomStartingSoilOrganicCarbon; }
            set { SetProperty(ref _useCustomStartingSoilOrganicCarbon, value); }
        }

        /// <summary>
        /// When enabled, will show a simplified results screen instead of the composite results view (with the various associated tabs). Also, enabling this will skip over the details
        /// view. Detail view items must still be created when skipping over the details view.
        /// </summary>
        public bool ShowSimplifiedResults
        {
            get { return _showSimplifiedResults; }
            set { SetProperty(ref _showSimplifiedResults, value); }
        }

        public bool HasComponents
        {
            get { return this.Components.Any(); }
        }

        /// <summary>
        /// This is the total amount of bales that were produced by harvests on the farm from all fields
        /// </summary>
        public int TotalBalesProducedByFarm
        {
            get => _totalBalesProducedByFarm;
            set => SetProperty(ref _totalBalesProducedByFarm, value, OnTotalBalesProducedByFarmChanged);
        }

        /// <summary>
        /// This is the total amount of bales that are remaining after placing bales on pasture for supplemental feed to grazing animals
        /// </summary>
        public int TotalBalesRemainingOnFarm
        {
            get => _totalBalesRemainingOnFarm;
            set => SetProperty(ref _totalBalesRemainingOnFarm, value);
        }

        /// <summary>
        /// The total number of bales that have been applied to all fields on the farm.
        /// </summary>
        public int TotalNumberOfBalesAppliedToFarm
        {
            get => _totalNumberOfBalesAppliedToFarm;
            set => SetProperty(ref _totalNumberOfBalesAppliedToFarm, value);
        }

        public bool FarmHasBales
        {
            get => _farmHasBales;
            set => SetProperty(ref _farmHasBales, value);
        }

        public int MapZoomLevel { get; set; }

        public bool UseFieldLevelYieldAssignement { get; set; }

        public bool IsCommandLineMode
        {
            get => _isCommandLineMode;
            set => SetProperty(ref _isCommandLineMode, value);
        }

        public bool UseCustomRunInTillage
        {
            get => _useCustomRunInTillage;
            set => SetProperty(ref _useCustomRunInTillage, value);
        }

        #endregion

        #region Public Methods

        public double GetTotalAreaOfFarm(bool includeNativeGrasslands, int year)
        {
            var fields = this.FieldSystemComponents.Where(x => x.CropViewItems.All(y => y.CropType.IsNativeGrassland() == includeNativeGrasslands));

            var area = fields.Sum(x => x.FieldArea);
            if (area > 0)
            {
                return area;
            }
            else
            {
                return 1;
            }
        }

        public List<int> GetYearsWithAnimals()
        {
            var years = new List<int>();

            foreach (var animalComponentBase in this.AnimalComponents)
            {
                foreach (var animalGroup in animalComponentBase.Groups)
                {
                    foreach (var animalGroupManagementPeriod in animalGroup.ManagementPeriods)
                    {
                        var startYear = animalGroupManagementPeriod.Start.Year;
                        var endYear = animalGroupManagementPeriod.End.Year;

                        if (years.Contains(startYear) == false)
                        {
                            years.Add(startYear);
                        }

                        if (years.Contains(endYear) == false)
                        {
                            years.Add(endYear);
                        }
                    }
                }
            }

            return years;
        }

        public List<ManureStateType> GetManureStateTypesInUseOnFarm(AnimalType animalType)
        {
            var componentCategory = animalType.GetComponentCategoryFromAnimalType();
            var components = new List<AnimalComponentBase>();

            if (componentCategory == ComponentCategory.BeefProduction)
            {
                components.AddRange(this.BeefCattleComponents.Cast<AnimalComponentBase>());
            }
            else if (componentCategory == ComponentCategory.Dairy)
            {
                components.AddRange(this.DairyComponents.Cast<AnimalComponentBase>());
            }
            else if (componentCategory == ComponentCategory.Swine)
            {
                components.AddRange(this.SwineComponents.Cast<AnimalComponentBase>());
            }
            else if (componentCategory == ComponentCategory.Sheep)
            {
                components.AddRange(this.SheepComponents.Cast<AnimalComponentBase>());
            }
            else if (componentCategory == ComponentCategory.Poultry)
            {
                components.AddRange(this.PoultryComponents.Cast<AnimalComponentBase>());
            }
            else
            {
                components.AddRange(this.OtherLivestockComponents.Cast<AnimalComponentBase>());
            }

            var stateTypes = new List<ManureStateType>();
            foreach (var animalComponentBase in components)
            {
                foreach (var animalGroup in animalComponentBase.Groups)
                {
                    foreach (var animalGroupManagementPeriod in animalGroup.ManagementPeriods)
                    {
                        var manureHandlingSystem = animalGroupManagementPeriod.ManureDetails.StateType;
                        stateTypes.Add(manureHandlingSystem);
                    }
                }
            }

            return stateTypes.Distinct().ToList();
        }

        /// <summary>
        /// When user add/removes/edits a manure application, the animal results cache needs to be cleared so that we recalculate manure spreading emissions
        /// </summary>
        public void ResetAnimalResults()
        {
            foreach (var animalComponent in this.AnimalComponents)
            {
                animalComponent.ResultsCalculated = false;
            }
        }

        /// <summary>
        /// Returns the total volume of all manure applications made on a particular date using a particular type of manure (beef, dairy, etc.)
        /// </summary>
        public double GetTotalVolumeOfManureAppliedByDate(DateTime dateTime, AnimalType animalType)
        {
            var result = 0.0;

            foreach (var fieldSystemComponent in this.FieldSystemComponents)
            {
                var viewItem = fieldSystemComponent.GetSingleYearViewItem();
                var manureApplications = viewItem.ManureApplicationViewItems.Where(x => x.DateOfApplication.Date.Equals(dateTime.Date)
                    && x.AnimalType == animalType
                    && x.ManureLocationSourceType == ManureLocationSourceType.Livestock);

                foreach (var manureApplicationViewItem in manureApplications)
                {
                    // Get volume multiplied by field area
                    var amount = manureApplicationViewItem.AmountOfManureAppliedPerHectare;
                    var totalVolumeOfApplication = amount * viewItem.Area;

                    result += totalVolumeOfApplication;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if a component exists for this farm with the associated GUID.
        /// </summary>
        public bool ComponentExists(Guid guid)
        {
            return this.Components.Any(component => component.Guid == guid);
        }

        /// <summary>
        /// Determines if an animal group exists in one of the animal components on the farm.
        /// </summary>
        public bool AnimalGroupExists(Guid guid)
        {
            foreach (var componentBase in this.Components)
            {
                if (componentBase is AnimalComponentBase animalComponent)
                {
                    foreach (var animalGroup in animalComponent.Groups)
                    {
                        if (animalGroup.Guid == guid)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// When a farm is initialized, defaults are assigned to the farm. The user can then change these values if they wish. This data is therefore held here in the farm object since it is specific
        /// to this farm instance and lookups should be made here and not from the provider class since this method will persist changes.
        /// </summary>
        public Table_30_Default_Bedding_Material_Composition_Data GetBeddingMaterialComposition(
            BeddingMaterialType beddingMaterialType,
            AnimalType animalType)
        {
            var result = new Table_30_Default_Bedding_Material_Composition_Data();

            AnimalType animalLookupType;
            if (animalType.IsBeefCattleType())
            {
                animalLookupType = AnimalType.Beef;
            }
            else if (animalType.IsDairyCattleType())
            {
                animalLookupType = AnimalType.Dairy;
            }
            else if (animalType.IsSheepType())
            {
                animalLookupType = AnimalType.Sheep;
            }
            else if (animalType.IsSwineType())
            {
                animalLookupType = AnimalType.Swine;
            }
            else if (animalType.IsPoultryType())
            {
                animalLookupType = AnimalType.Poultry;
            }
            else
            {
                // Other animals have a value for animal group (Horses, Goats, etc.)
                animalLookupType = animalType;
            }

            result = this.DefaultsCompositionOfBeddingMaterials.SingleOrDefault(x => x.BeddingMaterial == beddingMaterialType && x.AnimalType == animalLookupType);
            if (result != null)
            {
                return result;
            }
            else
            {
                Trace.TraceError($"{nameof(Farm)}.{nameof(GetBeddingMaterialComposition)}: unable to return bedding material data for {animalType.GetDescription()}, and {beddingMaterialType.GetHashCode()}. Returning default value of 1.");

                return new Table_30_Default_Bedding_Material_Composition_Data();
            }
        }

        public void ClearStageStates()
        {
            foreach (var stageState in this.StageStates)
            {
                stageState.ClearState();
            }
        }

        public int GetStartYearOfEarliestRotation()
        {
            var stageState = this.StageStates.OfType<FieldSystemDetailsStageState>().Single();

            return stageState.DetailsScreenViewCropViewItems.Select(x => x.Year).Min();
        }

        public int GetEndYearOfEarliestRotation()
        {
            var stageState = this.StageStates.OfType<FieldSystemDetailsStageState>().Single();

            return stageState.DetailsScreenViewCropViewItems.Select(x => x.Year).Max();
        }

        public void Initialize()
        {
            this.IsInitialized = true;
        }

        public ChosenClimateAcquisition ClimateAcquisitionStringToEnum(string climateAcquisitionString)
        {
            if (climateAcquisitionString == "Custom")
            {
                return ChosenClimateAcquisition.Custom;
            }
            else if (climateAcquisitionString == "NASA")
            {
                return ChosenClimateAcquisition.NASA;
            }
            else
            {
                return ChosenClimateAcquisition.SLC;
            }
        }

        public FieldSystemComponent GetFieldSystemComponent(Guid guid)
        {
            var result = this.FieldSystemComponents.SingleOrDefault(x => x.Guid == guid);

            return result;
        }

        public double GetTotalAreaOfFarmOccupiedByAnnualCrops()
        {
            // TODO: need to do a check on fallow field components so that total past fallow area (from all components) must be less than or equal to
            // total current fallow area plus the total annual area of the farm (from all components)

            var totalArea = 0d;

            foreach (var fieldSystemComponent in this.FieldSystemComponents)
            {
                var singleYearViewItem = fieldSystemComponent.GetSingleYearViewItem();
                if (singleYearViewItem != null)
                {
                    totalArea += singleYearViewItem.Area;
                }
            }

            return totalArea;
        }

        public double GetAnnualPrecipitation(int year)
        {
            return this.ClimateData.GetTotalPrecipitationForYear(year);
        }

        public double GetAnnualEvapotranspiration(int year)
        {
            return this.ClimateData.GetTotalEvapotranspirationForYear(year);
        }

        public double GetGrowingSeasonPrecipitation(int year)
        {
            return this.ClimateData.GetGrowingSeasonPrecipitation(year);
        }

        public double GetGrowingSeasonEvapotranspiration(int year)
        {
            return this.ClimateData.GetGrowingSeasonEvapotranspiration(year);
        }

        /// <summary>
        /// Returns all manure application made on this farm
        /// </summary>
        /// <param name="monthsAndDaysData">The month to check for manure applications</param>
        /// <param name="animalType">The type of animal manure</param>
        /// <returns></returns>
        public IList<ManureApplicationViewItem> GetManureApplicationsForMonth(
            MonthsAndDaysData monthsAndDaysData,
            AnimalType animalType)
        {
            var fields = this.FieldSystemComponents.ToList();
            var crops = fields.Select(field => field.GetSingleYearViewItem()).ToList();
            var manureApplications = crops.SelectMany(crop => crop.ManureApplicationViewItems).ToList();
            var manureApplicationsByManureType = manureApplications.Where(application => application.AnimalType == animalType).ToList();
            var manureApplicationsInMonth = manureApplicationsByManureType.Where(manureApplication => monthsAndDaysData.DateIsInMonth(manureApplication.DateOfApplication)).ToList();

            return manureApplicationsInMonth;
        }

        public double GetTotalVolumeAppliedOfManureAppliedForMonth(
            MonthsAndDaysData monthsAndDaysData,
            AnimalType animalType)
        {
            var result = 0.0;

            var animalSearchCategory = animalType.GetCategory();

            foreach (var fieldSystemComponent in this.FieldSystemComponents)
            {
                var crop = fieldSystemComponent.GetSingleYearViewItem();
                foreach (var manureApplicationViewItem in crop.ManureApplicationViewItems)
                {
                    var dateIsInMonth = monthsAndDaysData.DateIsInMonth(manureApplicationViewItem.DateOfApplication);
                    var categoryOfManureApplication = manureApplicationViewItem.AnimalType.GetCategory();
                    if (categoryOfManureApplication == animalSearchCategory && dateIsInMonth)
                    {
                        result += manureApplicationViewItem.AmountOfManureAppliedPerHectare * crop.Area;
                    }
                }
            }

            return result;
        }

        public FieldSystemDetailsStageState GetFieldSystemDetailsStageState()
        {
            var stageState = this.StageStates.OfType<FieldSystemDetailsStageState>().SingleOrDefault();
            if (stageState != null)
            {
                return stageState;
            }
            else
            {
                return new FieldSystemDetailsStageState();
            }
        }

        public ObservableCollection<ErrorInformation> GetErrors()
        {
            // Iterate over each component (Farm.Components) in the farm and (create this method first) call GetErrors() from each component. 
            // Then combine the list errors and return from here to the caller

            var errors = new ObservableCollection<ErrorInformation>();
            foreach (var component in Components)
            {
                if (component is FieldSystemComponent comp)
                {
                    errors.AddRange(comp.GetErrors());
                }
            }
            return errors;
        }

        public bool HasCoordinates()
        {
            return this.Latitude != 0 && this.Latitude != 0;
        }

        public Diet GetDietByName(DietType dietType)
        {
            var result = this.Diets.FirstOrDefault(x => x.DietType == dietType);
            if (result != null)
            {
                return result;
            }
            else
            {
                // Old farms will not have newly added diets - return a catch-all diet in this case
                return this.Diets.FirstOrDefault(x => x.AnimalType == AnimalType.NotSelected);
            }
        }

        /// <summary>
        /// Returns a list of <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> for a given <see cref="FieldSystemComponent"/>.
        /// </summary>
        public IList<CropViewItem> GetDetailViewItemsForField(FieldSystemComponent fieldSystemComponent)
        {
            var detailsStageState = this.GetFieldSystemDetailsStageState();
            if (detailsStageState == null)
            {
                return new List<CropViewItem>();
            }
            else
            {
                return detailsStageState.GetMainCropsByField(fieldSystemComponent);
            }
        }

        public IList<CropViewItem> GetCropDetailViewItems()
        {
            return this.GetFieldSystemDetailsStageState().DetailsScreenViewCropViewItems;
        }

        public List<CropViewItem> GetCropViewItemsByYear(int year, bool includeNativeGrassland = false)
        {
            var result = new List<CropViewItem>();

            foreach (var fieldSystemComponent in this.FieldSystemComponents)
            {
                foreach (var cropViewItem in fieldSystemComponent.CropViewItems.Where(x => x.Year == year && x.CropType.IsNativeGrassland() == includeNativeGrassland))
                {
                    result.Add(cropViewItem);
                }
            }

            return result;
        }

        public List<CropViewItem> GetCropDetailViewItemsByYear(int year, bool includeRangeland)
        {
            if (includeRangeland)
            {
                return this.GetFieldSystemDetailsStageState().DetailsScreenViewCropViewItems.Where(x => x.Year == year).ToList();
            }
            else
            {
                return this.GetFieldSystemDetailsStageState().DetailsScreenViewCropViewItems.Where(x => x.Year == year && x.IsNativeGrassland == false).ToList();
            }
        }

        public List<int> GetListOfActiveYears()
        {
            var result = new List<int>();

            // Fields
            var stageState = this.GetFieldSystemDetailsStageState();
            var distinctYears = stageState.DetailsScreenViewCropViewItems.Select(x => x.Year).Distinct().ToList();
            result.AddRange(distinctYears);

            // Animals
            foreach (var animalComponentBase in this.AnimalComponents)
            {
                foreach (var animalGroup in animalComponentBase.Groups)
                {
                    foreach (var animalGroupManagementPeriod in animalGroup.ManagementPeriods)
                    {
                        if (result.Contains(animalGroupManagementPeriod.Start.Year) == false)
                        {
                            result.Add(animalGroupManagementPeriod.Start.Year);
                        }


                        if (result.Contains(animalGroupManagementPeriod.End.Year) == false)
                        {
                            result.Add(animalGroupManagementPeriod.End.Year);
                        }
                    }
                }
            }

            return result;
        }

        public List<ManagementPeriod> GetAllManagementPeriods()
        {
            var result = new List<ManagementPeriod>();

            foreach (var animalComponentBase in this.AnimalComponents)
            {
                foreach (var animalGroup in animalComponentBase.Groups)
                {
                    foreach (var managementPeriod in animalGroup.ManagementPeriods)
                    {
                        result.Add(managementPeriod);
                    }
                }
            }

            return result;
        }

        public SoilData GetPreferredSoilData(CropViewItem cropViewItem)
        {
            var fieldComponent = this.GetFieldSystemComponent(cropViewItem.FieldSystemComponentGuid);
            if (fieldComponent == null || fieldComponent.SoilData == null || fieldComponent.SoilData.PolygonId == 0)
            {
                // Old farms won't have soil data set on fields, use farm level soil data instead

                // Return farm soil type
                return this.DefaultSoilData;
            }
            else
            {
                if (fieldComponent.UseFieldLevelSoilData)
                {
                    return fieldComponent.SoilData;
                }
                else
                {
                    return this.DefaultSoilData;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnTotalBalesProducedByFarmChanged()
        {
            this.FarmHasBales = this.TotalBalesProducedByFarm > 0;
        }

        private void FarmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(this.IsBasicMode), StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.IsBasicMode)
                {
                    this.EnableCarbonModelling = false;
                    this.ShowSimplifiedResults = true;
                    this.ShowDetailsOnComponentSelectionView = false;
                }
                else
                {
                    this.ShowSimplifiedResults = false;
                }
            }
        }

        private void ComponentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.RaisePropertyChanged(nameof(this.HasComponents));
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
            }

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
            }
        }

        #endregion

        #region Private Methods



        #endregion
    }
}