#region Imports

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Feed;
using Newtonsoft.Json;
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
        private List<Guid> _farmsForComparisonGuids = new List<Guid>();
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

        /// <summary>
        /// The guid to restore <see cref="ActiveFarm"/> from after loading. Prefers the persisted
        /// <see cref="ActiveFarmGuid"/>; falls back to the guid of the farm embedded in older files. The
        /// <see cref="ActiveFarmGuid"/> getter cannot be used for this because the constructor always assigns an empty
        /// <see cref="Farm"/>, which would mask the value read from file.
        /// </summary>
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
        /// The farms selected for comparison are farms already held in <see cref="ApplicationData.Farms"/>, so writing
        /// them here stored a complete extra copy of each one. Persist their guids instead;
        /// <see cref="ApplicationData"/> restores the references after loading.
        /// </summary>
        /// <remarks>
        /// Replace rather than populate on read. The getter returns a new list, so Json.NET's default handling for a
        /// collection property - add to whatever the getter returns - would fill a discarded list and never call the
        /// setter, leaving the selection unrestored.
        /// </remarks>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<Guid> FarmsForComparisonGuids
        {
            get
            {
                // The live selection wins whenever the collection exists, including when it is empty. Falling back to
                // the stored guids on an empty collection meant a selection the user cleared was written to file and
                // restored on the next load. The stored guids are only needed between reading a file and restoring the
                // references from it, and the constructor always creates the collection.
                if (_farmsForComparison != null)
                {
                    return _farmsForComparison.Select(x => x.Guid).ToList();
                }

                return _farmsForComparisonGuids;
            }
            set { _farmsForComparisonGuids = value ?? new List<Guid>(); }
        }

        /// <summary>
        /// Suppresses writing the farms while still allowing them to be read, so files written before
        /// <see cref="FarmsForComparisonGuids"/> existed keep their selection.
        /// </summary>
        public bool ShouldSerializeFarmsForComparison()
        {
            return false;
        }

        /// <summary>
        /// The guids to restore <see cref="FarmsForComparison"/> from after loading. Prefers the persisted guids and
        /// falls back to the farms embedded in older files.
        /// </summary>
        internal IEnumerable<Guid> GetPersistedFarmsForComparisonGuids()
        {
            if (_farmsForComparisonGuids != null && _farmsForComparisonGuids.Any())
            {
                return _farmsForComparisonGuids;
            }

            if (_farmsForComparison != null)
            {
                return _farmsForComparison.Select(x => x.Guid).ToList();
            }

            return new List<Guid>();
        }

        /// <summary>
        /// Replaces the contents of <see cref="FarmsForComparison"/> with the given farms. A farm that no longer exists
        /// is dropped - it cannot be compared, and the selection is rebuilt by the comparison view in any case.
        /// </summary>
        internal void RestoreFarmsForComparison(IEnumerable<Farm> farms)
        {
            if (_farmsForComparison == null)
            {
                _farmsForComparison = new ObservableCollection<Farm>();
            }

            _farmsForComparison.Clear();
            foreach (var farm in farms)
            {
                _farmsForComparison.Add(farm);
            }

            _farmsForComparisonGuids = _farmsForComparison.Select(x => x.Guid).ToList();

            this.RaisePropertyChanged(nameof(this.FarmsForComparison));
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