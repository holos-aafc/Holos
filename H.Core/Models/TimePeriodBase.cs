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
        #region Constructors

        public TimePeriodBase()
        {
            Start = DateTime.Now;
            StartYear = Start.Year;
            End = DateTime.Now;
            EndYear = End.Year;
            NumberOfDays = 0;
        }

        #endregion

        #region Fields

        private DateTime _start;
        private DateTime _end;
        private int _numberOfDays;
        private int _startYear;
        private int _endYear;

        #endregion

        #region Properties

        public DateTime Start
        {
            get => _start;
            set => SetProperty(ref _start, value, OnStartChanged);
        }

        public DateTime End
        {
            get => _end;
            set => SetProperty(ref _end, value, OnEndChanged);
        }

        public TimeSpan Duration
        {
            get
            {
                var numberOfYears = EndYear - StartYear + 1;
                var numberOfDays = numberOfYears * 365 - 1;

                return TimeSpan.FromDays(numberOfDays);
            }
        }

        public int NumberOfDays
        {
            get => _numberOfDays;
            set => SetProperty(ref _numberOfDays, value, OnNumberOfDaysChanged);
        }

        /// <summary>
        ///     The start year of the period. This is used for some components that need a beginning year. (i.e. a
        ///     <see cref="FieldSystemComponent" /> needs to have a starting ICBM equilibrium year)
        /// </summary>
        public int StartYear
        {
            get => _startYear;
            set => SetProperty(ref _startYear, value, OnStartYearChanged);
        }

        public int EndYear
        {
            get => _endYear;
            set
            {
                if (value < StartYear) return;

                SetProperty(ref _endYear, value, OnEndYearChanged);
            }
        }

        #endregion

        #region Event Handlers

        private void OnStartYearChanged()
        {
            if (StartYear == 0) return;

            Start = new DateTime(StartYear, 1, 1);

            var numberOfYears = EndYear - StartYear + 1;

            var numberOfDays = numberOfYears * 365;
            NumberOfDays = numberOfDays;
        }

        private void OnEndYearChanged()
        {
            End = new DateTime(EndYear, End.Month, End.Day);
        }

        private void OnStartChanged()
        {
            RaisePropertyChanged(nameof(Duration));
            End = Start.AddDays(_numberOfDays);
        }

        private void OnEndChanged()
        {
            RaisePropertyChanged(nameof(Duration));
        }

        private void OnNumberOfDaysChanged()
        {
            End = Start.AddDays(_numberOfDays);
            EndYear = End.Year;
        }

        #endregion
    }
}