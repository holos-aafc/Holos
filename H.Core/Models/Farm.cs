#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Shelterbelt;
using H.Core.Providers;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Feed;
using H.Core.Providers.Soil;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Models
{
    public partial class Farm : ModelBase
    {
        #region Fields

        public enum ChosenClimateAcquisition
        {
            /// <summary>
            ///     Uses the 'old' default (non-daily) temperatures where normals were used to extract daily values. This is deprecated
            ///     in favor of NASA climate data
            /// </summary>
            SLC,

            /// <summary>
            ///     Used with the CLI where the user can specify default monthly values in a climate settings file
            /// </summary>
            Custom,

            /// <summary>
            ///     Daily climate data is downloaded from NASA website API
            /// </summary>
            NASA,

            /// <summary>
            ///     Used with the CLI where the user can specify default daily values in a custom CSV file
            /// </summary>
            InputFile
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

        private readonly ShelterbeltEnabledFromHardinessZoneConverter _shelterbeltFromHardinessZoneConverter =
            new ShelterbeltEnabledFromHardinessZoneConverter();

        #endregion

        #region Constructors

        public Farm()
        {
            HTraceListener.AddTraceListener();

            Defaults = new Defaults();
            Diets = new ObservableCollection<Diet>();
            DefaultManureCompositionData = new ObservableCollection<DefaultManureCompositionData>();
            DefaultsCompositionOfBeddingMaterials =
                new ObservableCollection<Table_30_Default_Bedding_Material_Composition_Data>();
            YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;
            CarbonModellingEquilibriumYear = CoreConstants.IcbmEquilibriumYear;
            CarbonModellingEquilibriumStartDate = new DateTime(CarbonModellingEquilibriumYear, 1, 1);
            CarbonModellingEndDate = new DateTime(DateTime.Now.Year + CoreConstants.IcbmProjectionPeriod, 12, 31);

            ShowAvailableComponentsList = true;
            ShowSimplifiedResults = false;
            IsBasicMode = false;
            EnableCarbonModelling = true;
            ShowExportEmissionsInFinalReport = true;

            StageStates = new List<StageStateBase>();
            Components.CollectionChanged += ComponentsOnCollectionChanged;

            PropertyChanged += FarmPropertyChanged;

            ClimateData = new ClimateData();
            GeographicData = new GeographicData();
            AnnualSoilN2OBreakdown = new Table_15_Default_Soil_N2O_Emission_BreakDown_Provider();
        }

        /// <summary>
        ///     we need this to set the available timeframes when we set climate acquisition
        /// </summary>
        /// <param name="chosenClimateAcquisition">our type of climate acquisition</param>
        public void SetAvailableTimeFrame(ChosenClimateAcquisition chosenClimateAcquisition)
        {
            if (chosenClimateAcquisition == ChosenClimateAcquisition.NASA)
                AvailableTimeFrames = new List<TimeFrame>
                {
                    TimeFrame.TwoThousandToCurrent,
                    TimeFrame.NineteenNinetyToTwoThousand,
                    TimeFrame.NineteenEightyToNineteenNinety,
                    TimeFrame.ProjectionPeriod
                };
            else if (chosenClimateAcquisition == ChosenClimateAcquisition.SLC)
                AvailableTimeFrames = new List<TimeFrame>
                {
                    TimeFrame.NineteenFiftyToNineteenEighty,
                    TimeFrame.NineteenSixtyToNineteenNinety,
                    TimeFrame.NineteenSeventyToTwoThousand,
                    TimeFrame.NineteenEightyToTwoThousandTen,
                    TimeFrame.NineteenNinetyToTwoThousandSeventeen
                };
        }

        #endregion

        #region Properties

        public bool ResultsCalculated
        {
            get
            {
                if (Components.Any() == false) return false;

                return Components.All(x => x.ResultsCalculated);
            }
        }

        public bool IsBasicMode
        {
            get => _isBasicMode;
            set { SetProperty(ref _isBasicMode, value, () => { RaisePropertyChanged(nameof(IsAdvancedMode)); }); }
        }

        public bool IsAdvancedMode => IsBasicMode == false;

        public List<TimeFrame> AvailableTimeFrames
        {
            get => _availableTimeFrame;
            set => SetProperty(ref _availableTimeFrame, value);
        }

        public YieldAssignmentMethod YieldAssignmentMethod
        {
            get => _yieldAssignmentMethod;
            set => SetProperty(ref _yieldAssignmentMethod, value);
        }

        public bool IsSelectedForComparison { get; set; }

        public ChosenClimateAcquisition ClimateAcquisition
        {
            get => _climateAcquisition;
            set => SetProperty(ref _climateAcquisition, value);
        }

        public MeasurementSystemType MeasurementSystemType
        {
            get => _measurementSystemType;
            set => SetProperty(ref _measurementSystemType, value);
        }

        public bool MeasurementSystemSelected
        {
            get => _measurementSystemSelected;
            set => SetProperty(ref _measurementSystemSelected, value);
        }

        public ObservableCollection<Diet> Diets { get; set; }

        public ObservableCollection<DefaultManureCompositionData> DefaultManureCompositionData { get; set; }

        public ObservableCollection<Table_30_Default_Bedding_Material_Composition_Data>
            DefaultsCompositionOfBeddingMaterials { get; set; }

        /// <summary>
        ///     Indicates the location of the farm
        /// </summary>
        public int PolygonId
        {
            get => _polygonId;
            set => SetProperty(ref _polygonId, value);
        }

        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public List<StageStateBase> StageStates { get; set; }

        /// <summary>
        ///     A collection of all the components the user has selected for this farm.
        /// </summary>
        public ObservableCollection<ComponentBase> Components { get; set; } = new ObservableCollection<ComponentBase>();

        /// <summary>
        ///     The default allocation of total N2O emissions within the year
        /// </summary>
        public Table_15_Default_Soil_N2O_Emission_BreakDown_Provider AnnualSoilN2OBreakdown
        {
            get => _annualSoilN2OBreakdown;
            set => SetProperty(ref _annualSoilN2OBreakdown, value);
        }

        /// <summary>
        ///     This property has to be held in the farm (or storage) and not on base view model. If this property is held in base
        ///     view model, then
        ///     each instance of a view model (timeline, results, etc.) would each have a separate instance of the property and
        ///     that won't work.
        /// </summary>
        public bool EnableDebugDisplay
        {
            get => _enableDebugDisplay;
            set => SetProperty(ref _enableDebugDisplay, value);
        }

        /// <summary>
        ///     Additional user comments for the <see cref="Farm" />.
        /// </summary>
        public string Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }

        /// <summary>
        ///     Indicates which province the user has selected. Do not use this value for calculations/lookups. Use the province
        ///     associated with the
        ///     <see cref="DefaultSoilData" /> instead since the two values for the province could differ if the selected polygon
        ///     spans two polygons.
        /// </summary>
        public Province Province
        {
            get => _province;
            set { SetProperty(ref _province, value, () => { RaisePropertyChanged(nameof(ProvinceDescription)); }); }
        }

        public string ProvinceDescription => Province.GetDescription();

        public GeographicData GeographicData
        {
            get => _geographicData;
            set => SetProperty(ref _geographicData, value);
        }

        /// <summary>
        ///     The default soil data selected by the user if there was more than one soil component found within the selected
        ///     polygon. The user
        ///     has the option to define a field-level soil type as well <see cref="FieldSystemComponent.SoilData" />.
        /// </summary>
        public SoilData DefaultSoilData => GeographicData?.DefaultSoilData;

        public bool ShowAdditionalColumnsOnDietForumtorView
        {
            get => _showAdditionalColumnsOnDietForumtorView;
            set => SetProperty(ref _showAdditionalColumnsOnDietForumtorView, value);
        }

        public bool ShowDefaultDietsInDietFormulator
        {
            get => _showDefaultDietsInDietFormulator;
            set => SetProperty(ref _showDefaultDietsInDietFormulator, value);
        }

        public bool ShowDetailsOnComponentSelectionView
        {
            get => _showDetailsOnComponentSelectionView;
            set => SetProperty(ref _showDetailsOnComponentSelectionView, value);
        }

        public bool EnableCarbonModelling
        {
            get => _enableCarbonModelling;
            set => SetProperty(ref _enableCarbonModelling, value);
        }

        public bool ShowAdditionalInformationInADView
        {
            get => _showAdditionalInformationInADView;
            set => SetProperty(ref _showAdditionalInformationInADView, value);
        }

        /// <summary>
        ///     Checks if hardiness zone data exists for the selected farm. The shelterbelt component is
        ///     only available if we have hardiness zone data available for the selected location.
        ///     Returns True if data is available.
        ///     Return False otherwise.
        /// </summary>
        public bool IsShelterbeltComponentAvailable =>
            _shelterbeltFromHardinessZoneConverter.Convert(GeographicData.HardinessZone);

        public IEnumerable<AnimalComponentBase> AnimalComponents =>
            DairyComponents.Concat(BeefCattleComponents)
                .Concat(SwineComponents)
                .Concat(SheepComponents)
                .Concat(PoultryComponents)
                .Concat(OtherLivestockComponents).Cast<AnimalComponentBase>();

        /// <summary>
        ///     Indicates if the farm has any components in which multi-year inputs are possible
        /// </summary>
        public bool HasMultiYearComponents =>
            FieldSystemComponents.Any() || Components.OfType<ShelterbeltComponent>().Any();

        public IEnumerable<ComponentBase> DairyComponents
        {
            get { return Components.Where(x => x.ComponentCategory == ComponentCategory.Dairy); }
        }

        public IEnumerable<FieldSystemComponent> FieldSystemComponents
        {
            get
            {
                return Components.Where(x => x.GetType() == typeof(FieldSystemComponent)).Cast<FieldSystemComponent>();
            }
        }

        public IEnumerable<AnaerobicDigestionComponent> AnaerobicDigestionComponents => Components
            .Where(x => x.GetType() == typeof(AnaerobicDigestionComponent)).Cast<AnaerobicDigestionComponent>();

        public IEnumerable<ComponentBase> BeefCattleComponents
        {
            get { return Components.Where(x => x.ComponentCategory == ComponentCategory.BeefProduction); }
        }

        public IEnumerable<ComponentBase> SwineComponents
        {
            get { return Components.Where(x => x.ComponentCategory == ComponentCategory.Swine); }
        }

        public IEnumerable<ComponentBase> SheepComponents
        {
            get { return Components.Where(x => x.ComponentCategory == ComponentCategory.Sheep); }
        }

        public IEnumerable<ComponentBase> PoultryComponents
        {
            get { return Components.Where(x => x.ComponentCategory == ComponentCategory.Poultry); }
        }

        public IEnumerable<ComponentBase> OtherLivestockComponents
        {
            get { return Components.Where(x => x.ComponentCategory == ComponentCategory.OtherLivestock); }
        }

        /// <summary>
        ///     A collection of Animal Types on the farm based on the currently added animal components to the farm by the user. If
        ///     a component is part of a specific group
        ///     then a single entry of an animal represents that entire group. The "OtherLiveStock" component category is
        ///     representing by each individual animal type
        ///     inside the category.
        /// </summary>
        public ObservableCollection<AnimalType> FarmAnimalComponentsAnimalTypes { get; set; } =
            new ObservableCollection<AnimalType>();


        public double TotalAgriculturalArea
        {
            get
            {
                return FieldSystemComponents.Where(x => x.GetSingleYearViewItem() != null)
                    .Sum(x => x.GetSingleYearViewItem().Area);
            }
        }

        public double TotalIrrigatedArea
        {
            get
            {
                var fieldSystemComponents = FieldSystemComponents.Where(x => x.IsIrrigated);
                var sum = fieldSystemComponents.Sum(x => x.GetSingleYearViewItem().Area);

                return sum;
            }
        }

        /// <summary>
        ///     Used to indicate the default starting date for the history of a field.
        /// </summary>
        public DateTime CarbonModellingEquilibriumStartDate
        {
            get => _carbonModellingEquilibriumStartDate;
            set => SetProperty(ref _carbonModellingEquilibriumStartDate, value);
        }

        /// <summary>
        ///     Used to indicate the end date for the history of a field.
        /// </summary>
        public DateTime CarbonModellingEndDate
        {
            get => _carbonModellingEndDate;
            set => SetProperty(ref _carbonModellingEndDate, value);
        }

        /// <summary>
        ///     Used to indicate the default starting year for the history of a field.
        /// </summary>
        public int CarbonModellingEquilibriumYear
        {
            get => _carbonModellingEquilibriumYear;
            set
            {
                SetProperty(ref _carbonModellingEquilibriumYear, value,
                    () => { CarbonModellingEquilibriumStartDate = new DateTime(value, 1, 1); });
            }
        }

        public bool ShowFieldSystemResultsAsGrid
        {
            get => _showFieldSystemResultsAsGrid;
            set => SetProperty(ref _showFieldSystemResultsAsGrid, value);
        }


        public Defaults Defaults
        {
            get => _defaults;
            set => SetProperty(ref _defaults, value);
        }

        /// <summary>
        ///     For CLI, this is the path to the directory that contains all the input files for the farm.
        /// </summary>
        public string CliInputPath { get; set; }

        /// <summary>
        ///     For CLI, the name of the settings file for the farm.
        /// </summary>
        public string SettingsFileName { get; set; }

        /// <summary>
        ///     The path to a custom daily climate data file.
        /// </summary>
        public string ClimateDataFileName { get; set; }

        /// <summary>
        ///     Climate data for the farm
        /// </summary>
        public ClimateData ClimateData
        {
            get => _climateData;
            set => SetProperty(ref _climateData, value);
        }

        public string PathToYieldInputFile
        {
            get => _pathToYieldInputFile;
            set => SetProperty(ref _pathToYieldInputFile, value);
        }

        /// <summary>
        ///     Determines if the details view should be displayed based on the components in the <see cref="Farm" />.
        /// </summary>
        public bool HasComponentsThatRequireDetailsView()
        {
            return Components.OfType<FieldSystemComponent>().Any();
        }

        public double StartingSoilOrganicCarbon
        {
            get => _startingSoilOrganicCarbon;
            set => SetProperty(ref _startingSoilOrganicCarbon, value);
        }

        public bool UseCustomStartingSoilOrganicCarbon
        {
            get => _useCustomStartingSoilOrganicCarbon;
            set => SetProperty(ref _useCustomStartingSoilOrganicCarbon, value);
        }

        /// <summary>
        ///     When enabled, will show a simplified results screen instead of the composite results view (with the various
        ///     associated tabs). Also, enabling this will skip over the details
        ///     view. Detail view items must still be created when skipping over the details view.
        /// </summary>
        public bool ShowSimplifiedResults
        {
            get => _showSimplifiedResults;
            set => SetProperty(ref _showSimplifiedResults, value);
        }

        public bool HasComponents => Components.Any();

        /// <summary>
        ///     This is the total amount of bales that were produced by harvests on the farm from all fields
        /// </summary>
        public int TotalBalesProducedByFarm
        {
            get => _totalBalesProducedByFarm;
            set => SetProperty(ref _totalBalesProducedByFarm, value, OnTotalBalesProducedByFarmChanged);
        }

        /// <summary>
        ///     This is the total amount of bales that are remaining after placing bales on pasture for supplemental feed to
        ///     grazing animals
        /// </summary>
        public int TotalBalesRemainingOnFarm
        {
            get => _totalBalesRemainingOnFarm;
            set => SetProperty(ref _totalBalesRemainingOnFarm, value);
        }

        /// <summary>
        ///     The total number of bales that have been applied to all fields on the farm.
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
            var fields = FieldSystemComponents.Where(x =>
                x.CropViewItems.All(y => y.CropType.IsNativeGrassland() == includeNativeGrasslands));

            var area = fields.Sum(x => x.FieldArea);
            if (area > 0) return area;

            return 1;
        }

        public List<int> GetYearsWithAnimals()
        {
            var years = new List<int>();

            foreach (var animalComponentBase in AnimalComponents)
            foreach (var animalGroup in animalComponentBase.Groups)
            foreach (var animalGroupManagementPeriod in animalGroup.ManagementPeriods)
            {
                var startYear = animalGroupManagementPeriod.Start.Year;
                var endYear = animalGroupManagementPeriod.End.Year;

                if (years.Contains(startYear) == false) years.Add(startYear);

                if (years.Contains(endYear) == false) years.Add(endYear);
            }

            return years;
        }

        public List<ManureStateType> GetManureStateTypesInUseOnFarm(AnimalType animalType)
        {
            var componentCategory = animalType.GetComponentCategoryFromAnimalType();
            var components = new List<AnimalComponentBase>();

            if (componentCategory == ComponentCategory.BeefProduction)
                components.AddRange(BeefCattleComponents.Cast<AnimalComponentBase>());
            else if (componentCategory == ComponentCategory.Dairy)
                components.AddRange(DairyComponents.Cast<AnimalComponentBase>());
            else if (componentCategory == ComponentCategory.Swine)
                components.AddRange(SwineComponents.Cast<AnimalComponentBase>());
            else if (componentCategory == ComponentCategory.Sheep)
                components.AddRange(SheepComponents.Cast<AnimalComponentBase>());
            else if (componentCategory == ComponentCategory.Poultry)
                components.AddRange(PoultryComponents.Cast<AnimalComponentBase>());
            else
                components.AddRange(OtherLivestockComponents.Cast<AnimalComponentBase>());

            var stateTypes = new List<ManureStateType>();
            foreach (var animalComponentBase in components)
            foreach (var animalGroup in animalComponentBase.Groups)
            foreach (var animalGroupManagementPeriod in animalGroup.ManagementPeriods)
            {
                var manureHandlingSystem = animalGroupManagementPeriod.ManureDetails.StateType;
                stateTypes.Add(manureHandlingSystem);
            }

            return stateTypes.Distinct().ToList();
        }

        /// <summary>
        ///     When user add/removes/edits a manure application, the animal results cache needs to be cleared so that we
        ///     recalculate manure spreading emissions
        /// </summary>
        public void ResetAnimalResults()
        {
            foreach (var animalComponent in AnimalComponents) animalComponent.ResultsCalculated = false;
        }

        /// <summary>
        ///     Returns the total volume of all manure applications made on a particular date using a particular type of manure
        ///     (beef, dairy, etc.)
        /// </summary>
        public double GetTotalVolumeOfManureAppliedByDate(DateTime dateTime, AnimalType animalType)
        {
            var result = 0.0;

            foreach (var fieldSystemComponent in FieldSystemComponents)
            {
                var viewItem = fieldSystemComponent.GetSingleYearViewItem();
                var manureApplications = viewItem.ManureApplicationViewItems.Where(x =>
                    x.DateOfApplication.Date.Equals(dateTime.Date)
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
        ///     Determines if a component exists for this farm with the associated GUID.
        /// </summary>
        public bool ComponentExists(Guid guid)
        {
            return Components.Any(component => component.Guid == guid);
        }

        /// <summary>
        ///     Determines if an animal group exists in one of the animal components on the farm.
        /// </summary>
        public bool AnimalGroupExists(Guid guid)
        {
            foreach (var componentBase in Components)
                if (componentBase is AnimalComponentBase animalComponent)
                    foreach (var animalGroup in animalComponent.Groups)
                        if (animalGroup.Guid == guid)
                            return true;

            return false;
        }

        /// <summary>
        ///     When a farm is initialized, defaults are assigned to the farm. The user can then change these values if they wish.
        ///     This data is therefore held here in the farm object since it is specific
        ///     to this farm instance and lookups should be made here and not from the provider class since this method will
        ///     persist changes.
        /// </summary>
        public Table_30_Default_Bedding_Material_Composition_Data GetBeddingMaterialComposition(
            BeddingMaterialType beddingMaterialType,
            AnimalType animalType)
        {
            var result = new Table_30_Default_Bedding_Material_Composition_Data();

            AnimalType animalLookupType;
            if (animalType.IsBeefCattleType())
                animalLookupType = AnimalType.Beef;
            else if (animalType.IsDairyCattleType())
                animalLookupType = AnimalType.Dairy;
            else if (animalType.IsSheepType())
                animalLookupType = AnimalType.Sheep;
            else if (animalType.IsSwineType())
                animalLookupType = AnimalType.Swine;
            else if (animalType.IsPoultryType())
                animalLookupType = AnimalType.Poultry;
            else
                // Other animals have a value for animal group (Horses, Goats, etc.)
                animalLookupType = animalType;

            result = DefaultsCompositionOfBeddingMaterials.SingleOrDefault(x =>
                x.BeddingMaterial == beddingMaterialType && x.AnimalType == animalLookupType);
            if (result != null) return result;

            Trace.TraceError(
                $"{nameof(Farm)}.{nameof(GetBeddingMaterialComposition)}: unable to return bedding material data for {animalType.GetDescription()}, and {beddingMaterialType.GetHashCode()}. Returning default value of 1.");

            return new Table_30_Default_Bedding_Material_Composition_Data();
        }

        public void ClearStageStates()
        {
            foreach (var stageState in StageStates) stageState.ClearState();
        }

        public int GetStartYearOfEarliestRotation()
        {
            var stageState = StageStates.OfType<FieldSystemDetailsStageState>().Single();

            if (stageState.DetailsScreenViewCropViewItems.Any() == false)
                return ClimateData.DailyClimateData.Min(x => x.Date.Year);

            return stageState.DetailsScreenViewCropViewItems.Select(x => x.Year).Min();
        }

        public int GetEndYearOfEarliestRotation()
        {
            var stageState = StageStates.OfType<FieldSystemDetailsStageState>().Single();

            if (stageState.DetailsScreenViewCropViewItems.Any() == false)
                return ClimateData.DailyClimateData.Max(x => x.Date.Year);

            return stageState.DetailsScreenViewCropViewItems.Select(x => x.Year).Max();
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public ChosenClimateAcquisition ClimateAcquisitionStringToEnum(string climateAcquisitionString)
        {
            if (climateAcquisitionString == "Custom") return ChosenClimateAcquisition.Custom;

            if (climateAcquisitionString == "NASA") return ChosenClimateAcquisition.NASA;

            if (climateAcquisitionString == "InputFile") return ChosenClimateAcquisition.InputFile;

            return ChosenClimateAcquisition.SLC;
        }

        public FieldSystemComponent GetFieldSystemComponent(Guid guid)
        {
            var result = FieldSystemComponents.SingleOrDefault(x => x.Guid == guid);

            return result;
        }

        public double GetTotalAreaOfFarmOccupiedByAnnualCrops()
        {
            // TODO: need to do a check on fallow field components so that total past fallow area (from all components) must be less than or equal to
            // total current fallow area plus the total annual area of the farm (from all components)

            var totalArea = 0d;

            foreach (var fieldSystemComponent in FieldSystemComponents)
            {
                var singleYearViewItem = fieldSystemComponent.GetSingleYearViewItem();
                if (singleYearViewItem != null) totalArea += singleYearViewItem.Area;
            }

            return totalArea;
        }

        public double GetAnnualPrecipitation(int year)
        {
            return ClimateData.GetTotalPrecipitationForYear(year);
        }

        public double GetAnnualEvapotranspiration(int year)
        {
            return ClimateData.GetTotalEvapotranspirationForYear(year);
        }

        public double GetGrowingSeasonPrecipitation(int year)
        {
            return ClimateData.GetGrowingSeasonPrecipitation(year);
        }

        public double GetGrowingSeasonEvapotranspiration(int year)
        {
            return ClimateData.GetGrowingSeasonEvapotranspiration(year);
        }

        /// <summary>
        ///     Returns all manure application made on this farm
        /// </summary>
        /// <param name="monthsAndDaysData">The month to check for manure applications</param>
        /// <param name="animalType">The type of animal manure</param>
        /// <returns></returns>
        public IList<ManureApplicationViewItem> GetManureApplicationsForMonth(
            MonthsAndDaysData monthsAndDaysData,
            AnimalType animalType)
        {
            var fields = FieldSystemComponents.ToList();
            var crops = fields.Select(field => field.GetSingleYearViewItem()).ToList();
            var manureApplications = crops.SelectMany(crop => crop.ManureApplicationViewItems).ToList();
            var manureApplicationsByManureType =
                manureApplications.Where(application => application.AnimalType == animalType).ToList();
            var manureApplicationsInMonth = manureApplicationsByManureType.Where(manureApplication =>
                monthsAndDaysData.DateIsInMonth(manureApplication.DateOfApplication)).ToList();

            return manureApplicationsInMonth;
        }

        public double GetTotalVolumeAppliedOfManureAppliedForMonth(
            MonthsAndDaysData monthsAndDaysData,
            AnimalType animalType)
        {
            var result = 0.0;

            var animalSearchCategory = animalType.GetCategory();

            foreach (var fieldSystemComponent in FieldSystemComponents)
            {
                var crop = fieldSystemComponent.GetSingleYearViewItem();
                foreach (var manureApplicationViewItem in crop.ManureApplicationViewItems)
                {
                    var dateIsInMonth = monthsAndDaysData.DateIsInMonth(manureApplicationViewItem.DateOfApplication);
                    var categoryOfManureApplication = manureApplicationViewItem.AnimalType.GetCategory();
                    if (categoryOfManureApplication == animalSearchCategory && dateIsInMonth)
                        result += manureApplicationViewItem.AmountOfManureAppliedPerHectare * crop.Area;
                }
            }

            return result;
        }

        public FieldSystemDetailsStageState GetFieldSystemDetailsStageState()
        {
            var stageState = StageStates.OfType<FieldSystemDetailsStageState>().SingleOrDefault();
            if (stageState != null) return stageState;

            return new FieldSystemDetailsStageState();
        }

        public ObservableCollection<ErrorInformation> GetErrors()
        {
            // Iterate over each component (Farm.Components) in the farm and (create this method first) call GetErrors() from each component. 
            // Then combine the list errors and return from here to the caller

            var errors = new ObservableCollection<ErrorInformation>();
            foreach (var component in Components)
                if (component is FieldSystemComponent comp)
                    errors.AddRange(comp.GetErrors());

            return errors;
        }

        public bool HasCoordinates()
        {
            return Latitude != 0 && Latitude != 0;
        }

        public Diet GetDietByName(DietType dietType)
        {
            var result = Diets.FirstOrDefault(x => x.DietType == dietType);
            if (result != null) return result;

            // Old farms will not have newly added diets - return a catch-all diet in this case
            return Diets.FirstOrDefault(x => x.AnimalType == AnimalType.NotSelected);
        }

        /// <summary>
        ///     Returns a list of <see cref="H.Core.Models.LandManagement.Fields.CropViewItem" /> for a given
        ///     <see cref="FieldSystemComponent" />.
        /// </summary>
        public IList<CropViewItem> GetDetailViewItemsForField(FieldSystemComponent fieldSystemComponent)
        {
            var detailsStageState = GetFieldSystemDetailsStageState();
            if (detailsStageState == null) return new List<CropViewItem>();

            return detailsStageState.GetMainCropsByField(fieldSystemComponent);
        }

        public IList<CropViewItem> GetCropDetailViewItems()
        {
            return GetFieldSystemDetailsStageState().DetailsScreenViewCropViewItems;
        }

        public List<CropViewItem> GetCropViewItemsByYear(int year, bool includeNativeGrassland = false)
        {
            var result = new List<CropViewItem>();

            foreach (var fieldSystemComponent in FieldSystemComponents)
            foreach (var cropViewItem in fieldSystemComponent.CropViewItems.Where(x =>
                         x.Year == year && x.CropType.IsNativeGrassland() == includeNativeGrassland))
                result.Add(cropViewItem);

            return result;
        }

        public List<CropViewItem> GetCropDetailViewItemsByYear(int year, bool includeRangeland)
        {
            if (includeRangeland)
                return GetFieldSystemDetailsStageState().DetailsScreenViewCropViewItems.Where(x => x.Year == year)
                    .ToList();

            return GetFieldSystemDetailsStageState().DetailsScreenViewCropViewItems
                .Where(x => x.Year == year && x.IsNativeGrassland == false).ToList();
        }

        public List<int> GetListOfActiveYears()
        {
            var result = new List<int>();

            // Fields
            var stageState = GetFieldSystemDetailsStageState();
            var distinctYears = stageState.DetailsScreenViewCropViewItems.Select(x => x.Year).Distinct().ToList();
            result.AddRange(distinctYears);

            // Animals
            foreach (var animalComponentBase in AnimalComponents)
            foreach (var animalGroup in animalComponentBase.Groups)
            foreach (var animalGroupManagementPeriod in animalGroup.ManagementPeriods)
            {
                var startYear = animalGroupManagementPeriod.Start.Year;
                var endYear = animalGroupManagementPeriod.End.Year;

                for (var i = startYear; i <= endYear; i++)
                    if (result.Contains(i) == false)
                        result.Add(i);
            }

            return result;
        }

        public List<ManagementPeriod> GetAllManagementPeriods()
        {
            var result = new List<ManagementPeriod>();

            foreach (var animalComponentBase in AnimalComponents)
            foreach (var animalGroup in animalComponentBase.Groups)
            foreach (var managementPeriod in animalGroup.ManagementPeriods)
                result.Add(managementPeriod);

            return result;
        }

        public SoilData GetPreferredSoilData(CropViewItem cropViewItem)
        {
            var fieldComponent = GetFieldSystemComponent(cropViewItem.FieldSystemComponentGuid);
            if (fieldComponent == null || fieldComponent.SoilData == null || fieldComponent.SoilData.PolygonId == 0)
                // Old farms won't have soil data set on fields, use farm level soil data instead
                // Return farm soil type
                return DefaultSoilData;

            if (fieldComponent.UseFieldLevelSoilData) return fieldComponent.SoilData;

            return DefaultSoilData;
        }

        public List<CropViewItem> GetAllCropViewItems()
        {
            var result = new List<CropViewItem>();

            foreach (var fieldSystemComponent in FieldSystemComponents)
            {
                foreach (var viewItem in fieldSystemComponent.CropViewItems) result.Add(viewItem);

                foreach (var viewItem in fieldSystemComponent.CoverCrops) result.Add(viewItem);
            }

            var stageState = GetFieldSystemDetailsStageState();
            foreach (var viewItem in stageState.DetailsScreenViewCropViewItems) result.Add(viewItem);

            return result;
        }

        public List<HayImportViewItem> GetHayImportsUsingImportedHayFromSourceField(
            FieldSystemComponent fieldUsedAsSource)
        {
            var result = new List<HayImportViewItem>();

            foreach (var fieldSystemComponent in FieldSystemComponents)
            foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
            {
                var hayImports =
                    cropViewItem.HayImportViewItems.Where(x => x.FieldSourceGuid.Equals(fieldUsedAsSource.Guid));
                result.AddRange(hayImports);
            }

            return result;
        }

        public List<HayImportViewItem> GetHayImportsUsingImportedHayFromSourceField(Guid fieldSystemGuid)
        {
            var field = GetFieldSystemComponent(fieldSystemGuid);
            if (field != null) return GetHayImportsUsingImportedHayFromSourceField(field);

            return new List<HayImportViewItem>();
        }

        /// <summary>
        ///     Returns all <see cref="HayImportViewItem" /> that have used harvests from the <see cref="FieldSystemComponent" />
        ///     identified by the <see cref="Guid" />.
        /// </summary>
        public List<HayImportViewItem> GetHayImportsUsingImportedHayFromSourceFieldByYear(Guid fieldSystemGuid,
            int year)
        {
            var field = GetFieldSystemComponent(fieldSystemGuid);
            if (field != null)
            {
                return GetHayImportsUsingImportedHayFromSourceField(field).Where(x => x.Start.Year.Equals(year))
                    .ToList();
                ;
            }

            return new List<HayImportViewItem>();
        }

        #endregion

        #region Event Handlers

        private void OnTotalBalesProducedByFarmChanged()
        {
            FarmHasBales = TotalBalesProducedByFarm > 0;
        }

        private void FarmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IsBasicMode), StringComparison.InvariantCultureIgnoreCase))
            {
                if (IsBasicMode)
                {
                    EnableCarbonModelling = false;
                    ShowSimplifiedResults = true;
                    ShowDetailsOnComponentSelectionView = false;
                }
                else
                {
                    ShowSimplifiedResults = false;
                }
            }
        }

        private void ComponentsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            RaisePropertyChanged(nameof(HasComponents));
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
            }

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
            }
        }

        #endregion
    }
}