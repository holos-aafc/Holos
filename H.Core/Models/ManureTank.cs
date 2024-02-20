using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models
{
    /// <summary>
    /// A storage tank for a particular type of manure (beef, dairy, etc.)
    /// </summary>
    public class ManureTank : StorageTankBase
    {
        #region Fields

        private Dictionary<int, List<GroupEmissionsByDay>> _dailyResults;

        private ManureStateType _manureStateType;
        private AnimalType _animalType;

        #endregion

        #region Constructors

        public ManureTank()
        {
            _dailyResults = new Dictionary<int, List<GroupEmissionsByDay>>();
        }

        #endregion

        #region Properties

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        public ManureStateType ManureStateType
        {
            get => _manureStateType;
            set => SetProperty(ref _manureStateType, value);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(AnimalType)}: {AnimalType}";
        }

        public void AddDailyResultToTank(GroupEmissionsByDay groupEmissionsByDay)
        {
            var dayOfYear = groupEmissionsByDay.DateTime.DayOfYear;

            if (_dailyResults.ContainsKey(dayOfYear))
            {
                var resultsForDay = _dailyResults[dayOfYear];
                resultsForDay.Add(groupEmissionsByDay);
            }
            else
            {
                _dailyResults[dayOfYear] = new List<GroupEmissionsByDay>() {groupEmissionsByDay};
            }
        }

        public double GetVolumeCreatedOnDate(DateTime dateTime)
        {
            var result = 0d;

            var dayOfYear = dateTime.DayOfYear;
            if (_dailyResults.ContainsKey(dayOfYear))
            {
                var allDailyResults = _dailyResults[dayOfYear];
                return allDailyResults.Sum(x => x.TotalVolumeOfManureAvailableForLandApplicationInKilograms);
            }

            return result;
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}