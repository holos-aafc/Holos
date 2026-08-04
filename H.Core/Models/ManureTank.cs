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

        // Daily manure volume (kg) flowing into this tank, summed across every management period that feeds it.
        private readonly SortedDictionary<DateTime, double> _inflowKilogramsByDate = new SortedDictionary<DateTime, double>();

        // Day-by-day storage state, produced by ComputeDailyStorage: the net-of-removals volume (the Eq. 4.1.3-6
        // denominator) and the shared removal fraction on each day.
        private readonly Dictionary<DateTime, double> _netOfRemovalsByDate = new Dictionary<DateTime, double>();
        private readonly Dictionary<DateTime, double> _removalFractionByDate = new Dictionary<DateTime, double>();

        private ManureStateType _manureStateType;
        private AnimalType _animalType;

        #endregion

        #region Constructors

        public ManureTank()
        {
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

        /// <summary>
        /// The net-of-removals volume (kg) in this tank on each day - the corrected Equation 4.1.3-6 denominator, shared
        /// by every management period feeding the tank. Populated by <see cref="ComputeDailyStorage"/>.
        /// </summary>
        public IReadOnlyDictionary<DateTime, double> NetOfRemovalsByDate => _netOfRemovalsByDate;

        /// <summary>
        /// The fraction of this tank removed on each day (0 to the daily cap). Populated by <see cref="ComputeDailyStorage"/>.
        /// </summary>
        public IReadOnlyDictionary<DateTime, double> RemovalFractionByDate => _removalFractionByDate;

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(AnimalType)}: {AnimalType}";
        }

        /// <summary>
        /// Adds a day's manure volume (kg) flowing into this tank. Called once per contributing management period per
        /// day; volumes on the same date are summed, so the accumulated inflow is the whole tank's, not one period's.
        /// </summary>
        public void AddDailyInflow(DateTime date, double kilograms)
        {
            var day = date.Date;
            var existing = _inflowKilogramsByDate.TryGetValue(day, out var value) ? value : 0;
            _inflowKilogramsByDate[day] = existing + kilograms;
        }

        /// <summary>
        /// Rolls the tank's storage forward one day at a time over its accumulated inflow: each day's net-of-removals
        /// volume is yesterday's remaining volume (reduced by yesterday's removal fraction) plus today's inflow, and
        /// each day's removal fraction is that day's removed volume over the net volume, capped at the daily maximum.
        /// The removed volume is supplied by <paramref name="removedVolumeOnDate"/> so the tank stays independent of how
        /// removals are sourced.
        /// </summary>
        public void ComputeDailyStorage(Func<DateTime, double> removedVolumeOnDate)
        {
            _netOfRemovalsByDate.Clear();
            _removalFractionByDate.Clear();

            var previousNet = 0d;
            var previousFraction = 0d;
            foreach (var day in _inflowKilogramsByDate) // SortedDictionary enumerates in ascending date order
            {
                var net = ManureStorageMath.NetAmountInStorage(previousNet, day.Value, previousFraction);
                var removed = removedVolumeOnDate(day.Key);
                var fraction = ManureStorageMath.BoundRemovalFraction(net > 0 ? removed / net : 0);

                _netOfRemovalsByDate[day.Key] = net;
                _removalFractionByDate[day.Key] = fraction;

                previousNet = net;
                previousFraction = fraction;
            }
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}