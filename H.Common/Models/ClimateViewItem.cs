namespace H.Common.Models
{
    /// <summary>
    /// Contains properties that are tied to the Grid shown for the climate items in the Climate Data page.
    /// </summary>
    public class ClimateViewItem : ModelBase
    {
        private int _startYear;
        private int _endYear;
        private bool _extractMonthlyData;
        private int _julianStartDay;
        private int _julianEndDay;

        public ClimateViewItem()
        {
            Latitude = 0;
            Longitude = 0;
            StartYear = 1980;
            EndYear = 1980;
            JulianStartDay = 0;
            JulianEndDay = 0;
            ExtractMonthlyData = false;
        }


        /// <summary>
        /// The start year for which climate data is required.
        /// </summary>
        public int StartYear
        {
            get => _startYear;
            set
            {
                if (value < 1980) value = 0;
                SetProperty(ref _startYear, value);
            }
        }

        /// <summary>
        /// The end year for which climate data is required.
        /// </summary>
        public int EndYear
        {
            get => _endYear;
            set
            {
                if (value < 1980) value = 0;
                SetProperty(ref _endYear, value);
            }
        }

        /// <summary>
        /// The start day (in julian) for which climate data is required.
        /// </summary>
        public int JulianStartDay
        {
            get => _julianStartDay;
            set
            {
                if (value is < 0 or > 365) value = 0;
                SetProperty(ref _julianStartDay, value);
            }
        }

        /// <summary>
        /// The end day (in julian) for which climate data is required.
        /// </summary>
        public int JulianEndDay
        {
            get => _julianEndDay;
            set
            {
                if (value is < 0 or > 365) value = 0;
                SetProperty(ref _julianEndDay, value);
            }
        }

        /// <summary>
        /// A property that checks if a user wants to extract monthly PPT data for a specific year.
        /// </summary>
        public bool ExtractMonthlyData
        {
            get => _extractMonthlyData;
            set => SetProperty(ref _extractMonthlyData, value);
        }
    }
}
