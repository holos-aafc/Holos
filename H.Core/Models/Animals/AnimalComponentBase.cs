#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Models.Animals
{
    /// <summary>
    /// </summary>
    public abstract class AnimalComponentBase : ComponentBase
    {
        #region Fields

        private ObservableCollection<AnimalGroup> _groups;

        #endregion

        #region Constructors

        protected AnimalComponentBase()
        {
            HTraceListener.AddTraceListener();
            this.Groups = new ObservableCollection<AnimalGroup>();

            this.PropertyChanged += OnPropertyChanged;

            this.Groups.CollectionChanged -= GroupsOnCollectionChanged;
            this.Groups.CollectionChanged += GroupsOnCollectionChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<AnimalGroup> Groups
        {
            get { return _groups; }
            set { SetProperty(ref _groups, value); }
        }

        #endregion

        #region Fields

        #endregion

        #region Public Methods

        public double CalculateAverageLitterSize(
            Tuple<AnimalType, ProductionStages> youngAnimalTypeAndStage, 
            List<Tuple<AnimalType, ProductionStages>> otherAnimalsAndStages)
        {
            var youngAnimalGroups = this.Groups.Where(x => x.GroupType == youngAnimalTypeAndStage.Item1);
            var totalYoungAnimals = 0d;
            foreach (var youngAnimalGroup in youngAnimalGroups)
            {
                totalYoungAnimals += youngAnimalGroup.GetNumberOfAnimalsInFirstManagementPeriodByProductionStage(
                    productionStage: youngAnimalTypeAndStage.Item2);
            }

            var totalOtherAnimals = 0d;
            foreach (var animalType in otherAnimalsAndStages)
            {
                var otherAnimalGroups = this.Groups.Where(x => x.GroupType == animalType.Item1);
                foreach (var otherAnimalGroup in otherAnimalGroups)
                {
                    totalOtherAnimals += otherAnimalGroup.GetNumberOfAnimalsInFirstManagementPeriodByProductionStage(
                        productionStage: animalType.Item2);
                }
            }

            return totalYoungAnimals / totalOtherAnimals;
        }

        /// <summary>
        /// We don't recalculate results if these properties change
        /// </summary>
        public static bool IsAnimalComponentPropertyRelatedToCalculations(string propertyName)
        {
            if (propertyName.Equals(nameof(FieldSystemComponent.ResultsCalculated)))  // Prevent loop
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Searches through the component's group of animals looking for the group of animals that has the same pairing number as that of the given young animal group.
        /// </summary>
        /// <param name="youngAnimalGroup">The group of young animals that belong the a parent group.</param>
        /// <param name="parentGroupType">The <see cref="AnimalType"/>of the parent <see cref="AnimalGroup"/></param>
        /// <returns>A single <see cref="AnimalGroup"/> that is the parent of the specified young animal group or null if no associated parent group was found</returns>
        public AnimalGroup GetAssociatedParentGroup(
            AnimalGroup youngAnimalGroup,
            AnimalType parentGroupType)
        {
            if (parentGroupType.IsLactatingType() == false)
            {
                Trace.TraceInformation($"{nameof(AnimalComponentBase)}.{nameof(GetAssociatedParentGroup)}: parent group type '{parentGroupType.GetDescription()}' is not a lactating animal group type, so there can be no associate parent group for this group of young animals: '{youngAnimalGroup.GroupType.GetDescription()}'.");

                return null;
            }

            // A parent group can have many groups of young animals, but for any group of young animals, there can only be one parent group
            if (youngAnimalGroup.GroupPairingNumber > 0)
            {
                return this.Groups.FirstOrDefault(group => @group.GroupType == parentGroupType && @group.GroupPairingNumber.Equals(youngAnimalGroup.GroupPairingNumber));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the <see cref="ManagementPeriod"/> of the parent group of animals for a given <see cref="AnimalGroup"/> of young animals on a given <see cref="DateTime"/>.
        ///
        /// It is possible to have null returned when there is no <see cref="ManagementPeriod"/> belonging to the parent <see cref="AnimalGroup"/> that contains the specified date.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to search by</param>
        /// <param name="youngAnimalGroup">The young <see cref="AnimalGroup"/> to search by</param>
        /// <param name="parentGroupType">The parent <see cref="AnimalType"/> to search by</param>
        /// <returns>The <see cref="ManagementPeriod"/> if found, null otherwise</returns>
        public ManagementPeriod GetAssociatedManagementPeriodOfParentGroup(
            DateTime dateTime,
            AnimalGroup youngAnimalGroup,
            AnimalType parentGroupType)
        {
            var parentAnimalGroup = this.GetAssociatedParentGroup(
                youngAnimalGroup: youngAnimalGroup,
                parentGroupType: parentGroupType);

            if (parentAnimalGroup != null)
            {
                return parentAnimalGroup.GetManagementPeriodByDate(dateTime);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches through the component's group of animals looking for animal groups that have the same pairing number as the parent group type (i.e. searches for all
        /// calf groups that are associated with a group of cows). The parent <see cref="AnimalGroup"/> must be a lactating type of animal otherwise no associated
        /// young animals will be returned.
        /// </summary>
        /// <param name="parentGroup">The parent <see cref="AnimalGroup"/></param>
        /// <param name="childGroupType">The <see cref="AnimalType"/> of the child <see cref="AnimalGroup"/>s</param>
        /// <returns>A collection of associated child <see cref="AnimalGroup"/>s</returns>
        public List<AnimalGroup> GetAssociatedYoungAnimalGroups(
            AnimalGroup parentGroup,
            AnimalType childGroupType)
        {
            var result = new List<AnimalGroup>();

            if (parentGroup.GroupType.IsLactatingType() == false)
            {
                return result;
            }

            // Parent group pairing number will be 0 if group is not paired with another (child) group (or group is not a 'mother' type group - i.e. bulls). Only look for groups with a pairing number that isn't zero.
            if (parentGroup.GroupPairingNumber > 0)
            {
                var groups = this.Groups.Where(group => @group.GroupType == childGroupType && @group.GroupPairingNumber.Equals(parentGroup.GroupPairingNumber)).ToList();
                if (groups.Any())
                {
                    result.AddRange(groups);
                }
            }

            return result;
        }

        /// <summary>
        /// Searches through the list of groups and finds all <see cref="ManagementPeriod"/> for the given <see cref="AnimalType"/> and the specified <see cref="DateTime"/>.
        /// </summary>
        public List<ManagementPeriod> GetAssociateManagementPeriodsByGroup(AnimalType animalType, DateTime dateTime)
        {
            var result = new List<ManagementPeriod>();

            var animalGroups = this.Groups.Where(x => x.GroupType == animalType);
            foreach (var animalGroup in animalGroups)
            {
                var managementPeriod = animalGroup.GetManagementPeriodByDate(dateTime);
                if (managementPeriod != null)
                {
                    result.Add(managementPeriod);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the total number of young animals associated with a parent <see cref="AnimalGroup"/> by <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to search by</param>
        /// <param name="parentGroup">The <see cref="AnimalGroup"/> of the young animals</param>
        /// <param name="childGroupType">The <see cref="AnimalType"/> of the young animals</param>
        /// <returns>The number of young animals</returns>
        public int GetTotalNumberOfYoungAnimalsByDate(DateTime dateTime, AnimalGroup parentGroup, AnimalType childGroupType)
        {
            var result = 0;

            if (parentGroup.GroupType.IsLactatingType() == false || (parentGroup.GroupType == childGroupType))
            {
                return result;
            }

            var associatedYoungAnimalGroups = this.GetAssociatedYoungAnimalGroups(
                parentGroup: parentGroup,
                childGroupType: childGroupType);

            foreach (var associatedYoungAnimalGroup in associatedYoungAnimalGroups)
            {
                var numberOfYoungAnimalsOnDate = associatedYoungAnimalGroup.GetNumberOfAnimalsByDate(
                    dateTime: dateTime);

                result += numberOfYoungAnimalsOnDate;
            }

            return result;
        }

        /// <summary>
        /// Calculates the average fertility rate.
        /// </summary>
        public double GetFertilityRate(AnimalType targetAnimalType, IEnumerable<ProductionStages> productionStages)
        {
            var fertilityRates = new List<double>();

            foreach (var animalGroup in this.Groups.Where(x => x.GroupType == targetAnimalType))
            {
                var totalDaysInAllProductionStages = 0.0;
                foreach (var productionStage in productionStages)
                {
                    var totalDaysInThisProductionStage = animalGroup.ManagementPeriods.Where(x => x.ProductionStage == productionStage).Sum(x => x.Duration.TotalDays);
                    totalDaysInAllProductionStages += totalDaysInThisProductionStage;
                }               

                var fertilityRate = 365 / totalDaysInAllProductionStages;

                fertilityRates.Add(fertilityRate);
            }

            return fertilityRates.Average();
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is AnimalComponentBase animalComponent)
            {
            }
        }

        private void GroupsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
            }

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
            }
        }

        private void AnimalGroupOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is AnimalGroup animalGroup)
            {
            }
        }        

        #endregion
    }
}