#region Imports

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Infrastructure;

namespace H.Core.Models
{
    /// <summary>
    /// </summary>
    public class ComponentBase : TimePeriodBase
    {
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

        #region Constructors

        public ComponentBase()
        {
            this.TimePeriodCategory = TimePeriodCategory.Current;

            this.HistoricalComponents.CollectionChanged += HistoricalComponentsOnCollectionChanged;
            this.ProjectedComponents.CollectionChanged += ProjectedComponentsOnCollectionChanged;

            this.PropertyChanged += this.ComponentOnPropertyChanged;
        }

        #endregion

        #region Properties        

        public ObservableCollection<ComponentBase> HistoricalComponents { get; set; } = new ObservableCollection<ComponentBase>();
        public ObservableCollection<ComponentBase> ProjectedComponents { get; set; } = new ObservableCollection<ComponentBase>();

        public ComponentType ComponentType { get; set; }

        public ComponentCategory ComponentCategory
        {
            get { return _componentCategory; }
            set
            {
                SetProperty(ref _componentCategory, value, () => { base.RaisePropertyChanged(nameof(this.ComponentCategoryDisplayString)); });
            }
        }

        /// <summary>
        /// When creating historical or projected time period components, set this value to the GUID of the component that represents the current time period.
        /// </summary>
        public Guid CurrentPeriodComponentGuid { get; set; }

        public bool TimePeriodCategoryIsCurrent
        {
            get { return this.TimePeriodCategory == TimePeriodCategory.Current; }
        }

        public virtual string ComponentName
        {
            get { return nameof(this.GetType); }
        }

        /// <summary>
        /// Returns a string as defined in the resource file for the component type.
        /// </summary>
        public string ComponentTypeString
        {
            get { return this.ComponentType.GetDescription(); }
        }

        /// <summary>
        /// Returns the component category string as defined in the resource file.
        /// </summary>
        public string ComponentCategoryDisplayString
        {
            get { return this.ComponentCategory.GetDescription(); }
        }

        /// <summary>
        /// Returns the component type string as defined in the resource file or as set in the component constructor.
        /// </summary>
        public string ComponentNameDisplayString
        {
            get { return _componentNameDisplayString; }
            set { this.SetProperty(ref _componentNameDisplayString, value); }
        }

        /// <summary>
        /// Returns a user-friendly string describing what the component does or what the component is.
        /// </summary>
        public string ComponentDescriptionString
        {
            get { return _componentDescriptionString; }
            set { this.SetProperty(ref _componentDescriptionString, value); }
        }

        public bool ApplyChangesToOtherGroups
        {
            get { return _applyChangesToOtherGroups; }
            set { this.SetProperty(ref _applyChangesToOtherGroups, value); }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            set { this.SetProperty(ref _hasErrors, value); }
        }

        public TimePeriodCategory TimePeriodCategory
        {
            get { return _timePeriodCategory; }
            set { SetProperty(ref _timePeriodCategory, value, () => { base.RaisePropertyChanged(nameof(this.TimePeriodCategoryIsCurrent)); }); }
        }

        /// <summary>
        /// A property that must be set so that historical/projected components will align with their current component on the timeline view(s). This is currently being set to the <see cref="ModelBase.Name"/> property whenever it is updated.
        /// </summary>
        public string GroupPath
        {
            get { return _groupPath; }
            set { SetProperty(ref _groupPath, value); }
        }

        public bool HideComponentInListOfMyComponents
        {
            get { return _hideComponentInListOfMyComponents; }
            set { SetProperty(ref _hideComponentInListOfMyComponents, value); }
        }

        /// <summary>
        /// A string that contains information for display on a timeline bar.
        /// </summary>
        public string TimelineInformationString
        {
            get { return _timelineInformationString; }
            set { SetProperty(ref _timelineInformationString, value); }
        }

        public string ComponentSelectionViewSecondaryDisplayString
        {
            get { return _componentSelectionViewSecondaryDisplayString; }
            set { SetProperty(ref _componentSelectionViewSecondaryDisplayString, value); }
        }

        /// <summary>
        /// Used as a 'dirty' flag to indicate if result calculations have been performed for a component. If properties on an <see cref="AnimalGroup"/>, <see cref="ManagementPeriod"/>, etc. are modified, this
        /// flag must be set to false so results will be recalculated. Results calculation is expensive so this is used to reduce time needed to produce results. Result services can used this flag to provide cached
        /// results.
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
            allComponents.AddRange(this.HistoricalComponents);
            allComponents.Add(this);
            allComponents.AddRange(this.ProjectedComponents);

            var indexOfUpdatedComponent = allComponents.IndexOf(updatedComponent);
            var nextComponent = allComponents.ElementAtOrDefault(indexOfUpdatedComponent + 1);
            if (nextComponent != null)
            {
                nextComponent.StartYear = updatedComponent.EndYear + 1;
            }
        }

        private void UpdateEndYearOfPreviousComponent(ComponentBase updatedComponent)
        {
            var allComponents = new List<ComponentBase>();
            allComponents.AddRange(this.HistoricalComponents);
            allComponents.Add(this);
            allComponents.AddRange(this.ProjectedComponents);

            var indexOfUpdatedComponent = allComponents.IndexOf(updatedComponent);
            var previousComponent = allComponents.ElementAtOrDefault(indexOfUpdatedComponent - 1);
            if (previousComponent != null)
            {
                previousComponent.EndYear = updatedComponent.StartYear - 1;
            }
        }

        #endregion

        #region Event Handlers

        private void ProjectedComponentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = notifyCollectionChangedEventArgs.NewItems[0];
                if (addedItem is ComponentBase component)
                {
                    component.PropertyChanged += ComponentOnPropertyChanged;
                }
            }
        }

        private void ComponentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is ComponentBase component)
            {
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(this.EndYear)))
                {
                    this.UpdateStartYearOfSubsequentComponent(component);
                }

                if (propertyChangedEventArgs.PropertyName.Equals(nameof(this.StartYear)))
                {
                    this.UpdateEndYearOfPreviousComponent(component);
                }

                if (propertyChangedEventArgs.PropertyName.Equals(nameof(this.Name)))
                {
                }
            }
        }

        private void HistoricalComponentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = notifyCollectionChangedEventArgs.NewItems[0];
                if (addedItem is ComponentBase component)
                {
                    component.PropertyChanged += ComponentOnPropertyChanged;
                }
            }
        }

        #endregion
    }
}