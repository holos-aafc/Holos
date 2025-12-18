using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Infrastructure;

namespace H.Core.Models
{
    /// <summary>
    /// </summary>
    public class ComponentBase : TimePeriodBase
    {
        #region Constructors

        public ComponentBase()
        {
            TimePeriodCategory = TimePeriodCategory.Current;

            HistoricalComponents.CollectionChanged += HistoricalComponentsOnCollectionChanged;
            ProjectedComponents.CollectionChanged += ProjectedComponentsOnCollectionChanged;

            PropertyChanged += ComponentOnPropertyChanged;
        }

        #endregion

        #region Fields

        private TimePeriodCategory _timePeriodCategory;
        private ComponentCategory _componentCategory;

        private bool _resultsCalculated;
        private bool _applyChangesToOtherGroups;
        private bool _hasErrors;
        private bool _hideComponentInListOfMyComponents;

        private string _componentNameDisplayString;
        private string _componentDescriptionString;
        private string _componentSelectionViewSecondaryDisplayString;
        private string _timelineInformationString;
        private string _groupPath;

        #endregion

        #region Properties

        public ObservableCollection<ComponentBase> HistoricalComponents { get; set; } =
            new ObservableCollection<ComponentBase>();

        public ObservableCollection<ComponentBase> ProjectedComponents { get; set; } =
            new ObservableCollection<ComponentBase>();

        public ComponentType ComponentType { get; set; }

        public ComponentCategory ComponentCategory
        {
            get => _componentCategory;
            set
            {
                SetProperty(ref _componentCategory, value,
                    () => { RaisePropertyChanged(nameof(ComponentCategoryDisplayString)); });
            }
        }

        /// <summary>
        ///     When creating historical or projected time period components, set this value to the GUID of the component that
        ///     represents the current time period.
        /// </summary>
        public Guid CurrentPeriodComponentGuid { get; set; }

        public bool TimePeriodCategoryIsCurrent => TimePeriodCategory == TimePeriodCategory.Current;

        public virtual string ComponentName => nameof(GetType);

        /// <summary>
        ///     Returns a string as defined in the resource file for the component type.
        /// </summary>
        public string ComponentTypeString => ComponentType.GetDescription();

        /// <summary>
        ///     Returns the component category string as defined in the resource file.
        /// </summary>
        public string ComponentCategoryDisplayString => ComponentCategory.GetDescription();

        /// <summary>
        ///     Returns the component type string as defined in the resource file or as set in the component constructor.
        /// </summary>
        public string ComponentNameDisplayString
        {
            get => _componentNameDisplayString;
            set => SetProperty(ref _componentNameDisplayString, value);
        }

        /// <summary>
        ///     Returns a user-friendly string describing what the component does or what the component is.
        /// </summary>
        public string ComponentDescriptionString
        {
            get => _componentDescriptionString;
            set => SetProperty(ref _componentDescriptionString, value);
        }

        public bool ApplyChangesToOtherGroups
        {
            get => _applyChangesToOtherGroups;
            set => SetProperty(ref _applyChangesToOtherGroups, value);
        }

        public bool HasErrors
        {
            get => _hasErrors;
            set => SetProperty(ref _hasErrors, value);
        }

        public TimePeriodCategory TimePeriodCategory
        {
            get => _timePeriodCategory;
            set
            {
                SetProperty(ref _timePeriodCategory, value,
                    () => { RaisePropertyChanged(nameof(TimePeriodCategoryIsCurrent)); });
            }
        }

        /// <summary>
        ///     A property that must be set so that historical/projected components will align with their current component on the
        ///     timeline view(s). This is currently being set to the <see cref="ModelBase.Name" /> property whenever it is updated.
        /// </summary>
        public string GroupPath
        {
            get => _groupPath;
            set => SetProperty(ref _groupPath, value);
        }

        public bool HideComponentInListOfMyComponents
        {
            get => _hideComponentInListOfMyComponents;
            set => SetProperty(ref _hideComponentInListOfMyComponents, value);
        }

        /// <summary>
        ///     A string that contains information for display on a timeline bar.
        /// </summary>
        public string TimelineInformationString
        {
            get => _timelineInformationString;
            set => SetProperty(ref _timelineInformationString, value);
        }

        public string ComponentSelectionViewSecondaryDisplayString
        {
            get => _componentSelectionViewSecondaryDisplayString;
            set => SetProperty(ref _componentSelectionViewSecondaryDisplayString, value);
        }

        /// <summary>
        ///     Used as a 'dirty' flag to indicate if result calculations have been performed for a component. If properties on an
        ///     <see cref="AnimalGroup" />, <see cref="ManagementPeriod" />, etc. are modified, this
        ///     flag must be set to false so results will be recalculated. Results calculation is expensive so this is used to
        ///     reduce time needed to produce results. Result services can used this flag to provide cached
        ///     results.
        /// </summary>
        public bool ResultsCalculated
        {
            get => _resultsCalculated;
            set => SetProperty(ref _resultsCalculated, value);
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(ComponentType)}: {ComponentType}";
        }

        #endregion

        #region Public Methods

        public virtual void Initialize()
        {
        }

        public virtual ObservableCollection<ErrorInformation> GetErrors()
        {
            return new ObservableCollection<ErrorInformation>();
        }

        #endregion

        #region Private Methods

        private void UpdateStartYearOfSubsequentComponent(ComponentBase updatedComponent)
        {
            var allComponents = new List<ComponentBase>();
            allComponents.AddRange(HistoricalComponents);
            allComponents.Add(this);
            allComponents.AddRange(ProjectedComponents);

            var indexOfUpdatedComponent = allComponents.IndexOf(updatedComponent);
            var nextComponent = allComponents.ElementAtOrDefault(indexOfUpdatedComponent + 1);
            if (nextComponent != null) nextComponent.StartYear = updatedComponent.EndYear + 1;
        }

        private void UpdateEndYearOfPreviousComponent(ComponentBase updatedComponent)
        {
            var allComponents = new List<ComponentBase>();
            allComponents.AddRange(HistoricalComponents);
            allComponents.Add(this);
            allComponents.AddRange(ProjectedComponents);

            var indexOfUpdatedComponent = allComponents.IndexOf(updatedComponent);
            var previousComponent = allComponents.ElementAtOrDefault(indexOfUpdatedComponent - 1);
            if (previousComponent != null) previousComponent.EndYear = updatedComponent.StartYear - 1;
        }

        #endregion

        #region Event Handlers

        private void ProjectedComponentsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = notifyCollectionChangedEventArgs.NewItems[0];
                if (addedItem is ComponentBase component) component.PropertyChanged += ComponentOnPropertyChanged;
            }
        }

        private void ComponentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is ComponentBase component)
            {
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(EndYear)))
                    UpdateStartYearOfSubsequentComponent(component);

                if (propertyChangedEventArgs.PropertyName.Equals(nameof(StartYear)))
                    UpdateEndYearOfPreviousComponent(component);

                if (propertyChangedEventArgs.PropertyName.Equals(nameof(Name)))
                {
                }
            }
        }

        private void HistoricalComponentsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = notifyCollectionChangedEventArgs.NewItems[0];
                if (addedItem is ComponentBase component) component.PropertyChanged += ComponentOnPropertyChanged;
            }
        }

        #endregion
    }
}