#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Providers.Feed;

#endregion

namespace H.Core.Services
{
    /// <summary>
    /// </summary>
    public class AnimalComponentHelper : IAnimalComponentHelper
    {
        #region Fields

        private readonly IMapper _animalGroupMapper;
        private readonly IMapper _managementPeriodMapper;
        private readonly ITimePeriodHelper _timePeriodHelper = new TimePeriodHelper(); 

        #endregion

        #region Constructors

        public AnimalComponentHelper()
        {
            var animalGroupMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<AnimalGroup, AnimalGroup>()
                    .ForMember(y => y.Guid, z => z.Ignore())
                    .ForMember(y => y.ManagementPeriods, z => z.Ignore());
            });

            _animalGroupMapper = animalGroupMapperConfiguration.CreateMapper();

            var managementPeriodMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<ManagementPeriod, ManagementPeriod>()
                    .ForMember(y => y.Guid, z => z.Ignore())
                    .ForMember(y => y.HousingDetails, z => z.Ignore())
                    .ForMember(y => y.ManureDetails, z => z.Ignore());
            });

            _managementPeriodMapper = managementPeriodMapperConfiguration.CreateMapper();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public IEnumerable<MonthsAndDaysData> GetMonthlyBreakdownFromManagementPeriod(ManagementPeriod managementPeriod)
        {
            var result = new List<MonthsAndDaysData>();

            var start = managementPeriod.Start;
            var duration = managementPeriod.Duration;
            var end = start.Add(duration);

            // Get list of months and years in time period
            var monthsAndYearsList = _timePeriodHelper.GetMonthsBetweenDates(start, end).ToList();
            for (int i = 0; i < monthsAndYearsList.Count(); i++)
            {
                var monthlyObject = monthsAndYearsList.ElementAt(i);

                var month = monthlyObject.Month;
                var year = monthlyObject.Year;

                var monthlyInputData = new MonthsAndDaysData
                {
                    Month = month,
                    Year = year,
                    DaysInMonth = _timePeriodHelper.GetNumberOfDaysOccupyingMonth(start, duration, month, year),
                    StartDay = i == 0 ? start.Day : 1, // Only the first object will have the startdate of the management period, all others will start on the first day of the month
                };

                result.Add(monthlyInputData);
            }

            // Calculate start and end weights of the animals for each of the months in the management period
            for (int i = 0; i < result.Count; i++)
            {
                var monthlyData = result.ElementAt(i);

                if (i == 0)
                {
                    monthlyData.StartWeightInMonth = managementPeriod.StartWeight;

                    var gainInMonth = managementPeriod.PeriodDailyGain * monthlyData.DaysInMonth;
                    monthlyData.EndWeightInMonth = monthlyData.StartWeightInMonth + gainInMonth;
                }
                else
                {
                    var previousMonth = result.ElementAt(i - 1);
                    var currentMonth = result.ElementAt(i);

                    currentMonth.StartWeightInMonth = previousMonth.EndWeightInMonth;
                    var gainInMonth = managementPeriod.PeriodDailyGain * monthlyData.DaysInMonth;
                    currentMonth.EndWeightInMonth = currentMonth.StartWeightInMonth + gainInMonth;
                }
            }

            return result;
        }
        
        public string GetUniqueGroupName(IEnumerable<AnimalGroup> animalGroups, AnimalGroup animalGroup)
        {
            var i = 1;

            // Don't add number to group name at first (i.e. just use "Heifers group" and not "Heifer group #1") since it is more cleaner.
            var proposedName = animalGroup.AnimalTypeString;
            while (animalGroups.Any(group => group.Name == proposedName))
            {
                proposedName = animalGroup.AnimalTypeString + " " + string.Format(Properties.Resources.InterpolatedGroupNumber, ++i);
            }

            return proposedName;
        }

        public int GetNextPairingNumber(IEnumerable<AnimalGroup> animalGroups)
        {
            var pairNumbersInUse = animalGroups.Select(x => x.GroupPairingNumber).Distinct();
            var nextPairNumber = 0;
            do
            {
                nextPairNumber++;
            } while (pairNumbersInUse.Contains(nextPairNumber));

            return nextPairNumber;
        }

        public string GetUniqueManagementPeriodName(AnimalGroup group)
        {
            return this.GetUniqueManagementPeriodName(group.ManagementPeriods);
        }

        public string GetUniqueManagementPeriodName(IEnumerable<ManagementPeriod> animalGroupManagementPeriods)
        {
            var i = 1;
            var periodCount = animalGroupManagementPeriods.Count();

            var proposedName = string.Format(Properties.Resources.InterpolatedManagementPeriodNumber, i);
            while (animalGroupManagementPeriods.Any(group => group.Name == proposedName))
                proposedName = string.Format(Properties.Resources.InterpolatedManagementPeriodNumber, ++i);

            return proposedName;
        }

        public ManagementPeriod ReplicateManagementPeriod(ManagementPeriod managementPeriodToReplicate)
        {
            var managementPeriod = new ManagementPeriod();

            _managementPeriodMapper.Map(managementPeriodToReplicate, managementPeriod);

            managementPeriod.SelectedDiet = Diet.CopyDiet(managementPeriodToReplicate.SelectedDiet);

            this.ReplicateHousingDetails(managementPeriodToReplicate, managementPeriod);
            this.ReplicateManureDetails(managementPeriodToReplicate, managementPeriod);

            return managementPeriod;
        }

        public void ReplicateManureDetails(ManagementPeriod from, ManagementPeriod to)
        {
            var configuration = new MapperConfiguration(x =>
            {
                x.CreateMap<ManureDetails, ManureDetails>()
                 .ForMember(y => y.Name, z => z.Ignore())
                 .ForMember(y => y.Guid, z => z.Ignore());
            });

            var mapper = configuration.CreateMapper();

            var manureDetails = new ManureDetails();
            mapper.Map(from.ManureDetails, manureDetails);
            to.ManureDetails = manureDetails;
        }

        public void ReplicateHousingDetails(ManagementPeriod from, ManagementPeriod to)
        {
            var configuration = new MapperConfiguration(x =>
            {
                x.CreateMap<HousingDetails, HousingDetails>()
                 .ForMember(y => y.Name, z => z.Ignore())
                 .ForMember(y => y.Guid, z => z.Ignore());
            });

            var mapper = configuration.CreateMapper();

            var housingDetails = new HousingDetails();
            mapper.Map(from.HousingDetails, housingDetails);
            to.HousingDetails = housingDetails;
        }

        public AnimalComponentBase ReplicateAnimalComponent(AnimalComponentBase component)
        {
            var animalComponent = (AnimalComponentBase)Activator.CreateInstance(component.GetType());

            this.ReplicateAnimalComponent(animalComponent, component);
            animalComponent.Name = component.Name;

            /*
             * Needs to be true else the replicate component in a copied farm will have double groups
             * b/c Holos will reinitialize each group creating another copy
             */

            animalComponent.IsInitialized = true;

            return animalComponent;
        }

        public void ReplicateAnimalComponent(ComponentBase destination, ComponentBase source)
        {
            // The component to copy to
            var to = destination as AnimalComponentBase;

            // Clear any groups on existing component since we want both collections have the same groups
            to.Groups.Clear();

            // The component to copy from
            var from = source as AnimalComponentBase;

            foreach (var animalGroup in from.Groups)
            {
                if (animalGroup is AnimalGroup group)
                {
                    var copiedGroup = new AnimalGroup();
                    _animalGroupMapper.Map(group, copiedGroup);
                    to.Groups.Add(copiedGroup);

                    foreach (var managementPeriod in group.ManagementPeriods)
                    {
                        var copiedManagementPeriod = this.ReplicateManagementPeriod(managementPeriod);
                        copiedGroup.ManagementPeriods.Add(copiedManagementPeriod);
                    }
                }
            }
        }

        public void InitializeComponent(AnimalComponentBase component, Farm farm)
        {
            var name = this.GetUniqueComponentName(component, farm);

            component.Name = name;
            component.ComponentDescriptionString = name;
            component.GroupPath = name;

            // Use the carbon modelling start year for now to indicate the start year of the current set of management practices for the component.
            component.StartYear = farm.CarbonModellingEquilibriumYear;
            component.EndYear = DateTime.Now.Year;
        }

        public bool AnimalGroupHasDmiCalculations(AnimalGroup animalGroup)
        {
            if (animalGroup.GroupType.IsOtherAnimalType() || animalGroup.GroupType.IsPoultryType() || animalGroup.GroupType == AnimalType.DairyCalves)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a unique name for each animal component.
        /// </summary>
        private string GetUniqueComponentName(AnimalComponentBase component, Farm farm)
        {
            var i = 2;

            // Don't add number to component name at first (i.e. just use "Cow-Calf" and not "Cow-Calf #1") since it is more cleaner.
            var proposedName = component.ComponentNameDisplayString;

            // While the names are the same, try and make a unique name for this component.
            while (farm.Components.Where(x => string.IsNullOrWhiteSpace(x.Name) == false).Any(y => y.Name.Equals(proposedName)))
            {                
                proposedName = component.ComponentNameDisplayString + " #" + (i++);
            }

            return proposedName;
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}