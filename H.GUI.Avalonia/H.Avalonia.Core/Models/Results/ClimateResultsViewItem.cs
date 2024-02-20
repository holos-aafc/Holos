using H.Common.Models;

namespace H.Avalonia.Core.Models.Results
{
    /// <summary>
    /// Contains properties that are tied to the Grid shown for the climate results page.
    /// </summary>
    public class ClimateResultsViewItem : ModelBase
    {
        private int _year;
        private double _totalPET;
        private double _totalPPT;
        private double _monthlyPPT;

        /// <summary>
        /// The year for which data is extracted.
        /// </summary>
        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }


        /// <summary>
        /// The evapotranspiration amount for the given year.
        /// </summary>
        public double TotalPET
        {
            get => _totalPET;
            set => SetProperty(ref _totalPET, value);
        }

        /// <summary>
        /// The total precipitation amount for the given year.
        /// </summary>
        public double TotalPPT
        {
            get => _totalPPT;
            set => SetProperty(ref _totalPPT, value);
        }

        /// <summary>
        /// A monthly precipitation amount for a range of months specified by the user.
        /// </summary>
        public double MonthlyPPT
        {
            get => _monthlyPPT;
            set => SetProperty(ref _monthlyPPT, value);
        }
    }
}
