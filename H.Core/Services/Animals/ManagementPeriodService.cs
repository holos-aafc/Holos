using H.Core.Properties;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;
using H.Core.Providers.Feed;
using H.Core.Services.Initialization;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using System.Collections.Specialized;

namespace H.Core.Services.Animals
{
    public partial class ManagementPeriodService : IManagementPeriodService
    {
        #region Fields

        protected readonly HousingTypeProvider _housingTypeProvider = new HousingTypeProvider();
        protected readonly ManureHandlingSystemProvider _manureHandlingSystemProvider = new ManureHandlingSystemProvider();

        protected readonly IAnimalComponentHelper _animalComponentHelper;
        protected readonly IInitializationService _initializationService;

        #endregion

        #region Constructors

        public ManagementPeriodService()
        {
        }

        public ManagementPeriodService(IInitializationService initializationService, IAnimalComponentHelper animalComponentHelper)
        {
            if (initializationService != null)
            {
                _initializationService = initializationService;
            }
            else
            {
                throw new ArgumentNullException(nameof(initializationService));
            }

            if (animalComponentHelper != null)
            {
                _animalComponentHelper = animalComponentHelper;
            }
            else
            {
                throw new ArgumentNullException(nameof(animalComponentHelper));
            }
        }

        #endregion

        #region Private Methods

        private ManagementPeriod AddManagementPeriodToAnimalGroup(Farm farm, AnimalGroup group, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (group == null)
            {
                return null;
            }

            group.PropertyChanged -= animalGroupOnPropertyChanged;

            if (group.ManagementPeriods.Any())
            {
                var lastManagementPeriod = group.ManagementPeriods.OrderBy(x => x.Start).Last();
                var managementPeriod = _animalComponentHelper.ReplicateManagementPeriod(lastManagementPeriod);

                managementPeriod.Name = _animalComponentHelper.GetUniqueManagementPeriodName(group);
                managementPeriod.StartWeight = lastManagementPeriod.EndWeight;

                // Add one day so items in timeline are back-to-back
                var numberOfDays = lastManagementPeriod.NumberOfDays;
                var startDate = lastManagementPeriod.End.AddDays(1);
                var endDate = startDate.AddDays(numberOfDays);

                // Set end date first so validation checks performed in ManagementPeriod.End will pass
                managementPeriod.End = endDate;

                // Set end date second so validation checks performed in ManagementPeriod.Start will pass
                managementPeriod.Start = startDate;

                managementPeriod.NumberOfDays = numberOfDays;
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
                managementPeriod.EndWeight = managementPeriod.StartWeight + managementPeriod.NumberOfDays * managementPeriod.PeriodDailyGain;

                group.ManagementPeriods.Add(managementPeriod);

                return managementPeriod;
            }
            else
            {
                var managementPeriod = new ManagementPeriod();

                managementPeriod.AnimalGroupGuid = group.Guid;
                managementPeriod.DietGroupType = group.GroupTypeDiet;
                managementPeriod.AnimalType = group.GroupType;
                managementPeriod.AnimalGroupGuid = group.Guid;
                managementPeriod.Name = _animalComponentHelper.GetUniqueManagementPeriodName(group);
                managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
                managementPeriod.End = managementPeriod.Start.AddDays(30);
                managementPeriod.NumberOfDays = 31; // Inclusive of all calendar days
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                var firstDefaultDietForAnimalType = farm.Diets.FirstOrDefault(x => x.IsDefaultDiet && x.AnimalType.GetCategory() == managementPeriod.AnimalType.GetCategory());

                // These animal types do not have a default diet. Diet information is specified manually by user.
                if (managementPeriod.AnimalType.IsPoultryType() ||
                    managementPeriod.AnimalType.IsOtherAnimalType())
                {
                    firstDefaultDietForAnimalType = farm.Diets.SingleOrDefault(x => x.IsCustomPlaceholderDiet);
                }

                managementPeriod.SelectedDiet = firstDefaultDietForAnimalType;
                managementPeriod.DietAdditive = DietAdditiveType.None;
                
                managementPeriod.HousingDetails.HousingType = this.GetValidHousingTypes(farm, managementPeriod, group.GroupType).FirstOrDefault();
                managementPeriod.HousingDetails.BeddingMaterialType = this.GetValidBeddingTypes(group.GroupType).FirstOrDefault();
                managementPeriod.ManureDetails.StateType = this.GetValidManureStateType(group.GroupType, farm, managementPeriod).FirstOrDefault();

                group.ManagementPeriods.Add(managementPeriod);

                return managementPeriod;
            }
        }

        private ManagementPeriod AddOtherManagementPeriodToAnimalGroup(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            return managementPeriod;
        }

        private Diet GetDefaultDietForGroup(Farm farm, AnimalType animalType, DietType dietType)
        {
            var result = farm.Diets.FirstOrDefault(x => x.IsDefaultDiet && x.AnimalType.GetCategory() == animalType.GetCategory() && x.DietType == dietType);
            if (result == null)
            {
                result = farm.Diets.FirstOrDefault(x => x.IsDefaultDiet && x.AnimalType.GetCategory() == animalType.GetCategory());
            }
            return result;
        }

