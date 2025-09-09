﻿#region Imports

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Models.Animals
{
    /// <summary>
    /// </summary>
    public class AnimalGroup : ModelBase
    {
        #region Constructors

        public AnimalGroup()
        {
            HTraceListener.AddTraceListener();

            ManagementPeriods.CollectionChanged -= ManagementPeriodsOnCollectionChanged;
            ManagementPeriods.CollectionChanged += ManagementPeriodsOnCollectionChanged;

            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(AnimalTypeString)}: {AnimalTypeString}";
        }

        #endregion

        #region Fields

        private AnimalType _groupType;
        private AnimalType _groupTypeDiet;

        private double _litterSizeOfBirthingAnimal;
        private double _weightOfPigletsAtBirth;
        private double _weightOfWeanedAnimals;

        private bool _excludeFromEmissionResults;

        #endregion

        #region Properties

        /// <summary>
        ///     The total number of days of rest between animal production system cycles. This is the time needed to clean
        ///     pens/barns etc. The total number
        ///     of days in the production period (non-rest days) is calculated by totaling the number of days over all the
        ///     <see cref="ManagementPeriods" /> in
        ///     the production period.
        /// </summary>
        public int NumberOfDaysRestBetweenCycles { get; set; }

        public ObservableCollection<ManagementPeriod> ManagementPeriods { get; set; } =
            new ObservableCollection<ManagementPeriod>();

        /// <summary>
        ///     The average number of young animals (calves, lambs, piglets, etc.) that are born for each pregnant animal in this
        ///     <see cref="AnimalGroup" />.
        ///     (heads)
        /// </summary>
        public double LitterSizeOfBirthingAnimal
        {
            get => _litterSizeOfBirthingAnimal;
            set => SetProperty(ref _litterSizeOfBirthingAnimal, value);
        }

        public string AnimalTypeString => GroupType.GetDescription();

        public AnimalType GroupType
        {
            get => _groupType;
            set { SetProperty(ref _groupType, value, () => { RaisePropertyChanged(nameof(AnimalTypeString)); }); }
        }

        /// <summary>
        ///     Some animal group types share a diet because they do not have their own. This is used to indicate which diet an
        ///     animal can use. A group can be assigned
        ///     a specific diet directly by assigning to the <see cref="ManagementPeriod.SelectedDiet" /> property.
        /// </summary>
        public AnimalType GroupTypeDiet
        {
            get => _groupTypeDiet;
            set => SetProperty(ref _groupTypeDiet, value);
        }

        /// <summary>
        ///     Allows for one or more groups to be paired together (i.e. a cow group must be paired with a calf group so they
        ///     would have the same pair number)
        /// </summary>
        public int GroupPairingNumber { get; set; }

        /// <summary>
        ///     A string to indicate which groups of animals are paired together.
        /// </summary>
        public string GroupPairingDescription { get; set; }

        /// <summary>
        ///     (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double WeightOfWeanedAnimals
        {
            get => _weightOfWeanedAnimals;
            set => SetProperty(ref _weightOfWeanedAnimals, value);
        }

        /// <summary>
        ///     (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double WeightOfPigletsAtBirth
        {
            get => _weightOfPigletsAtBirth;
            set => SetProperty(ref _weightOfPigletsAtBirth, value);
        }

        /// <summary>
        ///     Some animal groups don't produce any emission and should be excluded from the final reporting (e.g. nursing lambs,
        ///     chicken eggs, poults, etc.)
        /// </summary>
        public bool ExcludeFromEmissionResults
        {
            get => _excludeFromEmissionResults;
            set => SetProperty(ref _excludeFromEmissionResults, value);
        }

        #endregion

        #region Public Methods

        public double GetNumberOfAnimalsInFirstManagementPeriodByProductionStage(ProductionStages productionStage)
        {
            var firstManagementPeriod = ManagementPeriods.OrderBy(x => x.Start)
                .Where(x => x.ProductionStage == productionStage).FirstOrDefault();
            if (firstManagementPeriod != null) return firstManagementPeriod.NumberOfAnimals;

            return 0;
        }

        /// <summary>
        ///     Calculates the weight change of a group of animals who are pregnant for the gestation period.
        /// </summary>
        /// <returns>The change in weight (kg)</returns>
        public double CalculateWeightChangeOfPregnantAnimal()
        {
            var gestationManagementPeriods = ManagementPeriods
                .Where(x => x.ProductionStage == ProductionStages.Gestating).OrderBy(x => x.Start).ToList();
            if (gestationManagementPeriods.Any())
            {
                var startWeightOfGestation = gestationManagementPeriods.First().StartWeight;
                var endWeightOfGestation = gestationManagementPeriods.Last().EndWeight;

                return endWeightOfGestation - startWeightOfGestation;
            }

            return 0;
        }

        public int TotalNumberOfAnimalsSurviving()
        {
            var startingNumber = 0;
            var endingNumber = 0;

            for (var i = 0; i < ManagementPeriods.Count; i++)
            {
                if (i == 0)
                {
                    var firstManagementPeriod = ManagementPeriods.ElementAtOrDefault(i);
                    if (firstManagementPeriod != null) startingNumber = firstManagementPeriod.NumberOfAnimals;
                }

                if (i == ManagementPeriods.Count - 1)
                {
                    var lastManagementPeriod = ManagementPeriods.ElementAtOrDefault(i);
                    if (lastManagementPeriod != null) endingNumber = lastManagementPeriod.NumberOfAnimals;
                }
            }

            if (startingNumber == endingNumber) return startingNumber; // No loss of animals

            if (startingNumber > endingNumber)
                // Lost animals
                return startingNumber - endingNumber + startingNumber;

            // Gained animals
            return endingNumber - startingNumber + startingNumber;
        }

        public int GetNumberOfAnimalDuringMonth(int month, int year)
        {
            var managementPeriod = GetManagementPeriodByMonthAndYear(month, year);
            if (managementPeriod != null) return managementPeriod.NumberOfAnimals;

            return 0;
        }

        /// <summary>
        ///     Searches the <see cref="AnimalGroup" />s <see cref="ManagementPeriods" /> to see if animals existed on a particular
        ///     date.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime" /> to search by</param>
        /// <returns>The number of animals that existed on the given date or 0 if no animals are defined for the date</returns>
        public int GetNumberOfAnimalsByDate(DateTime dateTime)
        {
            var result = 0;

            var managementPeriod = GetManagementPeriodByDate(dateTime);
            if (managementPeriod != null) result = managementPeriod.NumberOfAnimals;

            return result;
        }

        /// <summary>
        ///     Searches through the <see cref="AnimalGroup" />s <see cref="ManagementPeriods" /> to see if a
        ///     <see cref="ManagementPeriod" /> exists that contains the given
        ///     date.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime" /> to search by</param>
        /// <returns>
        ///     The <see cref="ManagementPeriod" /> that contains the given <see cref="DateTime" />, or null if the
        ///     <see cref="ManagementPeriod" /> doesn't exist
        /// </returns>
        public ManagementPeriod GetManagementPeriodByDate(DateTime dateTime)
        {
            var result = ManagementPeriods.OrderBy(managementPeriod => managementPeriod.Start)
                .FirstOrDefault(managementPeriod =>
                    managementPeriod.Start <= dateTime && managementPeriod.End >= dateTime);
            if (result != null) return result;

            // When considering a group of cows and associated calves, the user will have two separate management periods. If the cow group and the calf group don't have overlapping
            // dates for the two separate management periods, then the result will be null.

            return null;
        }

        /// <summary>
        ///     Returns the management period that contains the given month and year.
        /// </summary>
        public ManagementPeriod GetManagementPeriodByMonthAndYear(int month, int year)
        {
            var managementPeriod = ManagementPeriods.OrderBy(x => x.Start)
                .FirstOrDefault(x => x.Start.Year >= year && x.Start.Month <= month);
            if (managementPeriod != null) return managementPeriod;

            // When considering a group of cows and associated calves, the user will have two separate management periods. If the cow group and the calf group don't have overlapping start and end
            // dates for the management period, then the management period will be null.

            Trace.TraceError(
                $"{nameof(AnimalGroup)}.{nameof(GetManagementPeriodByMonthAndYear)}: no management period found for {Name} ({AnimalTypeString}), with month '{month}' and year '{year}'");

            return null;
        }

        #endregion

        #region Private Methods

        private void UpdateSubsequentStartDates(int index)
        {
            if (ManagementPeriods.Any() == false) return;

            while (true)
            {
                // Need to update subsequent management period start dates
                var currentManagementPeriod = ManagementPeriods.ElementAt(index);
                var nextManagementPeriod = ManagementPeriods.ElementAtOrDefault(index + 1);
                if (nextManagementPeriod != null)
                {
                    nextManagementPeriod.Start = currentManagementPeriod.End.AddDays(1);
                    index++;
                }
                else
                {
                    break;
                }
            }
        }

        private void UpdateSubsequentStartWeights(int index)
        {
            if (ManagementPeriods.Any() == false) return;

            while (true)
            {
                // Need to update subsequent management period start weights
                var currentManagementPeriod = ManagementPeriods.ElementAt(index);
                var nextManagementPeriod = ManagementPeriods.ElementAtOrDefault(index + 1);
                if (nextManagementPeriod != null)
                {
                    nextManagementPeriod.StartWeight = currentManagementPeriod.EndWeight;
                    nextManagementPeriod.EndWeight = nextManagementPeriod.StartWeight +
                                                     nextManagementPeriod.PeriodDailyGain *
                                                     nextManagementPeriod.NumberOfDays;
                    index++;
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }

        protected virtual void ManagementPeriodsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var item = notifyCollectionChangedEventArgs.NewItems[0];
                if (item is ManagementPeriod managementPeriod)
                    managementPeriod.PropertyChanged += ManagementPeriodOnPropertyChanged;
            }

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                var indexOfItemRemoved = notifyCollectionChangedEventArgs.OldStartingIndex;
                var index = 0;
                if (indexOfItemRemoved > 0) index = --indexOfItemRemoved;

                UpdateSubsequentStartDates(index);
                UpdateSubsequentStartWeights(index);
            }
        }

        private void ManagementPeriodOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is ManagementPeriod managementPeriod)
            {
                if (propertyChangedEventArgs.PropertyName.Equals(nameof(ManagementPeriod.NumberOfDays)))
                {
                    // Get index of the ManagementPeriod that sent the PropertyChanged event
                    var index = ManagementPeriods.IndexOf(
                        ManagementPeriods.SingleOrDefault(x => x.Guid == managementPeriod.Guid));
                    if (index >= 0) UpdateSubsequentStartDates(index);
                }

                if (propertyChangedEventArgs.PropertyName.Equals(nameof(ManagementPeriod.EndWeight)))
                {
                    // Get index of the ManagementPeriod that sent the PropertyChanged event
                    var index = ManagementPeriods.IndexOf(
                        ManagementPeriods.SingleOrDefault(x => x.Guid == managementPeriod.Guid));
                    if (index >= 0) UpdateSubsequentStartWeights(index);
                }
            }
        }

        #endregion
    }
}