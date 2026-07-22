#region Imports

using System;
using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Feed;
using Prism.Mvvm;

#endregion

namespace H.Core.Models
{
    /// <summary>
    /// </summary>
    public class GlobalSettings : BindableBase
    {
        #region Fields

        private Farm _activeFarm;
        private Guid _activeFarmGuid;
        private ObservableCollection<Farm> _farmsForComparison;
        private bool _enableMultiFarmComparison;
        private Defaults _multiFarmComparisonDefaults;
        private ManagementPeriod _multiFarmManagementPeriod;

        private FieldSystemDetailsColumnsVisibility _fieldSystemDetailsColumnsVisibility;
        private FieldResultsColumnsVisibility _fieldResultsColumnsVisibility;
        private ManureModelColumnsVisibility _manureModelColumnVisibility;
        #endregion

        #region Constructors

        public GlobalSettings()
        {
            this.ActiveFarm = new Farm();
            this.MultiFarmComparisonDefaults = new Defaults();
            this.CustomFeedIngredients = new ObservableCollection<FeedIngredient>();
            this.FarmsForComparison = new ObservableCollection<Farm>();
            this.MultiFarmManagementPeriod = new ManagementPeriod();
            this.CropDefaults = new ObservableCollection<CropViewItem>();

            this.EnableMultiFarmComparison = false;
            this.ManureModelResultsColumnVisibility = new ManureModelColumnsVisibility();
            this.FieldDetailsColumnsVisibility = new FieldSystemDetailsColumnsVisibility();
            this.FieldResultsColumnsVisibility = new FieldResultsColumnsVisibility();
        }

        #endregion

        #region Properties

        public ObservableCollection<CropViewItem> CropDefaults { get; set; }
        public ObservableCollection<FeedIngredient> CustomFeedIngredients { get; set; }

        public Farm ActiveFarm
        {
            get { return _activeFarm; }
            set { this.SetProperty(ref _activeFarm, value); }
        }

        /// <summary>
        /// The active farm is one of the farms in <see cref="ApplicationData.Farms"/>, so writing it here stored a
        /// second complete copy of that farm. Persist its <see cref="ModelBase.Guid"/> instead;
        /// <see cref="ApplicationData"/> re-points <see cref="ActiveFarm"/> at the matching instance after loading.
        /// </summary>
        public Guid ActiveFarmGuid
        {
            get { return _activeFarm != null ? _activeFarm.Guid : _activeFarmGuid; }
            set { _activeFarmGuid = value; }
        }

        /// <summary>
        /// Suppresses writing the farm while still allowing it to be read, so files written before
        /// <see cref="ActiveFarmGuid"/> existed continue to identify their active farm.
        /// </summary>
        public bool ShouldSerializeActiveFarm()
        {
            return false;
        }

        /// <summary>
        /// The guid to restore <see cref="ActiveFarm"/> from after loading. Prefers the persisted
        /// <see cref="ActiveFarmGuid"/>; falls back to the guid of the farm embedded in older files. The
        /// <see cref="ActiveFarmGuid"/> getter cannot be used for this because the constructor always assigns an empty
        /// <see cref="Farm"/>, which would mask the value read from file.
        /// </summary>
        /// <summary>
        /// Re-points <see cref="ActiveFarm"/> at <paramref name="farm"/> after loading. This assigns the field directly
        /// because <see cref="ModelBase.Equals(object)"/> compares by <see cref="ModelBase.Guid"/>, so the duplicate
        /// read from file compares equal to the farm in the collection and SetProperty would skip the assignment -
        /// leaving the two as separate objects.
        /// </summary>
        internal void RestoreActiveFarm(Farm farm)
        {
            _activeFarm = farm;
            _activeFarmGuid = farm != null ? farm.Guid : Guid.Empty;

            this.RaisePropertyChanged(nameof(this.ActiveFarm));
        }

        internal Guid GetPersistedActiveFarmGuid()
        {
            if (_activeFarmGuid != Guid.Empty)
            {
                return _activeFarmGuid;
            }

            return _activeFarm != null ? _activeFarm.Guid : Guid.Empty;
        }

        public bool EnableMultiFarmComparison
        {
            get { return _enableMultiFarmComparison; }
            set { SetProperty(ref _enableMultiFarmComparison, value); }
        }

        public ObservableCollection<Farm> FarmsForComparison
        {
            get { return _farmsForComparison; }
            set { SetProperty(ref _farmsForComparison, value); }
        }

        /// <summary>
        /// A set of defaults that should be applied to all farms selected for comparison. Each farm will use values from this object when running calculations
        /// </summary>
        public Defaults MultiFarmComparisonDefaults
        {
            get { return _multiFarmComparisonDefaults; }
            set { SetProperty(ref _multiFarmComparisonDefaults, value); }
        }

        public ManagementPeriod MultiFarmManagementPeriod
        {
            get { return _multiFarmManagementPeriod; }
            set { SetProperty(ref _multiFarmManagementPeriod, value); }
        }

        public FieldResultsColumnsVisibility FieldResultsColumnsVisibility
        {
            get { return _fieldResultsColumnsVisibility; }
            set { SetProperty(ref _fieldResultsColumnsVisibility, value); }
        }
        public FieldSystemDetailsColumnsVisibility FieldDetailsColumnsVisibility
        {
            get { return _fieldSystemDetailsColumnsVisibility; }
            set { SetProperty(ref _fieldSystemDetailsColumnsVisibility, value); }
        }
        public ManureModelColumnsVisibility ManureModelResultsColumnVisibility
        {
            get { return _manureModelColumnVisibility; }
            set { SetProperty(ref _manureModelColumnVisibility, value); }
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}