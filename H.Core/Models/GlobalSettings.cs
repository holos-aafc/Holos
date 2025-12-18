#region Imports

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