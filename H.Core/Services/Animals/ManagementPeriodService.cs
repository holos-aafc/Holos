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

        protected ObservableCollection<HousingType> ValidHousingTypes { get; set; } = new ObservableCollection<HousingType>(Enum.GetValues(typeof(HousingType)).Cast<HousingType>().Where(x => x != HousingType.Custom));
        protected ObservableCollection<BeddingMaterialType> ValidBeddingMaterialTypes { get; set; } = new ObservableCollection<BeddingMaterialType>(Enum.GetValues(typeof(BeddingMaterialType)).Cast<BeddingMaterialType>());
        protected ObservableCollection<ManureStateType> ValidManureManagementTypes { get; set; } = new ObservableCollection<ManureStateType>(Enum.GetValues(typeof(ManureStateType)).Cast<ManureStateType>().Where(x => x != ManureStateType.Custom));

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

                this.UpdateValidHousingTypes(farm, group, bindingManagementPeriod);
                this.UpdateValidManureHandlingSystems(farm, group, bindingManagementPeriod);
                this.UpdateValidBeddingTypes(farm, managementPeriod);

                managementPeriod.HousingDetails.HousingType = ValidHousingTypes.FirstOrDefault();
                managementPeriod.HousingDetails.BeddingMaterialType = ValidBeddingMaterialTypes.FirstOrDefault();
                managementPeriod.ManureDetails.StateType = ValidManureManagementTypes.FirstOrDefault();

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

        protected void UpdateValidHousingTypes(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod)
        {
            var validHousingTypes = _housingTypeProvider.GetValidHousingTypes(animalGroup.GroupType);
            var thisAnimalCategoryCanHavePasture = validHousingTypes.Any(x => x == HousingType.Pasture);

            this.ValidHousingTypes.Clear();
            this.ValidHousingTypes.AddRange(validHousingTypes);

            var farmContainsFields = farm.Components.OfType<FieldSystemComponent>().Any();
            if (farmContainsFields)
            {
                if (thisAnimalCategoryCanHavePasture)
                {
                    if (this.ValidHousingTypes.Contains(HousingType.Pasture) == false)
                    {
                        this.ValidHousingTypes.Add(HousingType.Pasture);
                    }
                }
            }
            
            else
            {
                this.ValidHousingTypes.Remove(HousingType.Pasture);

                this.ValidHousingTypes.Remove(HousingType.Pasture);
                if (bindingManagementPeriod != null)
                {
                    bindingManagementPeriod.HousingDetails.HousingType = this.ValidHousingTypes.FirstOrDefault();
                }
            }
        }

        protected void UpdateValidManureHandlingSystems(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod)
        {
            var validHandlingSystems = _manureHandlingSystemProvider.GetValidManureStateTypesByAnimalCategory(animalGroup.GroupType);
            var thisAnimalCategoryCanHavePasture = validHandlingSystems.Any(x => x == ManureStateType.Pasture);

            this.ValidManureManagementTypes.Clear();
            this.ValidManureManagementTypes.AddRange(validHandlingSystems);

            var farmContainsFields = farm.Components.OfType<FieldSystemComponent>().Any();
            if (farmContainsFields)
            {
                if (thisAnimalCategoryCanHavePasture)
                {
                    if (this.ValidManureManagementTypes.Contains(ManureStateType.Pasture) == false)
                    {
                        this.ValidManureManagementTypes.Add(ManureStateType.Pasture);
                    }
                }
            }
            else
            {
                this.ValidManureManagementTypes.Remove(ManureStateType.Pasture);
                if (bindingManagementPeriod != null)
                {
                    bindingManagementPeriod.ManureDetails.StateType = this.ValidManureManagementTypes.FirstOrDefault();
                }
            }
        }

        protected void UpdateValidBeddingTypes(Farm farm, ManagementPeriod managementPeriod)
        {
            if (managementPeriod.AnimalType.IsBeefCattleType())
            {
                this.ValidBeddingMaterialTypes = new ObservableCollection<BeddingMaterialType>()
                    { BeddingMaterialType.Straw, BeddingMaterialType.WoodChip };
            }

            if (managementPeriod.AnimalType.IsDairyCattleType())
            {
                this.ValidBeddingMaterialTypes = new ObservableCollection<BeddingMaterialType>()
                {
                    BeddingMaterialType.Sand,
                    BeddingMaterialType.SeparatedManureSolid,
                    BeddingMaterialType.StrawLong,
                    BeddingMaterialType.StrawChopped,
                    BeddingMaterialType.Shavings,
                    BeddingMaterialType.Sawdust,
                };
            }

            if (managementPeriod.AnimalType.IsSwineType())
            {
                this.ValidBeddingMaterialTypes = new ObservableCollection<BeddingMaterialType>()
                {
                    BeddingMaterialType.None,
                    BeddingMaterialType.StrawLong,
                    BeddingMaterialType.StrawChopped,
                };
            }
        }
        #endregion
    }
}
