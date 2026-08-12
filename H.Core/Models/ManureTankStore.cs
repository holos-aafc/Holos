using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;

namespace H.Core.Models
{
    /// <summary>
    /// The set of manure tanks for one farm calculation, keyed by (animal category, year, manure state type). Intended
    /// to be created once per run and shared across the pipeline so there is a single source of truth for tank
    /// accounting: the animal results populate each tank's daily storage, ManureService adds the whole-year totals, and
    /// downstream consumers - exports, indirect N2O, soil-carbon inputs, anaerobic digestion - read the same tanks
    /// (issue #451 follow-up).
    /// </summary>
    public class ManureTankStore
    {
        private readonly List<ManureTank> _tanks = new List<ManureTank>();

        public IReadOnlyList<ManureTank> Tanks => _tanks;

        /// <summary>
        /// Returns the tank for the (animal category, year, manure state type) triple, creating and adding it if it does
        /// not already exist. There is at most one tank per triple.
        /// </summary>
        public ManureTank GetOrCreate(AnimalType animalType, int year, ManureStateType manureStateType)
        {
            var tank = _tanks.SingleOrDefault(x =>
                x.AnimalType.GetCategory() == animalType.GetCategory() &&
                x.Year == year &&
                x.ManureStateType == manureStateType);

            if (tank == null)
            {
                tank = new ManureTank { AnimalType = animalType, Year = year, ManureStateType = manureStateType };
                _tanks.Add(tank);
            }

            return tank;
        }

        public void Clear()
        {
            _tanks.Clear();
        }
    }
}
