using H.Core.Models.Animals;

namespace H.Core.Models.Infrastructure
{
    public class ADManagementPeriodViewItem : SubstrateViewItemBase
    {
        #region Fields

        private bool _isSelected;

        #endregion

        #region Constructors

        public ADManagementPeriodViewItem()
        {
            DailyPercentageOfManureAdded = 100;
        }

        #endregion

        #region Properties

        public ManagementPeriod ManagementPeriod { get; set; }
        public AnimalComponentBase AnimalComponent { get; set; }
        public AnimalGroup AnimalGroup { get; set; }

        public string ManureStateTypeString { get; set; }
        public string AnimalTypeString { get; set; }
        public string ComponentName { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        ///     Percentage
        /// </summary>
        public double DailyPercentageOfManureAdded { get; set; }

        /// <summary>
        ///     Fraction
        /// </summary>
        public double DailyFractionOfManureAdded => DailyPercentageOfManureAdded / 100.0;

        #endregion
    }
}