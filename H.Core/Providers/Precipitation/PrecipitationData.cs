using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using H.Core.Enumerations;

namespace H.Core.Providers.Precipitation
{
    /// <summary>
    /// </summary>
    public class PrecipitationData : MonthlyValueBase<double>
    {
        #region Fields

        private double _growingSeasonPrecipitation;

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If one of the monthly values changes, we need to recalculate the growing season precipitation - but not if the user explicitly sets the growing season precipitation
            if (e.PropertyName.Equals(nameof(GrowingSeasonPrecipitation)) == false)
                GrowingSeasonPrecipitation = CalculateGrowingSeasonPrecipitation();
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     please by default always attach the handlers
        /// </summary>
        public PrecipitationData() : this(true)
        {
        }

        /// <summary>
        ///     For the gui to work properly we need to hold off attaching the event handlers on construction
        /// </summary>
        /// <param name="attach">true to attach the event handlers or false otherwise</param>
        public PrecipitationData(bool attach)
        {
            if (attach) AttachEventHandlers();
        }

        #endregion

        #region Properties

        public double GrowingSeasonPrecipitation
        {
            get => _growingSeasonPrecipitation;
            set => SetProperty(ref _growingSeasonPrecipitation, value);
        }

        public int PolygonId { get; set; }

        #endregion

        #region Public Methods

        public double CalculateGrowingSeasonPrecipitation()
        {
            var data = new List<double>
            {
                May,
                June,
                July,
                August,
                September,
                October
            };

            return data.Sum();
        }

        public double GetTotalAnnualPrecipitation()
        {
            return January +
                   February +
                   March +
                   April +
                   May +
                   June +
                   July +
                   August +
                   September +
                   October +
                   November +
                   December;
        }

        public List<double> GetAveragedYearlyValues()
        {
            var list = new List<double>
            {
                January,
                February,
                March,
                April,
                May,
                June,
                July,
                August,
                September,
                October,
                November,
                December
            };

            var yearlyAverages = new List<double>();
            var numberOfDaysInEachMonthList = new List<int>
            {
                (int)DaysInMonth.January,
                (int)DaysInMonth.February,
                (int)DaysInMonth.March,
                (int)DaysInMonth.April,
                (int)DaysInMonth.May,
                (int)DaysInMonth.June,
                (int)DaysInMonth.July,
                (int)DaysInMonth.August,
                (int)DaysInMonth.September,
                (int)DaysInMonth.October,
                (int)DaysInMonth.November,
                (int)DaysInMonth.December
            };

            for (var i = 0; i < numberOfDaysInEachMonthList.Count; i++)
            {
                var currentMonthAndDayCount = numberOfDaysInEachMonthList.ElementAt(i);
                var currentInputValue = list.ElementAt(i);
                for (var j = 0; j < currentMonthAndDayCount; j++)
                    yearlyAverages.Add(currentInputValue / currentMonthAndDayCount);
            }

            return yearlyAverages;
        }

        /// <summary>
        ///     For use in the GUI
        /// </summary>
        public void AttachEventHandlers()
        {
            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        #endregion
    }
}