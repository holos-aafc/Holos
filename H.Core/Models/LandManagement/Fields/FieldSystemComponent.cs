#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Infrastructure;
using H.Core;
using H.Core.Models.LandManagement.Rotation;

#endregion

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// </summary>
    public class FieldSystemComponent : ComponentBase
    {
        #region Fields

        private string _fieldName;
        private double _fieldArea;
        private double _startingSoilOrganicCarbon;

        private bool _beginOrderingAtStartYearOfRotation;
        private bool _useCustomStartingSoilOrganicCarbon;

        private ObservableCollection<CropViewItem> _runInPeriodItems;

        #endregion

        #region Constructors

        public FieldSystemComponent()
        {
            this.ComponentNameDisplayString = Properties.Resources.LabelField;
            this.ComponentDescriptionString = Properties.Resources.ToolTipFieldsComponent;
            this.ComponentCategory = ComponentCategory.LandManagement;
            this.ComponentType = ComponentType.Field;

            this.RunInPeriodItems = new ObservableCollection<CropViewItem>();

            this.FieldArea = 1.0;

            this.PropertyChanged -= OnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;

            this.CropViewItems.CollectionChanged -= CropViewItemsOnCollectionChanged;
            this.CropViewItems.CollectionChanged += CropViewItemsOnCollectionChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<CropViewItem> CropViewItems { get; set; } = new ObservableCollection<CropViewItem>();

        /// <summary>
        /// This will hold a collection of items that represent winter crops, cover crops, or undersown crops (all of these will also be called secondary crops) if any was specified by user. There will always
        /// be exactly one view item in this collection created for each (main) crop view item added by the user in the component selection view.
        /// </summary>
        public ObservableCollection<CropViewItem> CoverCrops { get; set; } = new ObservableCollection<CropViewItem>();

        /// <summary>
        /// TODO: This is no longer used and should be deleted once it is safe to delete properties on farms without resetting all data on user system.
        /// </summary>
        [Obsolete]
        public string FieldName
        {
            get { return _fieldName; }
            set { this.SetProperty(ref _fieldName, value); }
        }

        /// <summary>
        /// Allows to user to specify the annual order of the crops they entered
        /// </summary>
        public bool BeginOrderingAtStartYearOfRotation
        {
            get => _beginOrderingAtStartYearOfRotation;
            set => SetProperty(ref _beginOrderingAtStartYearOfRotation, value);
        }

        /// <summary>
        /// Total area of the field component. Any changes to this property will update all <see cref="CropViewItems"/> associated with this <see cref="FieldSystemComponent"/>.
        /// </summary>
        public double FieldArea
        {
            get { return _fieldArea; }
            set { this.SetProperty(ref _fieldArea, value); }
        }

        public string CropString
        {
            get { return string.Join(", ", this.CropViewItems.Select(x => x.CropTypeString)); }
        }

        /// <summary>
        /// Used by the fields view in the rotation component
        /// </summary>
        public string CropStringWithYears
        {
            get { return string.Join(", ", this.CropViewItems.Select(x => x.CropTypeStringWithYear)); }
        }

        /// <summary>
        /// Used by the fields view in the rotation component
        /// </summary>
        public ObservableCollection<CropViewItem> CropViewItemsMaxThree
        {
            get
            {
                var result = new ObservableCollection<CropViewItem>();

                if (this.BeginOrderingAtStartYearOfRotation)
                {
                    result.AddRange(this.CropViewItems.OrderBy(x => x.Year).Take(3));


                }
                else
                {
                    result.AddRange(this.CropViewItems.OrderByDescending(x => x.Year).Take(3));
                }

                return result;
            }
        }

        public string CropStringWithYearsMaxThreeItems
        {
            get
            {
                if (this.BeginOrderingAtStartYearOfRotation)
                {
                    return string.Join(", ", this.CropViewItems.OrderBy(x => x.Year).Select(x => x.CropTypeStringWithYear).Take(3));
                }
                else
                {
                    return string.Join(", ", this.CropViewItems.OrderByDescending(x => x.Year).Select(x => x.CropTypeStringWithYear).Take(3));
                }
            }
        }

        public bool IsAnnual
        {
            get
            {
                if (this.GetSingleYearViewItem() != null &&
                    this.GetSingleYearViewItem().CropType.IsAnnual())
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsFallow
        {
            get
            {
                if (this.GetSingleYearViewItem() != null &&
                    this.GetSingleYearViewItem().CropType.IsFallow())
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsGrassland
        {
            get
            {
                if (this.GetSingleYearViewItem() != null &&
                    this.GetSingleYearViewItem().CropType.IsGrassland())
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsNativeGrassland
        {
            get
            {
                if (this.GetSingleYearViewItem() != null &&
                    this.GetSingleYearViewItem().CropType.IsGrassland() &&
                    this.GetSingleYearViewItem().IsNativeGrassland == true)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsPerennial
        {
            get
            {
                if (this.GetSingleYearViewItem() != null &&
                    this.GetSingleYearViewItem().CropType.IsPerennial())
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsPasture
        {
            get
            {
                if (this.GetSingleYearViewItem() != null && this.GetSingleYearViewItem().CropType.IsPasture())
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsIrrigated
        {
            get
            {
                if (this.GetSingleYearViewItem() != null && this.GetSingleYearViewItem().AmountOfIrrigation > 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// A past perennial is a crop that is currently an annual but was previously a perennial (grassland was broken)
        /// </summary>
        public bool IsPastPerennial
        {
            get { return this.IsAnnual && this.GetSingleYearViewItem().IsBrokenGrassland; }
        }

        public bool IsCurrentPerennial
        {
            get { return this.IsPerennial && this.IsPastPerennial == false; }
        }

        public bool IsPartOfRotationComponent { get; set; }

        public ObservableCollection<CropViewItem> RunInPeriodItems
        {
            get => _runInPeriodItems;
            set => SetProperty(ref _runInPeriodItems, value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// We don't recalculate results if these properties change
        /// </summary>
        public static bool IsFieldComponentPropertyRelatedToCalculations(string propertyName)
        {
            if (propertyName.Equals(nameof(FieldSystemComponent.ResultsCalculated)) || // Prevent loop
                propertyName.Equals(nameof(FieldSystemComponent.Name)) ||
                propertyName.Equals(nameof(FieldSystemComponent.CropString)) ||
                propertyName.Equals(nameof(FieldSystemComponent.CropStringWithYears)) ||
                propertyName.Equals(nameof(FieldSystemComponent.CropViewItemsMaxThree)) ||
                propertyName.Equals(nameof(FieldSystemComponent.ComponentDescriptionString)) ||
                propertyName.Equals(nameof(FieldSystemComponent.GroupPath)) ||
                propertyName.Equals(nameof(FieldSystemComponent.TimelineInformationString)) ||
                propertyName.Equals(nameof(FieldSystemComponent.CropStringWithYearsMaxThreeItems)) ||
                propertyName.Equals(nameof(FieldSystemComponent.Duration)) ||
                propertyName.Equals(nameof(FieldSystemComponent.Start)) ||
                propertyName.Equals(nameof(FieldSystemComponent.End)) ||
                propertyName.Equals(nameof(FieldSystemComponent.IsAnnual)) ||
                propertyName.Equals(nameof(FieldSystemComponent.IsPerennial)) ||
                propertyName.Equals(nameof(FieldSystemComponent.IsGrassland)) ||
                propertyName.Equals(nameof(FieldSystemComponent.IsFallow)) ||
                propertyName.Equals(nameof(FieldSystemComponent.NumberOfDays)) ||
                propertyName.Equals(nameof(FieldSystemComponent.Description)))
            {
                return false;
            }

            return true;
        }

        public List<CropViewItem> GetAllItemsInPerennialStand(Guid groupGuid)
        {
            return this.CropViewItems.Where(x => x.PerennialStandGroupId == groupGuid).OrderBy(x => x.YearInPerennialStand).ToList();
        }

        /// <summary>
        /// Since this component supports multi year and single year modes (ICBM vs non-ICBM) this method returns the correct view (the most recent item) item when farm is in single-year mode.
        /// </summary>
        /// <returns>The most recent view items for the field, or null if there are not items define for the field</returns>
        public CropViewItem GetSingleYearViewItem()
        {
            var mostRecentViewItem = this.CropViewItems.OrderByDescending(viewItem => viewItem.Year).FirstOrDefault();

            return mostRecentViewItem;
        }

        /// <summary>
        /// Returns the size of first rotation for a field system component. If there are any historical components associated with this component,
        /// the number of crops in the earliest (first) historical system will be returned. If there are no historical components, then the number
        /// of crops associated with the current component will be returned. This value is used when calculating results - specifically when calculating
        /// the equilibrium year as that calculation uses averages from the crops found in the first rotation (component) for the field.
        /// </summary>
        public int SizeOfFirstRotationInField()
        {
            var result = 1;

            if (this.HistoricalComponents.Any())
            {
                var firstHistoricalComponent = this.HistoricalComponents.OrderBy(x => x.Start).First();
                if (firstHistoricalComponent is FieldSystemComponent fieldSystemComponent)
                {
                    return fieldSystemComponent.CropViewItems.Count;
                }
            }
            else
            {
                return this.CropViewItems.Count;
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

        #endregion

        #region Private Methods

        private void UpdateTimelineInformationString()
        {
            this.TimelineInformationString = this.CropString;
        }

        #endregion

        #region Event Handlers

        private void CropViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.UpdateTimelineInformationString();

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = notifyCollectionChangedEventArgs.NewItems[0];
                if (addedItem is CropViewItem viewItem)
                {
                    viewItem.PropertyChanged += CropViewItemOnPropertyChanged;
                }
            }

            this.RaisePropertyChanged(nameof(this.CropString));
            this.RaisePropertyChanged(nameof(this.CropStringWithYears));
            this.RaisePropertyChanged(nameof(this.CropStringWithYearsMaxThreeItems));
            this.RaisePropertyChanged(nameof(this.CropViewItemsMaxThree));
        }

        private void CropViewItemOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is CropViewItem viewItem)
            {
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(CropViewItem.CropType)))
                {
                    base.RaisePropertyChanged(nameof(this.IsAnnual));
                    base.RaisePropertyChanged(nameof(this.IsFallow));
                    base.RaisePropertyChanged(nameof(this.IsGrassland));
                    base.RaisePropertyChanged(nameof(this.IsPerennial));
                }

                // Update timeline string when crop type (etc.) changes
                this.UpdateTimelineInformationString();

                this.RaisePropertyChanged(nameof(this.CropString));
                this.RaisePropertyChanged(nameof(this.CropStringWithYears));
                this.RaisePropertyChanged(nameof(this.CropStringWithYearsMaxThreeItems));
                this.RaisePropertyChanged(nameof(this.CropViewItemsMaxThree));
            }

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is FieldSystemComponent fieldSystemComponent)
            {
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(this.CropString)))
                {
                    this.UpdateTimelineInformationString();
                }

                // Update the area of all crop view items associated with this field
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(this.FieldArea)))
                {
                    Trace.TraceInformation($"{nameof(FieldSystemComponent)}.{nameof(OnPropertyChanged)}: field area changed, updating area of associated crop view items");

                    foreach (var viewItem in this.CropViewItems)
                    {
                        viewItem.Area = fieldSystemComponent.FieldArea;
                    }
                }
            }
        }

        #endregion
    }
}
