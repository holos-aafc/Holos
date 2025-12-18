#region Imports

using System;
using System.Globalization;
using H.Core.Models.Animals;
using H.Core.Providers;

#endregion

namespace H.Core.Models
{
    /// <summary>
    /// A DTO used to transfer data from an <see cref="ManagementPeriod" /> object which spans a period of time, into an object
    /// that is used for monthly-based reporting. A <see cref="ManagementPeriod"/> can span multiple months, whereas a
    /// <see cref="MonthsAndDaysData"/> object can only span between a start day and end day within one particular month.
    /// </summary>
    public class MonthsAndDaysData
    {
        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// The start date within the month (not the start date of the management period)
        /// </summary>
        public DateTime StartDate {
            get
            {
                return new DateTime(this.Year, this.Month, this.StartDay);
            }
        }

        /// <summary>
        /// The end date within the month (not the end date of the management period)
        /// </summary>
        public DateTime EndDate 
        {
            get
            {
                // Subtract 1 since we are including the start date in the calculation
                return this.StartDate.AddDays(this.DaysInMonth - 1);
            }
        }

        /// <summary>
        /// An object that holds animal type information and timespan that animal is in certain state (i.e. backgrounding
        /// state, finishing state, etc.)
        /// </summary>
        public AnimalGroup AnimalGroup { get; set; }

        /// <summary>
        /// Month value that can be used to lookup temperature data.
        /// </summary>
        public int Month { get; set; }

        public string MonthString
        {
            get
            {
                // Used to format month into a string instead of a number.
                var dateTime = new DateTime(DateTime.Now.Year, this.Month, 1);

                return dateTime.ToString("MMMM", CultureInfo.CurrentUICulture);
            }
        }

        public double StartWeightInMonth { get; set; }
        public double EndWeightInMonth { get; set; }

        public int StartDay { get; set; }

        /// <summary>
        /// Value needed to indicate how many days in month (i.e. a management period may only last from Jan. 1 - Jan. 15). This is not the number of calendar days
        /// in the month.
        /// </summary>
        public int DaysInMonth { get; set; }

        public int Year { get; set; }

        /// <summary>
        /// Geographic data based on users location that will be used to lookup temperature, precipitation, etc.
        /// </summary>
        public GeographicData GeographicData { get; set; }

        public ManagementPeriod ManagementPeriod { get; set; }

        /// <summary>
        /// The total number of calves
        /// </summary>
        public int NumberOfYoungAnimals { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{nameof(AnimalGroup)}: {AnimalGroup}, {nameof(Month)}: {this.MonthString}, {nameof(Year)}: {Year}";
        }

        /// <summary>
        /// Checks if a date falls within the start day and end day within the month. Manure application dates, harvest dates, etc.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool DateIsInMonth(DateTime dateTime)
        {
            return dateTime >= this.StartDate && dateTime <= this.EndDate;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}