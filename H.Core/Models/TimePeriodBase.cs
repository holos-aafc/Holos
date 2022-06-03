#region Imports

using System;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

#endregion

namespace H.Core.Models
{
    /// <summary>
    /// </summary>
    public class TimePeriodBase : ModelBase, ITimePeriodItem
    {
        #region Fields

        private DateTime _start;
        private DateTime _end;
        private int _numberOfDays;
        private int _startYear;
        private int _endYear;

        #endregion

        #region Constructors

        public TimePeriodBase()
        {
            this.Start = DateTime.Now;
            this.StartYear = this.Start.Year;
            this.End = DateTime.Now;
            this.EndYear = this.End.Year;
            this.NumberOfDays = 0;
        }

        #endregion

        #region Properties

        public DateTime Start
        {
            get { return _start; }
            set { this.SetProperty(ref _start, value, this.OnStartChanged); }
        }

        public DateTime End
        {
            get { return _end; }
            set { this.SetProperty(ref _end, value, this.OnEndChanged); }
        }

        public TimeSpan Duration
        {
            get
            {
                var numberOfYears = (this.EndYear - this.StartYear) + 1;
                var numberOfDays = numberOfYears * 365 - 1;

                return TimeSpan.FromDays(numberOfDays);
            }
        }

        public int NumberOfDays
        {
            get { return _numberOfDays; }
            set { this.SetProperty(ref _numberOfDays, value, this.OnNumberOfDaysChanged); }
        }

        /// <summary>
        /// The start year of the period. This is used for some components that need a beginning year. (i.e. a <see cref="FieldSystemComponent"/> needs to have a starting ICBM equilibrium year)
        /// </summary>
        public int StartYear
        {
            get { return _startYear; }
            set
            {
                this.SetProperty(ref _startYear, value, this.OnStartYearChanged);
            }
        }

        public int EndYear
        {
            get { return _endYear; }
            set 
            {
                if (value < this.StartYear)
                {
                    return;
                }
                
                this.SetProperty(ref _endYear, value, this.OnEndYearChanged);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        private void OnStartYearChanged()
        {
            if (this.StartYear == 0)
            {
                return;
            }

            this.Start = new DateTime(this.StartYear, 1, 1);

            var numberOfYears = this.EndYear - this.StartYear + 1;

            var numberOfDays = numberOfYears * 365;
            this.NumberOfDays = numberOfDays;
        }

        private void OnEndYearChanged()
        {
            this.End = new DateTime(this.EndYear, this.End.Month, this.End.Day);
        }

        private void OnStartChanged()
        {
            this.RaisePropertyChanged(nameof(this.Duration));
            this.End = this.Start.AddDays(_numberOfDays);
        }

        private void OnEndChanged()
        {
            this.RaisePropertyChanged(nameof(this.Duration));
        }

        private void OnNumberOfDaysChanged()
        {
            this.End = this.Start.AddDays(_numberOfDays);
            this.EndYear = this.End.Year;
        }

        #endregion 
    }
}