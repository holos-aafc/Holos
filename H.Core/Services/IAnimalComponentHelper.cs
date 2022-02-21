using System.Collections.Generic;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services
{
    public interface IAnimalComponentHelper
    {
        /// <summary>
        /// Creates a unique name for a <see cref="ManagementPeriod"/>. This ensures each new <see cref="ManagementPeriod"/> created by the user
        /// has a non-duplicated name.
        /// </summary>
        string GetUniqueManagementPeriodName(IEnumerable<ManagementPeriod> animalGroupManagementPeriods);

        /// <summary>
        /// Creates a unique name for a <see cref="ManagementPeriod"/> given a set of existing <see cref="ManagementPeriod"/>s from an <see cref="AnimalGroup"/>. 
        /// This ensures each new <see cref="ManagementPeriod"/> created by the user has a non-duplicated name.
        /// </summary>
        string GetUniqueManagementPeriodName(AnimalGroup group);

        /// <summary>
        /// Replicates/copies a <see cref="ManagementPeriod"/>.
        /// </summary>
        ManagementPeriod ReplicateManagementPeriod(ManagementPeriod managementPeriodToReplicate);

        /// <summary>
        /// Replicates/copies <see cref="ManureDetails"/>.
        /// </summary>
        void ReplicateManureDetails(ManagementPeriod source, ManagementPeriod destination);

        /// <summary>
        /// Replicates/copies <see cref="HousingDetails"/>.
        /// </summary>
        void ReplicateHousingDetails(ManagementPeriod source, ManagementPeriod destination);

        /// <summary>
        /// Allows the user to create a copy of an animal component.
        /// </summary>
        AnimalComponentBase ReplicateAnimalComponent(AnimalComponentBase component);

        /// <summary>
        /// Allows the user to create a copy of an animal component.
        /// </summary>
        void ReplicateAnimalComponent(ComponentBase component, ComponentBase componentToReplicate);

        /// <summary>
        /// Common logic to initilize an animal component.
        /// </summary>
        void InitializeComponent(AnimalComponentBase component, Farm farm);

        /// <summary>
        /// Returns a new name for a group of animals that is unique among a list of existing animal groups.
        /// </summary>
        string GetUniqueGroupName(IEnumerable<AnimalGroup> animalGroups, AnimalGroup animalGroup);

        /// <summary>
        /// Returns a number that is used to pair a group of calves with a group of cows.
        /// </summary>
        int GetNextPairingNumber(IEnumerable<AnimalGroup> animalGroups);

        /// <summary>
        /// GUI uses a management period which can span multiple months - but algorithms use single months for calculations. Method takes in a <see cref="ManagementPeriod"/>
        /// and returns a collection of objects that represent single months with the total number of days in that month. The returned collection of objects are all of the
        /// months that are contained in the <see cref="ManagementPeriod"/>.
        /// </summary>
        IEnumerable<MonthsAndDaysData> GetMonthlyBreakdownFromManagementPeriod(ManagementPeriod managementPeriod);

        /// <summary>
        /// Not all animal groups have equations to calculate DMI at this time
        /// </summary>
        bool AnimalGroupHasDmiCalculations(AnimalGroup animalGroup);
    }
}