#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Properties;
using H.Core.Providers.Soil;

#endregion

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// </summary>
    public class FieldSystemComponent : ComponentBase
    {
        #region Constructors

        public FieldSystemComponent()
        {
            ComponentNameDisplayString = Resources.LabelField;
            ComponentDescriptionString = Resources.ToolTipFieldsComponent;
            ComponentCategory = ComponentCategory.LandManagement;
            ComponentType = ComponentType.Field;

            FieldArea = 1.0;

            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;

            CropViewItems.CollectionChanged -= CropViewItemsOnCollectionChanged;
            CropViewItems.CollectionChanged += CropViewItemsOnCollectionChanged;

            // Use farm soil data by default (instead of field specific soil data)
            UseFieldLevelSoilData = false;
            SoilData = new SoilData();
            SoilDataAvailableForField = new ObservableCollection<SoilData>();
        }

        #endregion

        #region Private Methods

        private void UpdateTimelineInformationString()
        {
            TimelineInformationString = CropString;
        }

        #endregion

        #region Fields

        private string _fieldName;
        private double _fieldArea;

        private bool _beginOrderingAtStartYearOfRotation;
        private bool _useFieldLevelSoilData;

        private SoilData _soilData;
        private ObservableCollection<SoilData> _soilDataAvailableForField;

        #endregion

        #region Properties

        public ObservableCollection<SoilData> SoilDataAvailableForField
        {
            get => _soilDataAvailableForField;
            set => SetProperty(ref _soilDataAvailableForField, value);
        }

        /// <summary>
        ///     By default, <see cref="Providers.Soil.SoilData" /> associated with the farm will be used (across all fields). This
        ///     flag allows for field-specific <see cref="Providers.Soil.SoilData" />
        ///     to be used.
        /// </summary>
        public bool UseFieldLevelSoilData
        {
            get => _useFieldLevelSoilData;
            set => SetProperty(ref _useFieldLevelSoilData, value);
        }

        /// <summary>
        ///     Allow for field specific soil data (as opposed to one type of soil being used for all fields on the farm)
        /// </summary>
        public SoilData SoilData
        {
            get => _soilData;
            set => SetProperty(ref _soilData, value);
        }

        /// <summary>
        ///     Allow for field specific yield assignments (as opposed to one type of yield assignment used for all fields on the
        ///     farm)
        /// </summary>
        public YieldAssignmentMethod YieldAssignmentMethod { get; set; }

        public ObservableCollection<CropViewItem> CropViewItems { get; set; } =
            new ObservableCollection<CropViewItem>();

        /// <summary>
        ///     This will hold a collection of items that represent winter crops, cover crops, or undersown crops (all of these
        ///     will also be called secondary crops) if any was specified by user. There will always
        ///     be exactly one view item in this collection created for each (main) crop view item added by the user in the
        ///     component selection view.
        /// </summary>
        public ObservableCollection<CropViewItem> CoverCrops { get; set; } = new ObservableCollection<CropViewItem>();

        /// <summary>
        ///     TODO: This is no longer used and should be deleted once it is safe to delete properties on farms without resetting
        ///     all data on user system.
        /// </summary>
        [Obsolete]
        public string FieldName
        {
            get => _fieldName;
            set => SetProperty(ref _fieldName, value);
        }

        /// <summary>
        ///     Allows to user to specify the annual order of the crops they entered
        /// </summary>
        public bool BeginOrderingAtStartYearOfRotation
        {
            get => _beginOrderingAtStartYearOfRotation;
            set => SetProperty(ref _beginOrderingAtStartYearOfRotation, value);
        }

        /// <summary>
        ///     Total area of the field component. Any changes to this property will update all <see cref="CropViewItems" />
        ///     associated with this <see cref="FieldSystemComponent" />.
        /// </summary>
        public double FieldArea
        {
            get => _fieldArea;
            set => SetProperty(ref _fieldArea, value);
        }

        public string CropString
        {
            get { return string.Join(", ", CropViewItems.Select(x => x.CropTypeString)); }
        }

        /// <summary>
        ///     Used by the fields view in the rotation component
        /// </summary>
        public string CropStringWithYears
        {
            get { return string.Join(", ", CropViewItems.Select(x => x.CropTypeStringWithYear)); }
        }

        /// <summary>
        ///     Used by the fields view in the rotation component
        /// </summary>
        public ObservableCollection<CropViewItem> CropViewItemsMaxThree
        {
            get
            {
                var result = new ObservableCollection<CropViewItem>();

                if (BeginOrderingAtStartYearOfRotation)
                    result.AddRange(CropViewItems.OrderBy(x => x.Year).Take(3));
                else
                    result.AddRange(CropViewItems.OrderByDescending(x => x.Year).Take(3));

                return result;
            }
        }

        public string CropStringWithYearsMaxThreeItems
        {
            get
            {
                if (BeginOrderingAtStartYearOfRotation)
                    return string.Join(", ",
                        CropViewItems.OrderBy(x => x.Year).Select(x => x.CropTypeStringWithYear).Take(3));

                return string.Join(", ",
                    CropViewItems.OrderByDescending(x => x.Year).Select(x => x.CropTypeStringWithYear).Take(3));
            }
        }

        public bool IsAnnual
        {
            get
            {
                if (GetSingleYearViewItem() != null &&
                    GetSingleYearViewItem().CropType.IsAnnual())
                    return true;

                return false;
            }
        }

        public bool IsFallow
        {
            get
            {
                if (GetSingleYearViewItem() != null &&
                    GetSingleYearViewItem().CropType.IsFallow())
                    return true;

                return false;
            }
        }

        public bool IsGrassland
        {
            get
            {
                if (GetSingleYearViewItem() != null &&
                    GetSingleYearViewItem().CropType.IsGrassland())
                    return true;

                return false;
            }
        }

        public bool IsNativeGrassland
        {
            get
            {
                if (GetSingleYearViewItem() != null &&
                    GetSingleYearViewItem().CropType.IsGrassland() &&
                    GetSingleYearViewItem().IsNativeGrassland)
                    return true;

                return false;
            }
        }

        public bool IsPerennial
        {
            get
            {
                if (GetSingleYearViewItem() != null &&
                    GetSingleYearViewItem().CropType.IsPerennial())
                    return true;

                return false;
            }
        }

        public bool IsPasture
        {
            get
            {
                if (GetSingleYearViewItem() != null && GetSingleYearViewItem().CropType.IsPasture()) return true;

                return false;
            }
        }

        public bool IsIrrigated
        {
            get
            {
                if (GetSingleYearViewItem() != null && GetSingleYearViewItem().AmountOfIrrigation > 0) return true;

                return false;
            }
        }

        /// <summary>
        ///     A past perennial is a crop that is currently an annual but was previously a perennial (grassland was broken)
        /// </summary>
        public bool IsPastPerennial => IsAnnual && GetSingleYearViewItem().IsBrokenGrassland;

        public bool IsCurrentPerennial => IsPerennial && IsPastPerennial == false;

        public bool IsPartOfRotationComponent { get; set; }

        #endregion

        #region Public Methods

        public List<CropViewItem> GetAllItemsInPerennialStand(Guid groupGuid)
        {
            return CropViewItems.Where(x => x.PerennialStandGroupId == groupGuid).OrderBy(x => x.YearInPerennialStand)
                .ToList();
        }

        /// <summary>
        ///     Since this component supports multi year and single year modes (ICBM vs non-ICBM) this method returns the correct
        ///     view (the most recent item) item when farm is in single-year mode.
        /// </summary>
        /// <returns>The most recent view items for the field, or null if there are not items define for the field</returns>
        public CropViewItem GetSingleYearViewItem()
        {
            var mostRecentViewItem = CropViewItems.OrderByDescending(viewItem => viewItem.Year).FirstOrDefault();

            return mostRecentViewItem;
        }

        public List<ManureApplicationViewItem> GetManureApplicationViewItems(AnimalType animalType)
        {
            var result = new List<ManureApplicationViewItem>();

            foreach (var viewItem in CropViewItems)
            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems.Where(x =>
                         x.AnimalType.GetCategory() == animalType.GetCategory()))
                result.Add(manureApplicationViewItem);

            return result;
        }

        /// <summary>
        ///     Returns the size of first rotation for a field system component. If there are any historical components associated
        ///     with this component,
        ///     the number of crops in the earliest (first) historical system will be returned. If there are no historical
        ///     components, then the number
        ///     of crops associated with the current component will be returned. This value is used when calculating results -
        ///     specifically when calculating
        ///     the equilibrium year as that calculation uses averages from the crops found in the first rotation (component) for
        ///     the field.
        /// </summary>
        public int SizeOfFirstRotationInField()
        {
            var result = 1;

            if (HistoricalComponents.Any())
            {
                var firstHistoricalComponent = HistoricalComponents.OrderBy(x => x.Start).First();
                if (firstHistoricalComponent is FieldSystemComponent fieldSystemComponent)
                    return fieldSystemComponent.CropViewItems.Count;
            }
            else
            {
                return CropViewItems.Count;
            }

            return result;
        }

        public override ObservableCollection<ErrorInformation> GetErrors()
        {
            var errors = new ObservableCollection<ErrorInformation>();
            //if (FieldArea == 0.0)
            //{
            //    errors.Add(new ErrorInformation(H.Core.Properties.Resources.ErrorFieldAreaCannotBeZero));
            //}
            return errors;
        }

        public bool IsGrazingManagementPeriodFromPasture(
            ManagementPeriod managementPeriod)
        {
            var housingDetails = managementPeriod.HousingDetails;
            var isNonNullPasture = housingDetails.PastureLocation != null;
            if (isNonNullPasture == false) return false;

            var isMatchingLocation = false;
            if (string.IsNullOrWhiteSpace(housingDetails.PastureLocation.Name) == false)
                isMatchingLocation = housingDetails.PastureLocation.Name.Equals(Name);
            else
                isMatchingLocation = housingDetails.PastureLocation.Guid.Equals(Guid);

            return isMatchingLocation;
        }

        public List<ManureApplicationViewItem> GetLivestockManureApplicationsInYear(int year)
        {
            var result = new List<ManureApplicationViewItem>();

            foreach (var cropViewItem in CropViewItems)
            foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
                if (manureApplicationViewItem.ManureLocationSourceType == ManureLocationSourceType.Livestock &&
                    manureApplicationViewItem.DateOfApplication.Year == year)
                    result.Add(manureApplicationViewItem);

            return result;
        }

        public bool HasLivestockManureApplicationsInYear(int year)
        {
            return GetLivestockManureApplicationsInYear(year).Any();
        }

        public List<DigestateApplicationViewItem> GetLivestockDigestateApplicationsInYear(int year)
        {
            var result = new List<DigestateApplicationViewItem>();

            foreach (var cropViewItem in CropViewItems)
            foreach (var digestateApplicationViewItem in cropViewItem.DigestateApplicationViewItems)
                if (digestateApplicationViewItem.ManureLocationSourceType == ManureLocationSourceType.Livestock &&
                    digestateApplicationViewItem.DateCreated.Year == year)
                    result.Add(digestateApplicationViewItem);

            return result;
        }

        public bool HasLivestockDigestateApplicationsInYear(int year)
        {
            return GetLivestockDigestateApplicationsInYear(year).Any();
        }

        public bool HasImportedManureApplicationsInYear(int year)
        {
            return GetImportedManureApplicationsInYear(year).Any();
        }

        public List<ManureApplicationViewItem> GetImportedManureApplicationsInYear(int year)
        {
            var result = new List<ManureApplicationViewItem>();

            foreach (var cropViewItem in CropViewItems)
            foreach (var importedManureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
                if (importedManureApplicationViewItem.ManureLocationSourceType == ManureLocationSourceType.Imported &&
                    importedManureApplicationViewItem.DateOfApplication.Year == year)
                    result.Add(importedManureApplicationViewItem);

            return result;
        }

        public bool HasImportedDigestateApplicationsInYear(int year)
        {
            return GetImportedDigestateApplicationsInYear(year).Any();
        }

        public List<DigestateApplicationViewItem> GetImportedDigestateApplicationsInYear(int year)
        {
            var result = new List<DigestateApplicationViewItem>();

            foreach (var cropViewItem in CropViewItems)
            foreach (var digestateApplicationViewItem in cropViewItem.DigestateApplicationViewItems)
                if (digestateApplicationViewItem.ManureLocationSourceType == ManureLocationSourceType.Imported &&
                    digestateApplicationViewItem.DateCreated.Year == year)
                    result.Add(digestateApplicationViewItem);

            return result;
        }

        public bool HasHayHarvestInYear(int year)
        {
            return GetHarvestViewItemsInYear(year).Any();
        }

        public List<HarvestViewItem> GetHarvestViewItemsInYear(int year)
        {
            var result = new List<HarvestViewItem>();

            foreach (var cropViewItem in CropViewItems)
            {
                var byYear = cropViewItem.HarvestViewItems.Where(x => x.Start.Year.Equals(year));
                result.AddRange(byYear);
            }

            return result;
        }

        public List<HarvestViewItem> GetHarvestViewItems()
        {
            var result = new List<HarvestViewItem>();

            foreach (var cropViewItem in CropViewItems)
            {
                var harvestViewItems = cropViewItem.HarvestViewItems;
                result.AddRange(harvestViewItems);
            }

            return result;
        }

        public List<HayImportViewItem> GetHayImports(ResourceSourceLocation baleSource)
        {
            var result = new List<HayImportViewItem>();

            foreach (var cropViewItem in CropViewItems)
            {
                var harvestViewItems = cropViewItem.HayImportViewItems.Where(x => x.SourceOfBales.Equals(baleSource));
                result.AddRange(harvestViewItems);
            }

            return result;
        }

        public double GetDryMatterWeightOfHayHarvestsInYear(int year)
        {
            return GetHarvestViewItemsInYear(year).Sum(x => x.AboveGroundBiomassDryWeight);
        }

        public double GetDryMatterWeightOfHayHarvests()
        {
            var result = 0d;

            foreach (var harvestViewItem in GetHarvestViewItems())
                result += harvestViewItem.AboveGroundBiomassDryWeight;

            return result;
        }

        public double GetDryMatterWeightOfHayImports(
            ResourceSourceLocation sourceOfBales = ResourceSourceLocation.OnFarm)
        {
            var result = 0d;

            foreach (var harvestViewItem in GetHayImports(sourceOfBales))
                result += harvestViewItem.AboveGroundBiomassDryWeight;

            return result;
        }

        #endregion

        #region Event Handlers

        private void CropViewItemsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateTimelineInformationString();

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = notifyCollectionChangedEventArgs.NewItems[0];
                if (addedItem is CropViewItem viewItem) viewItem.PropertyChanged += CropViewItemOnPropertyChanged;
            }

            RaisePropertyChanged(nameof(CropString));
            RaisePropertyChanged(nameof(CropStringWithYears));
            RaisePropertyChanged(nameof(CropStringWithYearsMaxThreeItems));
            RaisePropertyChanged(nameof(CropViewItemsMaxThree));
        }

        private void CropViewItemOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is CropViewItem viewItem)
            {
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(CropViewItem.CropType)))
                {
                    RaisePropertyChanged(nameof(IsAnnual));
                    RaisePropertyChanged(nameof(IsFallow));
                    RaisePropertyChanged(nameof(IsGrassland));
                    RaisePropertyChanged(nameof(IsPerennial));
                }

                // Update timeline string when crop type (etc.) changes
                UpdateTimelineInformationString();

                RaisePropertyChanged(nameof(CropString));
                RaisePropertyChanged(nameof(CropStringWithYears));
                RaisePropertyChanged(nameof(CropStringWithYearsMaxThreeItems));
                RaisePropertyChanged(nameof(CropViewItemsMaxThree));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is FieldSystemComponent fieldSystemComponent)
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(CropString)))
                    UpdateTimelineInformationString();
        }

        #endregion
    }
}