        /// <summary>
        /// The available housing types depends on the type of components on the farm. If there is an animal component on the farm that is housed on a pasture, there must be
        /// at least 1 field component on that farm as well. Therefore, pasture should not be an item in the list unless 1 field has been added to the farm
        /// </summary>
        public List<HousingType> GetValidHousingTypes(Farm farm, ManagementPeriod bindingManagementPeriod,
            AnimalType animalType)
        {
            var types = new List<HousingType>();

            var validHousingTypes = _housingTypeProvider.GetValidHousingTypes(animalType);
            types.AddRange(validHousingTypes);

            var thisAnimalCategoryCanHavePasture = validHousingTypes.Any(x => x == HousingType.Pasture);

            var farmContainsFields = farm.Components.OfType<FieldSystemComponent>().Any();
            if (farmContainsFields)
            {
                if (thisAnimalCategoryCanHavePasture)
                {
                    if (types.Contains(HousingType.Pasture) == false)
                    {
                        types.Add(HousingType.Pasture);
                    }
                }
            }
            else
            {
                types.Remove(HousingType.Pasture);

                types.Remove(HousingType.Pasture);
                if (bindingManagementPeriod != null)
                {
                    bindingManagementPeriod.HousingDetails.HousingType = types.FirstOrDefault();
                }
            }

            return types;
        }

        /// <summary>
        /// The available manure handling systems depend on the type of components on the farm. If there is an animal component on the farm that is housed on pasture, there must be
        /// at least 1 field component on that farm as well. Therefore, pasture should not be an item in the list unless 1 field has been added to the farm
        /// </summary>
        public List<ManureStateType> GetValidManureStateType(AnimalType animalType, Farm farm, ManagementPeriod managementPeriod)
        {
            var manureStateTypes = new List<ManureStateType>();

            var animalCategory = animalType.GetCategory();

            var validHandlingSystems = _manureHandlingSystemProvider.GetValidManureStateTypesByAnimalCategory(animalCategory);
            manureStateTypes.AddRange(validHandlingSystems);

            var thisAnimalCategoryCanHavePasture = validHandlingSystems.Any(x => x == ManureStateType.Pasture);

            var farmContainsFields = farm.Components.OfType<FieldSystemComponent>().Any();
            if (farmContainsFields)
            {
                if (thisAnimalCategoryCanHavePasture)
                {
                    if (manureStateTypes.Contains(ManureStateType.Pasture) == false)
                    {
                        manureStateTypes.Add(ManureStateType.Pasture);
                    }
                }
            }
            else
            {
                manureStateTypes.Remove(ManureStateType.Pasture);
                if (managementPeriod != null)
                { 
                    managementPeriod.ManureDetails.StateType = manureStateTypes.FirstOrDefault();
                }
            }

            return manureStateTypes;
        }

        public ObservableCollection<BeddingMaterialType> GetValidBeddingTypes(AnimalType animalType)
        {
            var validTypes = new ObservableCollection<BeddingMaterialType>();

            if (animalType.IsBeefCattleType())
            {
                validTypes.Add(BeddingMaterialType.None);
                validTypes.Add(BeddingMaterialType.WoodChip);
                validTypes.Add(BeddingMaterialType.Straw);
            }

            if (animalType.IsDairyCattleType())
            {
                validTypes.Add(BeddingMaterialType.None);
                validTypes.Add(BeddingMaterialType.Sand);
                validTypes.Add(BeddingMaterialType.SeparatedManureSolid);
                validTypes.Add(BeddingMaterialType.StrawLong);
                validTypes.Add(BeddingMaterialType.StrawChopped);
                validTypes.Add(BeddingMaterialType.Shavings);
                validTypes.Add(BeddingMaterialType.Sawdust);
            }

            if (animalType.IsSwineType())
            {
                validTypes.Add(BeddingMaterialType.None);
                validTypes.Add(BeddingMaterialType.StrawLong);
                validTypes.Add(BeddingMaterialType.StrawChopped);
            }

            if (animalType.IsSheepType())
            {
                validTypes.Add(BeddingMaterialType.None);
                validTypes.Add(BeddingMaterialType.Straw);
                validTypes.Add(BeddingMaterialType.Shavings);
            }

            if (animalType.IsPoultryType())
            {
                validTypes.Add(BeddingMaterialType.None);
                validTypes.Add(BeddingMaterialType.Straw);
                validTypes.Add(BeddingMaterialType.Shavings);
                validTypes.Add(BeddingMaterialType.Sawdust);
            }

            if (animalType.IsOtherAnimalType())
            {
                validTypes.Add(BeddingMaterialType.None);
                validTypes.Add(BeddingMaterialType.Straw);
            }

            return validTypes;
        }
        #endregion
    }
}
