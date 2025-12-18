namespace H.Core.Models.Results
{
    public class EstimatesOfProductionResultsViewItem : ResultsViewItemBase
    {
        #region Fields

        private int _year;
        private ComponentCategory _category;
        private string _componentTypeString;
        private double _harvest;
        private double _area;
        private double _landAppliedManure;
        private double _beef;
        private double _milk;
        private double _fpcm;
        private double _lamb;
        private string _componentName;
        private string _monthString;
        private string _managementPeriodName;

        #endregion

        #region Properties

        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        public ComponentCategory Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public string ComponentTypeString
        {
            get => _componentTypeString;
            set => SetProperty(ref _componentTypeString, value);
        }

        public double Harvest
        {
            get => _harvest;
            set => SetProperty(ref _harvest, value);
        }

        public double Area
        {
            get => _area;
            set => SetProperty(ref _area, value);
        }

        public double LandAppliedManure
        {
            get => _landAppliedManure;
            set => SetProperty(ref _landAppliedManure, value);
        }

        public double Beef
        {
            get => _beef;
            set => SetProperty(ref _beef, value);
        }

        public double Milk
        {
            get => _milk;
            set => SetProperty(ref _milk, value);
        }

        /// <summary>
        ///     FPCM
        /// </summary>
        public double FatAndProteinCorrectedMilk
        {
            get => _fpcm;
            set => SetProperty(ref _fpcm, value);
        }

        public double Lamb
        {
            get => _lamb;
            set => SetProperty(ref _lamb, value);
        }

        public string ComponentName
        {
            get => _componentName;
            set => SetProperty(ref _componentName, value);
        }

        public string MonthString
        {
            get => _monthString;
            set => SetProperty(ref _monthString, value);
        }

        public string ManagementPeriodName
        {
            get => _managementPeriodName;
            set => SetProperty(ref _managementPeriodName, value);
        }

        #endregion
    }
}