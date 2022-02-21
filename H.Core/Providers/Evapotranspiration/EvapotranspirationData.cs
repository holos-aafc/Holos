#region Imports

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Evapotranspiration
{
    /// <summary>
    /// </summary>
    public class EvapotranspirationData : MonthlyValueBase<double>
    {
        #region Fields

        private double _growingSeasonEvapotranspiration;        

        #endregion

        #region Constructors

        /// <summary>
        /// please by default always attach the handlers
        /// </summary>
        public EvapotranspirationData() : this(true) { }

        /// <summary>
        /// For the gui to work properly we need to hold off attaching the event handlers on construction
        /// </summary>
        /// <param name="attach">true to attach the event handlers or false otherwise</param>
        public EvapotranspirationData(bool attach)
        {
            if (attach)
            {
                this.AttachEventHandlers();
            }
        }

        #endregion

        #region Properties

        public int PolygonId { get; set; }

        public double GrowingSeasonEvapotranspiration
        {
            get => _growingSeasonEvapotranspiration;
            set => SetProperty(ref _growingSeasonEvapotranspiration, value);
        }

        #endregion

        #region Public Methods

        public double CalculateGrowingSeasonEvapotranspiration()
        {
            var data = new List<double>()
            {
                this.May,
                this.June,
                this.July,
                this.August,
                this.September,
                this.October,
            };

            return data.Sum();
        }

        public List<double> GetAveragedYearlyValues()
        {
            var list = new List<double>
            {
                this.January,
                this.February,
                this.March,
                this.April,
                this.May,
                this.June,
                this.July,
                this.August,
                this.September,
                this.October,
                this.November,
                this.December
            };

            var yearlyAverages = new List<double>();
            var numberOfDaysInEachMonthList = new List<int>
            {
                (int) DaysInMonth.January,
                (int) DaysInMonth.February,
                (int) DaysInMonth.March,
                (int) DaysInMonth.April,
                (int) DaysInMonth.May,
                (int) DaysInMonth.June,
                (int) DaysInMonth.July,
                (int) DaysInMonth.August,
                (int) DaysInMonth.September,
                (int) DaysInMonth.October,
                (int) DaysInMonth.November,
                (int) DaysInMonth.December
            };

            for (int i = 0; i < numberOfDaysInEachMonthList.Count; i++)
            {
                var currentMonthAndDayCount = numberOfDaysInEachMonthList.ElementAt(i);
                var currentInputValue = list.ElementAt(i);
                for (var j = 0; j < currentMonthAndDayCount; j++)
                {
                    yearlyAverages.Add(currentInputValue / currentMonthAndDayCount);
                }
            }

            return yearlyAverages;
        }

        public double GetTotalAnnualEvapotranspiration()
        {
            return this.January +
                   this.February +
                   this.March +
                   this.April +
                   this.May +
                   this.June +
                   this.July +
                   this.August +
                   this.September +
                   this.October +
                   this.November +
                   this.December;
        }

        /// <summary>
        /// For use in the GUI
        /// </summary>
        public void AttachEventHandlers()
        {
            this.PropertyChanged -= OnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If one of the monthly values changes, we need to recalculate the growing season precipitation - but not if the user explicitly sets the growing season precipitation
            if (e.PropertyName.Equals(nameof(GrowingSeasonEvapotranspiration)) == false)
            {
                this.GrowingSeasonEvapotranspiration = this.CalculateGrowingSeasonEvapotranspiration();
            }
        }

        #endregion
    }
}