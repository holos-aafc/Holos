#region Imports

using System.Collections.ObjectModel;
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
        #region Constructors

        public GlobalSettings()
        {
            ActiveFarm = new Farm();
            MultiFarmComparisonDefaults = new Defaults();
            CustomFeedIngredients = new ObservableCollection<FeedIngredient>();
            FarmsForComparison = new ObservableCollection<Farm>();
            MultiFarmManagementPeriod = new ManagementPeriod();
            CropDefaults = new ObservableCollection<CropViewItem>();

            EnableMultiFarmComparison = false;
            ManureModelResultsColumnVisibility = new ManureModelColumnsVisibility();
            FieldDetailsColumnsVisibility = new FieldSystemDetailsColumnsVisibility();
            FieldResultsColumnsVisibility = new FieldResultsColumnsVisibility();
        }

        #endregion

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

        #region Properties

        public ObservableCollection<CropViewItem> CropDefaults { get; set; }
        public ObservableCollection<FeedIngredient> CustomFeedIngredients { get; set; }

        public Farm ActiveFarm
        {
            get => _activeFarm;
            set => SetProperty(ref _activeFarm, value);
        }

        public bool EnableMultiFarmComparison
        {
            get => _enableMultiFarmComparison;
            set => SetProperty(ref _enableMultiFarmComparison, value);
        }

        public ObservableCollection<Farm> FarmsForComparison
        {
            get => _farmsForComparison;
            set => SetProperty(ref _farmsForComparison, value);
        }

        /// <summary>
        ///     A set of defaults that should be applied to all farms selected for comparison. Each farm will use values from this
        ///     object when running calculations
        /// </summary>
        public Defaults MultiFarmComparisonDefaults
        {
            get => _multiFarmComparisonDefaults;
            set => SetProperty(ref _multiFarmComparisonDefaults, value);
        }

        public ManagementPeriod MultiFarmManagementPeriod
        {
            get => _multiFarmManagementPeriod;
            set => SetProperty(ref _multiFarmManagementPeriod, value);
        }

        public FieldResultsColumnsVisibility FieldResultsColumnsVisibility
        {
            get => _fieldResultsColumnsVisibility;
            set => SetProperty(ref _fieldResultsColumnsVisibility, value);
        }

        public FieldSystemDetailsColumnsVisibility FieldDetailsColumnsVisibility
        {
            get => _fieldSystemDetailsColumnsVisibility;
            set => SetProperty(ref _fieldSystemDetailsColumnsVisibility, value);
        }

        public ManureModelColumnsVisibility ManureModelResultsColumnVisibility
        {
            get => _manureModelColumnVisibility;
            set => SetProperty(ref _manureModelColumnVisibility, value);
        }

        #endregion
    }
}